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
            public Step[] stepData;

            public Config(Step[] _stepData)
            {
                if (_stepData == null)
                {
                    throw new System.Exception("Step Data must be specified in order to use Sequenced behavior");
                }
                stepData = _stepData;
            }
        }

        [DataMember]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        private Sequence m_sequence;

        public Action CurrentAction
        { get { return m_sequence.CurrentAction; } }

        public override void Init(Entity entity, BehaviorConfig config)
        {
            var conf = (Config)config;
            m_sequence = new Sequence
            {
                stepData = conf.stepData,
                actor = entity
            };
            Tick.Chain.ChainPath(entity.Behaviors).AddHandler(
                e => m_sequence.TickAction()
            );
        }

        public List<IntVector2> GetMovs()
        {
            return m_sequence.GetMovs();
        }
    }
}