using System.Collections.Generic;
using Meta;

namespace Hopper.Meta.Template
{
    public partial class ChainHandlersCode
    {
        public StaticClassSymbolWrapper staticClass;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => staticClass.exportedMethods;
    }
}