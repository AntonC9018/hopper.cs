using System.Collections.Generic;
using Hopper.Utils.FS;

namespace Hopper.Core.Items
{
    public class PoolFS<T> : FS<T> where T : SubPool, new()
    {
        // This should contain the strings of `currently active` pools
        // E.g. if it is zone 1, floor 2, ~.~.enemy should map to z1.f2.enemy 
        // TODO: fill this up via world
        public List<string> m_tildeMap;

        public PoolFS()
        {
            m_tildeMap = new List<string>();
        }

        protected override string[] Split(string path)
        {
            var split = base.Split(path);
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] == "~")
                {
                    split[i] = m_tildeMap[i];
                }
            }
            return split;
        }
    }
}