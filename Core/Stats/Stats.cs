using Hopper.Utils.Chains;
using Hopper.Utils.FS;
using System.Collections.Generic;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Stats
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
    public struct StatWrapper<T>
    {
        Index<T> Index;
        System.Action<T> Default;
    }

    public struct StatPath<T> : IPath
    {
        public System.Func<Entity, T> Stat;
    }

    public interface IStat 
    {
        IStat Copy();
    }

    public partial class Stats : IComponent
    {
        [Inject] public Stats defaultStats;
        public Dictionary<Identifier, IStat> store;

        public void Init()
        {
            store = new Dictionary<Identifier, IStat>();
        }

        public T GetLazy<T>(Index<T> index) where T : IStat
        {
            if (!store.ContainsKey(index.Id))
            {   
                Set(index, (T) defaultStats.GetLazy(index).Copy());
            }
            return Get(index);
        }

        public T GetRaw<T>(Index<T> index) where T : IStat
        {
            return (T) store[index.Id];
        }

        public T Get<T>(Index<T> index) where T : IStat
        {
            // TODO: chain iteration
            return (T) GetRaw(index).Copy();
        }

        public void Set<T>(Index<T> index, T stat) where T : IStat
        {
            store[index.Id] = stat;
        }

        public T GetRawLazy<T>(Index<T> index) where T : IStat
        {
            if (!store.ContainsKey(index.Id))
            {   
                Set(index, (T) defaultStats.GetLazy(index).Copy());
            }
            return GetRaw(index);
        }
    }
}