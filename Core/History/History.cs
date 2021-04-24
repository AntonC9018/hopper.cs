using System.Collections.Generic;
using Hopper.Utils;
using System.Linq;
using Hopper.Core.Components;

namespace Hopper.Core.History
{
    public partial class History : IComponent
    {
        private List<IUpdateInfo> m_updates;

        public History()
        {
            m_updates = new List<IUpdateInfo>();
        }

        public void Add(IUpdateInfo info)
        {
            m_updates.Add(info);
        }

        public void Clear()
        {
            if (m_updates.Count == 0)
            {
                return;
            }
            m_updates.Clear();
        }
    }
}