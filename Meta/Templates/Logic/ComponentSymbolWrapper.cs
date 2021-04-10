using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Meta
{
    public class ComponentSymbolWrapper : ClassSymbolWrapperBase
    {   
        public HashSet<IFieldSymbol> flaggedFields;
        public AliasMethodSymbolWrapper[] aliasMethods;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        new public void Init(ProjectContext projectContext)
        {
            base.Init(projectContext);
            flaggedFields = GetFlaggedFields();
            aliasMethods = GetAliasMethods(projectContext.globalAliases);
        }


        public override string TypeText => "component";
    }
}