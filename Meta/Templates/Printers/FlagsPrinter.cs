using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class FlagsPrinter
    {
        public FlagEnumSymbolWrapper flag;

        public FlagsPrinter(FlagEnumSymbolWrapper flag)
        {
            this.flag = flag;
        }

        public void Reset(FlagEnumSymbolWrapper flag)
        {
            this.flag = flag;
        }

        public IEnumerable<string> Usings() => flag.Usings();
    }
}