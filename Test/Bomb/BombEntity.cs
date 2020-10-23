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
        public static SimpleAction explodeAction = new SimpleAction(
            (e, _) => Explosion.Explode(e.Pos, 1, e.World));
        public static SimpleAction dieAction = new SimpleAction(
            (e, _) => e.Die());
        public static JoinedAction dieAndExplodeAction =
            new JoinedAction(dieAction, explodeAction);

        public static Step[] steps = new Step[]
        {
            new Step
            {
                repeat = 3,
                action = null
            },
            new Step
            {
                action = dieAndExplodeAction
            }
        };

        public static StatManager defaultStats = GetDefaultStats();

        private static StatManager GetDefaultStats()
        {
            var stats = new StatManager();
            var res = new Attack.Source.Resistance();
            res.Add(Explosion.AtkSource.Id, 10);
            stats.GetRaw(Attack.Source.Resistance.Path.String, res);
            stats.GetRaw(Push.Resistance.Path.String, new Push.Resistance { pierce = 0 });
            return stats;
        }

        public static EntityFactory<BombEntity> CreateFactory()
        {
            return new EntityFactory<BombEntity>()
                .AddBehavior<Attackable>()
                .Retouch(Attackableness.Constant(AtkCondition.IF_NEXT_TO))
                .AddBehavior<Pushable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(steps))
                .AddSetupListener(bomb => bomb.Stats.DefaultStats = defaultStats);
        }

        public static EntityFactory<BombEntity> Factory = CreateFactory();
    }
}