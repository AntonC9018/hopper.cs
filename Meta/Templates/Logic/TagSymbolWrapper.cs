using Microsoft.CodeAnalysis;

namespace Meta
{
    public class TagSymbolWrapper : NamedTypeSymbolWrapper
    {
        public TagSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText() => "tag";
    }
}