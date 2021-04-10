
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Meta
{
    public class StaticClassSymbolWrapper : ClassSymbolWrapperBase
    {
        public ExportedMethodSymbolWrapper[] exportedMethods;

        public StaticClassSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        new public void Init(ProjectContext ctx)
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