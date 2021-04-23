using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class FieldSymbolWrapper
    {
        public IFieldSymbol symbol;

        public FieldSymbolWrapper(IFieldSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string Namespace => symbol.ContainingNamespace.GetFullName();
        public string FullyQualifiedName => $"{symbol.GetFullQualification()}.{Name}";
    }
}