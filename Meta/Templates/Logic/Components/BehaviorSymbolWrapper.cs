using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta
{
    public class BehaviorSymbolWrapper : ComponentSymbolWrapper
    {
        public ContextSymbolWrapper context;
        public bool ShouldGenerateActivation;
        public bool ShouldGenerateCheckDo;
        public string ActivationAlias;

        public ChainWrapper[] chains;
        public bool HasCheck() => chains.Any(chain => chain.Name == "Check");
        public bool HasDo() => chains.Any(chain => chain.Name == "Do");
        public bool ShouldGenerateActivationShortcuts => ActivationAlias != null;

        public override string TypeText => "behavior";

        public BehaviorSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override void Init(ProjectContext projectContext)
        {
            base.Init(projectContext);

            if (projectContext.globalComponents.ContainsKey(symbol.Name))
            {
                throw new GeneratorException($"The behavior {symbol.Name} has been defined twice, which is not allowed.");
            }

            projectContext.globalComponents.Add(symbol.Name, this);

            // Initialize the context class symbol wrapper
            var ctx_symbol = symbol.GetMembers().FirstOrDefault(s => s.Name == "Context");
            if (ctx_symbol == null)
            {
                throw new GeneratorException($"The {symbol.Name} behavior did not define a nested Context class.\nNote: Any behavior must define a Context class. If you do not have any chains in the behavior, make it a simple component. Behaviors by design differ from components in that they exploit chains.");
            }
            if (ctx_symbol.Kind != SymbolKind.NamedType)
            {
                throw new GeneratorException($"The Context defined inside {symbol.Name} must be a class");
            }
            context = new ContextSymbolWrapper((INamedTypeSymbol)ctx_symbol);
            context.Init();

            // Get the chains
            var chainsAttribute = symbol.GetAttributes().SingleOrDefault(a =>
                SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.chainsAttribute));

            // See if we have the AutoActivation attribute
            var autoActivation = symbol.GetAttributes().SingleOrDefault(a => 
                SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.autoActivationAttribute));

            if (chainsAttribute != null)
            {
                chains = chainsAttribute.ConstructorArguments.Single().Values
                    .Select(arg => new ChainWrapper((string)arg.Value, symbol)).ToArray();
            }

            if (!symbol.GetAttributes().Any(
                a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.noActivationAttribute))) 
            {
                if (autoActivation == null && chainsAttribute == null)
                {
                    throw new GeneratorException($"The behavior {symbol.Name} must be decorated with either the AutoActivation attribute, in which case the chains generated by default will be Check and Do, and/or with the Chains attribute, which will provide autogeneration for the specified chain names.");
                }

                if (chainsAttribute == null)
                {
                    chains = new ChainWrapper[] { 
                        new ChainWrapper("Check", symbol), 
                        new ChainWrapper("Do", symbol)
                    };
                }


                ShouldGenerateActivation = autoActivation != null;
                ShouldGenerateCheckDo = HasCheck() && HasDo();

                if (ShouldGenerateActivation && !ShouldGenerateCheckDo)
                {
                    throw new GeneratorException($"The behavior {symbol.Name} asked for an autogenerated activation but did not specify the necessary Check and Do chains. Be sure to either omit the Chains attibute altogether when using the AutoActivation attribute, or append Check and Do to the list of chains.");
                }

                // Find activation alias attribute
                var activation = symbol.GetAttributes().FirstOrDefault(a =>
                        SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.activationAliasAttribute));

                if (autoActivation != null)
                {
                    if (activation != null)
                    {
                        throw new GeneratorException($"Found a request for automatic generation of an activation method beside the ActivationAlias attribute in the behavior {symbol.Name}. This is not allowed.");
                    }
                    else
                    {
                        // See AutoActivationAttribute. It always takes in one argument.
                        ActivationAlias = (string)autoActivation.ConstructorArguments.Single().Value;
                    }
                }
                else if (activation == null)
                {
                    throw new GeneratorException($"The behavior {symbol.Name} neither provided an activation method marked for aliasing (mark the class with ActivationAlias attibute and define a function named Activate that takes in the following arguments: {context.JoinedParamTypeNames()}), nor asked for automatic generation of an activation method (mark the class with the AutoActivation attribute). You must do at least one of these. \nAlternatively, mark the behavior with the NoActivation attribute to skip code generation for activation altogether.");
                }
                else
                {
                    // See ActivationAliasAttribute. It always takes in one argument.
                    ActivationAlias = (string)activation.ConstructorArguments.Single().Value;
                    
                    // Make sure there is a method that takes the same arguments that the context expects
                    var activationMethods = symbol.GetMembers()
                        .OfType<IMethodSymbol>()
                        .Where(m => m.Name == "Activate")
                        .Where(m => m.ParameterTypesEqual(context.notOmitted));

                    if (activationMethods.Count() == 0)
                    {
                        throw new GeneratorException($"No suitable activation method found for the {symbol.Name} behavior. To resolve, provide an activation method that would take the following arguments: {context.JoinedParamTypeNames()}\nAlternatively, apply the AutoActivation attribute for such a method to be generated automatically.\nYou may also apply the Omit attribute to members of your Context class or give them default values, if the activation must not require them.\nAlternatively, mark the behavior with the NoActivation attribute to skip code generation for activation altogether.");
                    }
                }

                if (projectContext.globalAliases.Contains(ActivationAlias))
                {
                    throw new GeneratorException($"Duplicate alias name {ActivationAlias} in behavior {symbol.Name}.");
                }
            }
            else
            {
                if (autoActivation != null)
                {
                    throw new GeneratorException($"The behavior {symbol.Name} both the NoActivation attribute and AutoActivation attribute were found. You must remove one or the other.");
                }

                if (chainsAttribute == null)
                {
                    chains = new ChainWrapper[0];
                }
            }
        }

        // This must be called after all the behaviors have been added to the dictionary
        // Since this could query them for context and chains.
        public override void AfterInit(ProjectContext projectContext)
        {
            exportedMethods = GetAllExportedMethods(projectContext).ToArray();
        }

        private IEnumerable<ExportedMethodSymbolWrapper> GetAllExportedMethods(ProjectContext projectContext)
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
                    else
                    {
                        yield return new ExportedMethodSymbolWrapper(projectContext, method, attribute);
                    }
                }
            }
        }
        
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<U> FilterMap<T, U>(this IEnumerable<T> sequence, System.Func<T, U> map)
        {
            foreach (var element in sequence)
            {
                var result = map(element);
                if (result != null) yield return result;
            }
        }
    }
}