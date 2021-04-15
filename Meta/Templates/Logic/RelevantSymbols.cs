using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Meta
{
    // Singleton
    public class RelevantSymbols
    {
        public static RelevantSymbols Instance; 

        public INamedTypeSymbol entity;
        public INamedTypeSymbol icopyable;
        public INamedTypeSymbol icomponent;
        public INamedTypeSymbol ibehavior;
        public INamedTypeSymbol itag;
        public INamedTypeSymbol aliasAttribute;
        public INamedTypeSymbol activationAliasAttribute;
        public INamedTypeSymbol autoActivationAttribute;
        public INamedTypeSymbol noActivationAttribute;
        public INamedTypeSymbol chainsAttribute;
        public INamedTypeSymbol injectAttribute;
        public INamedTypeSymbol flagsAttribute;
        public INamedTypeSymbol exportAttribute;
        public INamedTypeSymbol omitAttribute;

        public INamedTypeSymbol boolType;
        public INamedTypeSymbol voidType;

        public static void TryInitializeSingleton(Compilation compilation)
        {
            if (Instance == null)
            {
                Instance = new RelevantSymbols();
                Instance.Init(compilation);
            }
        }
        
        public static INamedTypeSymbol GetComponentSymbol(Compilation compilation, string name)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Components.{name}");
        }

        public static INamedTypeSymbol GetKnownSymbol(Compilation compilation, System.Type t)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName(t.FullName);
        }

        public void Init(Compilation compilation)
        {
            entity = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Entity");
            icopyable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Utils.ICopyable");
            icomponent      = GetComponentSymbol(compilation, "IComponent");
            ibehavior       = GetComponentSymbol(compilation, "IBehavior");
            itag            = GetComponentSymbol(compilation, "ITag");
            aliasAttribute  = GetKnownSymbol(compilation, typeof(AliasAttribute));
            chainsAttribute = GetKnownSymbol(compilation, typeof(ChainsAttribute));
            injectAttribute = GetKnownSymbol(compilation, typeof(InjectAttribute));
            flagsAttribute  = GetKnownSymbol(compilation, typeof(FlagsAttribute));
            exportAttribute = GetKnownSymbol(compilation, typeof(ExportAttribute));
            omitAttribute   = GetKnownSymbol(compilation, typeof(OmitAttribute));
            activationAliasAttribute = GetKnownSymbol(compilation, typeof(ActivationAliasAttribute));
            autoActivationAttribute = GetKnownSymbol(compilation, typeof(AutoActivationAttribute));
            noActivationAttribute = GetKnownSymbol(compilation, typeof(NoActivationAttribute));
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
            voidType = compilation.GetSpecialType(SpecialType.System_Void);
        }
    }
}