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

        new public void Init(GlobalContext ctx)
        {
            exportedMethods = GetNonNativeExportedMethods(ctx).ToArray();
            
            // If there are no exported methods, this class is unusable anyway
            // and should be either given another symbol, or thrown away.
            if (exportedMethods.Length > 0)
                base.Init(ctx);
        }

        public bool ShouldGenerate() 
        {
            return exportedMethods.Length > 0;
        }

        public override string TypeText => "Static Class";
    }
}