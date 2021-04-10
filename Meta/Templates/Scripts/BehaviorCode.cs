using System.Collections.Generic;
using Meta;

namespace Hopper.Meta.Template
{
    public partial class BehaviorCode
    {
        public BehaviorSymbolWrapper behavior;
        public BehaviorSymbolWrapper component => behavior;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => behavior.exportedMethods;
        public IEnumerable<string> Usings() => behavior.Usings();
    }
}