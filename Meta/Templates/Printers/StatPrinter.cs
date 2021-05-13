using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class StatPrinter
    {
        public StatType stat;

        public StatPrinter(StatType stat)
        {
            this.stat = stat;
        }

        public StatPrinter(){}

    }

    public partial class StatStartPrinter
    {
        public StatPrinter statPrinter;
        public string Namespace;

        public StatStartPrinter(string rootNamespace)
        {
            this.statPrinter = new StatPrinter();
            this.Namespace = $"{rootNamespace}.Stat";
        }

        public void ResetStat(StatType stat)
        {
            this.statPrinter.stat = stat;
        }
    }
}