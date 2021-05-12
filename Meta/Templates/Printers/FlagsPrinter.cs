using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class FlagsPrinter
    {
        public FlagEnumSymbolWrapper flag;
        public IEnumerable<string> Usings() => flag.Usings();
    }
}