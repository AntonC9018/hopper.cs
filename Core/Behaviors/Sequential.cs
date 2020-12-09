using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Hopper.Utils.Vector;
using Hopper.Utils;

namespace Hopper.Core.Behaviors
{
    [DataContract]
    public class Sequential : Behavior
    {
        public class Config
        {
            public readonly System.Func<ISequence> GetSequence;

            public Config(System.Func<ISequence> GetSequence)
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
        private ISequence m_sequence;

        public Action CurrentAction => m_sequence.CurrentAction;

        private void Init(Config config)
        {
            m_sequence = config.GetSequence();

            Tick.Chain.ChainPath(m_entity.Behaviors).AddHandler(
                e => m_sequence.TickAction(m_entity)
            );
        }

        public List<IntVector2> GetMovs()
        {
            return m_sequence.GetMovs(m_entity);
        }

        public void ApplyCurrentAlgo(Acting.Event ev)
        {
            ((Sequence)m_sequence).ApplyCurrentAlgo(ev);
        }
    }
}