using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.Explosion
{
    public class Explosion
    {
        public static readonly Attack.Source AtkSource = new Attack.Source { resistance = 0 };
        public static readonly Push.Source PushSource = new Push.Source { resistance = 0 };
        public static readonly WorldEventPath<IntVector2> EventPath = new WorldEventPath<IntVector2>();

        private static Attack BaseAtk =>
            new Attack
            {
                damage = 3,
                power = 2,
                sourceId = AtkSource.Id,
                pierce = 1
            };

        private static Push BasePush =>
            new Push
            {
                power = 2,
                pierce = 1,
                distance = 1,
                sourceId = PushSource.Id
            };

        private static TargetLayers TargetLayers = new TargetLayers
        {
            targeted = Layer.DROPPED
                | Layer.ITEM
                | Layer.MISC
                | Layer.PROJECTILE
                | Layer.REAL
                | Layer.TRAP
                | Layer.FLOOR
                | Layer.WALL
                | ExtendedLayer.ABOVE,
            skip = 0
        };

        private static readonly MultiAtkTargetProvider targetProvider =
            new MultiAtkTargetProvider(PieceAttackPattern.Under, TargetLayers);
        private static Attackable.Params CreateMeta() => new Attackable.Params(BaseAtk, null);

        // TODO: Add a parameter for exposion attack stat
        public static UndirectedAction DefaultExplodeAction(int radius) =>
            Action.CreateSimple(
                e => Explosion.Explode(e.Pos, radius, e.World),
                e => PredictExplodePositions(e, radius)
            );

        /// <summary>
        /// Predict function for an explosion of a given radius.
        /// Will need to be modified when instead of positions we allow info structs.
        /// </summary>
        public static IEnumerable<IntVector2> PredictExplodePositions(Entity entity, int radius)
        {
            foreach (var vec in Spiral(-radius, -radius, radius, radius))
            {
                yield return entity.Pos + vec;
            }
        }

        public static void Explode(IntVector2 pos, int radius, World world)
        {
            foreach (var vec in Spiral(-radius, -radius, radius, radius))
            {
                IntVector2 current_pos = pos + vec;
                if (world.grid.IsOutOfBounds(current_pos) == false)
                {
                    IntVector2 knockback_dir = vec.Sign();
                    ExplodeCell(current_pos, knockback_dir, world);
                }
            }
        }

        public static void ExplodeCell(IntVector2 pos, IntVector2 knockbackDir, World world)
        {
            var targets = targetProvider.GetTargets(
                new Hopper.Core.Targeting.Dummy(pos, world), knockbackDir);

            foreach (var target in targets)
            {
                target.entity.Behaviors.Get<Attackable>()
                    .Activate(knockbackDir, CreateMeta());
                target.entity.Behaviors.TryGet<Pushable>()
                    ?.Activate(knockbackDir, BasePush);

                // complete block stops consequent exposions (kind of ugly, leave for now)
                if (target.entity.Layer.IsOfLayer(ExtendedLayer.ABOVE))
                {
                    break;
                }
            }
            // spawn particles through some mechanism 
            EventPath.Fire(world, pos);
        }

        public static IEnumerable<IntVector2> Spiral(int start_x, int start_y, int end_x, int end_y)
        {
            while (start_x <= end_x && start_y <= end_y)
            {
                // the first row from the remaining rows
                for (int i = start_y; i <= end_y; ++i)
                {
                    yield return new IntVector2(start_x, i);
                }
                start_x++;

                // the last column from the remaining columns
                for (int i = start_x; i <= end_x; ++i)
                {
                    yield return new IntVector2(i, end_y);
                }
                end_y--;

                // the last row from the remaining rows
                if (start_x <= end_x)
                {
                    for (int i = end_y; i >= start_y; --i)
                    {
                        yield return new IntVector2(end_x, i);
                    }
                    end_x--;
                }

                // the first column from the remaining columns
                if (start_y <= end_y)
                {
                    for (int i = end_x; i >= start_x; --i)
                    {
                        yield return new IntVector2(i, start_y);
                    }
                    start_y++;
                }
            }
        }
    }
}