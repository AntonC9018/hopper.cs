using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    // Singleton
    public static class RelevantSymbols
    {
        private static bool IsInited;

        public static INamedTypeSymbol entity;
        public static INamedTypeSymbol icopyable;
        public static INamedTypeSymbol icomponent;
        public static INamedTypeSymbol ibehavior;
        public static INamedTypeSymbol itag;
        public static INamedTypeSymbol aliasAttribute;
        public static INamedTypeSymbol activationAliasAttribute;
        public static INamedTypeSymbol autoActivationAttribute;
        public static INamedTypeSymbol noActivationAttribute;
        public static INamedTypeSymbol chainsAttribute;
        public static INamedTypeSymbol injectAttribute;
        public static INamedTypeSymbol flagsAttribute;
        public static INamedTypeSymbol exportAttribute;
        public static INamedTypeSymbol omitAttribute;
        public static INamedTypeSymbol entityTypeAttribute;
        public static INamedTypeSymbol slotAttribute;
        public static INamedTypeSymbol instanceExportAttribute;
        public static INamedTypeSymbol requiringInitAttribute;
        public static INamedTypeSymbol identifyingStatAttribute;

        public static INamedTypeSymbol boolType;
        public static INamedTypeSymbol voidType;
        public static INamedTypeSymbol istandartActivateable;
        public static INamedTypeSymbol ipredictable;
        public static INamedTypeSymbol iundirectedActivateable;
        public static INamedTypeSymbol iundirectedPredictable;

        public static void TryInitializeSingleton(Compilation compilation)
        {
            if (!IsInited)
            {
                Init(compilation);
                IsInited = true;
            }
        }
        
        private static INamedTypeSymbol GetComponentSymbol(Compilation compilation, string name)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Components.{name}");
        }

        private static INamedTypeSymbol GetKnownSymbol(Compilation compilation, System.Type t)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName(t.FullName);
        }

        private static void Init(Compilation compilation)
        {
            entity = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Entity");
            icopyable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Utils.ICopyable");
            istandartActivateable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IStandartActivateable");
            ipredictable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IPredictable");
            iundirectedActivateable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IUndirectedActivateable");
            iundirectedPredictable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IUndirectedPredictable");
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
            autoActivationAttribute  = GetKnownSymbol(compilation, typeof(AutoActivationAttribute));
            noActivationAttribute    = GetKnownSymbol(compilation, typeof(NoActivationAttribute));
            entityTypeAttribute      = GetKnownSymbol(compilation, typeof(EntityTypeAttribute));
            instanceExportAttribute  = GetKnownSymbol(compilation, typeof(InstanceExportAttribute));
            requiringInitAttribute   = GetKnownSymbol(compilation, typeof(RequiringInitAttribute));
            identifyingStatAttribute = GetKnownSymbol(compilation, typeof(IdentifyingStatAttribute));
            slotAttribute = GetKnownSymbol(compilation, typeof(SlotAttribute));
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
            voidType = compilation.GetSpecialType(SpecialType.System_Void);
        }
    }
}