using System;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Test_Content.Explosion
{
    // slot for bomb (endless)
    // when N x bombs are picked, apply N x tinkers so that 
    // when bombs
    // TODO: The sprite renderer for bombs should subscribe to action succeed event
    // and change the sprite OR make a retoucher that would add a change state event into
    // the history object.
    public class BombEntity : Entity
    {
        public static readonly SimpleAction DefaultExplodeAction = new SimpleAction(
            (e, _) => Explosion.Explode(e.Pos, 1, e.World));
        public static readonly SimpleAction DieAction = new SimpleAction(
            (e, _) => e.Die());
        public static readonly JoinedAction DieAndExplodeAction =
            new JoinedAction(DieAction, DefaultExplodeAction);

        private static Step[] steps = new Step[]
        {
            new Step
            {
                repeat = 20,
                action = null
            },
            new Step
            {
                action = DieAndExplodeAction
            }
        };

        private static DefaultStats GetDefaultStats(Registry registry)
        {
            System.Console.WriteLine(Explosion.AtkSource.GetId(registry));
            return new DefaultStats(registry)
                .SetAtIndex(Attack.Source.Resistance.Path, Explosion.AtkSource.GetId(registry), 10)
                .Set(Push.Resistance.Path, new Push.Resistance { pierce = 0 });
        }

        public static EntityFactory<BombEntity> CreateFactory(CoreRetouchers retouchers)
        {
            return new EntityFactory<BombEntity>()
                .AddBehavior<Attackable>()
                .Retouch(retouchers.Attackness.Constant(Attackness.IF_NEXT_TO))
                .AddBehavior<Pushable>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(steps))
                .AddBehavior<Statused>()
                .Retouch(retouchers.Reorient.OnDisplace)
                .SetDefaultStats(GetDefaultStats);
        }
    }
}