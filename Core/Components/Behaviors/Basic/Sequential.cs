using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Hopper.Utils.Vector;
using Hopper.Utils;

namespace Hopper.Core.Components.Basic
{
    [DataContract]
    public class Sequential : Behavior, IInitable<Sequential.Config>
    {
        public class Config
        {
            public readonly System.Func<Sequence> GetSequence;

            public Config(System.Func<Sequence> GetSequence)
            {
                this.GetSequence = GetSequence;
            }

            public Config(Step[] stepData)
            {
                Assert.That(stepData != null, "Step Data must be specified in order to use Sequenced behavior");
                GetSequence = () => new Sequence(stepData);
            }
        }

        [DataMember]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public Sequence Sequence { get; private set; }

        public ParticularAction CurrentAction => Sequence.CurrentAction;

        public void Init(Config config)
        {
            Sequence = config.GetSequence();

            Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
                e => Sequence.TickAction(m_entity)
            );
        }

        public List<IntVector2> GetMovs()
        {
            Assert.That(Sequence.CurrentStep.movs != null);
            return Sequence.CurrentStep.movs(m_entity);
        }

        public void ApplyCurrentAlgo(Acting.Event ev)
        {
            Sequence.CurrentStep.algo(ev);
        }

        public static ConfigurableBehaviorFactory<Sequential, Config> Preset(Config config)
            => new ConfigurableBehaviorFactory<Sequential, Config>(null, config);
    }
}