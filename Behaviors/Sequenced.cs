using Chains;
using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Sequenced : Behavior
    {
        public Sequence m_sequence;
        public Entity m_entity;

        public Sequenced(Entity entity)
        {
        }

        public static BehaviorFactory s_factory = new BehaviorFactory(
            typeof(Sequenced), new ChainDefinition[] { }
        );

        public List<Vector2> GetMovs()
        {
            return new List<Vector2>();
        }
    }
}