using Microsoft.CodeAnalysis;
using System.Linq;

namespace Hopper.Meta
{
    public class EntityTypeWrapper : TypeSymbolWrapperBase
    {
        public EntityTypeWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "Entity Type";
    }
}