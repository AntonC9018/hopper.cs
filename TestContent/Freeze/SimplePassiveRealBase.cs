using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Freezing
{
    [EntityType(Abstract = true)]
    public static class SimplePassiveRealBase
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Faction.AddTo(subject, Faction.Flags.Environment);
            Displaceable.AddTo(subject, ExtendedLayer.BLOCK);
            Attackable.AddTo(subject, Attackness.ALWAYS);
            // TODO: Be able to manipulate stats in a simple way.
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Pushable.AddTo(subject);
            Damageable.AddTo(subject, new Health(1));
            Transform.AddTo(subject, Layer.REAL);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDisplaceable().DefaultPreset();
            subject.GetAttackable().DefaultPreset();
            subject.GetPushable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
        }
    }
}