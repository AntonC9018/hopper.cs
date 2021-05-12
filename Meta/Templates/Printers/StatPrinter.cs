using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class StatPrinter
    {
        public StatType stat;
    }

    public partial class StatStartPrinter
    {
        public StatPrinter statCodePrinter;
        public string Namespace;
    }
}