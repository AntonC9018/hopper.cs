using Hopper.Core;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent
{
    [EntityType]
    public static class RealBarrier
    {
        public static EntityFactory Factory;
        
        public static void AddComponents(Entity subject)
        {
            Transform.AddTo(subject, Layers.REAL, TransformFlags.Directed);
            // Stats.AddTo(subject, Registry.Global.Stats._map);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(Entity subject) {}
    }
}