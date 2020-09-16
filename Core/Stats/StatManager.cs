
using Chains;
using Handle = Utils.MyLinkedList.MyListNode<Chains.IEvHandler>;
using Core.FS;
using System.Collections.Generic;
using System.Reflection;

namespace Core
{

    public class StatFileContainer : File
    {
        public Chain<StatEvent> chain;
        public StatFile file;

        public StatFileContainer(StatFile file)
        {
            this.chain = new Chain<StatEvent>();
            this.file = (StatFile)file.Copy();
        }

        public override File Copy()
        {
            var statNode = new StatFileContainer((StatFile)file.Copy());
            return statNode;
        }
    }

    public class StatFile : File
    {
        public virtual void _Add(StatFile f, int sign)
        {
            // let's do it the dumbest way so that it works
            // maybe I'll figure out a better solution later
            var type = f.GetType();
            if (type != this.GetType())
            {
                throw new System.Exception("Can't add files of different types");
            }
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                var oldVal = (int)field.GetValue(this);
                var addVal = (int)field.GetValue(f);
                var newVal = oldVal + sign * addVal;
                field.SetValue(this, newVal);
            }
        }
    }

    public class MapFile : StatFile
    {
        public Dictionary<int, int> content = new Dictionary<int, int>();

        public override void _Add(StatFile f, int sign)
        {
            // we assume it is the same type 
            var otherFile = (MapFile)f;
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public int this[int id]
        {
            get => content[id];
            set => content[id] = value;
        }
    }

    public class ArrayFile : StatFile
    {
        public List<int> content = new List<int>();

        public override void _Add(StatFile f, int sign)
        {
            // we assume it is the same type 
            var otherFile = (ArrayFile)f;
            var otherArray = otherFile.content;
            for (int i = 0; i < content.Count; i++)
            {
                content[i] += otherArray[i] * sign;
            }
        }

        public int this[int index]
        {
            get => content[index];
            set => content[index] = value;
        }
    }

    public class StatEvent : EventBase
    {
        public File file;
    }

    public class StatDir : Directory
    {
        public override File GetFile(string fileName)
        {
            var node = (StatFileContainer)nodes[fileName];
            var ev = new StatEvent { file = (File)node.file.Copy() };
            node.chain.Pass(ev);
            return ev.file;
        }
    }

    public class StatManager : FS<StatDir>
    {
        // contains either directories or files
        public static FS<Directory> DefaultFS = new FS<Directory>();
        public Dictionary<Modifier, int> m_modifierCounts
            = new Dictionary<Modifier, int>();

        public Dictionary<ChainModifier, Handle> m_chainModifierHandles
            = new Dictionary<ChainModifier, Handle>();

        // TODO: init with a base directory
        public StatManager()
        {
            CopyDirectoryStructure(DefaultFS.m_baseDir, m_baseDir);
        }

        public void AddStatModifier(StatModifier modifier)
        {
            if (m_modifierCounts.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
            }
            m_modifierCounts[modifier] = 1;
            StatFile file = (StatFile)GetFile(modifier.path);
            file._Add(modifier.file, 1);
        }

        public void RemoveStatModifier(StatModifier modifier)
        {
            m_modifierCounts[modifier]--;
            StatFile file = (StatFile)GetFile(modifier.path);
            file._Add(modifier.file, -1);
        }

        public void AddChainModifier(ChainModifier modifier)
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
                return;
            }
            var node = (StatFileContainer)GetNode(modifier.path);
            var handle = node.chain.AddHandler(modifier.handler);
            m_chainModifierHandles[modifier] = handle;
            m_modifierCounts[modifier] = 0;
        }

        public void RemoveChainModifier(ChainModifier modifier)
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                var val = m_modifierCounts[modifier] - 1;
                m_modifierCounts[modifier] = val;
                if (val > 0)
                    return;
            }
            var node = (StatFileContainer)GetNode(modifier.path);
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

        protected override File CopyFileNode(File node)
        {
            return new StatFileContainer((StatFile)node.Copy());
        }
    }
}