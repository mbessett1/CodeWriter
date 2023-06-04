using System;
using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class AttributeSnippet : ICodeSnippet
    {
        public string Name { get; private set; }
        public List<object> AttrParams = new List<object>();

        public AttributeSnippet(string name, params object[] attrParams)
        {
            Name = name;
            AttrParams = attrParams.ToList();
        }

        public IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet();

            List<string> attrParams = new List<string>();
            string attrParamsExpanded = "";

            if (AttrParams.Any())
            {
                foreach (var attrParam in AttrParams)
                {
                    if (attrParam == null) continue;

                    if (attrParam is string && !((string)attrParam).StartsWith("typeof") )
                    {
                        // quote the string
                        attrParams.Add($"\"{attrParam}\"");
                    }
                    else if (attrParam is Type)
                    {
                        attrParams.Add($"typeof({((Type)attrParam).FullName})");
                    }
                    else  
                    {
                        attrParams.Add($"{attrParam}");
                    }
                }
                attrParamsExpanded = $"({string.Join(",", attrParams)})";
            }

            var attribute = $"[{Name}{attrParamsExpanded}]";

            return new CodeSnippet()
                .AddLine(attribute)
                .ToCSharp();

        }
        public string CSharp
        {
            get { return ToCSharp().ToText(); }
        }

    }
}