using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class SlotExtensionsPrinter
    {
        public string Namespace;
        public IEnumerable<SlotSymbolWrapper> slots;

        public SlotExtensionsPrinter(string Namespace, IEnumerable<SlotSymbolWrapper> slots)
        {
            this.Namespace = Namespace;
            this.slots = slots;
        }
    }
}