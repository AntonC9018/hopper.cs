using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;

namespace Hopper.Test_Content.Explosion
{
    public class BombEntity : Entity
    {
        public static readonly EntityFactory<BombEntity> Factory;
        public static readonly SimpleAction DefaultExplodeAction;
        public static readonly SimpleAction DieAction;
        public static readonly JoinedAction DieAndExplodeAction;
        private static Step[] Steps;

        public static EntityFactory<BombEntity> CreateFactory()
        {
            return new EntityFactory<BombEntity>()
                .AddBehavior<Attackable>(new Attackable.Config(Attackness.CAN_BE_ATTACKED_IF_NEXT_TO))
                .AddBehavior<Pushable>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Acting>(new Acting.Config(Algos.SimpleAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(Steps))
                .AddBehavior<Statused>()
                .Retouch(Reorient.OnDisplace)
                .SetDefaultStats(GetDefaultStats);
        }

        private static DefaultStats GetDefaultStats(PatchArea patchArea)
        {
            return new DefaultStats(patchArea)
                .SetAtIndex(Attack.Source.Resistance.Path, Explosion.AtkSource.Id, 10)
                .Set(Push.Resistance.Path, new Push.Resistance { pierce = 0 });
        }

        static BombEntity()
        {
            DefaultExplodeAction = new SimpleAction(
               (e, _) => Explosion.Explode(e.Pos, 1, e.World));
            DieAction = new SimpleAction(
               (e, _) => e.Die());
            DieAndExplodeAction =
               new JoinedAction(DieAction, DefaultExplodeAction);
            Steps = new Step[]
            {
                new Step
                {
                    repeat = 3,
                    action = null
                },
                new Step
                {
                    action = DieAndExplodeAction
                }
            };
            Factory = BombEntity.CreateFactory();
        }
    }
}