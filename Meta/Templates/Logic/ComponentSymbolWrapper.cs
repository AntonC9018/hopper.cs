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
        public IEnumerable<UsingDirectiveSyntax> usings;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol, ProjectContext projectContext) : base(symbol)
        {
            Init(projectContext);
        }

        private void Init(ProjectContext projectContext)
        {
            flaggedFields = GetFlaggedFields();
            aliasMethods = GetAliasMethods(projectContext.globalAliases);
            usings = GetUsingSyntax(projectContext._solution);
        }

        public IEnumerable<string> Usings()
        {
            return usings.Select(n => n.ToString());
        }

        public override string TypeText => "component";
    }
}