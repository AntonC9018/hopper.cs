using Microsoft.CodeAnalysis;
using Hopper.Shared.Attributes;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Hopper.Meta
{
    public sealed class ExportedMethodSymbolWrapper : IThing
    {
        public IMethodSymbol symbol;
        public ExportAttribute exportAttribute;
        public BehaviorSymbolWrapper referencedBehavior;

        public ContextSymbolWrapper Context => referencedBehavior.context;
        public string Name => symbol.Name;
        public string ContextName => Context.NameWithParentClass;
        public bool IsDynamic => exportAttribute.Dynamic;
        public string Priority => exportAttribute.Priority.ToString();
        public string Chain => exportAttribute.Chain;
        public string Identity => $"{Name} exported method";
        public string Location => $"{symbol.Locations.First()}";

        public string AdapterBody;


        public ExportedMethodSymbolWrapper(BehaviorSymbolWrapper behavior, IMethodSymbol symbol, ExportAttribute exportAttribute)
        {
            this.symbol = symbol;
            this.exportAttribute = exportAttribute;
        }

        public ExportedMethodSymbolWrapper(GenerationEnvironment projectContext, IMethodSymbol symbol, ExportAttribute exportAttribute)
        {
            this.symbol = symbol;
            this.exportAttribute = exportAttribute;
        }

        public void Init(BehaviorSymbolWrapper behavior)
        {
            this.referencedBehavior = behavior;
        }

        public bool InitWithErrorHandling(GenerationEnvironment env)
        {
            env.errorContext.PushThing(this);
            bool result = Init(env);
            env.errorContext.PopThing();
            return result;
        }

        private bool Init(GenerationEnvironment env)
        {
            var chain = exportAttribute.Chain;

            // The chain must be of form "ComponentName.ChainName"
            var indexOfDot = chain.IndexOf('.');
            if (indexOfDot == -1)
            {
                env.ReportError($"The chain name specified in the export attribute is not valid ({chain}). Expected form \"ComponentName.ChainName\"");
                return false;
            }

            var componentName = chain.Substring(0, indexOfDot);
            var chainName = chain.Substring(indexOfDot + 1, chain.Length - indexOfDot - 1);

            if (!env.components.ContainsKey(componentName))
            {
                env.ReportError($"The behavior with the name {componentName} specified in the export attribute of {Name} method did not exist.");
                return false;
            }

            var component = env.components[componentName];

            if (component is BehaviorSymbolWrapper behavior)
            {
                if (behavior.chains.Any(ch => ch.Name == chainName))
                {
                    this.referencedBehavior = behavior;
                }
                else
                {
                    env.ReportError($"The behavior with the name {componentName} specified in the export attribute of {Name} method did not define the referenced {chainName} chain.");
                    return false;
                }
            }
            else
            {
                env.ReportError($"The component with the name {componentName} specified in the export attribute of {Name} method is not a behavior.");
                return false;
            }

            AdapterBody = GetAdapterBody(env);

            return !(AdapterBody is null);
        }

        private string GetNamePrefixAtCall()
        {
            if (symbol.IsStatic)
            {
                // If it is inside a behavior while referencing that behavior.
                if (referencedBehavior.symbol == symbol.ContainingType)
                {
                    return "";
                }

                // If inside a context (which also means it references the same behavior).
                if (Context.symbol == symbol.ContainingType)
                {
                    return $"{ContextName}.";
                }

                // Otherwise, we're in some other type.
                // Return the longer qualification (all nested types).
                return $"{symbol.GetTypeQualification()}.";
            }

            // Else, we're not static.

            // If we're inside any component, get that component from the entity and call ourselves.
            if (symbol.ContainingType.HasInterface(RelevantSymbols.IComponent))
            {
                return $"ctx.{Context.ActorName}.GetComponent({symbol.ContainingType.Name}.Index).";
            }

            // If we're in a context class, just call ourselves directly
            if (Context.symbol == symbol.ContainingType)
            {
                return $"ctx.";
            }

            // If we're in a static class, we must be static, so accounted for above
            // Otherwise, we're a member of a non-static class, so just call ourselves directly.
            return "";
        }

        public string GetAdapterBody(GenerationEnvironment env)
        {
            StringBuilder sb_params = new StringBuilder();
            StringBuilder sb_call = new StringBuilder();

            // If the method returns bool, it is treated toward propagation.
            if (SymbolEqualityComparer.Default.Equals(symbol.ReturnType, RelevantSymbols.boolType))
            {
                sb_call.Append("ctx.Propagate = ");
            }

            sb_call.Append(GetNamePrefixAtCall());
            sb_call.Append($"{Name}(");

            foreach (var s in symbol.Parameters)
            {
                // TODO: allow get properties
                if (s.Name == Context.ActorName)
                {
                    sb_call.Append($"ctx.{Context.ActorName}, ");
                }
                // If the parameter is of Context type
                else if (SymbolEqualityComparer.Default.Equals(s.Type, Context.symbol))
                {
                    // The parameters need not be appended, since the handlers take ctx by default.
                    sb_call.Append("ctx, ");
                }
                // if ctx class has a field of that name and type, reference it directly
                else if (Context.ContainsFieldWithNameAndType(s.Name, s.Type))
                {
                    Console.WriteLine(s.RefKind.ToString());

                    if (s.RefKind == RefKind.Out)
                    {
                        sb_call.Append($"out ctx.{s.Name}, ");
                    }
                    else if (s.RefKind == RefKind.Ref)
                    {
                        sb_call.Append($"ref ctx.{s.Name}, ");
                    }
                    else if (s.RefKind == RefKind.In)
                    {
                        sb_call.Append($"in ctx.{s.Name}, ");
                    }
                    else
                    {
                        sb_params.AppendLine($"var _{s.Name} = ctx.{s.Name};");
                        sb_call.Append($"_{s.Name}, ");
                    }
                }
                // if it is of a component type, retrieve it from the entity 
                else if (s.Type.HasInterface(RelevantSymbols.IComponent))
                {
                    // if the name contains the name of an entity type field
                    // of the context followed by an underscore, get the component
                    // from that entity and save it.
                    int indexOf_ = s.Name.IndexOf('_');
                    bool success = false;
                    if (indexOf_ != -1)
                    {
                        string entity_name = s.Name.Substring(0, indexOf_);
                        if (Context.ContainsEntity(entity_name))
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
                    if (SymbolEqualityComparer.Default.Equals(s.Type, RelevantSymbols.entity))
                    {
                        env.ReportError($"The entity must be named \"actor\", like on the context class");
                        return null;
                    }

                    env.ReportError($"The name \"{s.Name}\" is invalid. It does not correspond directly to any of the Context fields and the type of the parameter was not a component type");
                    return null;
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