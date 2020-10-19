using System;
using System.Collections.Generic;
using Core.FS;
using Utils;

namespace Core.Items
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
        public Dictionary<int, PoolItem> items = new Dictionary<int, PoolItem>();
        public Random rng = new Random();
        public List<PoolItem> deck;
        public int index;

        public bool IsReadyToGenerate { get => deck != null; }

        public void GenerateDeck()
        {
            deck = new List<PoolItem>();
            index = 0;
            foreach (var item in items.Values)
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

        public abstract SubPool Copy(Dictionary<int, PoolItem> _items);
        public abstract PoolItem GetNextItem();
    }
}