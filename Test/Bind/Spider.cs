using System.Runtime.Serialization;
using Core;
using Core.Behaviors;
using Newtonsoft.Json;
using Core.Utils;

namespace Test
{
    [DataContract]
    public class Spider : Entity, ISelfBinder
    {
        public Spider() : base() { }

        [DataMember]
        [JsonConverter(typeof(Core.IHaveIdConverter<Entity>))]
        public Entity BoundEntity { get; set; }


        public static EntityFactory<Spider> Factory;

        static Step[] CreateSequenceData()
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

        static Spider()
        {
            Factory = new EntityFactory<Spider>()
                .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))
                .AddBehavior<Displaceable>()
                .AddBehavior<Moving>()
                .AddBehavior<Binding>(new Binding.Config { bindStatus = BindStatuses.NoMove })
                .AddBehavior<Attackable>()
                .Retouch(SelfBinding.NoMoveRetoucher);

            ClassUtils.AssureStaticallyConstructed(typeof(SelfBinding));
        }
    }
}