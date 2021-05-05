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
        }

        public void Init()
        {
            HashFields();
        }

        public Dictionary<string, IFieldSymbol> fieldsHashed;
        public HashSet<string> omitted;
        public List<IFieldSymbol> notOmitted;
        public string ActorName;

        public void HashFields()
        {
            fieldsHashed = new Dictionary<string, IFieldSymbol>();
            omitted = new HashSet<string>();
            notOmitted = new List<IFieldSymbol>();

            if (symbol.GetMembers().OfType<IPropertySymbol>().Any(p => p.Name == "actor"))
            {
                ActorName = "actor";
            }

            foreach (var s in symbol.GetTypeHierarchy())
            foreach (var field in s.GetInstanceFields())
            {
                fieldsHashed.Add(field.Name, field);

                if (field.Name == "actor")
                {
                    ActorName = "actor";
                    omitted.Add(field.Name);
                }
                else if (field.HasAttribute(RelevantSymbols.omitAttribute) || field.Name == "propagate")
                {
                    omitted.Add(field.Name);
                }
                else
                {
                    if (ActorName == null && field.Type == RelevantSymbols.entity)
                    {
                        ActorName = field.Name;
                    }
                    else
                    {
                        notOmitted.Add(field);
                    }
                }
            }

            if (ActorName == null) 
            {
                throw new GeneratorException("The context class must contain a field of type \"Entity\", representing the entity, or a property with name \"actor\"");
            }
        }

        public bool ContainsEntity(string name) => fieldsHashed.TryGetValue(name, out var t) 
            && SymbolEqualityComparer.Default.Equals(t, RelevantSymbols.entity)
            || name == ActorName;
        public bool ContainsFieldWithNameAndType(string name, ITypeSymbol type) 
        {
            if (fieldsHashed.TryGetValue(name, out var fieldSymbol))
            {
                return SymbolEqualityComparer.Default.Equals(fieldSymbol.Type, type);
            }
            return false;
        }
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