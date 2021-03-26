using System.Linq;
using Microsoft.CodeAnalysis;

namespace Meta
{
    public sealed class AliasMethodSymbolWrapper
    {
        public IMethodSymbol _symbol;
        public string _alias;

        public string Alias => _alias;
        public string MethodName => _symbol.Name;

        public AliasMethodSymbolWrapper(IMethodSymbol symbol, string alias)
        {
            _symbol = symbol;
            _alias = alias;
        }

        public string ParametersInSignature()
        {
            return string.Join(", ", _symbol.Parameters.Select(p => p.ToDisplayString()));
        }

        public string ParametersInInvocation()
        {
            return string.Join(", ", _symbol.Parameters.Select(p => p.Name));
        }
    }
}