using Microsoft.CodeAnalysis;
using System.Linq;

namespace Meta
{
    public class EntityTypeWrapper : ClassSymbolWrapperBase
    {
        public EntityTypeWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "Entity Type";
    }
}