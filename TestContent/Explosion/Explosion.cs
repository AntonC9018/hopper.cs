using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Utils.Vector;

namespace Hopper.TestContent
{
    // [Service]
    public static class Explosion
    {
        // public static readonly WorldEventPath<IntVector2> EventPath = new WorldEventPath<IntVector2>();

        private static Attack DefaultAttack =>
            new Attack
            {
                damage = 3,
                power = 2,
                source = Stat.Explosion.AttackSource.Index,
                pierce = 1
            };

        private static Push DefaultPush =>
            new Push
            {
                power = 2,
                pierce = 1,
                distance = 1,
                source = Stat.Explosion.PushSource.Index
            };

        private const Layer TargetedLayer = 
            Layer.DROPPED
            | Layer.ITEM
            | Layer.MISC
            | Layer.PROJECTILE
            | Layer.REAL
            | Layer.TRAP
            | Layer.FLOOR
            | Layer.WALL;


        // TODO: Add a parameter for exposion attack stat
        public static UndirectedAction DefaultExplodeAction(int radius) =>
            Action.CreateSimple(
                actor         => Explosion.Explode(actor.GetTransform().position, radius),
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

        public static GridManager Grid => World.Global.grid;

        public static void ExplodeBy(Entity entity)
        {
            entity.GetStats().GetLazy(Stat.Explosion.Index, out var explosion);
            Explode(entity.GetTransform().position, explosion.radius);
        }

        public static void Explode(IntVector2 pos, int radius)
        {
            foreach (var vec in IntVector2.Spiral(-radius, -radius, radius, radius))
            {
                IntVector2 current_pos = pos + vec;
                if (Grid.IsOutOfBounds(current_pos) == false)
                {
                    IntVector2 knockback_dir = vec.Sign();
                    ExplodeCell(current_pos, knockback_dir);
                }
            }
        }

        public static void ExplodeCell(IntVector2 pos, IntVector2 knockbackDir)
        {
            var targetTransforms = Grid.GetAllFromLayer(pos, knockbackDir, TargetedLayer);

            foreach (var transform in targetTransforms)
            {
                transform.entity.TryBeAttacked(null, DefaultAttack, knockbackDir);
                transform.entity.TryBePushed(DefaultPush, knockbackDir);
            }
        }
   }
}