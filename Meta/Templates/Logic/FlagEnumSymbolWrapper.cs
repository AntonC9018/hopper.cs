using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class FlagEnumSymbolWrapper : TypeSymbolWrapperBase
    {
        public FlagEnumSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "flag enum";
    }
}