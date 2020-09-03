using Chains;
using System.Collections.Generic;
using Vector;

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

        Sequence m_sequence;

        public Action CurrentAction
        { get { return m_sequence.CurrentAction; } }

        public Sequential(Entity entity, BehaviorConfig _conf)
        {
            var conf = (Config)_conf;
            m_sequence = new Sequence
            {
                stepData = conf.stepData,
                actor = entity
            };
            entity
                .GetBehavior<Tick>()
                .GetChain<Tick.Event>(Tick.s_chainName)
                .AddHandler<Tick.Event>(
                    e => m_sequence.TickAction()
                );
        }

        public List<IntVector2> GetMovs()
        {
            return m_sequence.GetMovs();
        }


    }
}