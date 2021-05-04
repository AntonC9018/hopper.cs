using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Floor
{
    [EntityType]
    public static class IceFloor
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Transform.AddTo(subject, Layer.REAL);
            FactionComponent.AddTo(subject, Faction.Enemy);
            Damageable.AddTo(subject, new Health(1));
            SlipperyComponent.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
        }

        public static void Retouch(EntityFactory factory)
        {
            factory.InitInWorldFunc += InitInWorld;
        }

        public static void InitInWorld(Transform transform)
        {
            transform.entity.GetSlipperyComponent().InitInWorld(transform);
        }
    }
}