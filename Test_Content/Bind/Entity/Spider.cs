using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Components.Basic;

namespace Hopper.TestContent.Bind
{
    [DataContract]
    public class Spider : Entity, ISelfBinder
    {
        public static EntityFactory<Spider> Factory = CreateFactory();

        [DataMember] public Entity BoundEntity { get; set; }
        public override Layer Layer => BoundEntity == null ? Layer.REAL : ExtendedLayer.ABOVE;

        private static Step[] CreateSequenceData()
        {
            var bindAction = Action.CreateBehavioral<Binding>();
            var moveAction = Action.CreateBehavioral<Moving>();

            var bindMoveAction = Action.CreateJoinedDirected(bindAction, moveAction);

            var stepData = new Step[]
            {
                new Step
                {
                    action = bindMoveAction,
                    movs = Movs.Diagonal,
                    successFunction = e => new Result
                    {
                        success = ((ISelfBinder)e).BoundEntity != null
                    }
                },
                new Step
                {
                    action = null,
                    successFunction = e => new Result
                    {
                        success = ((ISelfBinder)e).BoundEntity == null
                    },
                }
            };

            return stepData;
        }

        public static EntityFactory<Spider> CreateFactory()
        {
            return new EntityFactory<Spider>()
                .AddBehavior(Acting.Preset(new Acting.Config(Algos.EnemyAlgo)))
                .AddBehavior(Sequential.Preset(new Sequential.Config(CreateSequenceData())))
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Moving.Preset)
                .AddBehavior(Binding.Preset(Bind.StopMoveStatus))
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Damageable.Preset)
                .Retouch(Bind.StopMoveRetoucher);
        }
    }
}