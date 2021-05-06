using Hopper.Core;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent
{
    [EntityType]
    public static class Barrier
    {
        public static EntityFactory Factory;
        
        public static void AddComponents(Entity subject)
        {
            Transform.AddTo(subject, Layer.WALL);
            Directed.AddTo(subject);
            // Stats.AddTo(subject, Registry.Global._defaultStats);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject) {}
    }
}