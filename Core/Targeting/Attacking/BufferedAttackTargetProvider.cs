using System.Collections.Generic;
using Hopper.Core.Components.Basic;
using Hopper.Utils.Vector;
using System.Linq;
using Hopper.Utils;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Targeting
{
    public partial class BufferedAttackTargetProvider : IComponent
    {
        /// <summary>
        /// The pattern is used to make AttackTargetContext's initially.
        /// </summary>
        [Inject] public PieceAttackPattern _pattern;
        /// <summary>
        /// The function used to filter contexts gathered through the pattern.
        /// </summary>
        [Inject] public System.Action<AttackTargetingContext> _map;
        /// <summary>
        /// Only entities from this layer will be targeted.
        /// However the exact logic of this field depends on the map function.
        /// </summary>
        [Inject] public Layer _targetLayer;


        /// <summary>
        /// If there is an entity of this layer at the specified position, targeting
        /// the entity from the target layer from that cell will not even be consdered.
        /// Also, by some of the algorithms, this value is used to indicate which layers
        /// should be considered blocking layers. The entities from them will not be attacked,
        /// but they will still take part in figuring out which of the other entities are to be targeted.
        /// However the exact logic of this field depends on the map function.
        /// </summary>
        [Inject] public Layer _blockLayer;


        /// <summary>
        /// Get targets relative to the position of the given entity.
        /// The entity must have a transform components, otherwise this will crash.
        /// </summary>
        public AttackTargetingContext GetTargets(Entity attacker, IntVector2 attackDirection)
        {
            return GetTargets(attacker, attacker.GetTransform().position, attackDirection);
        }

        /// <summary>
        /// Get targets at the given position and return a targeting context 
        /// with all extra information. The pattern will be rotated by 
        /// the attack direction in order to produce correct targeted positions.
        /// If the attacker is null, the attacj us considered anonymous.
        /// </summary>
        public AttackTargetingContext GetTargets(
            Entity attacker, IntVector2 attackerPosition, IntVector2 attackDirection)
        {
            var context = new AttackTargetingContext(
                targetContexts   : _pattern.MakeContexts(attackerPosition, attackDirection).ToList(),
                pattern          : _pattern,
                attacker         : attacker,
                attackerPosition : attackerPosition,
                attackDirection  : attackDirection,
                targetedLayer    : _targetLayer,
                blockLayer       : _blockLayer
            );

            _map(context);

            return context;
        }

        /// <summary>
        /// Represent the most basic "dagger" pattern, which targets entities 
        /// one block in front of the position, from the REAL layer, and 
        /// considers entities from WALL layer blocks and uses the SingleSimpleMap,
        /// which jsut tries to return the first valid target.
        /// </summary>
        public static readonly BufferedAttackTargetProvider Simple = new BufferedAttackTargetProvider(
            new PieceAttackPattern(Piece.Default), SingleSimpleMap, Layer.REAL, Layer.WALL);


        /// <summary>
        /// Modifies the given target contexts to have the entity from the targeted layer.
        /// Modifies the attackness to the actual attackness of the entity if it is attackable.
        /// </summary>
        public static void SetTargetEntitiesAndBlocks(
            IEnumerable<AttackTargetContext> targetContexts, Layer targetedLayer, Layer blockLayer)
        {
            foreach (var ctx in targetContexts)
            {
                if ((
                    // Try getting the blocks first, since those block the posibillity 
                    // of attacking the entity at all.
                    World.Global.grid.TryGetTransformFromLayer(
                        ctx.position, ctx.direction, blockLayer, out ctx.normal.transform)
                    // Try getting actual targets next
                    || World.Global.grid.TryGetTransformFromLayer(
                        ctx.position, ctx.direction, targetedLayer, out ctx.normal.transform))

                        // Get its attackable afterwards
                        // ? Although this is probably not relevant for the walls.
                        && ctx.transform.entity.TryGetAttackable(out var attackable)
                    )
                {
                    ctx.attackness = attackable._attackness;
                }
            }
        }

        /// <summary>
        /// Check if for the context given, there are no entities at the skip layer,
        /// Sets the targeted entity and its attackness.
        /// Returns true is the entity can be attacked by default.
        /// </summary>
        public static bool _IsAttackableTarget_AndSetTransformAndAttackness(
                AttackTargetContext context, Layer targetedLayer, Layer skipLayer)
        {
            if (
                World.Global.grid.HasNoTransformAt(context.position, context.direction, skipLayer)
                && World.Global.grid.TryGetTransformFromLayer(
                    context.position, context.direction, targetedLayer, out context.normal.transform)
                && context.transform.entity.TryGetAttackable(out var attackable))
            {
                context.attackness = attackable._attackness;
                return _IsAttackableByDefault(context);
            }
            return false;
        }

        /// <summary>
        /// Leaves the first target context in the given targeting contexts for which the entity
        /// can be attacked. The targeting context will be emptied if there are no such entities.
        /// </summary>
        public static void SingleSimpleMap(AttackTargetingContext context)
        {
            var first = context.targetContexts.Where(
                c => _IsAttackableTarget_AndSetTransformAndAttackness(c, context.targetedLayer, context.blockLayer)).FirstOrDefault();
            
            if (context.targetContexts.Count > 0 && first != null)
            {
                context.targetContexts[0] = first;
                context.targetContexts.RemoveRange(1, context.targetContexts.Count - 1);
            }
        }

        /// <summary>
        /// This map should be used by most weapons that make use of pattern's features.
        /// This map should only be used if your weapon targets at most 1 target at a time.
        /// </summary>
        public static void SingleDefaultMap(AttackTargetingContext context)
        {
            SetTargetEntitiesAndBlocks(context.targetContexts, context.targetedLayer, context.blockLayer);
            context.targetContexts = context.targetContexts.Where(_IsAttackableOrBlock).ToList();
            if (context.targetContexts.Count == 0) return;
            context.targetContexts = 
                DiscardUnreachable(context.targetContexts, context.pattern)
                    .Where(c => !_IsBlock(c, context.blockLayer))
                    .ToList();
            if (context.targetContexts.Count == 0) return;
            KeepFirst_ThatCanBeAttacked_ByDefault(context.targetContexts);
        }

        /// <summary>
        /// This map should be used by most weapons that make use of pattern's features.
        /// This map should be used if your weapon may target more than 1 entity at a time.
        /// </summary>
        public static void MultiDefaultMap(AttackTargetingContext context)
        {
            SetTargetEntitiesAndBlocks(context.targetContexts, context.targetedLayer, context.blockLayer);
            context.targetContexts = context.targetContexts.Where(_IsAttackableOrBlock).ToList();
            if (context.targetContexts.Count == 0) return;
            context.targetContexts = 
                DiscardUnreachable(context.targetContexts, context.pattern)
                    .Where(c => !_IsBlock(c, context.blockLayer))
                    .ToList();
            if (context.targetContexts.Count == 0) return;
            context.targetContexts = TakeAll_ThatCanBeAttacked_ByDefault(context.targetContexts);
        }


        public static bool _IsAttackableOrBlock(AttackTargetContext context) => 
            context.attackness.AreEitherSet(Attackness.CAN_BE_ATTACKED | Attackness.IS_BLOCK);

        public static bool _IsAttackableByDefault(AttackTargetContext context) =>
            context.attackness.HasFlag(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT) 
            && (!context.attackness.HasFlag(Attackness.IF_NEXT_TO) || context.pieceIndex == 0);

        public static void KeepFirst_ThatCanBeAttacked_ByDefault(List<AttackTargetContext> contexts)
        {
            var first = contexts.Find(_IsAttackableByDefault);

            contexts.Clear();
            
            if (first != null)
            {
                contexts.Add(first);
            }
        }

        public static List<AttackTargetContext> TakeAll_ThatCanBeAttacked_ByDefault(IEnumerable<AttackTargetContext> contexts)
        {
            // take first that can be attacked by default
            var newContexts = contexts.Where(
                // we consider maybe to be
                t => t.attackness.AreEitherSet(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT)
            ).ToList();

            if (
                // if all are attackable only close
                newContexts.All(t => t.attackness.HasFlag(Attackness.IF_NEXT_TO))
                // and none are close
                && newContexts.None(t => t.pieceIndex == 0)
            )
            {
                newContexts.Clear();
            }

            return newContexts;
        }

        public static IEnumerable<AttackTargetContext> DiscardUnreachable(
            IEnumerable<AttackTargetContext> contexts, PieceAttackPattern pattern)
        {
            // we assume they are sorted
            // weaponEvent.targets.Sort((a, b) => a.piece.index - b.piece.index);

            bool[] blockedArray = new bool[contexts.Max(t => t.pieceIndex) + 1];

            foreach (var target in contexts)
            {
                int i = target.pieceIndex;
                var piece = pattern.pieces[i];

                // we prevent reach if we are a block
                blockedArray[i] = target.attackness.HasFlag(Attackness.IS_BLOCK);

                if (piece.reach.reachesAll || piece.reach.indices.All(j => blockedArray[j] == false))
                {
                    yield return target;
                }
            }
        }

        public static bool _IsBlock(AttackTargetContext context, Layer skipLayer)
        {
            return skipLayer.HasFlag(context.transform.layer);
        }
    }
}