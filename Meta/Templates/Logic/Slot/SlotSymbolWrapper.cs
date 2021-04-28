using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class SlotSymbolWrapper : FieldSymbolWrapper
    {
        public SlotAttribute attribute;

        public SlotSymbolWrapper(IFieldSymbol symbol, SlotAttribute attribute) : base(symbol)
        {
            this.attribute = attribute;
        }

        public bool ShouldGenerate => attribute.Name != null;
        public string SlotName => attribute.Name;
    }
}