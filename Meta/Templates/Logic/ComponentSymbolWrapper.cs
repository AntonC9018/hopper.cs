using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Meta
{
    public class ComponentSymbolWrapper : ClassSymbolWrapperBase
    {   
        public ExportedMethodSymbolWrapper[] exportedMethods;
        public HashSet<IFieldSymbol> flaggedFields;
        public AliasMethodSymbolWrapper[] aliasMethods;
        public IFieldSymbol[] injectedFields;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        new public void Init(ProjectContext projectContext)
        {
            base.Init(projectContext);
            flaggedFields = GetFlaggedFields();
            aliasMethods = GetAliasMethods(projectContext.globalAliases);
            injectedFields = GetInjectedFields().ToArray();
        }

        public void AfterInit(ProjectContext projectContext)
        {
            if (exportedMethods == null)
                exportedMethods = GetNonNativeExportedMethods(projectContext).ToArray();
        }

        public IEnumerable<IFieldSymbol> GetInjectedFields()
        {
            return symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.GetAttributes().Any(a => 
                    SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.injectAttribute)));
        }

        public override string TypeText => "component";

        public IEnumerable<string> InjectedParamNames => injectedFields.Select(f => f.Name);
        public IEnumerable<string> InjectedParamsWithTypes => injectedFields.Select(f => $"{f.Type.TypeToText()} {f.Name}");
        public string InjectedParams => String.Join(", ", InjectedParamsWithTypes);
        public string InjectedParamJoinedNames => String.Join(", ", InjectedParamNames);
        public string GetInjectedParamsWithLeadingComma() 
        {
            var ps = InjectedParamsWithTypes;
            if (ps.Any())
            {
                return ", " + String.Join(", ", ps);
            }
            return "";
        } 

        public bool ShouldGenerateInjectConstructor => !symbol.Constructors.Any(
            ctor => !ctor.IsImplicitlyDeclared 
                && !ctor.IsStatic
                && ctor.ParameterTypesEqual(injectedFields));
        public bool ShouldGenerateCopyConstructor => !symbol.Constructors.Any(
            ctor => !ctor.IsStatic
                && ctor.Arity == 1 
                && SymbolEqualityComparer.Default.Equals(ctor.Parameters.Single(), symbol));
    }
}