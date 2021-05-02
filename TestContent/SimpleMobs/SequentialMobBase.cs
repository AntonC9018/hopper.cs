using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;
using Hopper.Core.Targeting;
using Hopper.Core.History;

namespace Hopper.TestContent
{
    [EntityType(Abstract = true)]
    public static class SequentialMobBase
    {
        public static EntityFactory Factory;
        
        public static void AddComponents(Entity subject, System.Action<Acting.Context> Algorithm, params Step[] sequenceSteps)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.REAL);
            FactionComponent.AddTo(subject, Faction.Enemy);
            History.AddTo(subject);
            
            Acting    .AddTo(subject, Sequential.CalculateAction, Algorithm, Order.Entity);
            Moving    .AddTo(subject);
            Ticking   .AddTo(subject);
            Pushable  .AddTo(subject);
            Attacking .AddTo(subject);
            Sequential.AddTo(subject, new Sequence(sequenceSteps));
            Attackable.AddTo(subject, Attackness.ALWAYS);
            Damageable.AddTo(subject, new Health(1));
            Displaceable.AddTo(subject, ExtendedLayer.BLOCK);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetActing() .DefaultPreset(subject);
            subject.GetMoving() .DefaultPreset();
            subject.GetTicking().DefaultPreset();
            subject.GetPushable()  .DefaultPreset();
            subject.GetAttacking() .NoInventoryPreset();
            subject.GetAttackable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
            subject.GetDisplaceable().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}