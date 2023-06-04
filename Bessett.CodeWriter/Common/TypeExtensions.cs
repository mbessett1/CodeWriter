using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bessett.CodeWriter.Common
{
    internal static class TypeExtensions
    {
        public static string ToCSharp(this Type type)
        {
            if (type.IsGenericType)
            {
                var args = string.Join(",", type.GenericTypeArguments.Select(a => a.ToCSharp()));
                return $"{type.GetGenericTypeDefinition().Name.Replace("`1", "")}<{args}>";
            }
            return type.Name;
        }
    }

    internal static class StringExtensions
    {
        public static string InQuotes(this string text)
        {
            return $"\"{text}\"";
        }
    }

}
