
using Chains;
using Core.FS;
using System.Collections.Generic;

namespace Core.Stats
{

    public class StatManager : FS<StatDir>
    {
        // contains either directories or files
        public Dictionary<Modifier, int> m_modifierCounts
            = new Dictionary<Modifier, int>();
        public Dictionary<ChainModifier, Handle> m_chainModifierHandles
            = new Dictionary<ChainModifier, Handle>();

        public void AddStatModifier(StatModifier modifier)
        {
            if (m_modifierCounts.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
            }
            else
            {
                modifier.path.Path(this);
                m_modifierCounts[modifier] = 1;
            }
            var node = (StatFileContainer)GetNode(modifier.path.String);
            node.file._Add(modifier.file, 1);
        }

        public void RemoveStatModifier(StatModifier modifier)
        {
            m_modifierCounts[modifier]--;
            var node = (StatFileContainer)GetNode(modifier.path.String);
            node.file._Add(modifier.file, -1);
        }

        public void AddChainModifier(ChainModifier modifier)
        {
            if (m_chainModifierHandles.ContainsKey(modifier))
            {
                m_modifierCounts[modifier]++;
            }
            else
            {
                // lazy load
                modifier.path.Path(this);
                var node = (StatFileContainer)GetNode(modifier.path.String);
                var handle = node.chain.AddHandler(modifier.handler);
                m_chainModifierHandles[modifier] = handle;
                m_modifierCounts[modifier] = 1;
            }
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
            var node = (StatFileContainer)GetNode(modifier.path.String);
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

        public T Get<T>(IStatPath<T> statPath) where T : StatFile, new()
        {
            return statPath.Path(this);
        }
    }
}