using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class ExportedStuffClassSymbolWrapper : TypeSymbolWrapperBase
    {
        public ExportedStuffClassSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        protected override bool Init(GenerationEnvironment env)
        {
            // Workaround: If it is marked as ExportingClass, do after init after all inits.
            if (symbol.HasAttribute(RelevantSymbols.ExportingClassAttribute.symbol))
            {
                return base.Init(env) && env.TryAddExportingClass(this);
            }

            // If there are no exported methods, this class is unusable anyway
            // and should be either given another symbol, or thrown away.
            return base.Init(env) && base.AfterInit(env) 
                && (exportedMethods.Length > 0 || contributedChains.Length > 0)
                && env.TryAddExportingClass(this);
        }

        protected override bool AfterInit(GenerationEnvironment env)
        {
            return base.AfterInit(env);
        }

        public override string TypeText => "exporting class other";
    }
}