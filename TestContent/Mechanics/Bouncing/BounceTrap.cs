using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.BouncingNS
{
    [EntityType]
    public static class BounceTrap
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Transform.AddTo(subject, Layer.TRAP);
            FactionComponent.AddTo(subject, Faction.Enemy);
            Acting.AddTo(subject, ctx => Bouncing.UAction.Compile(), Algos.SimpleAlgo, Order.Trap);
            Damageable.AddTo(subject, new Health(1));
            Ticking.AddTo(subject);
            Bouncing.AddTo(subject);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetActing().DefaultPreset(subject);
            subject.GetBouncing().DefaultPreset(subject);
            subject.GetTicking().DefaultPreset();
        }

        public static void Retouch(EntityFactory factory)
        {
            Stats.AddInitTo(factory);
            Bouncing.AddInitTo(factory);
        }
    }
}