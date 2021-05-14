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
            // If there are no exported methods, this class is unusable anyway
            // and should be either given another symbol, or thrown away.
            return base.Init(env) && base.AfterInit(env) 
                && (exportedMethods.Length > 0 || moreChains.Length > 0);
        }

        public override string TypeText => "Static Class";
    }
}