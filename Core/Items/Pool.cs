using System.Collections.Generic;
using System.Linq;
using Hopper.Utils;

namespace Hopper.Core.Items
{
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
    
    public class SubPool
    {
        public Dictionary<Identifier, PoolItem> items;
        public int sum;

        public SubPool(Dictionary<Identifier, PoolItem> items)
        {
            this.items = items;
            this.sum = this.items.Values.Sum(it => it.amount * it.weight);
        }

        public SubPool(SubPool other)
        {
            this.items = new Dictionary<Identifier, PoolItem>(other.items);
            this.sum = other.sum;
        }

        public void AdjustAmount(Identifier itemIdentifier, int amount)
        {
            if (items.TryGetValue(itemIdentifier, out var item))
            {
                item.amount += amount;
                this.sum += amount * item.weight;
                items[itemIdentifier] = item;
            }
        }

        public void AdjustWeight(Identifier itemIdentifier, int weight)
        {
            if (items.TryGetValue(itemIdentifier, out var item))
            {
                item.weight += weight;
                sum += item.amount * weight;
                items[itemIdentifier] = item;
            }
        }

        public void Add(Identifier identifier, int amount, int weight)
        {
            Add(identifier, new PoolItem(amount, weight));
        }

        public void Add(Identifier identifier, PoolItem item)
        {
            Assert.That(!items.ContainsKey(identifier), "Cannot add the same item multiple times");
            sum += item.GetCost();
            items[identifier] = item;
        }

        public bool IsExhausted()
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
    
    public class Pool
    {
        public Pool templatePool;
        public Dictionary<Identifier, SubPool> subPools;
        public System.Random rng; // let's have just one rng for now
        
        public Pool(Pool templatePool)
        {
            this.templatePool = templatePool;

            this.subPools = new Dictionary<Identifier, SubPool>(templatePool.subPools.Count);
            foreach (var kvp in templatePool.subPools) 
                this.subPools[kvp.Key] = new SubPool(kvp.Value);

            this.rng = new System.Random(1);
        }

        public Pool()
        {
            this.rng = null;
            this.templatePool = null;
            
            this.subPools = new Dictionary<Identifier, SubPool>();
        }

        public void Add(Identifier poolId, SubPool subpool)
        {
            subPools[poolId] = subpool;
        }

        public Identifier DrawFrom(Identifier poolId)
        {
            var pool   = subPools[poolId];
            var roll   = rng.NextDouble();
            var itemId = pool.Draw(roll);

            foreach (var subpool in subPools.Values)
            {
                subpool.AdjustAmount(itemId, 1);
            }

            MaybeRefresh();

            return itemId;
        } 

        // Refreshes subpools at most once
        public void MaybeRefresh()
        {
            foreach (var kvp in subPools)
            {
                if (kvp.Value.IsExhausted())
                {
                    var reference = templatePool.subPools[kvp.Key];

                    foreach (var subpool in subPools.Values)
                    foreach (var item    in reference.items)
                    {
                        subpool.AdjustAmount(item.Key, item.Value.amount);
                    }

                    break;
                }
            }
        }
    }
}