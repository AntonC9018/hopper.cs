using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.PinningNS
{
    [EntityType]
    public static class Water
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.FLOOR);
            FactionComponent.AddTo(subject, Faction.Environment);
            Damageable.AddTo(subject, new Health(1));
            PinningComponent.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDamageable().DefaultPreset();
        }

        public static void Retouch(EntityFactory factory)
        {
            
            PinningComponent.AddInitTo(factory);
        }
    }
}