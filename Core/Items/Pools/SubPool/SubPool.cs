using System;
using System.Collections.Generic;
using Hopper.Utils.FS;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    public abstract class SubPool : File
    {
        public HashSet<PoolItem> items = new HashSet<PoolItem>();
        public PoolItem[] deck = null;
        public int index = 0;

        public void InitializeDeck(Random rng)
        {
            CreateDeckArray();
            GenerateDeck(rng);
            ReshuffleDeck(rng);
        }

        protected void CreateDeckArray()
        {
            int sum = 0;
            foreach (var item in items)
            {
                sum += item.quantity;
            }

            Assert.AreNotEqual(0, sum, "Subpool cannot be empty");

            deck = new PoolItem[sum];
        }

        protected void GenerateDeck(Random rng)
        {
            int j = 0;
            foreach (var item in items)
            {
                for (int i = 0; i < item.quantity; i++)
                {
                    deck[j++] = item;
                }
            }
        }

        public void ReshuffleDeck(Random rng)
        {
            index = 0;
            deck.Shuffle(rng);
        }

        public abstract PoolItem GetNextItem(Random rng);
    }
}