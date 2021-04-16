using Microsoft.CodeAnalysis;
using System.Linq;

namespace Hopper.Meta
{
    public class EntityTypeWrapper : ClassSymbolWrapperBase
    {
        public EntityTypeWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override string TypeText => "Entity Type";
    }
}