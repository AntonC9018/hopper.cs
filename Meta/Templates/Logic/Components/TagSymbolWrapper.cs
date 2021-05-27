using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class TagSymbolWrapper : ComponentSymbolWrapper
    {
        public TagSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "tag";
        public override string AliasOfHas => "Is";
    }
}