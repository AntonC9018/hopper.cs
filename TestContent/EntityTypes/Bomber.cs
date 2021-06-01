using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;
using static Hopper.Core.ActingNS.Action;

namespace Hopper.TestContent.SimpleMobs
{
    [EntityType]
    public static class Bomber
    {
        public static EntityFactory Factory;

        public static readonly JoinedAction DieAndExplodeAction = Join(
            Simple(actor => actor.TryDie()),
            Explosion.DefaultExplodeAction(1)
        ); 
        
        public static void AddComponents(Entity subject)
        {
            var steps = new Step[]
            {
                // 0: move. if near player, start exploding
                new Step
                {
                    successFunction = IsPlayerClose(fail : 1, success : 2),
                    action = Moving.Action,
                    movs = Movs.Basic,
                    algo = Algos.EnemyAlgo
                },
                // 1: wait 1 bit. if near player, start exploding
                new Step
                {
                    successFunction = IsPlayerClose(fail : -1, success : 1)
                },
                // 2: exploding, possibly not explode if the player is gone
                new Step
                {
                    successFunction = IsPlayerClose(fail : -1, success :  1),
                },
                // 3: 1 bit delay before the inevitable explosion
                new Step
                {
                    relativeStepIndexSuccess = 1
                },
                // 4: die and explode
                new Step
                {
                    action = DieAndExplodeAction,
                    algo = Algos.SimpleAlgo
                }
            };
            SequentialMobBase.AddComponents(subject, Algos.StepBased, steps);
        }

        public static void InitComponents(Entity subject)
        {
            SequentialMobBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
        }

        private static System.Func<Acting, Result> IsPlayerClose(int success, int fail)
        {
            return acting =>
            {
                bool close = false;
                var transform = acting.actor.GetTransform();
                if (transform.TryGetClosestPlayer(out var player)
                    && player.TryGetTransform(out var playerTransform))
                {
                    var absOffsetVec = (playerTransform.position - transform.position).Abs();
                    close = absOffsetVec.x <= 1 && absOffsetVec.y <= 1;
                }
                return new Result
                {
                    index = close ? success : fail
                };
            };
        }
    }
}