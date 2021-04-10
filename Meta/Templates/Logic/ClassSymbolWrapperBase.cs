using System.Collections.Generic;
using System.Linq;
using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Meta
{

    abstract public class ClassSymbolWrapperBase
    {
        public INamedTypeSymbol symbol;
        public ClassSymbolWrapperBase(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public HashSet<IFieldSymbol> GetFlaggedFields()
        {
            return symbol.GetMembers().OfType<IFieldSymbol>()
                .Where(field => !field.GetAttributes().IsEmpty).ToHashSet();
        }

        public IEnumerable<UsingDirectiveSyntax> GetUsingSyntax(Solution solution)
        {
            var doc = solution.GetDocument(symbol.Locations.First().SourceTree);
            var tree = doc.GetSyntaxTreeAsync().Result;
            return tree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>();
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

        public IEnumerable<IMethodSymbol> GetMethods()
        {
            return symbol.GetMembers().OfType<IMethodSymbol>();
        }

        public IEnumerable<ExportedMethodSymbolWrapper> GetNativeExportedMethods(ContextSymbolWrapper context)
        {
            foreach (var method in GetMethods())
            {
                if (method.TryGetExportAttribute(out var attribute))
                {
                    // If the chain string is null, it means that the methods reference the behavior
                    // class they are defined in. 
                    // TODO: This actually does have to specify the chain, just without the behavior class part.
                    // Either specify these two separately, as in Chain = "Do", Behavior = "Attackable"
                    // Or split by dot at this point.
                    if (attribute.Chain == null)
                    {
                        yield return new ExportedMethodSymbolWrapper(context, method, attribute);
                    }
                }
            }
        }

        public IEnumerable<ExportedMethodSymbolWrapper> GetNonNativeExportedMethods(ProjectContext context)
        {
            foreach (var method in GetMethods())
            {
                if (method.TryGetExportAttribute(out var attribute))
                {
                    if (attribute.Chain != null)
                    {
                        yield return new ExportedMethodSymbolWrapper(context, method, attribute);
                    }
                    else
                    {
                        // For now, add this check here but it should probably be elsewhere
                        // Report an error if the class is not a behavior.
                        if (!(this is BehaviorSymbolWrapper))
                        {
                            throw new GeneratorException($"The class {ClassName} marked a method for export but did not specify the chain path. Note: one may omit the chain path only if the method being exported is inside a behavior class.");
                        }
                    }
                }
            }
        }

        public abstract string TypeText { get; }
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.FullName();
        public string Calling => $"{ClassName} {TypeText}";
    }
}