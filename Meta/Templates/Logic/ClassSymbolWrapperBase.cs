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
        public IEnumerable<UsingDirectiveSyntax> usings;

        public ClassSymbolWrapperBase(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        protected void Init(ProjectContext projectContext)
        {
            usings = GetUsingSyntax(projectContext._solution);
        }

        public IEnumerable<string> Usings()
        {
            return usings.Select(n => n.ToString());
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

        public IEnumerable<IMethodSymbol> GetMethods()
        {
            return symbol.GetMethods();
        }

        public abstract string TypeText { get; }
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.FullName();
        public string Calling => $"{ClassName} {TypeText}";
    }
}