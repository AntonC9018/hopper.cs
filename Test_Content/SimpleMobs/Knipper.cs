using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Test_Content.Explosion;

namespace Hopper.Test_Content.SimpleMobs
{
    public class Knipper : Entity
    {
        public static EntityFactory<Knipper> Factory;
        private static readonly Step[] Steps;

        private static System.Func<Entity, Result> IsPlayerClose(int success, int fail)
        {
            return e =>
            {
                bool close = false;
                if (e.TryGetClosestPlayer(out var player))
                {
                    var absOffsetVec = (player.Pos - e.Pos).Abs();
                    close = absOffsetVec.x <= 1 && absOffsetVec.y <= 1;
                }
                return new Result
                {
                    index = close ? success : fail
                };
            };
        }


        public static EntityFactory<Knipper> CreateFactory()
        {
            return new EntityFactory<Knipper>()
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Pushable.Preset)
                .AddBehavior(Sequential.Preset(new Sequential.Config(Steps)))
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.StepBased)))
                .AddBehavior(Moving.Preset)
                .AddBehavior(Damageable.Preset)
                .AddBehavior(Displaceable.DefaultPreset);
        }

        static Knipper()
        {
            Steps = new Step[]
            {
                // 0: move. if near player, start exploding
                new Step
                {
                    successFunction = IsPlayerClose(fail : 1, success : 2),
                    action = Action.CreateBehavioral<Moving>(),
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
            Factory = CreateFactory();
        }
    }
}