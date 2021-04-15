using Hopper.Utils.Chains;
using Hopper.Utils.FS;
using System.Collections.Generic;
using Hopper.Core.Components;

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
    public class StatFS : FS<File>
    {
    }

    public partial class Stats : IComponent
    {
        // contains either directories or files
        private Dictionary<Modifier, int> m_modifierCounts
            = new Dictionary<Modifier, int>();
        private Dictionary<Modifier, Handle> m_chainModifierHandles
            = new Dictionary<Modifier, Handle>();
        private StatFS m_fs = new StatFS();

        public Stats(PatchArea patchArea = null)
        {
            m_chainModifierHandles = new Dictionary<Modifier, Handle>();
            m_modifierCounts = new Dictionary<Modifier, int>();
            m_fs = new StatFS();
            m_patchArea = patchArea;
        }

        public Stats(DefaultStats defaultStats)
        {
            m_chainModifierHandles = new Dictionary<Modifier, Handle>();
            m_modifierCounts = new Dictionary<Modifier, int>();
            m_fs = new StatFS();
            m_fs.BaseDir.CopyDirectoryStructureFrom(defaultStats.statManager.m_fs.BaseDir);
            m_patchArea = defaultStats.PatchArea;
        }

        public void AddStatModifier<T>(StatModifier<T> modifier) where T : File, IAddableWith<T>
        {
            if (m_modifierCounts.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
            }
            else
            {
                GetLazy<T>(modifier.path);
                m_modifierCounts[modifier] = 1;
            }
            var node = (StatFileContainer<T>)m_fs.GetNode(modifier.path.String);
            node.file._Add(modifier.file, 1);
        }

        public void RemoveStatModifier<T>(StatModifier<T> modifier) where T : File, IAddableWith<T>
        {
            m_modifierCounts[modifier]--;
            var node = (StatFileContainer<T>)m_fs.GetNode(modifier.path.String);
            node.file._Add(modifier.file, -1);
        }

        public void AddChainModifier<T>(ChainModifier<T> modifier) where T : File
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
            }
            else
            {
                // lazy load
                GetLazy<T>(modifier.path);
                var node = (StatFileContainer<T>)m_fs.GetNode(modifier.path.String);
                var handle = node.chain.AddHandler(modifier.handler);
                m_chainModifierHandles[modifier] = handle;
                m_modifierCounts[modifier] = 1;
            }
        }

        public void RemoveChainModifier<T>(ChainModifier<T> modifier) where T : File
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                int val = m_modifierCounts[modifier] - 1;
                m_modifierCounts[modifier] = val;
                if (val > 0)
                    return;
            }
            var node = (StatFileContainer<T>)m_fs.GetNode(modifier.path.String);
            var handle = m_chainModifierHandles[modifier];
            node.chain.RemoveHandler(handle);
        }

        public T GetLazy<T>(IStatPath<T> statPath) where T : File
        {
            var defaultValue = statPath.GetDefault(m_patchArea);
            return GetLazy(statPath.String, defaultValue);
        }

        public T GetLazy<T>(string path, T defaultFile) where T : File
        {
            var defaultValue = new StatFileContainer<T>(defaultFile);
            var statFile = (StatFileContainer<T>)m_fs.GetFileLazy(path, defaultValue);
            var file = statFile.Retrieve();
            return file;
        }

        public T GetUnsafe<T>(string path) where T : File
        {
            var statFile = (StatFileContainer<T>)m_fs.GetFile(path);
            var file = statFile.Retrieve();
            return file;
        }

        // This one is probably going to be used just for initial setup
        // The question of attributing default, entity-specific stats is still open
        public T GetRawLazy<T>(IStatPath<T> statPath) where T : File
        {
            var defaultValue = statPath.GetDefault(m_patchArea);
            return GetRawLazy(statPath.String, defaultValue);
        }

        public T GetRawLazy<T>(string path, T defaultFile) where T : File
        {
            var defaultValue = new StatFileContainer<T>(defaultFile);
            var statFile = (StatFileContainer<T>)m_fs.GetFileLazy(path, defaultValue);
            return statFile.file;
        }
    }
}