using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Sequenced : Behavior
    {
        public class Params : BehaviorParams
        {
            public StepData[] stepData;

            public Params(StepData[] _stepData)
            {
                if (_stepData == null)
                {
                    throw new System.Exception("Step Data must be specified in order to use Sequenced behavior");
                }
                stepData = _stepData;
            }
        }

        Sequence m_sequence;
        Entity m_entity;

        public Action CurrentAction
        {
            get
            {
                return m_sequence.CurrentAction;
            }
        }

        public Sequenced(Entity entity, BehaviorParams _pars)
        {
            var pars = (Params)_pars;
            m_sequence = new Sequence
            {
                stepData = pars.stepData,
                actor = entity
            };
        }

        public List<Vector2> GetMovs()
        {
            return m_sequence.GetMovs();
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Sequenced), new ChainDefinition[] { }
        );




    }
}