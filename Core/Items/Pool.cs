using System.Collections.Generic;
using System.Linq;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public interface IPool
    {
        Identifier DrawFrom(Identifier poolId);
    }

    // I have decided to do a really simple less than pool based on tables.
    // The deck pool which I had before is wacky.
    // There is another, more advanced option that I could think of, based on binary trees.
    // It would allow for O(log N) insertion, removal. 
    // Whereas this one is O(N + M) removal but O(1 + M) insertion (N = num_items, M = num_subpools).
    // I will explore the binary tree version if I have time later and when the item pool 
    // becomes really clunky, until then it's not worth the time.
    public struct PoolItem
    {
        public int amount;
        public int weight;

        public PoolItem(int amount, int weight)
        {
            this.amount = amount;
            this.weight = weight;
        }

        public int GetCost() => amount * weight;
    }
    
    public class SubPool : Dictionary<Identifier, PoolItem>
    {
        public int sum;

        public SubPool(Dictionary<Identifier, PoolItem> items) : base(items)
        {
            this.sum = Values.Sum(it => it.GetCost());
        }

        public SubPool(SubPool other) : base(other)
        {
            this.sum = other.sum;
        }

        public SubPool(int capacity) : base(capacity)
        {
        }

        public SubPool() : base()
        {
        }

        public void AdjustAmount(Identifier itemIdentifier, int amount)
        {
            if (TryGetValue(itemIdentifier, out var item))
            {
                item.amount += amount;
                this.sum += amount * item.weight;
                this[itemIdentifier] = item;
            }
        }

        public void AdjustWeight(Identifier itemIdentifier, int weight)
        {
            if (TryGetValue(itemIdentifier, out var item))
            {
                item.weight += weight;
                sum += item.amount * weight;
                this[itemIdentifier] = item;
            }
        }

        public void Add(Identifier identifier, int amount, int weight)
        {
            Add(identifier, new PoolItem(amount, weight));
        }

        public new void Add(Identifier identifier, PoolItem item)
        {
            Assert.That(!ContainsKey(identifier), "Cannot add the same item multiple times");
            sum += item.GetCost();
            this[identifier] = item;
        }

        public bool IsExhausted()
        {
            return this.sum == 0;
        }

        public Identifier Draw(double roll)
        {
            Assert.That(!IsExhausted(), "Cannot draw from an empty or exhausted pool.");
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
    
    public class Pool : Dictionary<Identifier, SubPool>, IPool
    {
        public Pool templatePool;
        public System.Random rng; // let's have just one rng for now
        
        public Pool(Pool templatePool)
        {
            this.templatePool = templatePool;

            foreach (var kvp in templatePool) 
                this[kvp.Key] = new SubPool(kvp.Value);

            this.rng = new System.Random(1);
        }

        public Pool() : base()
        {
            this.rng = null;
            this.templatePool = null;
        }

        public Identifier DrawFrom(Identifier poolId)
        {
            var pool   = this[poolId];
            var roll   = rng.NextDouble();
            var itemId = pool.Draw(roll);

            foreach (var subpool in Values)
            {
                subpool.AdjustAmount(itemId, -1);
            }

            MaybeRefresh();

            return itemId;
        } 

        // Refreshes subpools at most once
        public void MaybeRefresh()
        {
            foreach (var kvp in this)
            {
                if (kvp.Value.IsExhausted())
                {
                    var reference = templatePool[kvp.Key];

                    foreach (var subpool in Values)
                    foreach (var item    in reference)
                    {
                        subpool.AdjustAmount(item.Key, item.Value.amount);
                    }

                    break;
                }
            }
        }
    }
}