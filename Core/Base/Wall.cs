using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    [EntityType]
    public static class Wall
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Attackable.AddTo(subject, Attackness.IS_BLOCK | Attackness.MAYBE);
            Damageable.AddTo(subject, new Health(1));

            Faction   .AddTo(subject, Faction.Flags.Environment);
            Transform .AddTo(subject, Layer.WALL);
            Stats     .AddTo(subject, Registry.Global._defaultStats);

        }
        
        public static void InitComponents(Entity subject)
        {
            subject.GetAttackable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}