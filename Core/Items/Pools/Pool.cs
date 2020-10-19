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

    public class Pool<SP, T> : FS<Directory>, ISuperPool
        where SP : SubPool, new()
        where T : IHaveId
    {
        private List<string> m_tildeMap;
        private Dictionary<int, int> m_quantities = new Dictionary<int, int>();
        private Dictionary<int, PoolItem> m_items = new Dictionary<int, PoolItem>();
        private int maxDepth = 0;
        private Random rng = new Random();

        private IEnumerable<Node> Expand(string pathItem, Directory currentDir)
        {
            if (pathItem == "*")
            {
                foreach (var item in currentDir.nodes.Values)
                {
                    yield return item;
                }
            }
            else
            {
                yield return currentDir.nodes[pathItem];
            }
        }

        private IEnumerable<Node> ExpandLazy<U>(string pathItem, Directory currentDir)
            where U : Node, new()
        {
            if (pathItem == "*")
            {
                foreach (var item in currentDir.nodes.Values)
                {
                    yield return item;
                }
            }
            else
            {
                if (!currentDir.nodes.ContainsKey(pathItem))
                {
                    currentDir.nodes.Add(pathItem, new U());
                }
                yield return currentDir.nodes[pathItem];
            }
        }

        private List<SP> GetSubPools(string path)
        {
            var splitPath = Split(path);
            maxDepth = Maths.Max(maxDepth, splitPath.Length);
            List<Node> currentNodes = new List<Node> { m_baseDir };
            for (int i = 0; i < splitPath.Length; i++)
            {
                var prevNodes = currentNodes;
                currentNodes = new List<Node>();
                var segment = splitPath[i] == "~" ? m_tildeMap[i] : splitPath[i];
                foreach (var node in currentNodes)
                {
                    foreach (var n in Expand(segment, (Directory)node))
                    {
                        currentNodes.Add(n);
                    }
                }
            }
            return currentNodes.ConvertAll(e => (SP)e);
        }

        private List<SP> GetSubPoolsLazy(string path)
        {
            var splitPath = Split(path);
            maxDepth = Maths.Max(maxDepth, splitPath.Length);
            List<Node> currentNodes = new List<Node> { m_baseDir };
            for (int i = 0; i < splitPath.Length; i++)
            {
                var prevNodes = currentNodes;
                currentNodes = new List<Node>();
                var segment = splitPath[i] == "~" ? m_tildeMap[i] : splitPath[i];
                foreach (var node in prevNodes)
                {
                    if (i < splitPath.Length - 1)
                    {
                        foreach (var n in ExpandLazy<Directory>(segment, (Directory)node))
                        {
                            currentNodes.Add(n);
                        }
                    }
                    else
                    {
                        foreach (var n in ExpandLazy<SP>(segment, (Directory)node))
                        {
                            currentNodes.Add(n);
                        }
                    }
                }
            }
            return currentNodes.ConvertAll(i => (SP)i);
        }

        private void AddItem(PoolItem item)
        {
            m_items.Add(item.id, item);
            m_quantities[item.id] = item.quantity;
        }


        // this operation only supports one specific path (no wildcards)
        public void Add(string path, int id, int quantity)
        {
            var subpool = (SP)GetFileLazy(path, new SP());
            var item = new PoolItem(id, quantity);
            subpool.items.Add(item);
        }

        public void Add(string path, IHaveId item, int quantity) => Add(path, item.Id, quantity);

        // make sure you don't use wildcards on intermediary levels
        public void AssureExists(string path)
        {
            GetSubPoolsLazy(path);
        }

        // this one supports wildcards
        public void Add(string path, PoolItem item)
        {
            var subpools = GetSubPoolsLazy(path);
            AddItem(item);
            foreach (var subpool in subpools)
            {
                subpool.items.Add(item);
            }
        }

        public void AddRange(string path, IEnumerable<PoolItem> items)
        {
            var subpools = GetSubPoolsLazy(path);
            foreach (var item in items)
            {
                AddItem(item);
            }
            foreach (var subpool in subpools)
            {
                foreach (var item in items)
                {
                    subpool.items.Add(item);
                }
            }
        }

        protected List<SP> GetAllSubPools()
        {
            List<Node> currentNodes = new List<Node>() { m_baseDir };
            while (!currentNodes.Any(e => e is SP))
            {
                var prevNodes = currentNodes;
                currentNodes = new List<Node>();
                foreach (var node in prevNodes)
                {
                    if (node is SP)
                    {
                        throw new System.Exception("Yikes");
                    }
                    foreach (var item in ((Directory)node).nodes.Values)
                    {
                        currentNodes.Add(item);
                    }
                }
            }
            return currentNodes.ConvertAll(e => (SP)e);
        }

        public Pool<SP, T> Copy()
        {
            var copy = new Pool<SP, T>();
            CopyDirectoryStructure(m_baseDir, copy.m_baseDir);
            foreach (var pool in m_items.Values)
            {
                copy.AddItem((PoolItem)pool.Copy());
            }
            foreach (var subpool in copy.GetAllSubPools())
            {
                var prev = subpool.items;
                subpool.items = new HashSet<PoolItem>();
                foreach (var it in prev)
                {
                    subpool.items.Add(copy.m_items[it.id]);
                }
            }
            return copy;
        }

        protected void Exhaust(SubPool subPool)
        {
            foreach (var item in subPool.items)
            {
                item.quantity = m_quantities[item.id];
                subPool.ReshuffleDeck(rng);
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
            var subPools = GetSubPoolsLazy(path);
            foreach (var s in subPools)
            {
                if (!s.IsReadyToGenerate)
                {
                    s.GenerateDeck(rng);
                }
            }
            var subPool = subPools[rng.Next(0, subPools.Count - 1)];
            var item = subPool.GetNextItem(rng);
            if (item == null)
            {
                Exhaust(subPool);
                item = subPool.GetNextItem(rng);
            }
            return item;
        }
    }
}