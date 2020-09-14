using System;
using System.Collections.Generic;
using Core.FS;
using Utils;

namespace Core.Items
{
    public class PoolItem
    {
        public int id;
        public int q;

        public PoolItem(int id, int q)
        {
            this.id = id;
            this.q = q;
        }

        public PoolItem Copy()
        {
            return (PoolItem)this.MemberwiseClone();
        }
    }

    public abstract class ISubPool : File
    {
        public Dictionary<int, PoolItem> items
            = new Dictionary<int, PoolItem>();
        public Random rng = new Random();
        public List<PoolItem> deck;
        public int index;

        public bool IsReadyToGenerate { get => deck != null; }

        public void GenerateDeck()
        {
            deck = new List<PoolItem>();
            index = 0;
            foreach (var item in items.Values)
            {
                for (int i = 0; i < item.q; i++)
                {
                    deck.Add(item);
                }
            }
            deck.Shuffle(rng);
        }
        public void ReshuffleDeck()
        {
            deck.Shuffle(rng);
            index = 0;
        }
        public abstract ISubPool Copy(Dictionary<int, PoolItem> items);
        public abstract PoolItem GetNextItem();
    }

    public class SubPool : ISubPool
    {
        public override ISubPool Copy(Dictionary<int, PoolItem> _items)
        {
            var sp = new SubPool();
            foreach (var id in this.items.Keys)
            {
                sp.items.Add(id, _items[id]);
            }
            return sp;
        }
        public override PoolItem GetNextItem()
        {
            if (index >= deck.Count)
            {
                return null;
            }
            var item = deck[index];
            index++;
            if (item.q == 0)
                return GetNextItem();
            item.q--;
            return item;
        }
    }

    public class EndlessSubPool : ISubPool
    {
        public override ISubPool Copy(Dictionary<int, PoolItem> _items)
        {
            var sp = new EndlessSubPool();
            foreach (var id in this.items.Keys)
            {
                sp.items.Add(id, _items[id]);
            }
            return sp;
        }
        public override PoolItem GetNextItem()
        {
            if (index == deck.Count - 1)
            {
                ReshuffleDeck();
            }
            var item = deck[index];
            index++;
            return item;
        }
    }

    public class Pool : Directory
    {
    }

    public class SuperPool<SP> : FS<Pool> where SP : ISubPool
    {
        public PoolDefinition<SP> poolDef;
        public Dictionary<int, PoolItem> items = new Dictionary<int, PoolItem>();

        public SuperPool(PoolDefinition<SP> poolDef)
        {
            this.poolDef = poolDef;
            foreach (var kvp in poolDef.items)
            {
                items.Add(kvp.Key, kvp.Value.Copy());
            }
            CopyDirectoryStructure(poolDef.m_baseDir, this.m_baseDir);
        }

        protected void Exhaust(ISubPool subPool)
        {
            foreach (var kvp in subPool.items)
            {
                kvp.Value.q = poolDef.items[kvp.Key].q;
                subPool.ReshuffleDeck();
            }
        }
        protected void ExhaustAll()
        {
            foreach (var id in items.Keys)
            {
                items[id].q = poolDef.items[id].q;
            }
        }
        public PoolItem GetNextItem(string path)
        {
            var subPool = (ISubPool)GetFile(path);
            if (!subPool.IsReadyToGenerate)
            {
                subPool.GenerateDeck();
            }
            var item = subPool.GetNextItem();
            if (item == null)
            {
                Exhaust(subPool);
                item = subPool.GetNextItem();
            }
            return item;
        }
        // public List<Item> GetNextItems(string path, int count)
        protected override File CopyFileNode(File node)
        {
            return ((ISubPool)node).Copy(items);
        }
    }

    public class PoolDefinition<SP> : FS<Pool> where SP : ISubPool
    {
        public Dictionary<int, PoolItem> items = new Dictionary<int, PoolItem>();
        public void RegisterItem(PoolItem item)
        {
            items[item.id] = item;
        }
        public void RegisterItems(IEnumerable<PoolItem> items)
        {
            foreach (var item in items)
            {
                this.items[item.id] = item;
            }
        }
        public void AddItemToPool(PoolItem item, string path)
        {
            var subPool = (ISubPool)GetFile(path);
            subPool.items.Add(item.id, item);
        }
        public void AddItemsToPool(IEnumerable<PoolItem> items, string path)
        {
            var subPool = (ISubPool)GetFile(path);
            foreach (var item in items)
                subPool.items.Add(item.id, item);
        }
    }
}