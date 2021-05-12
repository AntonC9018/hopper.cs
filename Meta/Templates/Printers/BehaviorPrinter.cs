using System.Collections.Generic;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class BehaviorPrinter
    {
        public BehaviorSymbolWrapper behavior;
        public BehaviorSymbolWrapper component => behavior;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => behavior.exportedMethods;
        public IEnumerable<string> Usings() => behavior.Usings();
    }
}