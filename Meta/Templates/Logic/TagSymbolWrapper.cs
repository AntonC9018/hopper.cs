using Microsoft.CodeAnalysis;

namespace Meta
{
    public class TagSymbolWrapper : ClassSymbolWrapperBase
    {
        public TagSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "tag";
    }
}