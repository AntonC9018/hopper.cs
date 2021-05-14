using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;

namespace Hopper.Meta
{
    public class ContextSymbolWrapper
    {
        public INamedTypeSymbol symbol;

        public ContextSymbolWrapper(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
            
            fieldsHashed = new Dictionary<string, IFieldSymbol>();
            omitted = new HashSet<string>();
            notOmitted = new List<IFieldSymbol>();
        }

        public bool TryInit(GenerationEnvironment env) => TryHashFields(env);

        private Dictionary<string, IFieldSymbol> fieldsHashed;
        private HashSet<string> omitted;
        public List<IFieldSymbol> notOmitted;
        public string ActorName { get; private set; }
        public bool IsActorAField => fieldsHashed.ContainsKey(ActorName);

        /// <summary>
        /// This function is independent of the type the Context is defined in.
        /// This function hashes fields for faster lookups.
        /// It also caches the necessary fields for convenient iteration.
        /// </summary>
        private bool TryHashFields(GenerationEnvironment env)
        {
            if (symbol.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == "actor"))
            {
                ActorName = "actor";
            }

            foreach (var s in symbol.GetTypeHierarchy())
            foreach (var field in s.GetInstanceFields())
            {
                fieldsHashed.Add(field.Name, field);

                if (field.Name == "actor" && field.Type == RelevantSymbols.entity)
                {
                    ActorName = "actor";
                    omitted.Add(field.Name);
                }
                else if (field.HasAttribute(RelevantSymbols.OmitAttribute.symbol) 
                    || field.IsImplicitlyDeclared)
                {
                    omitted.Add(field.Name);
                }
                else
                {
                    if (ActorName is null && field.Type == RelevantSymbols.entity)
                    {
                        ActorName = field.Name;
                    }
                    else
                    {
                        notOmitted.Add(field);
                    }
                }
            }

            if (ActorName is null) 
            {
                env.ReportError("The context class must contain a field of type \"Entity\", representing the entity, or a property with name \"actor\"");
                return false;
            }

            return true;
        }

        public bool ContainsEntity(string name) 
            => ContainsFieldWithNameAndType(name, RelevantSymbols.entity)
                || name == ActorName;

        public bool ContainsFieldWithNameAndType(string name, ITypeSymbol type) 
            => fieldsHashed.TryGetValue(name, out var fieldSymbol)
                && SymbolEqualityComparer.Default.Equals(fieldSymbol.Type, type);

        public bool ShouldBeOmitted(string name) => omitted.Contains(name);

        public string ParentClassName => symbol.ContainingType.Name;
        public string Name => symbol.Name;
        public string NameWithParentClass => $"{ParentClassName}.{Name}";


        /* Things mainly called in the template */
        public string JoinedParamsWithActor() => String.Join(", ", ParamsWithActor());
        public IEnumerable<string> ParamsWithActor()
        {
            yield return $"Entity {ActorName}";
            foreach (var p in ParamNamesWithTypes()) yield return p;
        }
        public IEnumerable<string> ParamNamesWithTypes() => notOmitted.Select(f => $"{f.Type.TypeToText()} {f.Name}");
        public IEnumerable<string> ParamNames() => notOmitted.Select(f => f.Name);
        
        public IEnumerable<string> ParamNamesWithActor() 
        {
            yield return ActorName;
            foreach (var p in ParamNames()) yield return p;
        }
        public string JoinedParamNamesWithActor() => String.Join(", ", ParamNamesWithActor());

        public string JoinedParamTypeNames() => notOmitted.CommaJoin(f => f.Type.Name);
    }
}