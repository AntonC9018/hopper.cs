using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Retouchers;
using Hopper.Shared.Attributes;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent.SimpleMobs
{
    [EntityType]
    public static class Skeleton
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            SequentialMobBase.AddComponents(subject,
                Algos.EnemyAlgo, 
                new Step
                {
                    action = Compose(Attacking.Action, Moving.Action),
                    movs = Movs.Basic
                },
                new Step()
            );
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(EntityFactory factory)
        {
            SequentialMobBase.Retouch(factory);
            Skip.SkipNoPlayerHandlerWrapper.HookTo(factory);
            Skip.SkipBlockedHandlerWrapper.HookTo(factory);
            Reorient.OnActionSuccessHandlerWrapper.HookTo(factory);
        }
    }
}