using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using Hopper.Utils.Vector;

namespace Hopper.TestContent.LaserNS
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

        private static readonly StraightPattern DefaultPattern = new StraightPattern(Layers.WALL);

        private static readonly UnbufferedTargetProvider DefaultShooting 
            = new UnbufferedTargetProvider(DefaultPattern, Layers.REAL, Faction.Any);

        public static readonly TargetProviderAction ShootAction 
            = new TargetProviderAction(DefaultShooting, DefaultAttack, DefaultPush);
    }

    public class TargetProviderAction : IAction, IPredictable
    {
        public UnbufferedTargetProvider _provider;
        public Attack _attack;
        public Push _push;

        public TargetProviderAction(UnbufferedTargetProvider provider, Attack attack, Push push)
        {
            _provider = provider;
            _attack = attack;
            _push = push;
        }

        public void Shoot(IntVector2 position, IntVector2 direction)
        {
            var targets = _provider.GetTargets(position, direction);

            foreach (var target in targets)
            {
                target.transform.entity.TryBeAttacked(null, _attack, direction);
                target.transform.entity.TryBePushed(_push, direction);
            }
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return _provider.PredictPositionsBy(actor, direction, info);
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            Shoot(actor.GetTransform().position, direction);
            return true;
        }
    }
}