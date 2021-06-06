using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.SlidingNS
{
    [EntityType]
    public static class IceFloor
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Transform.AddTo(subject, Layers.FLOOR, 0);
            FactionComponent.AddTo(subject, Faction.Enemy);
            Damageable.AddTo(subject, new Health(1));
            SlipperyComponent.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(EntityFactory factory)
        {
            SlipperyComponent.AddInitTo(factory);
        }
    }
}