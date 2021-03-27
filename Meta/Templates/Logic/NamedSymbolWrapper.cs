using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Meta
{
    public static class SymbolExtensions
    {
        public static string FullName(this INamespaceSymbol symbol)
        {
            Stack<string> names = new Stack<string>();

            while (symbol != null && symbol.Name != "")
            {
                names.Push(symbol.Name);
                symbol = symbol.ContainingNamespace;
            }

            return String.Join(".", names);
        }

        // TODO: This function is pretty jank. Learn the right way to do this.
        public static string TypeToText(this INamedTypeSymbol symbol)
        {
            var sb_type = new StringBuilder();

            // For now, fully qualify the types, since I have to idea how to find document
            // in which the behaviorr symbols were defined.
            if (symbol.ContainingType != null)
            {
                sb_type.Append(TypeToText(symbol.ContainingType));
                sb_type.Append('.');
            }
            else if (symbol.ContainingNamespace != null)
            {
                sb_type.Append(symbol.ContainingNamespace.FullName());
                sb_type.Append('.');
            }

            if (symbol.IsGenericType)
            {
                sb_type.Append(symbol.ContainingType);
                sb_type.Append(symbol.Name);
                sb_type.Append("<");

                foreach (var t in symbol.TypeArguments)
                {
                    sb_type.Append(TypeToText((INamedTypeSymbol)t));
                    sb_type.Append(", ");
                }

                sb_type.Remove(sb_type.Length - 2, 2);
                sb_type.Append(">");
            }
            else
            {
                sb_type.Append(symbol.Name);
            }

            return sb_type.ToString();
        }
    }

    abstract public class NamedTypeSymbolWrapper
    {
        public INamedTypeSymbol symbol;
        public NamedTypeSymbolWrapper(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.FullName();

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

        public abstract string TypeText();
    }
}