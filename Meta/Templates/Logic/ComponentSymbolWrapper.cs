using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Meta
{

    public class ComponentSymbolWrapper : NamedTypeSymbolWrapper
    {   
        public HashSet<IFieldSymbol> flaggedFields;
        private AliasMethodSymbolWrapper[] aliasMethods;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol, HashSet<string> globalAliases) : base(symbol)
        {
            Init(globalAliases);
        }

        private void Init(HashSet<string> globalAliases)
        {
            flaggedFields = GetFlaggedFields();
            aliasMethods = GetAliasMethods(globalAliases);
        }

        public override string TypeText() => "component";
    }
}