
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

        public void Init(ProjectContext ctx)
        {
            exportedMethods = GetNonNativeExportedMethods(ctx).ToArray();
        }

        public override string TypeText => "Static Class";

    }
}