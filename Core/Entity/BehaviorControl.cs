using System.Collections.Generic;
using System.Runtime.Serialization;
using Hopper.Core.Components;
using Newtonsoft.Json;

namespace Hopper.Core
{
    [DataContract]
    public class BehaviorControl : IWithWithChain
    {
        [DataMember]
        private readonly Dictionary<System.Type, IWithChain> m_behaviors =
            new Dictionary<System.Type, IWithChain>();

        // A setup method. May also be used at runtime, but setting up
        // behaviors in factory is prefered. Feel free to use while debugging.
        public void Add(System.Type t, Behavior behavior)
        {
            m_behaviors[t] = behavior;
        }

        public T Get<T>() where T : IWithChain
        {
            return (T)m_behaviors[typeof(T)];
        }

        public T TryGet<T>() where T : IWithChain
        {
            if (m_behaviors.ContainsKey(typeof(T)))
                return (T)m_behaviors[typeof(T)];
            return default(T);
        }

        public bool Has<T>() where T : Behavior
        {
            return m_behaviors.ContainsKey(typeof(T));
        }
    }
}