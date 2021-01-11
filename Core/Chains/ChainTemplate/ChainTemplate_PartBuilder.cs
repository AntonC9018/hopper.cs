
using System;
using Hopper.Core.Behaviors;
using Hopper.Utils.Chains;

namespace Hopper.Core.Chains
{
    public interface I_ChainTemplate_PartBuilder
    {
        IChainTemplate Template { get; }
    }
    public class ChainTemplate_PartBuilder<Event> : I_ChainTemplate_PartBuilder where Event : EventBase
    {
        private ChainTemplate<Event> m_template;
        public IChainTemplate Template { get => m_template.Clone(); }
        private ChainTemplateBuilder builder;

        public ChainTemplate_PartBuilder(ChainTemplateBuilder builder = null)
        {
            this.builder = builder;
            m_template = new ChainTemplate<Event>();
        }

        public ChainTemplate_PartBuilder<Event> AddHandler(Handler<Event> handler)
        {
            m_template.AddHandler(handler);
            return this;
        }

        public ChainTemplate_PartBuilder<Event> AddHandler(Action<Event> handler)
        {
            m_template.AddHandler(handler);
            return this;
        }

        public ChainTemplate_PartBuilder<Event> AddHandler(Action<Event> handlerFunc,
            PriorityRank priority = PriorityRank.Default)
        {
            m_template.AddHandler(handlerFunc, priority);
            return this;
        }

        public ChainTemplate_PartBuilder<Event> AddHandler(System.Action<Event> handlerFunc, int priority)
        {
            m_template.AddHandler(handlerFunc, priority);
            return this;
        }

        public ChainTemplateBuilder End()
        {
            return builder;
        }

        public ChainTemplate_PartBuilder<T> AddTemplate<T>(ChainName name)
            where T : EventBase
        {
            return builder.AddTemplate<T>(name);
        }
    }
}