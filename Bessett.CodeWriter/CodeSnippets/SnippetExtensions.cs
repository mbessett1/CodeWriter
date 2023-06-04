using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public static class SnippetExtensions
    {
        public static CodeSnippet OpenScope(this CodeSnippet snippet)
        {
            snippet.AddLine("{");
            return snippet;
        }
        public static CodeSnippet CloseScope(this CodeSnippet snippet)
        {
            snippet.AddLine("}");
            return snippet;
        }
        public static CodeSnippet EndScope(this CodeSnippet snippet)
        {
            snippet.AddLine("};");
            return snippet;
        }
        public static CodeSnippet BlankLine(this CodeSnippet snippet)
        {
            snippet.AddLine("");
            return snippet;
        }

        public static CodeSnippet EncloseInScope(this CodeSnippet snippet, params ICodeSnippet[] newSnippet)
        {
            snippet.OpenScope()
                .InsertSnippets(newSnippet)
                .CloseScope()
                ;
            return snippet;
        }
        public static CodeSnippet EncloseInScope(this CodeSnippet snippet, IEnumerable<ICodeSnippet> newSnippet)
        {
            snippet.OpenScope()
                .InsertSnippets(newSnippet)
                .CloseScope()
                ;
            return snippet;
        }

        public static CodeSnippet InsertSnippets(this CodeSnippet snippet, IEnumerable<ICodeSnippet> snippets)
        {
            foreach (var codeSnippet in snippets)
            {
                snippet.WithLines(codeSnippet.ToCSharp().Select(l => $"   {l}").ToList());
            }
            return snippet;
        }

        public static string AsCSharp(this Accessibility scope)
        {
            if (scope!= Accessibility.None)
                return $"{scope.ToString().ToLower()} ";
            return "";
        }

        public static string ToText(this IEnumerable<string> lines)
        {
            return $"{string.Join("\n", lines)}\n";
        }

    }
}