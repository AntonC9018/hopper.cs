using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public sealed class AliasMethodSymbolWrapper
    {
        public IMethodSymbol _symbol;
        public string _alias;

        public AliasMethodSymbolWrapper(IMethodSymbol symbol, string alias)
        {
            _symbol = symbol;
            _alias = alias;
        }

        /* Things mainly called in the template */
        public string Alias => _alias;
        public string Name => _symbol.Name;
        public bool ReturnTypeIsVoid() => SymbolEqualityComparer.Default.Equals(_symbol.ReturnType, RelevantSymbols.voidType);  
        public string ReturnType => ReturnTypeIsVoid() ? "void" : ((INamedTypeSymbol)_symbol.ReturnType).TypeToText();
        public string ParamsWithActor() => _symbol.Parameters.ParamsWithActor();
        public string Params() => _symbol.Parameters.Params();
        public IEnumerable<string> ParamNames() => _symbol.Parameters.ParamNames();
        public string JoinedParamNames() => _symbol.Parameters.JoinedParamNames();
        public IEnumerable<string> ParamTypeNames() => _symbol.Parameters.ParamTypeNames();
        public string JoinedParamTypeNames() => _symbol.Parameters.JoinedParamTypeNames();
    }
}