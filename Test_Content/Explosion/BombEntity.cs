using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Registries;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;
using Hopper.Core.Targeting;

namespace Hopper.TestContent.Explosion
{
    public class BombEntity : Entity
    {
        public static EntityFactory<BombEntity> Factory;
        public static readonly UndirectedAction DieAction;
        public static readonly UndirectedAction DieAndExplodeAction;
        private static readonly Step[] Steps;

        public static EntityFactory<BombEntity> CreateFactory()
        {
            return new EntityFactory<BombEntity>()
                .AddBehavior(Attackable.Preset(Attackness.CAN_BE_ATTACKED_IF_NEXT_TO))
                .AddBehavior(Pushable.Preset)
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.SimpleAlgo)))
                .AddBehavior(Sequential.Preset(new Sequential.Config(Steps)))
                .AddBehavior(Statused.Preset)
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
            DieAction = Action.CreateSimple(e => e.Die());
            DieAndExplodeAction = Action.CreateJoinedUndirected(DieAction, Explosion.DefaultExplodeAction(1));

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
            Factory = CreateFactory();
        }
    }
}