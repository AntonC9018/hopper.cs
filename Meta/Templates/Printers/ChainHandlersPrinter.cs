using System.Collections.Generic;
using System.Linq;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class ChainHandlersPrinter
    {
        public ExportedMethodsClassSymbolWrapper methodClass;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => 
            methodClass.exportedMethods.Where(m => m.symbol.IsStatic);

        public IEnumerable<ExportedMethodSymbolWrapper> ExportedInstanceMethods => 
            methodClass.exportedMethods.Where(m => !m.symbol.IsStatic);

        public IEnumerable<string> Usings() => methodClass.Usings();
    }
}