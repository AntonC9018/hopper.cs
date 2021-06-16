using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    [EntityType]
    public static class Wall
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Attackable.AddTo(subject, Attackness.MAYBE);
            Damageable.AddTo(subject, new Health(1));

            FactionComponent.AddTo(subject, Faction.Environment);
            Transform .AddTo(subject, Layers.WALL, 0);
            Stats     .AddTo(subject, Registry.Global.Stats._map);

        }
        
        public static void InitComponents(Entity subject)
        {
            subject.GetAttackable();
            subject.GetDamageable().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}