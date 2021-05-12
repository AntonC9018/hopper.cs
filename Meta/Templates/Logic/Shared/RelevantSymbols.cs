using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    // Singleton
    public static partial class RelevantSymbols
    {
        private static bool IsInited;

        public static INamedTypeSymbol entity;
        public static INamedTypeSymbol icopyable;
        public static INamedTypeSymbol IComponent;
        public static INamedTypeSymbol IBehavior;
        public static INamedTypeSymbol ITag;
        public static AttributeSymbolWrapper<AliasAttribute> aliasAttribute;
        public static AttributeSymbolWrapper<ActivationAliasAttribute> ActivationAliasAttribute;
        public static AttributeSymbolWrapper<AutoActivationAttribute> AutoActivationAttribute;
        public static AttributeSymbolWrapper<NoActivationAttribute> NoActivationAttribute;
        public static AttributeSymbolWrapper<ChainsAttribute> ChainsAttribute;
        public static AttributeSymbolWrapper<InjectAttribute> InjectAttribute;
        public static AttributeSymbolWrapper<FlagsAttribute> FlagsAttribute;
        public static AttributeSymbolWrapper<ExportAttribute> ExportAttribute;
        public static AttributeSymbolWrapper<OmitAttribute> omitAttribute;
        public static AttributeSymbolWrapper<EntityTypeAttribute> EntityTypeAttribute;
        public static AttributeSymbolWrapper<SlotAttribute> SlotAttribute;
        public static AttributeSymbolWrapper<InstanceExportAttribute> InstanceExportAttribute;
        public static AttributeSymbolWrapper<RequiringInitAttribute> RequiringInitAttribute;
        public static AttributeSymbolWrapper<IdentifyingStatAttribute> IdentifyingStatAttribute;
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

        public static INamedTypeSymbol GetKnownSymbol(Compilation compilation, System.Type t)
        {
            return (INamedTypeSymbol)compilation.GetTypeByMetadataName(t.FullName);
        }

        /// <summary>
        /// Store all of the symbols used in wrapper logic.
        /// These include all of the shared attributes as well as some types from Core. 
        /// This means this function must be called only after the Core project has been loaded.
        /// </summary>
        private static void Init(Compilation compilation)
        {
            entity = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.Entity");
            icopyable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Utils.ICopyable");
            istandartActivateable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IStandartActivateable");
            ipredictable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IPredictable");
            iundirectedActivateable = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IUndirectedActivateable");
            iundirectedPredictable  = (INamedTypeSymbol)compilation.GetTypeByMetadataName($"Hopper.Core.ActingNS.IUndirectedPredictable");
            IComponent               = GetComponentSymbol(compilation, "IComponent");
            IBehavior                = GetComponentSymbol(compilation, "IBehavior");
            ITag                     = GetComponentSymbol(compilation, "ITag");
            aliasAttribute          .Init(compilation);
            ActivationAliasAttribute.Init(compilation);
            AutoActivationAttribute .Init(compilation);
            NoActivationAttribute   .Init(compilation);
            ChainsAttribute         .Init(compilation);
            InjectAttribute         .Init(compilation);
            FlagsAttribute          .Init(compilation);
            ExportAttribute         .Init(compilation);
            omitAttribute           .Init(compilation);
            EntityTypeAttribute     .Init(compilation);
            SlotAttribute           .Init(compilation);
            InstanceExportAttribute .Init(compilation);
            RequiringInitAttribute  .Init(compilation);
            IdentifyingStatAttribute.Init(compilation);
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
            voidType = compilation.GetSpecialType(SpecialType.System_Void);
        }
    }
}