using System.Collections.Generic;
using System.Linq;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public struct LootTableItem
    {
        public int weight;

        public LootTableItem(int weight)
        {
            this.weight = weight;
        }

        public int GetCost() => weight;
    }
    
    public class LootSubTable
    {
        public Dictionary<Identifier, LootTableItem> items;
        public int sum;

        public LootSubTable(Dictionary<Identifier, LootTableItem> items)
        {
            this.items = items;
            this.sum = this.items.Values.Sum(it => it.GetCost());
        }

        public LootSubTable(LootSubTable other)
        {
            this.items = new Dictionary<Identifier, LootTableItem>(other.items);
            this.sum = other.sum;
        }

        public void AdjustWeight(Identifier itemIdentifier, int weight)
        {
            if (items.TryGetValue(itemIdentifier, out var item))
            {
                item.weight += weight;
                sum += weight;
                items[itemIdentifier] = item;
            }
        }

        public void Add(Identifier identifier, int weight)
        {
            Add(identifier, new LootTableItem(weight));
        }

        public void Add(Identifier identifier, LootTableItem item)
        {
            Assert.That(!items.ContainsKey(identifier), "Cannot add the same item multiple times");
            sum += item.GetCost();
            items[identifier] = item;
        }

        public bool IsEmpty()
        {
            return this.sum == 0;
        }

        public Identifier Draw(double roll)
        {
            int rolledSum = (int)(sum * roll);
            
            foreach (var kvp in items)
            {
                rolledSum -= kvp.Value.GetCost();

                if (rolledSum <= 0) return kvp.Key;
            }

            throw new System.Exception("The pool has been exhausted");
        }
    }

    public interface IPool
    {
        Identifier DrawFrom(Identifier poolId);
    }

    public class LootTable : IPool
    {
        public LootTable templatePool;
        public Dictionary<Identifier, LootSubTable> subTables;
        public System.Random rng; // let's have just one rng for now

        public LootTable(LootTable templatePool)
        {
            this.templatePool = templatePool;

            this.subTables = new Dictionary<Identifier, LootSubTable>(templatePool.subTables.Count);
            foreach (var kvp in templatePool.subTables)
                this.subTables[kvp.Key] = new LootSubTable(kvp.Value);

            this.rng = new System.Random(1);
        }

        public LootTable()
        {
            this.rng = null;
            this.templatePool = null;

            this.subTables = new Dictionary<Identifier, LootSubTable>();
        }

        public void Add(Identifier poolId, LootSubTable subTable)
        {
            subTables[poolId] = subTable;
        }

        public Identifier DrawFrom(Identifier poolId)
        {
            var pool = subTables[poolId];
            var roll = rng.NextDouble();
            return pool.Draw(roll);
        }
    }
}