using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;

namespace Hopper.TestContent.SimpleMobs
{
    public static class Dummy
    {
        public static EntityFactory Factory;
        
        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.REAL);
            Attackable.AddTo(subject, Attackness.ALWAYS);
            Faction.AddTo(subject, Faction.Flags.Enemy);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject)
        {
        }
    }
}