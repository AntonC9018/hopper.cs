using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Targeting;
using Utils.Vector;

namespace Test
{
    public class A
    {
        public static Attack.Source Source = new Attack.Source(0);
        public static Attack BaseAtt = new Attack
        {
            damage = 3,
            power = 1,
            sourceId = Source.Id,
            pierce = 1
        };

        public class BombTarget : MultiTarget<AtkTarget, Attack>
        {
            public override Layer TargetedLayer =>
                Layer.DROPPED
                & Layer.GOLD
                & Layer.MISC
                & Layer.PROJECTILE
                & Layer.REAL
                & Layer.TRAP
                & Layer.WALL;
            public override Layer SkipLayer => 0;
        }

        public static IProvideTargets<AtkTarget, Attack> targetProvider =
            TargetProvider.CreateMulti<AtkTarget, BombTarget, Attack>(
                Pattern.Under, Handlers.GeneralChain
            );

        public static void Att(IntVector2 pos, IntVector2 knockbackDir, World world)
        {
            var atkEvent = Target.CreateEvent<AtkTarget>(new Dummy(pos, world), knockbackDir);
            var targets = targetProvider.GetParticularTargets(atkEvent, BaseAtt);

            foreach (var target in targets)
            {
                target.targetEntity.Behaviors.Get<Attackable>().Activate(knockbackDir, BaseAtt);
            }
        }
    }
}