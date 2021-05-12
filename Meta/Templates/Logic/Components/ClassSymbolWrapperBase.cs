using System;
using System.Collections.Generic;
using System.Linq;
using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hopper.Meta
{

    abstract public class TypeSymbolWrapperBase
    {
        public INamedTypeSymbol symbol;
        public IEnumerable<UsingDirectiveSyntax> usings;

        public TypeSymbolWrapperBase(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        public virtual void Init(GlobalContext projectContext)
        {
            usings = GetUsingSyntax(projectContext._solution);
        }

        public bool InitWithErrorHandling(GlobalContext ctx)
        {
            try
            {
                Init(ctx);
            }
            catch (GeneratorException e)
            {
                Console.WriteLine($"An error occured while processing {Calling}, exported from {symbol.Locations.First()}:");
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public bool AfterInitWithErrorHandling(GlobalContext ctx)
        {
            try
            {
                AfterInit(ctx);
            }
            catch (GeneratorException e)
            {
                Console.WriteLine($"An error occured while processing {Calling}, exported from {symbol.Locations.First()}:");
                Console.WriteLine(e.Message);
                Console.WriteLine("");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
            return true;
        }

        public virtual void AfterInit(GlobalContext ctx){}

        public void WriteGenerationMessage()
        {
            Console.WriteLine($"Generating code for {Calling}");
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
                .FilterMap(m =>
                    m.TryGetAttribute(RelevantSymbols.aliasAttribute, out var alias) 
                        ? new AliasMethodSymbolWrapper(m, (string)alias.ConstructorArguments.Single().Value)
                        : null
                )
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

        public IEnumerable<ExportedMethodSymbolWrapper> GetNonNativeExportedMethods(GlobalContext projectContext)
        {
            foreach (var method in GetMethods())
            {
                if (method.TryGetExportAttribute(out var attribute))
                {
                    if (attribute.Chain != null)
                    {
                        yield return new ExportedMethodSymbolWrapper(projectContext, method, attribute);
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
        // The start of the name of the method that checks whether the given component is on the entity
        // Has are the values for behaviors and components, Is is the value for tags.
        public virtual string HasAlias => "Has";
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.GetFullName();
        public string FullyQualifiedClassName => $"{Namespace}.{ClassName}";
        public string Calling => $"{ClassName} {TypeText}";
        public string Staticity => symbol.IsStatic ? "static " : "";
        public bool IsExportingInstanceMethods => symbol.HasAttribute(RelevantSymbols.instanceExportAttribute);
    }
}