using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public struct AttributeSymbolWrapper<T>
    {
        public INamedTypeSymbol symbol;
        
        public void Init(Compilation compilation)
        {
            symbol = RelevantSymbols.GetKnownSymbol(compilation, typeof(T));
        }
    }
}