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
        private ExportAttribute exportAttribute;
        private IChainWrapper ReferencedChain;
        private ContextSymbolWrapper Context => ReferencedChain.Context;
        public string Name => symbol.Name;
        public string ChainName => ReferencedChain.Name;
        public string ContextName => Context.symbol.GetFullyQualifiedName();
        public bool IsDynamic => exportAttribute.Dynamic;
        public string Priority => exportAttribute.Priority.ToString();
        public string Identity => $"{Name} exported method";
        public string Location => $"{symbol.Locations.First()}";

        public string AdapterBody;


        public ExportedMethodSymbolWrapper(IMethodSymbol symbol, ExportAttribute exportAttribute)
        {
            this.symbol = symbol;
            this.exportAttribute = exportAttribute;
        }

        private bool InitWithChain(GenerationEnvironment env, IChainWrapper chain)
        {
            this.ReferencedChain = chain;
            AdapterBody = GetAdapterBody(env);
            return !(AdapterBody is null);
        }

        public bool TryInit(GenerationEnvironment env, IChainWrapper chain)
            => env.DoScoped(this, () => InitWithChain(env, chain));

        public bool TryInit(GenerationEnvironment env)
            => env.DoScoped(this, () => Init(env));

        private bool Init(GenerationEnvironment env)
        {
            var uid = exportAttribute.Chain;

            return env.ValidateChainUid(uid) 
                && env.chains.TryGetValue(uid, out var ReferencedChain);
        }

        private string GetNamePrefixAtCall()
        {
            if (symbol.IsStatic)
            {
                // If it is inside a behavior while referencing that behavior.
                // if (ReferencedBehavior.symbol == symbol.ContainingType)
                // {
                //     return "";
                // }

                // If inside a context (which also means it references the same behavior).
                if (Context.symbol == symbol.ContainingType)
                {
                    return $"{ContextName}.";
                }

                // Otherwise, we're in some type.
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
                    if (s.RefKind == RefKind.None)
                    {
                        sb_params.AppendLine($"var _{s.Name} = ctx.{s.Name};");
                        sb_call.Append($"_{s.Name}, ");
                    }
                    else
                    {
                        sb_call.Append($"{s.RefKind.AsKeyword()} ctx.{s.Name}, ");
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