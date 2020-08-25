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

    public class SubPool : File
    {
        public Dictionary<int, Item> items
            = new Dictionary<int, Item>();
        public Random rng = new Random();
        public List<Item> deck;
        public int index;

        public void GenerateList()
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

        public Item GetNextItem()
        {
            if (index == deck.Count - 1)
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

        public SubPool Copy(Dictionary<int, Item> items)
        {
            var sp = new SubPool();
            foreach (var (id, item) in this.items)
            {
                sp.items.Add(id, items[id]);
            }
            return sp;
        }
    }

    public class Pool : Directory
    {
    }

    public class SuperPool : FS<Pool>
    {
        public PoolDefinition poolDef;
        public Dictionary<int, Item> items;

        public SuperPool(PoolDefinition poolDef)
        {
            this.poolDef = poolDef;
            foreach (var (id, item) in poolDef.items)
            {
                items.Add(id, item.Copy());
            }
            CopyDirectoryStructure(poolDef.m_baseDir, this.m_baseDir);
        }

        protected void Exhaust(SubPool subPool)
        {
            foreach (var (id, item) in subPool.items)
            {
                item.q = poolDef.items[id].q;
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
            var subPool = (SubPool)GetFile(path);
            var item = subPool.GetNextItem();
            if (item == null)
            {
                Exhaust(subPool);
                item = subPool.GetNextItem();
            }
            return item;
        }
        protected override File CopyFileNode(File node)
        {
            return ((SubPool)node).Copy(items);
        }
    }

    public class PoolDefinition : FS<Pool>
    {
        public Dictionary<int, Item> items;
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
            var subPool = (SubPool)GetFile(path);
            subPool.items.Add(item.id, item);
        }
        public void AddItemsToPool(IEnumerable<Item> items, string path)
        {
            var subPool = (SubPool)GetFile(path);
            foreach (var item in items)
                subPool.items.Add(item.id, item);
        }
    }
}