using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta
{
    public class ChainWrapper : FieldSymbolWrapper, IThing
    {
        public new string Name { get; private set; }
        public ContextSymbolWrapper Context { get; private set; }
        public bool IsMore => symbol.Type == RelevantSymbols.Index;
        public bool IsWithPriorities => GetChainType() == RelevantSymbols.Chain;

        public string Identity => $"{Name} Chain{(IsMore ? "+" : "")}";
        public string Location => symbol.Locations.First().ToString();

        public ChainWrapper(IFieldSymbol symbol) : base(symbol)
        {
            Name = symbol.Name;
        }
        
        public ChainWrapper(IFieldSymbol symbol, ChainAttribute chainAttribute) : base(symbol)
        {
            Name = chainAttribute.Name is null ? symbol.Name : chainAttribute.Name;
        }

        public void InitBehavioral(ContextSymbolWrapper context)
        {
            Context = context;
        }

        public INamedTypeSymbol GetChainType()
        {
            var t = (INamedTypeSymbol) symbol.Type;

            return IsMore 
                // We assume it is an index
                // So the inner type of the type is the chain type
                ? (INamedTypeSymbol) t.TypeArguments.Single()
                : t;
        }

        public bool InitMore(GenerationEnvironment env)
        {
            // 1. Lookup the type of the context. It is the most nested generic type of the index type.
            var contextType = (INamedTypeSymbol) symbol.Type.GetLeafTypeArguments().First();

            // 2. See if that context has already been cached in the environment.
            // 3. Initialize the context if it is not found.
            if (!env.TryGetContextLazy(contextType, out var Context)) return false;

            return true;
        }

    }
}