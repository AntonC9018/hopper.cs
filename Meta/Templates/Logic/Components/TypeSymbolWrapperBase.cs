using System;
using System.Collections.Generic;
using System.Linq;
using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hopper.Meta
{

    abstract public class TypeSymbolWrapperBase : IThing
    {
        public INamedTypeSymbol symbol;
        public IEnumerable<UsingDirectiveSyntax> usings;
        public ExportedMethodSymbolWrapper[] exportedMethods;
        public ChainSymbolWrapper[] moreChains;

        public TypeSymbolWrapperBase(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
        }

        protected virtual bool Init(GenerationEnvironment env)
        {
            usings = GetUsingSyntax(env.Solution);
            return true;
        }

        protected virtual bool AfterInit(GenerationEnvironment env)
        {
            if (exportedMethods == null)
                exportedMethods = GetNonNativeExportedMethods(env).ToArray();
            return true;   
        }

        public bool TryInit(GenerationEnvironment env)
            => env.DoScoped(this, () => Init(env));

        public bool TryAfterInit(GenerationEnvironment env)
            => env.DoScoped(this, () => AfterInit(env));


        public void WriteGenerationMessage()
        {
            Console.WriteLine($"Generating code for {Identity}");
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

        private IEnumerable<AliasMethodSymbolWrapper> GetAliasMethodsHelper()
        {
            foreach (var m in symbol.GetMethods())
            {
                if (m.TryGetAttribute(RelevantSymbols.AliasAttribute, out var attr))
                {
                    yield return new AliasMethodSymbolWrapper(m, attr.Alias);
                }
            }
        }

        public AliasMethodSymbolWrapper[] GetAliasMethods(GenerationEnvironment env)
        {
            // Find aliases
            var aliasMethods = GetAliasMethodsHelper().ToArray();
            
            // Add alias strings to global aliases
            foreach (var aliasMethod in aliasMethods)
            {
                if (!env.aliases.Add(aliasMethod.Alias))
                {
                    env.ReportError($"Aliases must be unique across all types. When processing the {symbol.Name} behavior, found a duplicate alias name: {aliasMethod.Alias}");
                }
            }

            return aliasMethods;
        }

        public IEnumerable<ExportedMethodSymbolWrapper> GetNonNativeExportedMethods(GenerationEnvironment env)
        {
            foreach (var method in GetMethods())
            {
                if (method.TryGetExportAttribute(out var attribute))
                {
                    env.errorContext.PushThing(method);

                    if (attribute.Chain != null)
                    {
                        var m = new ExportedMethodSymbolWrapper(method, attribute);
                        if (m.TryInit(env)) yield return m;
                    }
                    else
                    {
                        // For now, add this check here but it should probably be elsewhere
                        // Report an error if the class is not a behavior.
                        if (!(this is BehaviorSymbolWrapper))
                        {
                            env.ReportError($"The class {ClassName} marked a method for export but did not specify the chain path. Note: one may omit the chain path only if the method being exported is inside a behavior class.");
                        }
                        env.errorContext.PopThing();
                        yield break;
                    }

                    env.errorContext.PopThing();
                }
            }
        }

        private IEnumerable<ChainSymbolWrapper> GetMoreChainsChains(GenerationEnvironment env)
        {
            foreach (var field in symbol.GetStaticFields())
            {
                if (field.TryGetAttribute(RelevantSymbols.ChainAttribute, out var chainAttribute))
                {
                    var wrapped = new ChainSymbolWrapper(field, chainAttribute);
                    
                    if (env.DoScoped(wrapped, () => wrapped.InitMore(env)))
                    {
                        yield return wrapped;
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
        public virtual string AliasOfHas => "Has";
        public string ClassName => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.GetFullName();
        public string FullyQualifiedClassName => $"{Namespace}.{ClassName}";
        public string Identity => $"{ClassName} {TypeText}";
        public string Location => $"{symbol.Locations.First()}";
        public string StaticityString => symbol.IsStatic ? "static " : "";
        public bool IsExportingInstanceMethods => symbol.HasAttribute(RelevantSymbols.InstanceExportAttribute.symbol);
    }
}