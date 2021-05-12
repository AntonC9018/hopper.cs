using System.Collections.Generic;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class ComponentPrinter
    {
        public ComponentSymbolWrapper component;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => component.exportedMethods;
        public IEnumerable<string> Usings() => component.Usings();
    }
}