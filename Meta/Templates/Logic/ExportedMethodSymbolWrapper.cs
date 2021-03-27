using Microsoft.CodeAnalysis;
using System.Text;

namespace Meta
{

    public sealed class ExportedMethodSymbolWrapper
    {
        public BehaviorSymbolWrapper component;
        public IMethodSymbol symbol;

        public string Name => symbol.Name;

        public ExportedMethodSymbolWrapper(BehaviorSymbolWrapper component, IMethodSymbol symbol)
        {
            this.component = component;
            this.symbol = symbol;
        }

        public string AdapterBody()
        {
            StringBuilder sb_params = new StringBuilder();
            StringBuilder sb_call = new StringBuilder();

            if (SymbolEqualityComparer.Default.Equals(symbol.ReturnType, RelevantSymbols.Instance.boolType))
            {
                sb_call.Append("ctx.propagate = ");
            }
            if (SymbolEqualityComparer.Default.Equals(component.context.symbol, symbol.ContainingType))
            {
                sb_call.Append(symbol.IsStatic 
                    ? $"{component.symbol.Name}.Context.{Name}(" 
                    : $"ctx.{Name}(");
            }
            else if (SymbolEqualityComparer.Default.Equals(component.symbol, symbol.ContainingType))
            {
                sb_call.Append(symbol.IsStatic 
                    ? $"{symbol.ContainingType.Name}.{Name}(" 
                    : $"ctx.actor.Get{symbol.ContainingType.Name}().{Name}(");
            }
            else
            {
                throw new GeneratorException("Could not have been defined here");
            }

            foreach (var s in symbol.Parameters)
            {
                // If the parameter is of Context type
                if (SymbolEqualityComparer.Default.Equals(s.Type, component.context.symbol))
                {
                    // The parameters need not be appended, since the handlers take ctx by default.
                    sb_call.Append("ctx, ");
                }
                // if ctx class has a field of that name and type, reference it directly
                else if (component.context.ContainsFieldWithNameAndType(s.Name, s.Type))
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
                        if (component.context.ContainsEntity(entity_name))
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
                    throw new GeneratorException($"The name {s.Name} is invalid. It does not correspond directly to any of the Context fields and the type of the parameter was not a component type");
                }
            }
        

            if (!symbol.Parameters.IsEmpty)
            {
                sb_call.Remove(sb_call.Length - 2, 2);
                sb_call.Append(");");
            }

            sb_params.Append(sb_call.ToString());
            return sb_params.ToString();
        }
    }
}