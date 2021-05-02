using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hopper.Meta
{
    public class ComponentSymbolWrapper : TypeSymbolWrapperBase
    {   
        public ExportedMethodSymbolWrapper[] exportedMethods;
        public HashSet<IFieldSymbol> flaggedFields;
        public AliasMethodSymbolWrapper[] aliasMethods;
        public InjectedFieldSymbolWrapper[] injectedFields;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        public override void Init(ProjectContext projectContext)
        {
            base.Init(projectContext);
            if (symbol.HasAttribute(RelevantSymbols.instanceExportAttribute))
            {
                throw new GeneratorException($"Components must not have the {RelevantSymbols.instanceExportAttribute.Name} attribute since its usage is ambiguous for components.");
            }
            flaggedFields = GetFlaggedFields();
            aliasMethods = GetAliasMethods(projectContext.globalAliases);
            injectedFields = GetInjectedFields().ToArray();
        }

        public override void AfterInit(ProjectContext projectContext)
        {
            if (exportedMethods == null)
                exportedMethods = GetNonNativeExportedMethods(projectContext).ToArray();
        }

        public IEnumerable<InjectedFieldSymbolWrapper> GetInjectedFields()
        {
            return symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.GetAttributes().Any(a => 
                    SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.injectAttribute)))
                .Select(f => new InjectedFieldSymbolWrapper(f));
        }

        public override string TypeText => "component";

        public IEnumerable<string> InjectedParamNames => injectedFields.Select(f => f.Name);
        public IEnumerable<string> InjectedParamsWithTypes => injectedFields.Select(f => $"{f.TypeName} {f.Name}");
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