using Chains;
using System.Collections.Generic;
using Utils.Vector;

namespace Core.Behaviors
{
    public class Sequential : Behavior
    {
        public class Config : BehaviorConfig
        {
            public StepData[] stepData;

            public Config(StepData[] _stepData)
            {
                if (_stepData == null)
                {
                    throw new System.Exception("Step Data must be specified in order to use Sequenced behavior");
                }
                stepData = _stepData;
            }
        }

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
            Tick.chain.ChainPath(entity).AddHandler(
                e => m_sequence.TickAction()
            );
        }

        public List<IntVector2> GetMovs()
        {
            return m_sequence.GetMovs();
        }
    }
}