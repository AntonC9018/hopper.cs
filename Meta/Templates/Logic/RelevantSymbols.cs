using System;
using Microsoft.CodeAnalysis;

namespace Meta
{
    // Singleton
    public class RelevantSymbols
    {
        public static RelevantSymbols Instance; 

        public INamedTypeSymbol entity;
        public INamedTypeSymbol icomponent;
        public INamedTypeSymbol ibehavior;
        public INamedTypeSymbol itag;
        public INamedTypeSymbol aliasAttribute;
        public INamedTypeSymbol activationAliasAttribute;
        public INamedTypeSymbol autoActivationAttribute;
        public INamedTypeSymbol chainsAttribute;
        public INamedTypeSymbol injectAttribute;
        public INamedTypeSymbol flagsAttribute;
        public INamedTypeSymbol exportAttribute;
        public INamedTypeSymbol omitAttribute;

        public INamedTypeSymbol boolType;

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
            activationAliasAttribute  = GetComponentSymbol(compilation, "ActivationAliasAttribute");
            autoActivationAttribute = GetComponentSymbol(compilation, "AutoActivationAttribute");
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
        }
    }
}