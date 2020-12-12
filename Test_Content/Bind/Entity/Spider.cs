using System.Runtime.Serialization;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Utils;

namespace Hopper.Test_Content.Bind
{
    [DataContract]
    public class Spider : Entity, ISelfBinder
    {
        public Spider() : base() { }

        [DataMember]
        public Entity BoundEntity { get; set; }

        public static EntityFactory<Spider> CreateFactory(BindContent bind) =>
            new EntityFactory<Spider>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))
                .AddBehavior<Displaceable>()
                .AddBehavior<Moving>()
                .AddBehavior<Binding>(new Binding.Config { bindStatus = bind.NoMove })
                .AddBehavior<Attackable>()
                .Retouch(bind.NoMoveRetoucher);

        private static Step[] CreateSequenceData()
        {
            var bindAction = new BehaviorAction<Binding>();
            var moveAction = new BehaviorAction<Moving>();

            var bindMoveAction = new CompositeAction(bindAction, moveAction);

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
    }
}