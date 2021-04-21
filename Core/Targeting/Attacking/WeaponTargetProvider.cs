using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat.Basic;
using Hopper.Utils.Vector;
using Hopper.Core.Predictions;
using System;
using System.Linq;
using Hopper.Utils;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Targeting
{
    public partial class WeaponTargetProvider : IComponent
    {
        [Inject] public PieceAttackPattern _pattern;
        [Inject] public System.Action<AttackTargetingContext> _map;
        [Inject] public Layer _targetLayer;
        [Inject] public Layer _skipLayer;


        public AttackTargetingContext GetTargets(Entity attacker, IntVector2 attackDirection)
        {
            return GetTargets(attacker, attacker.GetTransform().position, attackDirection);
        }

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
                blockLayer       : _skipLayer
            );

            _map(context);

            return context;
        }


        public static readonly WeaponTargetProvider Simple = new WeaponTargetProvider(
            new PieceAttackPattern(Piece.Default), SingleSimple, Layer.REAL, Layer.WALL);


        public static void SetTargetEntitiesAndBlocks(
            IEnumerable<AttackTargetContext> targetContexts, Layer targetedLayer)
        {
            foreach (var ctx in targetContexts)
            {
                if (World.Global.grid.TryGetTransformFromLayer(
                        ctx.position, ctx.direction, targetedLayer, out ctx.transform)
                    && ctx.transform.entity.TryGetAttackable(out var attackable))
                {
                    ctx.attackness = attackable._attackness;
                }
            }
        }

        // NOTE: THIS WILL MODIFY THE ATTACKNESS OF THE CONTEXT
        public static bool _IsAttackableTarget(AttackTargetContext context, Layer targetedLayer, Layer skipLayer)
        {
            if (
                World.Global.grid.HasNoTransformAt(context.position, context.direction, skipLayer)
                && World.Global.grid.TryGetTransformFromLayer(
                    context.position, context.direction, targetedLayer, out context.transform)
                && context.transform.entity.TryGetAttackable(out var attackable))
            {
                context.attackness = attackable._attackness;
                return _IsAttackableByDefault(context);
            }
            return false;
        }

        public static void SingleSimple(AttackTargetingContext context)
        {
            var first = context.targetContexts.Where(
                c => _IsAttackableTarget(c, context.targetedLayer, context.blockLayer)).FirstOrDefault();
            
            if (context.targetContexts.Count > 0 && first != null)
            {
                context.targetContexts[0] = first;
                context.targetContexts.RemoveRange(1, context.targetContexts.Count - 1);
            }
        }

        public static void SingleDefault(AttackTargetingContext context)
        {
            SetTargetEntitiesAndBlocks(context.targetContexts, context.targetedLayer);
            context.targetContexts = context.targetContexts.Where(_IsAttackableOrBlock).ToList();
            if (context.targetContexts.Count == 0) return;
            context.targetContexts = 
                DiscardUnreachable(context.targetContexts, context.pattern)
                    .Where(c => !_IsBlock(c, context.blockLayer))
                    .ToList();
            if (context.targetContexts.Count == 0) return;
            KeepFirst_ThatCanBeAttacked_ByDefault(context.targetContexts);
        }

        public static void MultiDefault(AttackTargetingContext context)
        {
            SetTargetEntitiesAndBlocks(context.targetContexts, context.targetedLayer);
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
            context.attackness.HasFlag(Attackness.CAN_BE_ATTACKED | Attackness.IS_BLOCK);

        public static bool _IsAttackableByDefault(AttackTargetContext context) =>
            context.attackness.HasFlag(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT)
            && (!context.attackness.HasFlag(Attackness.IF_NEXT_TO) || context.pieceIndex == 0);

        public static void KeepFirst_ThatCanBeAttacked_ByDefault(List<AttackTargetContext> contexts)
        {
            var first = contexts.Find(_IsAttackableByDefault);

            if (first != null)
            {
                contexts.Clear();
                contexts.Add(first);
            }
        }

        public static List<AttackTargetContext> TakeAll_ThatCanBeAttacked_ByDefault(IEnumerable<AttackTargetContext> contexts)
        {
            // take first that can be attacked by default
            var newContexts = contexts.Where(
                // we consider maybe to be
                t => t.attackness.HasFlag(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT)
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

                if (piece.reach.reachesAll || piece.reach.values.All(j => blockedArray[j] == false))
                {
                    yield return target;
                }
            }
        }

        public static bool _IsBlock(AttackTargetContext context, Layer skipLayer)
        {
            return context.transform.layer.HasFlag(skipLayer);
        }
    }
}