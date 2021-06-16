using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.SimpleMobs
{
    [EntityType(Abstract = true)]
    public static class SimplePassiveRealBase
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            FactionComponent.AddTo(subject, Faction.Environment);
            Displaceable.AddTo(subject, Layers.BLOCK);
            Attackable.AddTo(subject, Attackness.ALWAYS);
            // TODO: Be able to manipulate stats in a simple way.
            Stats.AddTo(subject, Registry.Global.Stats._map);
            Pushable.AddTo(subject);
            Damageable.AddTo(subject, new Health(1));
            Transform.AddTo(subject, Layers.REAL, TransformFlags.Default);
            Ticking.AddTo(subject);
            MoreChains.AddTo(subject, Registry.Global.MoreChains._map);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDisplaceable().DefaultPreset();
            subject.GetAttackable();
            subject.GetPushable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
            subject.GetTicking().DefaultPreset();
        }
    }
}