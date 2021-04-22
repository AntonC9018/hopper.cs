using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class SlotSymbolWrapper
    {
        public IFieldSymbol symbol;

        public SlotSymbolWrapper(IFieldSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.GetFullName();
        public string FullyQualifiedName => $"{symbol.GetFullQualification()}.{Name}";
    }
}