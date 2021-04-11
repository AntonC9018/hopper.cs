using System.Collections.Generic;
using Meta;

namespace Hopper.Meta.Template
{
    public partial class ComponentCode
    {
        public ComponentSymbolWrapper component;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => component.exportedMethods;
        public IEnumerable<string> Usings() => component.Usings();
    }
}