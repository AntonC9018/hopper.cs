using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;

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
        public HashSet<string> entities;

        public IEnumerable<KeyValuePair<string, IFieldSymbol>> NotOmitted() 
            => fieldsHashed.Where(kvp => !ShouldBeOmitted(kvp.Key));

        public string Params()
        {
            return String.Join(", ", NotOmitted().Select(kvp => kvp.Value.ToDisplayString()));
        }

        public IEnumerable<string> ParamNames()
        {
            return NotOmitted().Select(kvp => kvp.Key);
        }

        public void HashFields()
        {
            var ctx_fields = new List<IFieldSymbol>();

            {
                var s = symbol;
                do 
                {
                    ctx_fields.AddRange(s
                        .GetMembers().OfType<IFieldSymbol>()
                        .Where(field => !field.IsStatic && !field.IsConst));
                    s = symbol.BaseType;
                }
                while (s != null);
            }

            fieldsHashed = ctx_fields.ToDictionary(field => field.Name);
            entities = ctx_fields
                .Where(field => SymbolEqualityComparer.Default.Equals(field.ContainingType, RelevantSymbols.Instance.entity))
                .Select(field => field.Name).ToHashSet();

            // I cannot figure out how to check if the field was given a default value, so
            // I'm going to do this with an attribute instead
            omitted = ctx_fields.Where(field => field.GetAttributes()
                .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.omitAttribute)))
                .Select(field => field.Name).ToHashSet();
        }

        public bool ContainsEntity(string name) => entities.Contains(name);
        public bool ContainsFieldWithNameAndType(string name, ITypeSymbol type) 
        {
            return fieldsHashed.TryGetValue(name, out var t) && 
                SymbolEqualityComparer.Default.Equals(type, t.Type);
        }
        public bool ShouldBeOmitted(string name) => omitted.Contains(name);
    }
}