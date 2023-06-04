using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public partial class NamespaceSnippet
    {
        public NamespaceSnippet WithEnums(params EnumSnippet[] enumSnippets)
        {
            WithSnippets(enumSnippets);
            return this;
        }
    }
    public partial class ClassSnippet
    {
        public ClassSnippet WithEnums(params EnumSnippet[] enumSnippets)
        {
            WithSnippets(enumSnippets);
            return this;
        }
    }

    public class EnumSnippet : INamespaceMemberSnippet, IClassMemberSnippet
    {
        public class EnumValue
        {
            public string Name { get; set; }
            public int? Value { get; set; } = null;
            public List<AttributeSnippet> Attributes { get; set; } = new List<AttributeSnippet>();

        }

        public List<AttributeSnippet> Attributes { get; private set; } = new List<AttributeSnippet>();
        public string Name { get; protected set; }
        public Accessibility Scope { get; protected set; }

        public Dictionary<string, EnumValue> Values { get; private set; } = new Dictionary<string, EnumValue>();

        public EnumSnippet()
        {}

        public EnumSnippet WithAttribute(params AttributeSnippet[] attributes)
        {
            Attributes.AddRange(attributes);
            return this;
        }

        public EnumSnippet(Accessibility scope, string name, params AttributeSnippet[] attributes)
        {
            Name = name;
            Scope = scope;
            if (attributes.Length > 0) Attributes = attributes.ToList();
        }

        public EnumSnippet WithValue(string name, int? value = null)
        {
            if (Values.ContainsKey(name))
            {
                Values[name] = new EnumValue() {Name =name, Value = value};
            }
            else
            {
                Values.Add(name, new EnumValue() { Name = name, Value = value });
            }
            return this;
        }

        public EnumSnippet WithValue(string name, int? value = null, params AttributeSnippet[] attributes)
        {
            if (Values.ContainsKey(name))
            {
                Values[name] = new EnumValue() { Name = name, Value = value, Attributes = attributes.ToList()};
            }
            else
            {
                Values.Add(name, new EnumValue() { Name = name, Value = value, Attributes = attributes.ToList() });
            }
            return this;
        }

        private CodeSnippet EnumValues 
        {
            get
            {
                var enumBody = new List<string>();
                var code = new CodeSnippet();
                var keyValues = Values.ToArray();

                var keyValue = new KeyValuePair<string, EnumValue>();

                for (int i = 0; i < keyValues.Length; i++)
                {
                    keyValue = keyValues[i];

                    var enumCode = $"{keyValue.Key}{(keyValue.Value.Value!=null ? $" = {keyValue.Value.Value}" : "" )}";
                    if (i < (keyValues.Length-1))
                        enumCode += ",";


                    code.AddSnippets(keyValue.Value.Attributes).AddLine(enumCode);
                }

                return code;
            }
        }

        public  IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet()
                .AddLine($"{Scope.AsCSharp()} enum {Name}")
                .EncloseInScope(EnumValues)
                .BlankLine();

            return snippet.ToCSharp();
        }
        public string CSharp
        {
            get { return ToCSharp().ToText(); }
        }

    }
}