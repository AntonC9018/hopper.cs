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

        public static INamedTypeSymbol GetAttributeSymbol(Compilation compilation, string name)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Shared.Attributes.{name}");
        }

        public void Init(Compilation compilation)
        {
            entity = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Entity");
            icomponent      = GetComponentSymbol(compilation, "IComponent");
            ibehavior       = GetComponentSymbol(compilation, "IBehavior");
            itag            = GetComponentSymbol(compilation, "IBehavior");
            aliasAttribute  = GetAttributeSymbol(compilation, "AliasAttribute");
            chainsAttribute = GetAttributeSymbol(compilation, "ChainsAttribute");
            injectAttribute = GetAttributeSymbol(compilation, "InjectAttribute");
            flagsAttribute  = GetAttributeSymbol(compilation, "FlagsAttribute");
            exportAttribute = GetAttributeSymbol(compilation, "ExportAttribute");
            omitAttribute   = GetAttributeSymbol(compilation, "OmitAttribute");
            activationAliasAttribute = GetAttributeSymbol(compilation, "ActivationAliasAttribute");
            autoActivationAttribute = GetAttributeSymbol(compilation, "AutoActivationAttribute");
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
            voidType = compilation.GetSpecialType(SpecialType.System_Void);
        }
    }
}