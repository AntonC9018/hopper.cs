using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class ExportedMethodsClassSymbolWrapper : TypeSymbolWrapperBase
    {
        public ExportedMethodSymbolWrapper[] exportedMethods;

        public ExportedMethodsClassSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public bool TryInit(GlobalContext ctx)
        {
            exportedMethods = GetNonNativeExportedMethods(ctx).ToArray();
            
            // If there are no exported methods, this class is unusable anyway
            // and should be either given another symbol, or thrown away.
            bool shouldGenerate = exportedMethods.Length > 0;

            if (shouldGenerate)
                base.Init(ctx);

            return shouldGenerate;
        }

        public override string TypeText => "Static Class";
    }
}