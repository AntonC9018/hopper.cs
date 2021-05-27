using System.Collections.Generic;
using System.Linq;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class ExportedStuffPrinter
    {
        public ExportedStuffClassSymbolWrapper container;

        public ExportedStuffPrinter(ExportedStuffClassSymbolWrapper methodClass)
        {
            this.container = methodClass;
        }

        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => 
            container.exportedMethods.Where(m => m.symbol.IsStatic);
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedInstanceMethods => 
            container.exportedMethods.Where(m => !m.symbol.IsStatic);
        public IEnumerable<string> Usings() => container.Usings();
        public IEnumerable<ChainSymbolWrapper> MoreChains => container.contributedChains;
    }
}