using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.Stats.Basic;
using Core.Targeting;

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

        public class BombTarget : MultiTarget<AtkTarget>
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

        public static void Att(Cell cell)
        {

        }
    }
}