using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Meta
{
    public static class SymbolExtensions
    {
        public static string FullName(this INamespaceSymbol symbol)
        {
            Stack<string> names = new Stack<string>();

            while (symbol != null && symbol.Name != "")
            {
                names.Push(symbol.Name);
                symbol = symbol.ContainingNamespace;
            }

            return String.Join(".", names);
        }

        // TODO: This function is pretty jank. Learn the right way to do this.
        public static string TypeToText(this ITypeSymbol symbol)
        {
            var sb_type = new StringBuilder();
            TypeToTextUncertainBit(symbol, sb_type);
            return sb_type.ToString();
        }

        private static void TypeToTextUncertainBit(ITypeSymbol symbol, StringBuilder sb_type)
        {
            if (symbol is INamedTypeSymbol named_symbol)
            {
                sb_type.Append(symbol.Name);

                if (named_symbol.IsGenericType)
                {
                    sb_type.Append("<");

                    foreach (var t in named_symbol.TypeArguments)
                    {
                        sb_type.Append(TypeToText((INamedTypeSymbol)t));
                        sb_type.Append(", ");
                    }

                    sb_type.Remove(sb_type.Length - 2, 2);
                    sb_type.Append(">");
                }
            }
            else if (symbol is IArrayTypeSymbol array_symbol)
            {
                var type = array_symbol.ElementType;
                TypeToTextUncertainBit(type, sb_type);
                sb_type.Append("[]");
            }
        }

        public static string ParamsWithActor(this IEnumerable<IFieldSymbol> fields)
        {
            var first = fields.FirstOrDefault();
            if (first != null)
            {
                if (SymbolEqualityComparer.Default.Equals(first.Type, RelevantSymbols.Instance.entity))
                    return Params(fields);
                else
                    return $"Entity actor, {Params(fields)}";
            }
            return "Entity actor";
        }

        
        public static string ParamsWithActor(this IEnumerable<IParameterSymbol> parameters)
        {
            var first = parameters.FirstOrDefault();
            if (first != null)
            {
                if (SymbolEqualityComparer.Default.Equals(first.Type, RelevantSymbols.Instance.entity))
                    return Params(parameters);
                else
                    return $"Entity actor, {Params(parameters)}";
            }
            return "Entity actor";
        }

        public static string Params(this IEnumerable<IFieldSymbol> fields)
        {
            return String.Join(", ", fields.Select(p => $"{((ITypeSymbol)p.Type).TypeToText()} {p.Name}"));
        }
        

        public static string Params(this IEnumerable<IParameterSymbol> parameters)
        {
            return String.Join(", ", parameters.Select(p => $"{((ITypeSymbol)p.Type).TypeToText()} {p.Name}"));
        }

        public static IEnumerable<string> ParamNames(this IEnumerable<IFieldSymbol> fields)
        {
            return fields.Select(p => p.Name);
        }

        public static IEnumerable<string> ParamNames(this IEnumerable<IParameterSymbol> parameters)
        {
            return parameters.Select(p => p.Name);
        }

        public static IEnumerable<string> ParamTypeNames(this IEnumerable<IFieldSymbol> fields)
        {
            return fields.Select(p => p.Type.Name);
        }

        public static IEnumerable<string> ParamTypeNames(this IEnumerable<IParameterSymbol> parameters)
        {
            return parameters.Select(p => p.Name);
        }

        public static string JoinedParamNames(this IEnumerable<IFieldSymbol> fields)
        {
            return String.Join(", ", fields.Select(p => p.Name));
        }

        public static string JoinedParamNames(this IEnumerable<IParameterSymbol> parameters)
        {
            return String.Join(", ", parameters.Select(p => p.Name));
        }

        public static string JoinedParamTypeNames(this IEnumerable<IFieldSymbol> fields)
        {
            return String.Join(", ", ParamTypeNames(fields));
        }

        public static string JoinedParamTypeNames(this IEnumerable<IParameterSymbol> parameters)
        {
            return String.Join(", ", ParamTypeNames(parameters));
        }

        public static T MapToType<T>(this AttributeData attributeData) where T : Attribute
        {
            T attribute;
            if (attributeData.AttributeConstructor != null && attributeData.ConstructorArguments.Length > 0)
            {
                System.Console.WriteLine("Here-------------------------------------------");
                attribute = (T)Activator.CreateInstance(typeof(T), BindingFlags.Public, null,
                    attributeData.ConstructorArguments.Select(a => a.Value).ToArray());
            }
            else
            {
                attribute = (T)Activator.CreateInstance(typeof(T));
            }
            foreach (var p in attributeData.NamedArguments)
            {
                typeof(T).GetField(p.Key).SetValue(attribute, p.Value.Value);
            }
            return attribute;
        }

        public static bool TryGetExportAttribute(this IMethodSymbol method, out ExportAttribute attribute)
        {
            AttributeData attributeData = method.GetAttributes().FirstOrDefault(a =>
                SymbolEqualityComparer.Default.Equals(a.AttributeClass, RelevantSymbols.Instance.exportAttribute));

            if (attributeData == null)
            {
                attribute = null;
                return false;
            }

            attribute = attributeData.MapToType<ExportAttribute>();
            return true;
        }
        
        public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol symbol)
        {
            return symbol.GetMembers().OfType<IMethodSymbol>();
        }

        public static bool ParameterTypesEqual(this IMethodSymbol method, IEnumerable<IFieldSymbol> fields)
        {
            return method.Parameters.Select(m1 => m1.Type).SequenceEqual(
                fields.Select(field => field.Type), SymbolEqualityComparer.Default);
        }
    }
}