using System.Collections.Generic;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class ComponentPrinter
    {
        public readonly ComponentSymbolWrapper component;

        public ComponentPrinter(ComponentSymbolWrapper component)
        {
            this.component = component;
        }

        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => component.exportedMethods;
        public IEnumerable<string> Usings() => component.Usings();
        public IEnumerable<ChainSymbolWrapper> MoreChains => component.moreChains;
    }
}