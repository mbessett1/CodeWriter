using System.Collections.Generic;
using System.Text;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class CodeEntity : ICodeSnippet
    {
        private List<NamespaceSnippet> Namespaces { get; set; } =  new List<NamespaceSnippet>();
        private CodeSnippet Usings { get; set; } = new CodeSnippet();

        public CodeEntity WithUsing(string usingDeclaration)
        {
            Usings.AddLine($"using {usingDeclaration};");
            return this;
        }

        public CodeEntity AddNamespace(NamespaceSnippet namespaceSnippet)
        {
            Namespaces.Add(namespaceSnippet);
            return this;
        }

        public IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet()
                .AddSnippets(Usings)
                .AddSnippets(Namespaces);

            return snippet.ToCSharp();
        }

        public string CSharp
        {
            get { return ToCSharp().ToText(); }
        }

        public string SourceCode(bool includeLineNumbers = false)
        {
            int lineNo = 0 ;
            return SourceCode(includeLineNumbers, out lineNo, lineNo);
        }

        public string SourceCode(bool includeLineNumbers, out int lineNo, int startLineNo = 0)
        {
            var result = new StringBuilder();
            lineNo = startLineNo;

            foreach (var line in ToCSharp())
            {
                lineNo++;
                if (includeLineNumbers)
                {
                    result.Append(lineNo.ToString("D5"));
                }

                result.AppendLine(line);
            }

            return result.ToString();
        }

    }
}