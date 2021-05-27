using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta
{
    public interface IChainWrapper
    {
        string Name { get; }
        string FieldName { get; }
        ContextSymbolWrapper Context { get; }
        ChainContributionType ContributionType { get; }
        bool IsWithPriorities { get; }
        INamedTypeSymbol GetChainType();
        INamedTypeSymbol Parent { get; }
        string GetTypeText();
    }

    public static class ChainWrapperExtensions
    {
        public static string GetPrefix(this IChainWrapper wrapper) => wrapper.ContributionType.GetPrefix();

        private static ChainContributionType StripContributionType(ref string uid) => 
            ChainContribution.StripContributionType(ref uid);

        private struct ChainIdentifier
        {
            public ChainContributionType Type;
            public string Class;
            public string Chain;

            public static bool TryParse(string uid, ErrorContext ctx, out ChainIdentifier parsed)
            {
                var strippedUid = uid;
                var type = StripContributionType(ref strippedUid);
                var split = strippedUid.Split('.');

                if (split.Length != 2)
                {
                    ctx.Report($"{uid} had wrong format. Expecting format [+]<ExportingClassName>.<ChainName>");
                    parsed = default;
                    return false;
                }

                parsed = new ChainIdentifier
                {
                    Type = type,
                    Class = split[0],
                    Chain = split[1] 
                };
                return true;
            }
        }


        public static bool TrueOr(bool expression, System.Action or)
        {
            if (expression) return true;
            or();
            return false;
        }

        public static bool ValidateChainUidAgainst(this string uid, GenerationEnvironment env)
        {
            if (!ChainIdentifier.TryParse(uid, env.errorContext, out var parsed))
            {
                return false;
            }

            if (env.exportingClasses.TryGetValue(parsed.Class, out var exportingType))
            {
                switch (parsed.Type)
                {
                case ChainContributionType.More:
                case ChainContributionType.Global:
                    return TrueOr(
                        exportingType.contributedChains.Any(chain => chain.Name == parsed.Chain
                            && chain.ContributionType == parsed.Type),
                        () => env.ReportError($"{uid} references a non-existent chain: {parsed.Chain}."));

                case ChainContributionType.Instance:
                    if (exportingType is BehaviorSymbolWrapper behavior)
                    {
                        if (behavior.Chains.Any(chain => chain.Name == parsed.Chain)) 
                            return true;
                        env.ReportError($"{uid} references a non-existent behavior chain: {parsed.Chain}.");
                    }
                    else
                    {
                        env.ReportError($"{uid} referenced an exporting class that was not a behavior: {parsed.Class}. If you meant a static class, use '+{uid}' instead.");
                    }
                    return false;
                default:
                    return false;
                }
            }
            else
            {
                env.ReportError($"{uid} references a non-existent exporting class: {parsed.Class}");
            }
            return false;
        }

        public static string GetUid(this IChainWrapper wrapper)
            => $"{wrapper.GetPrefix()}{wrapper.Parent.Name}.{wrapper.Name}";
        public static string GetFullyQualifiedName(this IChainWrapper wrapper)
            => $"{wrapper.Parent.GetFullyQualifiedName()}.{wrapper.Name}";
        public static bool IsMore(this IChainWrapper wrapper)
            => wrapper.ContributionType == ChainContributionType.More;
        public static bool IsGlobal(this IChainWrapper wrapper)
            => wrapper.ContributionType == ChainContributionType.Global;
    }

    public class BehaviorActivationChainWrapper : IChainWrapper
    {
        public string Name { get; private set; }
        public INamedTypeSymbol Parent { get; private set; }
        public string FieldName => $"_{Name}Chain";
        public ContextSymbolWrapper Context { get; private set; }

        public BehaviorActivationChainWrapper(string name, INamedTypeSymbol parentSymbol, ContextSymbolWrapper context)
        {
            Name = name;
            Parent = parentSymbol;
            Context = context;
        }

        public ChainContributionType ContributionType => ChainContributionType.Instance;
        public bool IsWithPriorities => true;

        public INamedTypeSymbol GetChainType() => RelevantSymbols.Chain;

        public string GetTypeText() 
            => $"{GetChainType().Name}<{Context.symbol.GetFullyQualifiedName()}>";
    }

    public class ChainSymbolWrapper : FieldSymbolWrapper, IChainWrapper, IThing
    {
        private ChainAttribute _attribute;
        public ContextSymbolWrapper Context { get; private set; }

        public ChainContributionType ContributionType => _attribute.Type;
        public new string Name => _attribute.Name;
        public INamedTypeSymbol Parent => symbol.ContainingType;
        public string FieldName => symbol.Name;
        public bool IsWithPriorities => GetChainType() == RelevantSymbols.Chain;

        public string Identity => $"{Name} Chain{this.GetPrefix()}";
        public string Location => symbol.Locations.First().ToString();

        
        public ChainSymbolWrapper(IFieldSymbol symbol, ChainAttribute chainAttribute) : base(symbol)
        {
            _attribute = chainAttribute;
        }

        public INamedTypeSymbol GetChainType()
        {
            var t = (INamedTypeSymbol) symbol.Type;

            return symbol.IsStatic 
                // We assume it is an index
                // So the inner type of the type is the chain type
                ? (INamedTypeSymbol) t.TypeArguments.Single()
                : t;
        }

        public bool Init(GenerationEnvironment env)
        {
            // 1. Lookup the type of the context. It is the most nested generic type of the index type.
            var contextType = (INamedTypeSymbol) symbol.Type.GetLeafTypeArguments().First();

            // 2. See if that context has already been cached in the environment.
            // 3. Initialize the context if it is not found.
            if (!env.TryGetContextLazy(contextType, out var context)) return false;

            Context = context;

            return true;
        }

        public string GetTypeText() => 
            $"{GetChainType().GetFullyQualifiedName()}<{Context.symbol.GetFullyQualifiedName()}>";

    }
}