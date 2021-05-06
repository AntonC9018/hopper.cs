using Hopper.Utils.Chains;
using Hopper.Utils.FS;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using Hopper.Utils;
using Hopper.Core.WorldNS;

namespace Hopper.Core.Stat
{
    /* 
        The way stats work currently is to me a bit complicated.
        So, we have static `Path` objects, which contain string paths to stat objects.
        We store the files by strings, in a corresponding file system.
        For example, "atk/res" would be stored in this file system in the folder "atk" and
        the file "res" would contain the file with the Attack Resistance.
        Even though it is a dictionary, converting a json into stats is complicated, since it would involve
        somehow looking up the stat classes in order to instantiate the correct stats, or modifying the
        default stats to convert them only on lazy loading, while storing them in default stats in dictionaries.

        I have another solution, that would be more logical.
        Since we really need the path string for debugging and at setup, to load json files and then immediately
        convert them into stat objects, I propose the following:
        1. Assign each stat path (class) an id.
        2. Store stats in StatManager by these id's in an array or dictionary.
        3. Store stat paths in a string -> path dictionary. This would help with converting json into stats.

        The main thing is that it would simplify the access to stats at runtime
        (one level deep dictionary, just int lookups) while also simplifying the json conversion.

        E.g. this json:
        { "atk": { "base": { "damage": 1, "source": 0 } } }
        The path in this case would be looked up in the dictionary as "atk/base".
        The path contains the stat class information and so it would be easy to convert the json having the type.
        Having the path, we know the integer index / key in the stat manager for this stat.
        
        Now to groups of sources. This is not currently implemented. The idea is to target multiple stat sources.
        E.g. `fire attack sources` would map to e.g. fireball attack source, laser attack source, 
        hot coals attack source, lava attack source — whatever new fire source is added by mods.
        This will be defined by mods themselves. The indices for these groups will be stored on patch area.
        
        The method of storing stats doesn't affect how difficult this last task is.

        UPDATE:
        Now, that I've setup code generation, creating classes for each of the stats is going to
        be made trivially easy. 
    */

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

    public partial class Stats : IComponent
    {
        [Inject] public Dictionary<Identifier, IStat> defaultStats;
        public Dictionary<Identifier, IHolder> store;

        public void InitInWorld(Transform transform)
        {
            store = new Dictionary<Identifier, IHolder>();
        }

        public void GetLazy<T>(Index<T> index, out T stat) where T : struct, IStat 
        {
            if (!store.ContainsKey(index.Id))
            {   
                stat = (T) defaultStats[index.Id];
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
            return ref ((Holder<T>)store[index.Id]).item;
        }

        public void Get<T>(Index<T> index, out T stat) where T : struct, IStat
        {
            Assert.That(store.ContainsKey(index.Id), $"{index} stat not found in the dictionary");
            stat = ((Holder<T>)store[index.Id]).item;
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
                Set(index, (T) defaultStats[index.Id]);
            }
            return ref GetRaw(index);
        }
    }
}