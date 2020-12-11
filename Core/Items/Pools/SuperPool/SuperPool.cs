using System;
using System.Collections.Generic;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public class SuperPool<SP> : ISuperPool where SP : SubPool, new()
    {
        private PoolFS<SP> m_fs = new PoolFS<SP>();
        private Dictionary<int, int> m_quantities = new Dictionary<int, int>();
        private Dictionary<int, PoolItem> m_items = new Dictionary<int, PoolItem>();
        private Random m_rng = new Random();

        public SuperPool()
        {
            m_fs = new PoolFS<SP>();
            m_quantities = new Dictionary<int, int>();
            m_items = new Dictionary<int, PoolItem>();
            m_rng = new Random();
        }

        private SuperPool(SuperPool<SP> copyFrom)
        {
            m_fs = new PoolFS<SP>();
            m_fs.BaseDir.CopyDirectoryStructureFrom(copyFrom.m_fs.BaseDir);
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

        public void AddItemToSubpool(string subpoolPath, PoolItem item)
        {
            TryRegisterItem(item);

            var subpools = m_fs.GetNodesLazy(subpoolPath, new SP());
            foreach (var subpool in subpools)
            {
                ((SP)subpool).items.Add(item);
            }
        }

        public void AddItemToSubpool(string path, int id)
        {
            Assert.That(m_items.ContainsKey(id), "Item must be registered first");

            AddItemToSubpool(path, m_items[id]);
        }

        private void TryRegisterItem(PoolItem item)
        {
            if (!m_items.ContainsKey(item.id))
            {
                m_items.Add(item.id, item);
                m_quantities[item.id] = item.quantity;
            }
            else
            {
                Assert.AreEqual(m_items[item.id].quantity, item.quantity,
                    "Adding items with the same id but with a different quantity is not allowed");
            }
        }

        public void RegisterItem(PoolItem item)
        {
            m_items.Add(item.id, item);
            m_quantities[item.id] = item.quantity;
        }

        public void AddItemsToSubpool(string path, IEnumerable<PoolItem> items)
        {
            var subpools = m_fs.GetNodesLazy(path, new SP());
            foreach (var item in items)
            {
                TryRegisterItem(item);
            }
            foreach (var subpool in subpools)
            {
                foreach (var item in items)
                {
                    ((SP)subpool).items.Add(item);
                }
            }
        }

        public SuperPool<SP> Copy()
        {
            return new SuperPool<SP>(this);
        }

        protected void ResetSubpoolItems(SubPool subPool)
        {
            foreach (var item in subPool.items)
            {
                item.quantity = m_quantities[item.id];
                subPool.ReshuffleDeck(m_rng);
            }
        }

        public void ResetAll()
        {
            foreach (var item in m_items.Values)
            {
                item.quantity = m_quantities[item.id];
            }
            foreach (var subpool in m_fs.GetAllFiles())
            {
                subpool.ReshuffleDeck(m_rng);
            }
        }

        public void FinishConfiguring()
        {
            var files = m_fs.GetAllFiles();

            Assert.AreNotEqual(0, files.Count,
                "Must have at least one subpool before being able to finish configuring");

            foreach (var subpool in files)
            {
                subpool.InitializeDeck(m_rng);
            }
        }

        public PoolItem GetNextItem(string path)
        {
            Assert.AreNotEqual(0, m_fs.BaseDir.nodes.Count,
                $"No subpools exist for this superpool.");

            var subPoolCandidates = m_fs.GetFiles(path);

            int subpoolIndex = m_rng.Next(0, subPoolCandidates.Count - 1);
            var subPool = subPoolCandidates[subpoolIndex];

            var item = subPool.GetNextItem(m_rng);

            if (item == null)
            {
                ResetSubpoolItems(subPool);
                item = subPool.GetNextItem(m_rng);
            }

            return item;
        }
    }
}