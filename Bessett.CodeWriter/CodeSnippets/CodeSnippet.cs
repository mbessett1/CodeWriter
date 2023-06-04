using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class CodeSnippet : ICodeSnippet
    {
        protected List<string> CodeLines { get; set; } = new List<string>();

        public CodeSnippet WithLines(IEnumerable<string> codeLines)
        {
            CodeLines.AddRange(codeLines);
            return this;
        }
        public CodeSnippet AddSnippets(IEnumerable<ICodeSnippet> snippets)
        {
            foreach (var codeSnippet in snippets)
            {
                CodeLines.AddRange(codeSnippet.ToCSharp());
            }
            return this;
        }
        public CodeSnippet AddSnippets(params ICodeSnippet[] snippets)
        {
            foreach (var codeSnippet in snippets)
            {
                CodeLines.AddRange(codeSnippet.ToCSharp());
            }
            return this;
        }
        public CodeSnippet AddLine(string codeLine)
        {
            CodeLines.Add(codeLine);
            return this;
        }

        public virtual IEnumerable<string> ToCSharp()
        {
            return CodeLines.AsEnumerable();
        }

        public string CSharp
        {
            get { return CodeLines.ToText(); }
        }

        public static CodeSnippet FromLines(IEnumerable<string> lines)
        {
            return new CodeSnippet().WithLines(lines.Where(l => !string.IsNullOrEmpty(l)));
        }
        public static CodeSnippet FromSnippets(params ICodeSnippet[] snippets)
        {
            return new CodeSnippet().AddSnippets(snippets);
        }
        public static CodeSnippet FromSnippets(IEnumerable<ICodeSnippet> snippets)
        {
            return new CodeSnippet().AddSnippets(snippets);
        }
        public static CodeSnippet FromLine(string line)
        {
            return new CodeSnippet().AddLine(line);
        }
    }
}