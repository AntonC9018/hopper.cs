using System.Collections.Generic;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class ChainHandlersCode
    {
        public StaticClassSymbolWrapper staticClass;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => staticClass.exportedMethods;
        public IEnumerable<string> Usings() => staticClass.Usings();
    }
}