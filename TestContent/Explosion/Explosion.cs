using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent
{
    // [Service]
    public static partial class Explosion
    {
        [Chain("gExplosion")] 
        public static readonly Index<Chain<IntVector2>> ExplosionChainIndex = new Index<Chain<IntVector2>>();

        private static Attack DefaultAttack =
            new Attack
            {
                damage = 3,
                power = 2,
                source = Stat.Explosion.AttackSource.Index,
                pierce = 1
            };

        private static Push DefaultPush =
            new Push
            {
                power = 2,
                pierce = 1,
                distance = 1,
                source = Stat.Explosion.PushSource.Index
            };

        private const Layers TargetedLayer = 
            Layers.DROPPED
            | Layers.ITEM
            | Layers.MISC
            | Layers.PROJECTILE
            | Layers.REAL
            | Layers.TRAP
            | Layers.FLOOR
            | Layers.WALL;


        // TODO: Add a parameter for exposion attack stat
        public static SimplePredictableUndirectedAction DefaultExplodeAction(int radius) =>
            Simple(
                Adapt((actor) => Explosion.Explode(actor.GetTransform().position, radius)),
                (actor, info) => PredictExplodePositions(actor.GetTransform(), radius)
            );

        /// <summary>
        /// Predict function for an explosion of a given radius.
        /// Will need to be modified when instead of positions we allow info structs.
        /// </summary>
        public static IEnumerable<IntVector2> PredictExplodePositions(Transform transform, int radius)
        {
            foreach (var vec in IntVector2.Spiral(-radius, -radius, radius, radius))
            {
                yield return transform.position + vec;
            }
        }

        public static GridManager Grid => World.Global.Grid;

        public static void ExplodeBy(Entity entity)
        {
            entity.GetStats().GetLazy(Stat.Explosion.Index, out var explosion);
            Explode(entity.GetTransform().position, explosion.radius);
        }

        public static void Explode(IntVector2 position, int radius)
        {
            foreach (var vec in IntVector2.Spiral(-radius, -radius, radius, radius))
            {
                IntVector2 currentPosition = position + vec;
                if (Grid.IsOutOfBounds(currentPosition) == false)
                {
                    IntVector2 knockback_dir = vec.Sign();
                    ExplodeCell(currentPosition, knockback_dir);
                }
            }
        }

        public static void ExplodeCell(IntVector2 position, IntVector2 knockbackDir)
        {
            ExplosionPath.Get().Pass(position);

            var targetTransforms = Grid.GetAllFromLayer(position, knockbackDir, TargetedLayer);

            foreach (var transform in targetTransforms)
            {
                transform.entity.TryBeAttacked(null, DefaultAttack, knockbackDir);
                transform.entity.TryBePushed(DefaultPush, knockbackDir);
            }
        }
   }
}