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


        public string ParamsWithActor()
        {
            if (notOmitted.Count > 0)
            {
                if (SymbolEqualityComparer.Default.Equals(notOmitted[0].Type, RelevantSymbols.Instance.entity))
                    return Params();
                else
                    return $"Entity actor, {Params()}";
            }
            return "Entity actor";
        }

        

        public string Params()
        {
            return String.Join(", ", notOmitted.Select(p => $"{((INamedTypeSymbol)p.Type).TypeToText()} {p.Name}"));
        }

        public IEnumerable<string> ParamNames()
        {
            return notOmitted.Select(p => p.Name);
        }

        public string JoinedParamNames()
        {
            return String.Join(", ", notOmitted.Select(p => p.Name));
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

                        if (field.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.omitAttribute)) || field.HasConstantValue)
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
    }
}