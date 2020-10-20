using System;
using System.Collections.Generic;
using System.Linq;
using Core.FS;
using Utils;

namespace Core.Items
{

    public interface ISuperPool
    {
        PoolItem GetNextItem(string path);
    }

    public static class Pool
    {
        public static Pool<NormalSubPool, T> CreateNormal<T>() where T : IHaveId
        {
            return new Pool<NormalSubPool, T>();
        }
        public static Pool<EndlessSubPool, T> CreateEndless<T>() where T : IHaveId
        {
            return new Pool<EndlessSubPool, T>();
        }
    }

    public class PoolFS<T> : FS<Directory, T> where T : SubPool, new()
    {
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

    public class Pool<SP, T> : ISuperPool
        where SP : SubPool, new()
        where T : IHaveId
    {
        private PoolFS<SP> m_fs = new PoolFS<SP>();
        private Dictionary<int, int> m_quantities = new Dictionary<int, int>();
        private Dictionary<int, PoolItem> m_items = new Dictionary<int, PoolItem>();
        private Random m_rng = new Random();

        public Pool()
        {
            m_fs = new PoolFS<SP>();
            m_quantities = new Dictionary<int, int>();
            m_items = new Dictionary<int, PoolItem>();
            m_rng = new Random();
        }

        private Pool(Pool<SP, T> copyFrom)
        {
            m_fs = new PoolFS<SP>();
            m_fs.CopyDirectoryStructure(copyFrom.m_fs.BaseDir, m_fs.BaseDir);
            m_fs.m_tildeMap = new List<string>(copyFrom.m_fs.m_tildeMap);
            m_rng = copyFrom.m_rng; // TODO: copy from seed

            foreach (var pool in copyFrom.m_items.Values)
            {
                m_items.Add(pool.id, (PoolItem)pool.Copy());
            }

            m_quantities = new Dictionary<int, int>(copyFrom.m_quantities);

            foreach (var subpool in m_fs.GetAllFiles())
            {
                var prev = subpool.items;
                subpool.items = new HashSet<PoolItem>();
                foreach (var it in prev)
                {
                    subpool.items.Add(m_items[it.id]);
                }
            }
        }

        private void AddItem(PoolItem item)
        {
            m_items.Add(item.id, item);
            m_quantities[item.id] = item.quantity;
        }

        // this operation only supports one specific path (no wildcards)
        public void Add(string path, int id, int quantity)
        {
            var subpool = (SP)m_fs.GetFileLazy(path, new SP());
            var item = new PoolItem(id, quantity);
            subpool.items.Add(item);
        }

        public void Add(string path, IHaveId item, int quantity)
            => Add(path, item.Id, quantity);

        public void Add(string path, int id)
            => Add(path, m_items[id]);

        public void AddItems(IEnumerable<PoolItem> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        // make sure you don't use wildcards on intermediary levels
        // public void AssureExists(string path)
        // {
        //     GetNodesLazy(path);
        // }

        // this one supports wildcards
        public void Add(string path, PoolItem item)
        {
            var subpools = m_fs.GetNodesLazy(path, new SP());
            AddItem(item);
            foreach (var subpool in subpools)
            {
                ((SP)subpool).items.Add(item);
            }
        }

        public void AddRange(string path, IEnumerable<PoolItem> items)
        {
            var subpools = m_fs.GetNodesLazy(path, new SP());
            foreach (var item in items)
            {
                AddItem(item);
            }
            foreach (var subpool in subpools)
            {
                foreach (var item in items)
                {
                    ((SP)subpool).items.Add(item);
                }
            }
        }

        public Pool<SP, T> Copy()
        {
            return new Pool<SP, T>(this);
        }

        protected void Exhaust(SubPool subPool)
        {
            foreach (var item in subPool.items)
            {
                item.quantity = m_quantities[item.id];
                subPool.ReshuffleDeck(m_rng);
            }
        }

        protected void ExhaustAll()
        {
            foreach (var item in m_items.Values)
            {
                item.quantity = m_quantities[item.id];
            }
        }

        public PoolItem GetNextItem(string path)
        {
            var subPools = m_fs.GetFiles(path);
            foreach (var s in subPools)
            {
                if (!s.IsReadyToGenerate)
                {
                    s.GenerateDeck(m_rng);
                }
            }
            var subPool = subPools[m_rng.Next(0, subPools.Count - 1)];
            var item = subPool.GetNextItem(m_rng);
            if (item == null)
            {
                Exhaust(subPool);
                item = subPool.GetNextItem(m_rng);
            }
            return item;
        }
    }
}