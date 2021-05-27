using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public struct AttributeSymbolWrapper<T>
    {
        public INamedTypeSymbol symbol;

        private static INamedTypeSymbol GetKnownSymbol(Compilation compilation, System.Type t)
        {
            return (INamedTypeSymbol) compilation.GetTypeByMetadataName(t.FullName);
        }

        public void Init(Compilation compilation)
        {
            symbol = GetKnownSymbol(compilation, typeof(T));
        }
    }
}