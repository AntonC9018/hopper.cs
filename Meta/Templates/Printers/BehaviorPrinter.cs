using System.Collections.Generic;
using Hopper.Meta;

namespace Hopper.Meta.Template
{
    public partial class BehaviorPrinter
    {
        public readonly BehaviorSymbolWrapper behavior;

        public BehaviorPrinter(BehaviorSymbolWrapper behavior)
        {
            this.behavior = behavior;
        }

        public BehaviorSymbolWrapper component => behavior;
        public IEnumerable<ExportedMethodSymbolWrapper> ExportedMethods => behavior.exportedMethods;
        public IEnumerable<string> Usings() => behavior.Usings();
        public IEnumerable<ChainSymbolWrapper> MoreChains => behavior.contributedChains;
    }
}