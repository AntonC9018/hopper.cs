using Microsoft.CodeAnalysis;
using System.Linq;

namespace Hopper.Meta
{
    public struct InjectedFieldSymbolWrapper
    {
        public IFieldSymbol symbol;

        public InjectedFieldSymbolWrapper(IFieldSymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Name => symbol.Name;
        public string TypeName => symbol.Type.TypeToText();
        public bool IsCopyable => symbol.Type.AllInterfaces.Any(i => 
            SymbolEqualityComparer.Default.Equals(i, RelevantSymbols.icopyable));
    }
}