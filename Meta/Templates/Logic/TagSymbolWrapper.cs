using Microsoft.CodeAnalysis;

namespace Meta
{
    public class TagSymbolWrapper : ComponentSymbolWrapperBase
    {
        public TagSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "tag";
    }
}