using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;
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
        public List<IFieldSymbol> notOmitted;


        public void HashFields()
        {
            fieldsHashed = new Dictionary<string, IFieldSymbol>();
            omitted = new HashSet<string>();
            notOmitted = new List<IFieldSymbol>();
            {
                var stack = new Stack<INamedTypeSymbol>();

                {
                    var s = symbol;
                    do
                    {
                        stack.Push(s);
                        s = s.BaseType;
                    }
                    while (s != null); 
                }

                foreach (var s in stack)
                {
                    foreach (var field in s
                        .GetMembers().OfType<IFieldSymbol>()
                        .Where(f => !f.IsStatic && !f.IsConst))
                    {
                        fieldsHashed.Add(field.Name, field);

                        if (field.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.omitAttribute)) || field.HasConstantValue || field.Name == "propagate")
                        {
                            omitted.Add(field.Name);
                        }
                        else
                        {
                            notOmitted.Add(field);
                        }
                    }
                }
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


        /* Things mainly called in the template */
        public string ParamsWithActor() => notOmitted.ParamsWithActor();
        public string Params() => notOmitted.Params();
        public IEnumerable<string> ParamNames() => notOmitted.ParamNames();
        public string JoinedParamNames() => notOmitted.JoinedParamNames();
        public IEnumerable<string> ParamTypeNames() => notOmitted.ParamTypeNames();
        public string JoinedParamTypeNames() => notOmitted.JoinedParamTypeNames();
    }
}