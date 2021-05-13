using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Hopper.Shared.Attributes;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public static class SymbolExtensions
    {
        public static string GetFullyQualifiedName(this ISymbol symbol)
        {
            return $"{GetFullQualification(symbol)}.{symbol.Name}";
        }

        public static string GetFullQualification(this ISymbol symbol)
        {
            Stack<string> names = new Stack<string>();

            while (symbol.ContainingType != null && symbol.ContainingType.Name != "")
            {
                names.Push(symbol.ContainingType.Name);
                symbol = symbol.ContainingType;
            }

            foreach (var n in symbol.ContainingNamespace.GetNamespaceNames())
            {
                names.Push(n);
            }

            return String.Join(".", names);
        }

        public static string GetTypeQualification(this ISymbol symbol)
        {
            Stack<string> names = new Stack<string>();

            while (symbol.ContainingType != null && symbol.ContainingType.Name != "")
            {
                names.Push(symbol.ContainingType.Name);
                symbol = symbol.ContainingType;
            }

            return String.Join(".", names);
        }

        public static string GetFullName(this INamespaceSymbol symbol)
        {
            Stack<string> names = new Stack<string>();

            foreach (var n in symbol.GetNamespaceNames())
            {
                names.Push(n);
            }

            return String.Join(".", names);
        }

        public static IEnumerable<string> GetNamespaceNames(this INamespaceSymbol symbol)
        {
            while (symbol != null && symbol.Name != "")
            {
                yield return symbol.Name;
                symbol = symbol.ContainingNamespace;
            }
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

        public static string ParamsWithoutActor(this IEnumerable<IFieldSymbol> fields)
        {
            if (!fields.Any())
            {
                return "";
            }

            var first = fields.FirstOrDefault();

            if (first.Name == "actor" && first.Type == RelevantSymbols.entity)
            {
                return ", " + "{Params(fields.Skip(1))}";
            }
            return "Entity actor";
        }

        
        public static string ParamsWithActor(this IEnumerable<IParameterSymbol> parameters)
        {
            var first = parameters.FirstOrDefault();
            if (first != null)
            {
                if (SymbolEqualityComparer.Default.Equals(first.Type, RelevantSymbols.entity))
                    return Params(parameters);
                else
                    return $"Entity actor, {Params(parameters)}";
            }
            return "Entity actor";
        }


        public static string Params(this IEnumerable<IParameterSymbol> parameters)
        {
            return String.Join(", ", parameters.Select(p => $"{p.Type.TypeToText()} {p.Name}"));
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
                attribute = (T) Activator.CreateInstance(typeof(T), attributeData.GetActualConstuctorParams().ToArray());
            }
            else
            {
                attribute = (T) Activator.CreateInstance(typeof(T));
            }
            foreach (var p in attributeData.NamedArguments)
            {
                typeof(T).GetField(p.Key).SetValue(attribute, p.Value.Value);
            }
            return attribute;
        }

        public static IEnumerable<object> GetActualConstuctorParams(this AttributeData attributeData)
        {
            /*int params_index = attributeData.AttributeConstructor.Parameters.IndexOfFirst(p => p.IsParams);
            
            // There is no params parameter or it was passed as an array
            if (params_index == -1 || params_index == attributeData.ConstructorArguments.Length - 1) 
            {
                foreach (var arg in attributeData.ConstructorArguments)
                {
                    yield return arg.GetValue();
                }
            }
            // There is a params parameter and it was passed nothing
            // We cannot know the type of the params array (although we could get it by reflection)
            else if (params_index >= attributeData.ConstructorArguments.Length)
            {
                yield return null;
            }
            else
            {
                for (int i = 0; i < params_index; i++)
                {
                    yield return attributeData.ConstructorArguments[i].GetValue();
                }

                yield return attributeData.ConstructorArguments
                    .Skip(params_index).Select(arg => arg.GetValue()).ToArray();
            }
            */
            foreach (var arg in attributeData.ConstructorArguments)
            {
                if (arg.Kind == TypedConstantKind.Array)
                {
                    yield return arg.Values.Select(a => a.Value).OfType<string>().ToArray();
                }
                else
                {
                    yield return arg.Value;
                }
            }
        } 

        public static int IndexOfFirst<T>(this IEnumerable<T> e, Predicate<T> predicate)
        {
            int i = 0;
            foreach (var el in e)
            {
                if (predicate(el)) return i;
                i++;
            }
            return -1;
        }

        public static bool TryGetExportAttribute(this IMethodSymbol method, out ExportAttribute attribute)
        {
            return TryGetAttribute(method, RelevantSymbols.ExportAttribute, out attribute);
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

        public static bool TypeSequenceEqual(this IEnumerable<IParameterSymbol> parameters, IEnumerable<IFieldSymbol> fields)
        {
            return parameters.Select(m1 => m1.Type).SequenceEqual(
                fields.Select(field => field.Type), SymbolEqualityComparer.Default);
        }

        public static bool ParameterTypesEqual(this IMethodSymbol method, IEnumerable<InjectedFieldSymbolWrapper> fields)
        {
            return method.Parameters.Select(m1 => m1.Type).SequenceEqual(
                fields.Select(field => field.symbol.Type), SymbolEqualityComparer.Default);
        }

        // TODO: THIS IS NOT A SYMBOL!!
        public static int IndexOfFirstDifference(this string x, string y)
        {
            int count = x.Length;
            if (count > y.Length)
            {
                return IndexOfFirstDifference(y, x);
            }
            if (ReferenceEquals(x, y))
            {
                return -1;
            }
            for (int index = 0; index != count; ++index)
            {
                if (x[index] != y[index])
                    return index;
            }
            return count == y.Length ? -1 : count;
        }

        public static bool TryGetAttributeData(this ISymbol symbol, ISymbol attributeType, out AttributeData attributeData)
        {
            foreach (var a in symbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType))
                {
                    attributeData = a;
                    return true;
                }
            }
            attributeData = default;
            return false;
        }

        public static bool TryGetAttribute<T>(this ISymbol symbol, AttributeSymbolWrapper<T> attributeSymbolWrapper, out T attribute) where T : System.Attribute
        {
            if (TryGetAttributeData(symbol, attributeSymbolWrapper.symbol, out var attributeData))
            {
                attribute = attributeData.MapToType<T>();
                return true;
            }
            attribute = default;
            return false;
        }

        public static bool HasAttribute(this ISymbol symbol, ISymbol attributeType)
        {
            foreach (var a in symbol.GetAttributes())
            {
                if (SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasInterface(this ITypeSymbol symbol, ISymbol interfaceType)
        {
            foreach (var i in symbol.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(i, interfaceType))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsContainedInNamespace(this ISymbol symbol, INamespaceSymbol namespaceSymbol)
        {
            while (symbol.ContainingType != null) symbol = symbol.ContainingType;
            while (symbol.ContainingNamespace != null)
            {
                symbol = symbol.ContainingNamespace;
                if (namespaceSymbol == symbol)
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<INamedTypeSymbol> GetTypeHierarchyReverse(this INamedTypeSymbol symbol)
        {
            while (symbol != null) 
            {
                yield return symbol;
                symbol = symbol.BaseType;
            }
        }

        public static IEnumerable<INamedTypeSymbol> GetTypeHierarchy(this INamedTypeSymbol symbol)
        {
            return GetTypeHierarchyReverse(symbol).Reverse();
        }

        public static IEnumerable<IFieldSymbol> GetInstanceFields(this INamedTypeSymbol symbol)
        {
            return symbol.GetMembers().OfType<IFieldSymbol>().Where(f => !f.IsStatic && !f.IsConst);
        }

        public static string CommaJoin<T>(this IEnumerable<T> things, System.Func<T, string> func)
        {
            return System.String.Join(", ", things.Select(func));
        }

        public static IEnumerable<T> InitAndAfterInit<T>(
            this IEnumerable<T> guys, GenerationEnvironment context) where T : TypeSymbolWrapperBase
        {
            return guys
                .Where(g => g.InitWithErrorHandling(context))
                .Where(g => g.AfterInitWithErrorHandling(context));
        }
    }
}