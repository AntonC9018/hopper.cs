using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta
{
    public interface IChainWrapper
    {
        string Name { get; }
        string FieldName { get; }
        ContextSymbolWrapper Context { get; }
        bool IsMore { get; }
        bool IsWithPriorities { get; }
        INamedTypeSymbol GetChainType();
        string GetTypeText();
    }

    public static class ChainWrapperExtensions
    {
        public static string GetPrefix(this IChainWrapper wrapper) => wrapper.IsMore ? "+" : "";
        public static string GetUid(this IChainWrapper wrapper, string enclosingTypeName)
            => $"{wrapper.GetPrefix()}{enclosingTypeName}.{wrapper.Name}";
    }

    public class ImaginaryBehavioralChainWrapper : IChainWrapper
    {
        public string Name { get; private set; }
        public string FieldName => $"_{Name}Chain";
        public ContextSymbolWrapper Context { get; private set; }

        public ImaginaryBehavioralChainWrapper(string name, ContextSymbolWrapper context)
        {
            Name = name;
            Context = context;
        }

        public bool IsMore => false;
        public bool IsWithPriorities => true;

        public INamedTypeSymbol GetChainType() => RelevantSymbols.Chain;

        public string GetTypeText() 
            => $"{GetChainType().Name}<{Context.symbol.GetFullyQualifiedName()}>";
    }

    public class ChainSymbolWrapper : FieldSymbolWrapper, IChainWrapper, IThing
    {
        public new string Name { get; private set; }
        public string FieldName => symbol.Name;
        public ContextSymbolWrapper Context { get; private set; }
        public bool IsMore => symbol.Type == RelevantSymbols.Index;
        public bool IsWithPriorities => GetChainType() == RelevantSymbols.Chain;

        public string Identity => $"{Name} Chain{this.GetPrefix()}";
        public string Location => symbol.Locations.First().ToString();

        public ChainSymbolWrapper(IFieldSymbol symbol) : base(symbol)
        {
            Name = symbol.Name;
        }
        
        public ChainSymbolWrapper(IFieldSymbol symbol, ChainAttribute chainAttribute) : base(symbol)
        {
            Name = chainAttribute.Name is null ? symbol.Name : chainAttribute.Name;
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

        public bool Init(GenerationEnvironment env)
        {
            // 1. Lookup the type of the context. It is the most nested generic type of the index type.
            var contextType = (INamedTypeSymbol) symbol.Type.GetLeafTypeArguments().First();

            // 2. See if that context has already been cached in the environment.
            // 3. Initialize the context if it is not found.
            if (!env.TryGetContextLazy(contextType, out var context)) return false;

            Context = context;

            return true;
        }

        public string GetTypeText() => 
            $"{GetChainType().GetFullyQualifiedName()}<{Context.symbol.GetFullyQualifiedName()}>";

    }
}