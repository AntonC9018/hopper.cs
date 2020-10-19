using System.Collections.Generic;
using Core.FS;
using Utils;

namespace Core.Items
{

    public class Pool : Directory
    {
    }

    public interface ISuperPool
    {
        PoolItem GetNextItem(string path);
    }

    public class SuperPool<SP> : FS<Pool>, ISuperPool where SP : SubPool
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

        protected void Exhaust(SubPool subPool)
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
            var subPool = (SubPool)GetFile(path);
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

        protected override File CopyFileNode(File node)
        {
            return ((SubPool)node).Copy(items);
        }
    }
}