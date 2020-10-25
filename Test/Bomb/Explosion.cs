using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Targeting;
using Utils.Vector;

namespace Test
{
    public class Explosion
    {
        public static readonly Attack.Source AtkSource = new Attack.Source(0);
        public static readonly Attack BaseAtk = new Attack
        {
            damage = 3,
            power = 2,
            sourceId = AtkSource.Id,
            pierce = 1
        };

        public static readonly Push.Source PushSource = new Push.Source(0);
        public static readonly Push BasePush = new Push
        {
            power = 2,
            pierce = 1,
            sourceId = PushSource.Id
        };

        private static Layer TargetedLayer =
            Layer.DROPPED
            | Layer.GOLD
            | Layer.MISC
            | Layer.PROJECTILE
            | Layer.REAL
            | Layer.TRAP
            | Layer.WALL;
        private static Layer SkipLayer = 0;

        private static IProvideTargets<AtkTarget, Attack> targetProvider =
            TargetProvider.CreateMulti<AtkTarget, Attack>(
                Pattern.Under, Handlers.GeneralChain, SkipLayer, TargetedLayer
            );

        public static void Explode(IntVector2 pos, int radius, World world)
        {
            foreach (var vec in Spiral(-radius, -radius, radius, radius))
            {
                IntVector2 current_pos = pos + vec;
                if (world.m_grid.IsOutOfBounds(current_pos) == false)
                {
                    IntVector2 knockback_dir = vec.Sign();
                    ExplodeCell(current_pos, knockback_dir, world);
                }
            }
        }

        public static void ExplodeCell(IntVector2 pos, IntVector2 knockbackDir, World world)
        {
            var atkEvent = Target.CreateEvent<AtkTarget>(new Dummy(pos, world), knockbackDir);
            var targets = targetProvider.GetParticularTargets(atkEvent, BaseAtk);

            foreach (var target in targets)
            {
                target.targetEntity.Behaviors.Get<Attackable>().Activate(knockbackDir, BaseAtk);
                target.targetEntity.Behaviors.Get<Pushable>().Activate(knockbackDir, BasePush);
            }
            // TODO: spawn particles through some mechanism 
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