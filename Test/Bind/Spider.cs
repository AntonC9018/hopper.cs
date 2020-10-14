using System.Runtime.Serialization;
using Core;
using Core.Behaviors;
using Newtonsoft.Json;
using Utils;

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
            var bindAction = new SimpleAction(
                (entity, a) => entity.Behaviors.Get<Binding>()
                    .Activate(a, new Binding.Params { spice = BindStuff.StopMoveSpice })
            );

            var moveAction = new BehaviorAction<Moving>();

            var bindMoveAction = new CompositeAction(
                new Action[] { bindAction, moveAction }
            );

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
                .AddBehavior<Acting>(new Acting.Config { DoAction = Algos.EnemyAlgo })
                .AddBehavior<Sequential>(new Sequential.Config(CreateSequenceData()))
                .AddBehavior<Displaceable>()
                .AddBehavior<Moving>()
                .AddBehavior<Binding>()
                .AddBehavior<Attackable>()
                .RetouchAndSave(SelfBindingStuff.retoucher);

            ClassUtils.AssureStaticallyConstructed(typeof(BindStuff));
            ClassUtils.AssureStaticallyConstructed(typeof(SelfBindingStuff));
        }
    }
}