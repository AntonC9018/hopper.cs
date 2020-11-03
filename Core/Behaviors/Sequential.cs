using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Core.Utils.Vector;

namespace Core.Behaviors
{
    [DataContract]
    public class Sequential : Behavior
    {
        public class Config : BehaviorConfig
        {
            public readonly System.Func<ISequence> GetSequence;

            public Config(System.Func<ISequence> GetSequence)
            {
                this.GetSequence = GetSequence;
            }

            public Config(Step[] stepData)
            {
                if (stepData == null)
                {
                    throw new System.Exception("Step Data must be specified in order to use Sequenced behavior");
                }
                GetSequence = () => new Sequence(stepData);
            }
        }

        [DataMember]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        private ISequence m_sequence;

        public Action CurrentAction => m_sequence.CurrentAction;

        public override void Init(Entity entity, BehaviorConfig config)
        {
            m_entity = entity;

            var conf = (Config)config;
            m_sequence = conf.GetSequence();

            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(
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