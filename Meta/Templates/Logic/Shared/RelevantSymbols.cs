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
        public static INamedTypeSymbol Chain;
        public static INamedTypeSymbol Index;
        public static AttributeSymbolWrapper<AliasAttribute> AliasAttribute;
        public static AttributeSymbolWrapper<AutoActivationAttribute> AutoActivationAttribute;
        public static AttributeSymbolWrapper<ChainAttribute> ChainAttribute;
        public static AttributeSymbolWrapper<InjectAttribute> InjectAttribute;
        public static AttributeSymbolWrapper<FlagsAttribute> FlagsAttribute;
        public static AttributeSymbolWrapper<ExportAttribute> ExportAttribute;
        public static AttributeSymbolWrapper<OmitAttribute> OmitAttribute;
        public static AttributeSymbolWrapper<EntityTypeAttribute> EntityTypeAttribute;
        public static AttributeSymbolWrapper<SlotAttribute> SlotAttribute;
        public static AttributeSymbolWrapper<InstanceExportAttribute> InstanceExportAttribute;
        public static AttributeSymbolWrapper<RequiringInitAttribute> RequiringInitAttribute;
        public static AttributeSymbolWrapper<IdentifyingStatAttribute> IdentifyingStatAttribute;
        public static AttributeSymbolWrapper<ExportingClassAttribute> ExportingClassAttribute;
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
            return (INamedTypeSymbol) compilation.GetTypeByMetadataName($"Hopper.Core.Components.{name}");
        }

        private static INamedTypeSymbol GetHopperTypeSymbol(Compilation compilation, string name)
        {
            return (INamedTypeSymbol) compilation.GetTypeByMetadataName($"Hopper.{name}");
        }

        /// <summary>
        /// Store all of the symbols used in wrapper logic.
        /// These include all of the shared attributes as well as some types from Core. 
        /// This means this function must be called only after the Core project has been loaded.
        /// </summary>
        private static void Init(Compilation compilation)
        {
            entity                   = GetHopperTypeSymbol(compilation, "Core.Entity");
            icopyable                = GetHopperTypeSymbol(compilation, "Utils.ICopyable");
            istandartActivateable    = GetHopperTypeSymbol(compilation, "Core.ActingNS.IStandartActivateable");
            ipredictable             = GetHopperTypeSymbol(compilation, "Core.ActingNS.IPredictable");
            iundirectedActivateable  = GetHopperTypeSymbol(compilation, "Core.ActingNS.IUndirectedActivateable");
            iundirectedPredictable   = GetHopperTypeSymbol(compilation, "Core.ActingNS.IUndirectedPredictable");
            Chain                    = GetHopperTypeSymbol(compilation, "Utils.Chains.Chain`1");
            Index                    = GetHopperTypeSymbol(compilation, "Core.Index`1");
            IComponent               = GetComponentSymbol(compilation, nameof(IComponent));
            IBehavior                = GetComponentSymbol(compilation, nameof(IBehavior));
            ITag                     = GetComponentSymbol(compilation, nameof(ITag));
            AliasAttribute          .Init(compilation);
            AutoActivationAttribute .Init(compilation); 
            ChainAttribute          .Init(compilation);
            InjectAttribute         .Init(compilation);
            FlagsAttribute          .Init(compilation);
            ExportAttribute         .Init(compilation);
            OmitAttribute           .Init(compilation);
            EntityTypeAttribute     .Init(compilation);
            SlotAttribute           .Init(compilation);
            InstanceExportAttribute .Init(compilation);
            RequiringInitAttribute  .Init(compilation);
            IdentifyingStatAttribute.Init(compilation);
            ExportingClassAttribute .Init(compilation);
            boolType = compilation.GetSpecialType(SpecialType.System_Boolean);
            voidType = compilation.GetSpecialType(SpecialType.System_Void);
        }
    }
}