using Hopper.Utils.Chains;
using Hopper.Core.FS;
using System.Collections.Generic;
using Hopper.Core.Registry;

namespace Hopper.Core.Stats
{
    public class StatFS : FS<File>
    {
    }

    public class StatManager
    {
        // contains either directories or files
        private Dictionary<Modifier, int> m_modifierCounts
            = new Dictionary<Modifier, int>();
        private Dictionary<Modifier, Handle> m_chainModifierHandles
            = new Dictionary<Modifier, Handle>();
        private StatFS m_fs = new StatFS();
        public PatchArea m_patchArea;

        public StatManager(PatchArea patchArea = null)
        {
            m_chainModifierHandles = new Dictionary<Modifier, Handle>();
            m_modifierCounts = new Dictionary<Modifier, int>();
            m_fs = new StatFS();
            m_patchArea = patchArea;
        }

        public StatManager(DefaultStats defaultStats)
        {
            m_chainModifierHandles = new Dictionary<Modifier, Handle>();
            m_modifierCounts = new Dictionary<Modifier, int>();
            m_fs = new StatFS();
            m_fs.BaseDir.CopyDirectoryStructureFrom(defaultStats.statManager.m_fs.BaseDir);
            m_patchArea = defaultStats.Repository;
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
        public T GetRaw<T>(IStatPath<T> statPath) where T : File
        {
            var defaultValue = statPath.GetDefault(m_patchArea);
            return GetRaw(statPath.String, defaultValue);
        }

        public T GetRaw<T>(string path, T defaultFile) where T : File
        {
            var defaultValue = new StatFileContainer<T>(defaultFile);
            var statFile = (StatFileContainer<T>)m_fs.GetFileLazy(path, defaultValue);
            return statFile.file;
        }
    }
}