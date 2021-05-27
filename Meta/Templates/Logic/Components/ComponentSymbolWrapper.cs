using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hopper.Meta
{
    public class ComponentSymbolWrapper : TypeSymbolWrapperBase
    {   
        public HashSet<IFieldSymbol> flaggedFields;
        public AliasMethodSymbolWrapper[] aliasMethods;
        public InjectedFieldSymbolWrapper[] injectedFields;
        public bool HasInitInWorldMethod;

        public ComponentSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        protected override bool Init(GenerationEnvironment env)
        {
            if (symbol.HasAttribute(RelevantSymbols.InstanceExportAttribute.symbol))
            {
                env.ReportError($"Components must not have the InstanceExportAttribute since its usage is ambiguous for components.");
                return false;
            }

            if (base.Init(env))
            {
                flaggedFields  = GetFlaggedFields();
                aliasMethods   = GetAliasMethods(env);
                injectedFields = GetInjectedFields().ToArray();

                HasInitInWorldMethod = symbol.GetMethods().Any(m => !m.IsStatic && m.Name == "InitInWorld");
                return env.TryAddExportingClass(this);
            }

            return false;
        }

        public IEnumerable<InjectedFieldSymbolWrapper> GetInjectedFields()
        {
            return symbol.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(f => f.HasAttribute(RelevantSymbols.InjectAttribute.symbol))
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
            ctor => ctor.Arity == 1 
                && SymbolEqualityComparer.Default.Equals(ctor.Parameters.Single(), symbol));
        
        public bool IsStandartActivateable => symbol.AllInterfaces.Contains(RelevantSymbols.istandartActivateable);
        public bool IsPredictable => symbol.AllInterfaces.Contains(RelevantSymbols.ipredictable);
        public bool IsUndirectedActivateable => symbol.AllInterfaces.Contains(RelevantSymbols.iundirectedActivateable);
        public bool IsUndirectedPredictable => symbol.AllInterfaces.Contains(RelevantSymbols.iundirectedPredictable);
    }
}