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
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string input)
        {
            if (input.Length == 0) return input;

            var sb = new StringBuilder();

            sb.Append(char.ToLowerInvariant(input[0]));

            for (int i = 1; i < input.Length; i++)
            {
                char ch = input[i];

                if (char.IsUpper(ch))
                {
                    sb.Append('_');
                    sb.Append(char.ToLowerInvariant(ch));
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

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
    }
}