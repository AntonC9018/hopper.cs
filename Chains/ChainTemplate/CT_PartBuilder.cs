
using Core.Behaviors;

namespace Chains
{
    public interface I_CT_PartBuilder
    {
        ChainTemplate Template { get; }
    }
    public class CT_PartBuilder<Event> : I_CT_PartBuilder where Event : EventBase
    {
        private ChainTemplate<Event> m_template;
        public ChainTemplate Template { get => m_template.Clone(); }
        private ChainTemplateBuilder builder;

        public CT_PartBuilder(ChainTemplateBuilder builder = null)
        {
            this.builder = builder;
            m_template = new ChainTemplate<Event>();
        }

        public CT_PartBuilder<Event> AddHandler(EvHandler<Event> handler)
        {
            m_template.AddHandler(handler);
            return this;
        }

        public CT_PartBuilder<Event> AddHandler(
            System.Action<Event> handlerFunc,
            PriorityRanks priority = PriorityRanks.Default)
        {
            m_template.AddHandler(handlerFunc, priority);
            return this;
        }

        public ChainTemplateBuilder End()
        {
            return builder;
        }

        public CT_PartBuilder<T> AddTemplate<T>(ChainName name)
            where T : EventBase
        {
            return builder.AddTemplate<T>(name);
        }
    }
}