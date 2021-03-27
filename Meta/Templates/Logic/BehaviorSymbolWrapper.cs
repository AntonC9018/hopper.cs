using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meta
{
    public class BehaviorSymbolWrapper : ComponentSymbolWrapper
    {
        public ContextSymbolWrapper context;
        public AliasMethodSymbolWrapper[] aliasMethods;
        public ExportedMethodSymbolWrapper[] exportedMethods;
        public ISymbol[] usings;
        public bool ShouldGenerateActivation;
        public string ActivationAlias;

        public ChainWrapper[] chains;
        public bool HasCheck() => chains.Any(chain => chain.Name == "Check");
        public bool HasDo() => chains.Any(chain => chain.Name == "Do");

        public override string TypeText() => "behavior";

        public BehaviorSymbolWrapper(INamedTypeSymbol symbol, HashSet<string> globalAliases) : base(symbol, globalAliases)
        {
            Init();
        }

        private void Init()
        {
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

            // See if we have the AutoActivation attribute
            var autoActivation = symbol.GetAttributes().SingleOrDefault(a => 
                SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.autoActivationAttribute));
            
            // Get the chains
            var chainsAttribute = symbol.GetAttributes().SingleOrDefault(a =>
                SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.chainsAttribute));
            
            if (autoActivation == null && chainsAttribute == null)
            {
                throw new GeneratorException($"The behavior {symbol.Name} must be decorated with either the AutoActivation attribute, in which case the chains generated by default will be Check and Do, and/or with the Chains attribute, which will provide autogeneration for the specified chain names.");
            }

            chains = chainsAttribute == null

                ? new ChainWrapper[] { 
                    new ChainWrapper("Check", symbol), 
                    new ChainWrapper("Do", symbol) }

                : chainsAttribute.ConstructorArguments.Single().Values
                    .Select(arg => new ChainWrapper((string)arg.Value, symbol)).ToArray();


            ShouldGenerateActivation = autoActivation != null;

            if (ShouldGenerateActivation 
                && (!chains.Any(c => c.Name == "Check") || !chains.Any(c => c.Name == "Do")))
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
                throw new GeneratorException($"The behavior {symbol.Name} neither provided an activation method marked for aliasing (mark the class with ActivationAlias attibute and define a function named Activate that takes in the following arguments: {context.JoinedParamTypeNames()}), nor asked for automatic generation of an activation method (mark the class with the AutoActivation attribute). You must do at least one of these.");
            }
            else
            {
                // See ActivationAliasAttribute. It always takes in one argument.
                ActivationAlias = (string)activation.ConstructorArguments.Single().Value;
                
                // Make sure there is a method that takes the same arguments that the context expects
                var activationMethods = symbol.GetMembers()
                    .OfType<IMethodSymbol>()
                    .Where(m => m.Name == "Activate")
                    .Where(m => m.Parameters.Select(m1 => m1.Type).SequenceEqual(
                        context.notOmitted.Select(field => field.Type), SymbolEqualityComparer.Default));

                if (activationMethods.Count() == 0)
                {
                    throw new GeneratorException($"No suitable activation method found for the {symbol.Name} behavior. To resolve, provide an activation method that would take the following arguments: {context.JoinedParamTypeNames()}\nAlternatively, apply the AutoActivation attribute for such a method to be generated automatically.\nYou may also apply the Omit attribute to members of your Context class or give them default values, if the activation must not require them.");
                }
            }


            // Export methods
            exportedMethods = symbol.GetMembers().OfType<IMethodSymbol>()
                .Where(m => m.GetAttributes().Any(a =>
                        SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.exportAttribute)))
                .Select(m => new ExportedMethodSymbolWrapper(this, m))
                .ToArray();
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