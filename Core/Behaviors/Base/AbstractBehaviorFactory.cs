using System.Collections.Generic;
using Chains;
using Utils;

namespace Core.Behaviors
{
    public abstract class BehaviorFactory
    {
        protected static IdGenerator s_idGenerator = new IdGenerator();
        public int id;
        public static Dictionary<System.Type, int> s_idMap =
            new Dictionary<System.Type, int>();
        protected Dictionary<ChainName, ChainTemplate> m_templates =
            new Dictionary<ChainName, ChainTemplate>();
        public abstract Behavior Instantiate(Entity entity, BehaviorConfig conf);
    }
}