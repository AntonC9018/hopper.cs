using Microsoft.CodeAnalysis;
using Hopper.Shared.Attributes;
using System;
using System.Linq;
using System.Text;

namespace Meta
{
    public sealed class ExportedMethodSymbolWrapper
    {
        public ContextSymbolWrapper context;
        public IMethodSymbol symbol;
        public ExportAttribute exportAttribute;

        public string Name => symbol.Name;
        public string ContextName => context.NameWithParentClass;
        public bool IsDynamic => exportAttribute.Dynamic;
        public string Priority => exportAttribute.Priority.ToString();
        public string Chain => exportAttribute.Chain;


        public ExportedMethodSymbolWrapper(ContextSymbolWrapper context, IMethodSymbol symbol, ExportAttribute exportAttribute)
        {
            this.symbol = symbol;
            this.exportAttribute = exportAttribute;
            Init(context);
        }

        public ExportedMethodSymbolWrapper(ProjectContext projectContext, IMethodSymbol symbol, ExportAttribute exportAttribute)
        {
            this.symbol = symbol;
            this.exportAttribute = exportAttribute;
            Init(projectContext);
        }

        public void Init(ContextSymbolWrapper context)
        {
            this.context = context;
        }

        public void Init(ProjectContext projectContext)
        {
            var chain = exportAttribute.Chain;

            // The chain must be of form "ComponentName.ChainName"
            var indexOfDot = chain.IndexOf('.');
            if (indexOfDot == -1)
            {
                throw new GeneratorException($"The chain name specified in the export attribute of {Name} method is not valid ({chain}). Expected form \"ComponentName.ChainName\"");
            }

            var componentName = chain.Substring(0, indexOfDot);
            var chainName = chain.Substring(indexOfDot + 1, chain.Length - indexOfDot - 1);

            if (!projectContext.globalComponents.ContainsKey(componentName))
            {
                throw new GeneratorException($"The behavior with the name {componentName} specified in the export attribute of {Name} method did not exist.");
            }

            var component = projectContext.globalComponents[componentName];

            if (component is BehaviorSymbolWrapper behavior)
            {
                if (behavior.chains.Any(ch => ch.Name == chainName))
                {
                    this.context = behavior.context;
                }
                else
                {
                    throw new GeneratorException($"The behavior with the name {componentName} specified in the export attribute of {Name} method did not define the referenced {chainName} chain.");
                }
            }
            else
            {
                throw new GeneratorException($"The component with the name {componentName} specified in the export attribute of {Name} method is not a behavior.");
            }
        }

        public string AdapterBody()
        {
            StringBuilder sb_params = new StringBuilder();
            StringBuilder sb_call = new StringBuilder();

            if (SymbolEqualityComparer.Default.Equals(symbol.ReturnType, RelevantSymbols.Instance.boolType))
            {
                sb_call.Append("ctx.propagate = ");
            }
            if (SymbolEqualityComparer.Default.Equals(context.symbol, symbol.ContainingType))
            {
                sb_call.Append(symbol.IsStatic 
                    // NOTE: This allows for at most 1 level depth.
                    ? $"{ContextName}.{Name}(" 
                    : $"ctx.{Name}(");
            }
            else// if (SymbolEqualityComparer.Default.Equals(component.symbol, symbol.ContainingType))
            {
                sb_call.Append(symbol.IsStatic 
                    ? $"{symbol.ContainingType.Name}.{Name}("
                    // TODO: this is wrong for exported methods not in component classes
                    : $"ctx.actor.Get{symbol.ContainingType.Name}().{Name}(");
            }
            // else
            // {
            //     throw new GeneratorException("Could not have been defined here");
            // }

            foreach (var s in symbol.Parameters)
            {
                // If the parameter is of Context type
                if (SymbolEqualityComparer.Default.Equals(s.Type, context.symbol))
                {
                    // The parameters need not be appended, since the handlers take ctx by default.
                    sb_call.Append("ctx, ");
                }
                // if ctx class has a field of that name and type, reference it directly
                else if (context.ContainsFieldWithNameAndType(s.Name, s.Type))
                {
                    if (s.RefKind == RefKind.Out)
                    {
                        sb_call.Append($"out ctx.{s.Name}, ");
                    }
                    else if (s.RefKind == RefKind.Ref)
                    {
                        sb_call.Append($"ref ctx.{s.Name}, ");
                    }
                    else
                    {
                        sb_params.AppendLine($"var _{s.Name} = ctx.{s.Name};");
                        sb_call.Append($"_{s.Name}, ");
                    }
                }
                // if it is of a component type, retrieve it from the entity 
                else if (s.Type.AllInterfaces.Contains(RelevantSymbols.Instance.icomponent))
                {
                    // if the name contains the name of an entity type field
                    // of the context followed by an underscore, get the component
                    // from that entity and save it.
                    int indexOf_ = s.Name.IndexOf('_');
                    bool success = false;
                    if (indexOf_ != -1)
                    {
                        string entity_name = s.Name.Substring(0, indexOf_);
                        if (context.ContainsEntity(entity_name))
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
                        sb_params.AppendLine($"var _{s.Name} = ctx.actor.Get{s.Type.Name}();");
                        sb_call.Append($"_{s.Name}, ");
                    }
                }
                else
                {
                    throw new GeneratorException($"The name {s.Name} is invalid. It does not correspond directly to any of the Context fields and the type of the parameter was not a component type");
                }
            }

            if (!symbol.Parameters.IsEmpty)
            {
                sb_call.Remove(sb_call.Length - 2, 2);
            }
            sb_call.Append(");");

            sb_params.Append(sb_call.ToString());
            return sb_params.ToString();
        }
    }
}