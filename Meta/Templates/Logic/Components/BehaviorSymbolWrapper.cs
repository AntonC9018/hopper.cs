using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta
{
    public class BehaviorSymbolWrapper : ComponentSymbolWrapper
    {
        public IChainWrapper[] Chains { get; private set; }
        public override string TypeText => "behavior";


        // TODO: this only works for the particular class of behaviors which have been
        //       decorated with the AutoActivation attribute.
        public string ActivationAlias;
        public bool ShouldGenerateAutoActivation => ActivationAlias != null;
        public IEnumerable<BehaviorActivationChainWrapper> ActivationChains 
            => Chains.OfType<BehaviorActivationChainWrapper>();
        public ContextSymbolWrapper Context => ActivationChains.FirstOrDefault()?.Context;


        public BehaviorSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public bool TryInitActivationContext(GenerationEnvironment env, out ContextSymbolWrapper context)
        {
            // Initialize the context class symbol wrapper
            var ctx_symbol = symbol.GetMembers().FirstOrDefault(s => s.Name == "Context");
            if (ctx_symbol == null)
            {
                env.ReportError($"The {symbol.Name} behavior did not define a nested Context class.\nNote: Any behavior must define a Context class. If you do not have any chains in the behavior, make it a simple component. Behaviors by design differ from components in that they exploit chains.");
                context = default;
                return false;
            }
            if (!(ctx_symbol is INamedTypeSymbol named_ctx_symbol))
            {
                env.ReportError($"The Context defined inside {symbol.Name} must be a class");
                context = default;
                return false;
            }
            return env.TryCacheContext(named_ctx_symbol, out context);
        }

        private IEnumerable<ChainSymbolWrapper> GetExportedChains(GenerationEnvironment env)
        {
            foreach (var field in symbol.GetInstanceFields())
            {
                if (field.TryGetAttribute(RelevantSymbols.ChainAttribute, out var chainAttribute))
                {
                    var wrapped = new ChainSymbolWrapper(field, chainAttribute);
                    if (wrapped.Init(env) && env.TryAddChain(wrapped)) 
                    {
                        yield return wrapped;
                    }
                }
            }
        }

        protected override bool Init(GenerationEnvironment env)
        {
            env.errorContext.ClearFlag();

            if (!(base.Init(env))) return false;

            symbol.TryGetAttribute(RelevantSymbols.AutoActivationAttribute, out var autoActivation); 
            var exportedChains = GetExportedChains(env);

            if (autoActivation is null)
            {
                Chains = exportedChains.ToArray();

                if (Chains.Length == 0)
                {
                    env.ReportError("Behaviors must define at least a chain. Otherwise, they are just components");
                }
            }
            else
            {
                if (exportedChains.Any())
                {
                    env.ReportError($"Behaviors decorated with AutoActivation cannot define additional chains.");
                }                
                else
                {
                    if (!TryInitActivationContext(env, out var context)) return false;
                  
                    Chains = new IChainWrapper[] { 
                        new BehaviorActivationChainWrapper("Check", symbol, context), 
                        new BehaviorActivationChainWrapper("Do",    symbol, context)
                    };

                    foreach (var chain in Chains) 
                    { 
                        env.TryAddChain(chain); 
                    }

                    ActivationAlias = autoActivation.Alias;

                    if (env.aliases.Contains(ActivationAlias))
                    {
                        env.ReportError($"Duplicate alias name {ActivationAlias} in behavior {symbol.Name}.");
                    }
                }
            }

            return !env.errorContext.Flag;
        }

        // This must be called after all the behaviors have been added to the dictionary
        // Since this could query them for context and chains.
        protected override bool AfterInit(GenerationEnvironment env)
        {
            exportedMethods = GetAllExportedMethods(env).ToArray();
            return base.AfterInit(env);
        }

        private IEnumerable<ExportedMethodSymbolWrapper> GetAllExportedMethods(GenerationEnvironment env)
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
                    if (attribute.Chain is null)
                    {
                        var m = new ExportedMethodSymbolWrapper(method, attribute);
                        if (m.TryInit(env, Chains.First())) yield return m;
                    }
                    else
                    {
                        var m = new ExportedMethodSymbolWrapper(method, attribute);
                        if (m.TryInit(env)) yield return m;
                    }
                }
            }
        }
    }
}