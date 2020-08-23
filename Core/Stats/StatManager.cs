
using Chains;
using Handle = MyLinkedList.MyListNode<Chains.WeightedEventHandler>;
using Core.FS;
using System.Collections.Generic;

namespace Core
{
    public class StatNode : Node
    {
        public Chain chain;
        public File file;
    }

    public class StatEvent : EventBase
    {
        public File file;
    }

    public class StatDir : Directory
    {
        public override File GetFile(string fileName)
        {
            var node = (StatNode)nodes[fileName];
            var ev = new StatEvent { file = node.file };
            node.chain.Pass(ev);
            return ev.file;
        }
    }

    public class StatManager : FS<StatDir>
    {
        // contains either directories or files
        public static FS<Directory> s_defaultFS = new FS<Directory>();
        public Dictionary<StatModifier, int> m_statModifierCounts
            = new Dictionary<StatModifier, int>();

        public Dictionary<ChainModifier, Handle> m_chainModifierHandles
            = new Dictionary<ChainModifier, Handle>();

        // TODO: init with a base directory
        public StatManager()
        {
            CopyDirectoryStructure(s_defaultFS.m_baseDir, m_baseDir);
        }

        public void AddStatModifier(StatModifier modifier)
        {
            if (m_statModifierCounts.ContainsKey(modifier))
            {
                m_statModifierCounts[modifier]++;
            }
            m_statModifierCounts[modifier] = 1;
            File file = GetFile(modifier.path);
            file._Add(modifier.file, 1);
        }

        public void RemoveStatModifier(StatModifier modifier)
        {
            m_statModifierCounts[modifier]--;
            File file = GetFile(modifier.path);
            file._Add(modifier.file, -1);
        }

        public void AddChainModifier(ChainModifier modifier)
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                throw new System.Exception("Can't add more than one of the same chain modifiers");
            }
            var node = (StatNode)GetNode(modifier.path);
            var handle = node.chain.AddHandler(modifier.handler);
            m_chainModifierHandles[modifier] = handle;
        }

        public void RemoveChainModifier(ChainModifier modifier)
        {
            var node = (StatNode)GetNode(modifier.path);
            var handle = m_chainModifierHandles[modifier];
            node.chain.RemoveHandler(handle);
        }

        public void AddModifier(Modifier modifier)
        {
            if (modifier is StatModifier)
                AddStatModifier((StatModifier)modifier);

            else if (modifier is ChainModifier)
                AddChainModifier((ChainModifier)modifier);
        }

        public void RemoveModifier(Modifier modifier)
        {
            if (modifier is StatModifier)
                RemoveStatModifier((StatModifier)modifier);

            else if (modifier is ChainModifier)
                RemoveChainModifier((ChainModifier)modifier);
        }

        // TODO: lazy load
        void CopyDirectoryStructure(Directory from, StatDir to)
        {
            foreach (var (name, node) in from.nodes)
            {
                if (node is Directory)
                {
                    var subdir = new StatDir();
                    to.nodes.Add(name, subdir);
                    CopyDirectoryStructure((Directory)node, subdir);
                }
                else if (node is File)
                {
                    var file = new StatNode();
                    file.chain = new Chain();
                    file.file = ((File)node).Copy();
                    to.nodes.Add(name, file);
                }
            }
        }
    }
}