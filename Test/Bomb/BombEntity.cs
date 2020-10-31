using System;
using Core;
using Core.Behaviors;
using Core.Retouchers;
using Core.Stats;
using Core.Stats.Basic;
using Core.Targeting;

namespace Test
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

        private static DefaultStats defaultStats = GetDefaultStats();
        private static DefaultStats GetDefaultStats()
        {
            var res = new Attack.Source.Resistance();
            res.Add(Explosion.AtkSource.Id, 10);

            return new DefaultStats()
                .Set(Attack.Source.Resistance.Path, res)
                .Set(Push.Resistance.Path, new Push.Resistance { pierce = 0 });
        }

        public static EntityFactory<BombEntity> CreateFactory()
        {
            return new EntityFactory<BombEntity>()
                .AddBehavior<Attackable>()
                .Retouch(Attackableness.Constant(AtkCondition.IF_NEXT_TO))
                .AddBehavior<Pushable>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(steps))
                .AddBehavior<Statused>()
                .Retouch(Core.Retouchers.Reorient.OnDisplace)
                .SetDefaultStats(defaultStats);
        }

        public static EntityFactory<BombEntity> Factory = CreateFactory();
    }
}