using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Retouchers;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.SimpleMobs
{
    [EntityType]
    public static class Skeleton
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            var attackMoveAction = Action.CreateCompositeDirected(
                Action.CreateFromActivateable(Attacking.Index),
                Action.CreateFromActivateable(Moving.Index)
            );
            SequentialMobBase.AddComponents(subject,
                Algos.EnemyAlgo, 
                new Step
                {
                    action = attackMoveAction,
                    movs = Movs.Basic
                },
                new Step()
            );
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
            Skip.SkipNoPlayerHandlerWrapper.HookTo(subject);
            Skip.SkipBlockedHandlerWrapper.HookTo(subject);
            Reorient.OnActionSuccessHandlerWrapper.HookTo(subject);
        }
    }
}