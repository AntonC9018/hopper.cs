using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
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
            entity.EndOfLoopEvent += () =>
            {
                m_sequence.TickAction();
            };
        }

        public List<Vector2> GetMovs()
        {
            return m_sequence.GetMovs();
        }

        public static BehaviorFactory s_factory =
            new BehaviorFactory(typeof(Sequential));

    }
}