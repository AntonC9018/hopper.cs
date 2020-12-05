using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Utils.Vector;

namespace Hopper.Test_Content
{
    public class Knipper : Entity
    {
        private static System.Func<Entity, Result> IsPlayerClose(int success, int fail)
        {
            return e =>
            {
                var player = e.GetClosestPlayer();
                var absOffsetVec = (player.Pos - e.Pos).Abs();
                bool close = absOffsetVec.x <= 1 && absOffsetVec.y <= 1;
                return new Result
                {
                    index = close ? success : fail
                };
            };
        }

        private static Step[] steps = new Step[]
        {
            // 0: move. if near player, start exploding
            new Step
            {
                successFunction = IsPlayerClose(fail : 1, success : 2),
                action = new BehaviorAction<Moving>(),
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
                action = BombEntity.DieAndExplodeAction,
                algo = Algos.SimpleAlgo
            }
        };

        public static EntityFactory<Knipper> Factory = CreateFactory();
        private static EntityFactory<Knipper> CreateFactory()
        {
            return new EntityFactory<Knipper>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Sequential>(new Sequential.Config(steps))
                .AddBehavior<Acting>(new Acting.Config(Algos.StepBased))
                .AddBehavior<Moving>()
                .AddBehavior<Damageable>()
                .AddBehavior<Displaceable>();
        }
    }
}