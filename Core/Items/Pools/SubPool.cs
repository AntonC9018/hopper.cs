using System;
using System.Collections.Generic;
using Hopper.Core.FS;
using Hopper.Utils;

namespace Hopper.Core.Items
{
    // public interface ISubPool
    // {
    //     bool IsReadyToGenerate { get; }
    //     Dictionary<int, PoolItem> items { get; }
    //     ISubPool Copy(Dictionary<int, PoolItem> _items);
    //     void GenerateDeck();
    //     PoolItem GetNextItem();
    //     void ReshuffleDeck();
    // }

    public abstract class SubPool : File
    {
        public HashSet<PoolItem> items = new HashSet<PoolItem>();
        public List<PoolItem> deck;
        public int index;

        public bool IsReadyToGenerate { get => deck != null; }

        public void GenerateDeck(Random rng)
        {
            deck = new List<PoolItem>();
            index = 0;
            foreach (var item in items)
            {
                for (int i = 0; i < item.quantity; i++)
                {
                    deck.Add(item);
                }
            }
            deck.Shuffle(rng);
        }

        public void ReshuffleDeck(Random rng)
        {
            deck.Shuffle(rng);
            index = 0;
        }

        public abstract PoolItem GetNextItem(Random rng);
    }
}