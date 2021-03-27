using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Meta
{
    public class ContextSymbolWrapper
    {
        public INamedTypeSymbol symbol;

        public ContextSymbolWrapper(INamedTypeSymbol symbol)
        {
            this.symbol = symbol;
            HashFields();
        }

        public Dictionary<string, IFieldSymbol> fieldsHashed;
        public HashSet<string> omitted;
        public List<IFieldSymbol> notOmitted;


        public string ParamsWithActor()
        {
            if (notOmitted.Count > 0 && SymbolEqualityComparer.Default.Equals(
                notOmitted[0].Type, RelevantSymbols.Instance.entity))
            {
                return Params();
            }
            return $"Entity actor, {Params()}";
        }

        public string Params()
        {
            return String.Join(", ", notOmitted.Select(p => p.ToDisplayString()));
        }

        public IEnumerable<string> ParamNames()
        {
            return notOmitted.Select(p => p.Name);
        }

        public IEnumerable<string> ParamTypeNames()
        {
            return notOmitted.Select(p => p.Type.Name);
        }

        public string JoinedParamTypeNames()
        {
            return String.Join(", ", ParamTypeNames());
        }

        public void HashFields()
        {
            fieldsHashed = new Dictionary<string, IFieldSymbol>();

            {
                var s = symbol;
                do 
                {
                    foreach (var field in s
                        .GetMembers().OfType<IFieldSymbol>()
                        .Where(f => !f.IsStatic && !f.IsConst))
                    {
                        fieldsHashed.Add(field.Name, field);

                        if (field.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.omitAttribute)) || !field.HasConstantValue)
                        {
                            omitted.Add(field.Name);
                        }
                        else
                        {
                            notOmitted.Add(field);
                        }
                    }
                    s = symbol.BaseType;
                }
                while (s != null);
            }
        }

        public bool ContainsEntity(string name) => fieldsHashed.TryGetValue(name, out var t) 
            && SymbolEqualityComparer.Default.Equals(t, RelevantSymbols.Instance.entity);
        public bool ContainsFieldWithNameAndType(string name, ITypeSymbol type) 
        {
            return fieldsHashed.TryGetValue(name, out var t) && 
                SymbolEqualityComparer.Default.Equals(type, t.Type);
        }
        public bool ShouldBeOmitted(string name) => omitted.Contains(name);
    }
}