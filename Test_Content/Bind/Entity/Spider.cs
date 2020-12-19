using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils;

namespace Hopper.Test_Content.Bind
{
    [DataContract]
    public class Spider : Entity, ISelfBinder
    {
        public static readonly EntityFactory<Spider> Factory = CreateFactory();
        public Spider() : base() { }

        [DataMember]
        public Entity BoundEntity { get; set; }

        private static Step[] CreateSequenceData()
        {
            var bindAction = new BehaviorAction<Binding>();
            var moveAction = new BehaviorAction<Moving>();

            var bindMoveAction = new JoinedAction(bindAction, moveAction);

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
                .AddBehavior(Displaceable.Preset)
                .AddBehavior(Moving.Preset)
                .AddBehavior(Binding.Preset(BindStatuses.StopMove))
                .AddBehavior(Attackable.DefaultPreset)
                .Retouch(BindRetouchers.StopMoveRetoucher);
        }
    }
}