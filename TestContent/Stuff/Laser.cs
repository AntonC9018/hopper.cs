using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent
{
    public static class Laser
    {
        [IdentifyingStat] public static Attack.Source AttackSource = 
            new Attack.Source(Default : Attack.Source.Resistance.Default);
        [IdentifyingStat] public static Push.Source PushSource =            
            new Push.Source(Default : Push.Source.Resistance.Default);

        private static Attack DefaultAttack = new Attack
        (
            power  : 1,
            source : AttackSource.Index,
            damage : 2,
            pierce : 5
        );
        private static Push DefaultPush = new Push
        (
            source   : PushSource.Index,
            power    : 1,
            distance : 1,
            pierce   : 1
        );

        private static readonly StraightPattern DefaultPattern = new StraightPattern(Layer.WALL);

        private static readonly UnbufferedTargetProvider DefaultShooting 
            = new UnbufferedTargetProvider(DefaultPattern, Layer.REAL, Faction.Any);

        public static void Shoot(IntVector2 position, IntVector2 direction)
        {
            var targets = DefaultShooting.GetTargets(position, direction);

            foreach (var target in targets)
            {
                target.transform.entity.TryBeAttacked(null, DefaultAttack, direction);
                target.transform.entity.TryBePushed(DefaultPush, direction);
            }
        }

        public static readonly DirectedAction ShootAction = 
            Action.CreateSimple(Shoot, DefaultShooting.PredictPositionsBy);
    }
}