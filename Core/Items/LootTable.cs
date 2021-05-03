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
    
    public class LootTable : Dictionary<Identifier, LootTableItem>
    {
        public int sum;

        public LootTable(Dictionary<Identifier, LootTableItem> items) : base(items)
        {
            this.sum = Values.Sum(it => it.GetCost());
        }

        public LootTable(LootTable other) : base(other)
        {
            this.sum = other.sum;
        }

        public LootTable() : base()
        {
        }

        public LootTable(int capacity) : base(capacity)
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
            Assert.That(!IsEmpty(), "Cannot draw from an empty pool.");
            Assert.That(roll >= 0 && roll < 1, "Roll must be bound between 0 and 1");

            int rolledSum = (int)(sum * roll) + 1;
            
            foreach (var kvp in this)
            {
                rolledSum -= kvp.Value.GetCost();

                if (rolledSum <= 0) return kvp.Key;
            }

            return default;
        }
    }

    public class LootTablesWrapper : Dictionary<Identifier, LootTable>, IPool
    {
        public System.Random rng; // let's have just one rng for now

        public LootTablesWrapper(LootTablesWrapper templatePool) : base(templatePool)
        {
            this.rng = new System.Random(1);
        }

        public LootTablesWrapper() : base()
        {
            this.rng = new System.Random(1);
        }

        public Identifier DrawFrom(Identifier poolId)
        {
            var pool = this[poolId];
            var roll = rng.NextDouble();
            return pool.Draw(roll);
        }
    }
}