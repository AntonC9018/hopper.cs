using System.Collections.Generic;

namespace Hopper.Core.Mods
{
    public class ModsContent
    {
        internal Dictionary<System.Type, IMod> m_mods = new Dictionary<System.Type, IMod>();
        public T Get<T>() where T : IMod
        {
            return (T)m_mods[typeof(T)];
        }
    }
}