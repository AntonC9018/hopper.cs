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
    
    public class LootSubTable : Dictionary<Identifier, LootTableItem>
    {
        public int sum;

        public LootSubTable(Dictionary<Identifier, LootTableItem> items) : base(items)
        {
            this.sum = Values.Sum(it => it.GetCost());
        }

        public LootSubTable(LootSubTable other) : base(other)
        {
            this.sum = other.sum;
        }

        public LootSubTable()
        {
        }

        public LootSubTable(int capacity) : base(capacity)
        {
        }

        public void AdjustWeight(Identifier itemIdentifier, int weight)
        {
            if (TryGetValue(itemIdentifier, out var item))
            {
                item.weight += weight;
                sum += weight;
                this[itemIdentifier] = item;
            }
        }

        public void Add(Identifier identifier, int weight)
        {
            Add(identifier, new LootTableItem(weight));
        }

        public new void Add(Identifier identifier, LootTableItem item)
        {
            Assert.That(!ContainsKey(identifier), "Cannot add the same item multiple times");
            sum += item.GetCost();
            this[identifier] = item;
        }

        public bool IsEmpty()
        {
            return this.sum == 0;
        }

        public Identifier Draw(double roll)
        {
            int rolledSum = (int)(sum * roll) + 1;
            
            foreach (var kvp in this)
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