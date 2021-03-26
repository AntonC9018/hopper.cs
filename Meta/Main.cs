using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Hopper.Meta.Template;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Meta
{
    class Program
    {
        public static async Task Main()
        {
            MSBuildLocator.RegisterDefaults();
            msWorkspace = await InitWorkspace();
            if (!failFlag)
            {
                await Test2();
            }
        }

        static bool failFlag = false;
        static MSBuildWorkspace msWorkspace;
        const string coreProjectPath = @"../Core/Hopper_Core.csproj";
        static Project coreProject;

        public static async Task<MSBuildWorkspace> InitWorkspace(params string[] projectPaths)
        {
            try
            {
                msWorkspace = MSBuildWorkspace.Create();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
                return null;
            }

            msWorkspace.WorkspaceFailed += (s, args) => {
                if (args.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Console.WriteLine($"Unable to open the project.\n {args.Diagnostic.Message}");
                    failFlag = true;
                }
                else
                {
                    Console.WriteLine($"Warning while opening a project:\n {args.Diagnostic.Message}");
                }
            };

            coreProject = await msWorkspace.OpenProjectAsync(coreProjectPath);
            
            // Open the core project or the mod
            foreach (var projectName in projectPaths)
                await msWorkspace.OpenProjectAsync(projectName);

            return msWorkspace;
        }


        public class RelevantSymbols
        {
            public INamedTypeSymbol entity;
            public INamedTypeSymbol icomponent;
            public INamedTypeSymbol ibehavior;
            public INamedTypeSymbol itag;
            public INamedTypeSymbol aliasAttribute;
            public INamedTypeSymbol autoActivationAttribute;
            public INamedTypeSymbol chainsAttribute;
            public INamedTypeSymbol injectAttribute;
            public INamedTypeSymbol flagsAttribute;
            public INamedTypeSymbol exportAttribute;
            public INamedTypeSymbol omitAttribute;
            
            public static INamedTypeSymbol GetComponentSymbol(Compilation compilation, string name)
            {
                return (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Components.{name}");
            }

            public void Init(Compilation compilation)
            {
                entity = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Entity");
                icomponent      = GetComponentSymbol(compilation, "IComponent");
                ibehavior       = GetComponentSymbol(compilation, "IBehavior");
                itag            = GetComponentSymbol(compilation, "IBehavior");
                aliasAttribute  = GetComponentSymbol(compilation, "AliasAttribute");
                chainsAttribute = GetComponentSymbol(compilation, "ChainsAttribute");
                injectAttribute = GetComponentSymbol(compilation, "InjectAttribute");
                flagsAttribute  = GetComponentSymbol(compilation, "FlagsAttribute");
                exportAttribute = GetComponentSymbol(compilation, "ExportAttribute");
                omitAttribute   = GetComponentSymbol(compilation, "OmitAttribute");
                autoActivationAttribute = GetComponentSymbol(compilation, "AutoActivationAttribute");
            }
        }

        public static RelevantSymbols relevantSymbols; 

        public class ProjectContext
        {
            public Solution solution;
            public Project project;

            public HashSet<Project> projectSet;
            public Compilation compilation;

            public async void Init()
            {
                projectSet = new HashSet<Project>{project};
                compilation = await project.GetCompilationAsync();
            }

            public Task<IEnumerable<INamedTypeSymbol>> FindAllComponents()
            {
                return SymbolFinder.FindImplementationsAsync(
                    relevantSymbols.icomponent, solution, transitive: true, projectSet.ToImmutableHashSet()
                );
            }

            public Task<IEnumerable<INamedTypeSymbol>> FindAllBehaviors()
            {
                return SymbolFinder.FindImplementationsAsync(
                    relevantSymbols.ibehavior, solution, transitive: true, projectSet.ToImmutableHashSet()
                );
            }
        }

        public class ContextWrapper
        {
            public Dictionary<string, IFieldSymbol> fieldsHashed;
            public HashSet<string> withDefaultValue;
            public HashSet<string> entities;
            public INamedTypeSymbol symbol;

            public void HashFields()
            {
                var ctx_fields = new List<IFieldSymbol>();

                {
                    var s = symbol;
                    do 
                    {
                        ctx_fields.AddRange(s
                            .GetMembers().OfType<IFieldSymbol>()
                            .Where(field => !field.IsStatic && !field.IsConst));
                        s = symbol.BaseType;
                    }
                    while (s != null);
                }

                fieldsHashed = ctx_fields.ToDictionary(field => field.Name);
                entities = ctx_fields
                    .Where(field => SymbolEqualityComparer.Default.Equals(field.ContainingType, relevantSymbols.entity))
                    .Select(field => field.Name).ToHashSet();

                // I cannot figure out how to check if the field was given a default value, so
                // I'm going to do this with an attribute instead
                withDefaultValue = ctx_fields.Where(field => field.GetAttributes()
                    .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, relevantSymbols.omitAttribute)))
                    .Select(field => field.Name).ToHashSet();
            }

            public bool ContainsEntity(string name) => entities.Contains(name);
            public bool ContainsFieldWithNameAndType(string name, ITypeSymbol type) 
            {
                return fieldsHashed.TryGetValue(name, out var t) && 
                    SymbolEqualityComparer.Default.Equals(type, t.Type);
            }
            public bool HasDefaultValue(string name) => withDefaultValue.Contains(name);
        }

        public class MethodSymbolWrapper
        {
            public IMethodSymbol symbol;
            public ContextWrapper ctx;
            public INamedTypeSymbol exportingBehavior;
            public string alias = null;


            public string AdapterBody()
            {
                StringBuilder sb_params = new StringBuilder();
                StringBuilder sb_call = new StringBuilder();

                if (SymbolEqualityComparer.Default.Equals(ctx.symbol, symbol.ContainingType))
                {
                    sb_call.Append(symbol.IsStatic 
                        ? $"return {exportingBehavior.Name}.Context.{symbol.Name}(" 
                        : $"return ctx.{symbol.Name}(");
                }
                else if (SymbolEqualityComparer.Default.Equals(exportingBehavior, symbol.ContainingType))
                {
                    sb_call.Append(symbol.IsStatic 
                        ? $"return {symbol.ContainingType.Name}.{symbol.Name}(" 
                        : $"return ctx.actor.Get{symbol.ContainingType.Name}().{symbol.Name}(");
                }
                else
                {
                    throw new Exception("Could not have been defined here");
                }

                foreach (var s in symbol.Parameters)
                {
                    // If the parameter is of Context type
                    if (SymbolEqualityComparer.Default.Equals(s.Type, ctx.symbol))
                    {
                        // The parameters need not be appended, since the handlers take ctx by default.
                        sb_call.Append("ctx, ");
                    }
                    // if ctx class has a field of that name and type, reference it directly
                    else if (ctx.ContainsFieldWithNameAndType(s.Name, s.Type))
                    {
                        if (s.RefKind == RefKind.Out)
                        {
                            sb_call.Append($"out ctx.{s.Name}");
                        }
                        else
                        {
                            sb_params.AppendLine($"var _{s.Name} = ctx.{s.Name};");
                            sb_call.Append($"_{s.Name}, ");
                        }
                    }
                    // if it is of a component type, retrieve it from the entity 
                    else if (s.Type.AllInterfaces.Contains(relevantSymbols.icomponent))
                    {
                        // if the name contains the name of an entity type field
                        // of the context followed by an underscore, get the component
                        // from that entity and save it.
                        int indexOf_ = s.Name.IndexOf('_');
                        bool success = false;
                        if (indexOf_ != -1)
                        {
                            string entity_name = s.Name.Substring(0, indexOf_);
                            if (ctx.ContainsEntity(entity_name))
                            {
                                success = true;
                                sb_params.AppendLine($"var _{s.Name} = ctx.{entity_name}.Get{s.Type.Name}();");
                                sb_call.Append($"_{s.Name}, ");
                            }
                            else
                            {
                                // TODO: Report warning?
                            }
                        }
                        if (!success)
                        {
                            // get the component from entity. For now, assume that
                            // the entity is assumed to always contain the given component.
                            sb_params.AppendLine($"var _{s.Name} = ctx.entity.Get{s.Type.Name}();");
                            sb_call.Append($"_{s.Name}, ");
                        }
                    }
                    else
                    {
                        throw new Exception($"The name {s.Name} is invalid. It does not correspond directly to any of the Context fields and the type of the parameter was not a component type");
                    }
                }
            
                if (!symbol.Parameters.IsEmpty)
                {
                    sb_call.Remove(sb_call.Length - 2, 2);
                    sb_call.Append(");");
                }

                sb_call.Append(sb_params.ToString());
                return sb_call.ToString();
            }


            public bool ShouldCreateAlias()
            {
                return alias != null;
            }

            public string ParametersInSignature()
            {
                return string.Join(",", symbol.Parameters.Select(p => p.ToDisplayString()));
            }

            public string ParametersInInvocation()
            {
                return string.Join(",", symbol.Parameters.Select(p => p.Name));
            }
        }


        public class ComponentSymbolWrapper
        {   
            public ProjectContext context;
            public INamedTypeSymbol symbol;
            public HashSet<IFieldSymbol> flaggedFields;
            public HashSet<string> aliases;

            public bool IsBehavior => symbol.Interfaces.Contains(relevantSymbols.ibehavior);
            public bool IsTag => symbol.Interfaces.Contains(relevantSymbols.itag);
        }

        public class BehaviorSymbolWrapper : ComponentSymbolWrapper
        {

        }

        public static async Task Test2()
        {
            var ctx = new ProjectContext();
            ctx.project = coreProject;
            ctx.solution = msWorkspace.CurrentSolution;
            
            var implementations = await ctx.FindAllComponents();

            Console.WriteLine(implementations.Count());

            foreach (var behavior in implementations)
            {
                foreach (var method in behavior.GetMembers().OfType<IMethodSymbol>())
                foreach (var attrib in method.GetAttributes())
                {
                    if (SymbolEqualityComparer.Default.Equals(attrib.AttributeClass, aliasAttribute))
                    foreach (var arg in attrib.ConstructorArguments)
                    {
                        var t = (TypedConstant)arg;
                        Console.WriteLine(t.Value);
                    }
                }
            }
            return;
        }
    }
}