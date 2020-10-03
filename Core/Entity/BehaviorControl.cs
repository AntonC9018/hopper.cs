using System.Collections.Generic;
using System.Runtime.Serialization;
using Core.Behaviors;
using Newtonsoft.Json;

namespace Core
{
    [DataContract]
    public class BehaviorControl : IProvideBehavior
    {
        [DataMember]
        private readonly Dictionary<System.Type, Behavior> m_behaviors =
            new Dictionary<System.Type, Behavior>();

        // A setup method. May also be used at runtime, but setting up
        // behaviors in factory is prefered. Feel free to use while debugging.
        public void Add(System.Type t, Behavior behavior)
        {
            m_behaviors[t] = behavior;
        }

        public T Get<T>() where T : Behavior, new()
        {
            if (m_behaviors.ContainsKey(typeof(T)))
                return (T)m_behaviors[typeof(T)];
            return null;
        }

        public bool Has<T>() where T : Behavior, new()
        {
            return m_behaviors.ContainsKey(typeof(T));
        }
    }
}