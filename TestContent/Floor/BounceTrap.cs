using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Floor
{
    [EntityType]
    public static class BounceTrap
    {
        public static EntityFactory Factory;
        public static readonly UndirectedAction BounceAction = Action.CreateSimple(actor => actor.Bounce());

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.TRAP);
            FactionComponent.AddTo(subject, Faction.Enemy);
            Acting.AddTo(subject, ctx => BounceAction.ToParticular(), Algos.SimpleAlgo, Order.Trap);
            Damageable.AddTo(subject, new Health(1));
            Ticking.AddTo(subject);
            Bouncing.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetActing().DefaultPreset(subject);
            subject.GetBouncing().DefaltPreset(subject);
        }

        public static void Retouch(EntityFactory factory)
        {
            factory.InitInWorldFunc = InitInWorld;
        }

        public static void InitInWorld(Transform transform)
        {
            transform.entity.GetBouncing().InitInWorld(transform);
        }
    }
}