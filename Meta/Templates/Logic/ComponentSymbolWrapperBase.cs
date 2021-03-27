using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Meta
{

    abstract public class ComponentSymbolWrapperBase
    {
        public INamedTypeSymbol symbol;
        public ComponentSymbolWrapperBase(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public HashSet<IFieldSymbol> GetFlaggedFields()
        {
            return symbol.GetMembers().OfType<IFieldSymbol>()
                .Where(field => !field.GetAttributes().IsEmpty).ToHashSet();
        }

        public AliasMethodSymbolWrapper[] GetAliasMethods(HashSet<string> globalAliases)
        {
            // Find aliases
            var aliasMethods = symbol.GetMembers().OfType<IMethodSymbol>()
                .FilterMap(m => {
                    var alias = m.GetAttributes().FirstOrDefault(a =>
                        SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.aliasAttribute));
                    if (alias == null) return null;
                    return new AliasMethodSymbolWrapper(m, (string)alias.ConstructorArguments.Single().Value);
                })
                .ToArray();
            
            // Add alias strings to global aliases
            foreach (var aliasMethod in aliasMethods)
            {
                if (globalAliases.Contains(aliasMethod._alias))
                {
                    throw new GeneratorException($"Aliases must be unique across all types. When processing the {symbol.Name} behavior, found a duplicate alias name: {aliasMethod._alias}");
                }
                globalAliases.Add(aliasMethod._alias);
            }

            return aliasMethods;
        }

        public abstract string TypeText { get; }
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.FullName();
        public string Calling => $"{ClassName} {TypeText}";
    }
}