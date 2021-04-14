using Microsoft.CodeAnalysis;

namespace Meta
{
    public class TagSymbolWrapper : ComponentSymbolWrapper
    {
        public TagSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "tag";
        public override string HasAlias => "Is";
    }
}