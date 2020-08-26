using System;
using System.Collections.Generic;
using Core.FS;

namespace Core.Items
{
    public class Item
    {
        public int id;
        public int q;

        public Item(int id, int q)
        {
            this.id = id;
            this.q = q;
        }

        public Item Copy()
        {
            return (Item)this.MemberwiseClone();
        }
    }

    public abstract class ISubPool : File
    {
        public Dictionary<int, Item> items
            = new Dictionary<int, Item>();
        public Random rng = new Random();
        public List<Item> deck;
        public int index;

        public bool IsReadyToGenerate { get => deck != null; }

        public void GenerateDeck()
        {
            deck = new List<Item>();
            index = 0;
            foreach (var (id, item) in items)
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
        public abstract ISubPool Copy(Dictionary<int, Item> items);
        public abstract Item GetNextItem();
    }

    public class SubPool : ISubPool
    {
        public override ISubPool Copy(Dictionary<int, Item> items)
        {
            var sp = new SubPool();
            foreach (var (id, item) in this.items)
            {
                sp.items.Add(id, items[id]);
            }
            return sp;
        }
        public override Item GetNextItem()
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
        public override ISubPool Copy(Dictionary<int, Item> items)
        {
            var sp = new EndlessSubPool();
            foreach (var (id, item) in this.items)
            {
                sp.items.Add(id, items[id]);
            }
            return sp;
        }
        public override Item GetNextItem()
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
        public Dictionary<int, Item> items = new Dictionary<int, Item>();

        public SuperPool(PoolDefinition<SP> poolDef)
        {
            this.poolDef = poolDef;
            foreach (var (id, item) in poolDef.items)
            {
                items.Add(id, item.Copy());
            }
            CopyDirectoryStructure(poolDef.m_baseDir, this.m_baseDir);
        }

        protected void Exhaust(ISubPool subPool)
        {
            foreach (var (id, item) in subPool.items)
            {
                item.q = poolDef.items[id].q;
                subPool.ReshuffleDeck();
            }
        }
        protected void ExhaustAll()
        {
            foreach (var (id, item) in items)
            {
                item.q = poolDef.items[id].q;
            }
        }
        public Item GetNextItem(string path)
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
        public Dictionary<int, Item> items = new Dictionary<int, Item>();
        public void RegisterItem(Item item)
        {
            items[item.id] = item;
        }
        public void RegisterItems(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                this.items[item.id] = item;
            }
        }
        public void AddItemToPool(Item item, string path)
        {
            var subPool = (ISubPool)GetFile(path);
            subPool.items.Add(item.id, item);
        }
        public void AddItemsToPool(IEnumerable<Item> items, string path)
        {
            var subPool = (ISubPool)GetFile(path);
            foreach (var item in items)
                subPool.items.Add(item.id, item);
        }
    }
}