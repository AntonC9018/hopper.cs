using System.Collections.Generic;
using System.Linq;

namespace Chains
{
    public class ChainDefsBuilder
    {
        List<IChainDefBuilder> defs;
        public ChainDefBuilder<T> AddDef<T>(string name) where T : EventBase
        {
            var def = new ChainDefBuilder<T>(name);
            defs.Add(def);
            return def;
        }
        public ChainDef[] ToStatic()
        {
            return (ChainDef[])defs.Select(def => def.ToStatic());
        }
    }
    public struct ChainDef
    {
        public string name;
        public IEvHandler[] handlers;
    }
    public abstract class IChainDefBuilder
    {
        public string name;
        public List<IEvHandler> handlers;

        public ChainDef ToStatic()
        {
            return new ChainDef { name = name, handlers = handlers.ToArray() };
        }
    }
    public class ChainDefBuilder<Event> : IChainDefBuilder where Event : EventBase
    {
        public ChainDefBuilder(string name)
        {
            this.name = name;
            handlers = new List<IEvHandler>();
        }
        public void AddHandler(EvHandler<Event> handler)
        {
            handlers.Add(handler);
        }
    }

    public class ChainTemplateDefinition
    {
        public string name;
        IChainTemplate template;
        public IChainTemplate Template
        {
            get { return template.Clone(); }
            set { template = value; }
        }
    }
}