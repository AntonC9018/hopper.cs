using Hopper.Utils.Chains;
using Hopper.Utils.FS;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Stat
{
    public interface IStat 
    {
        IStat Copy();
    }

    public interface IHolder {};

    public class Holder<T> : IHolder where T : struct
    {
        public T item;
        public Holder(T item)
        {
            this.item = item;
        }
    }

    public class StatsBuilder : Dictionary<Identifier, IStat>
    {
        public StatsBuilder() : base()
        {
        }

        public StatsBuilder(IDictionary<Identifier, IStat> dictionary) : base(dictionary)
        {
        }

        public void Add<T>(Index<T> index, T stat) where T : IStat
        {
            Add(index.Id, stat);
        }

        public bool TryGet<T>(Index<T> index, out T stat) where T : IStat
        {
            if (TryGetValue(index.Id, out var t))
            {
                stat = (T) t;
                return true;   
            }
            stat = default;
            return false;
        }

        public T Get<T>(Index<T> index) where T : IStat
        {
            return (T) this[index.Id];
        }
    }

    public partial class Stats : IComponent
    {
        [Inject] public StatsBuilder template;
        public Dictionary<Identifier, IHolder> store = new Dictionary<Identifier, IHolder>();

        public T GetLazy<T>(Index<T> index) where T : struct, IStat
        {
            GetLazy(index, out var stat);
            return stat;
        }

        public void GetLazy<T>(Index<T> index, out T stat) where T : struct, IStat 
        {
            if (!store.ContainsKey(index.Id))
            {   
                if (!template.TryGet(index, out stat))
                {
                    stat = Registry.Global._defaultStats.Get(index);
                }
                Set(index, in stat);
            }
            else
            {
                Get(index, out stat);
            }
        }

        public ref T GetRaw<T>(Index<T> index) where T : struct, IStat
        {
            Assert.That(store.ContainsKey(index.Id), $"{index} stat not found in the dictionary");
            return ref ((Holder<T>) store[index.Id]).item;
        }

        public void Get<T>(Index<T> index, out T stat) where T : struct, IStat
        {
            Assert.That(store.ContainsKey(index.Id), $"{index} stat not found in the dictionary");
            stat = ((Holder<T>) store[index.Id]).item;
        }

        public T Get<T>(Index<T> index) where T : struct, IStat
        {
            Assert.That(store.ContainsKey(index.Id), $"{index} stat not found in the dictionary");
            return ((Holder<T>) store[index.Id]).item;
        }

        public ref T Set<T>(Index<T> index, in T stat) where T : struct, IStat
        {
            var holder = new Holder<T>(stat);
            store[index.Id] = holder;
            return ref holder.item;
        }

        public ref T GetRawLazy<T>(Index<T> index) where T : struct, IStat
        {
            if (!store.ContainsKey(index.Id))
            {   
                if (!template.TryGet(index, out var stat))
                {
                    stat = Registry.Global._defaultStats.Get(index);
                }
                Set(index, in stat);
            }
            return ref GetRaw(index);
        }
    }
}