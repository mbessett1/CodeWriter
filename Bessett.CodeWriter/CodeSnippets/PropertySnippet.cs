using System;
using System.Collections.Generic;
using System.Text;

namespace Bessett.CodeWriter.CodeSnippets
{
    public class PropertySnippet : IClassMemberSnippet, IInterfaceMemberSnippet, IStructMemberSnippet
    {
        protected List<AttributeSnippet> Attributes { get; private set; } = new List<AttributeSnippet>();
        public Accessibility Scope { get; private set; }
        public FieldDef Field { get; private set; } = new FieldDef();

        public PropertySnippet() {}

        public PropertySnippet(Accessibility scope, Type dataType, string name)
        {
            Field.WithName(name);
            Field.WithDataType(dataType);
            Scope = scope;
        }
        public PropertySnippet(Accessibility scope, string dataType, string name)
        {
            Field.WithName(name);
            Field.WithDataType(dataType);
            Scope = scope;
        }

        public PropertySnippet WithDefaultValue(object defaultValue)
        {
            Field.HasDefaultValue(defaultValue);
            return this;
        }
        public PropertySnippet WithAttribute(params AttributeSnippet[] attributes)
        {
            Attributes.AddRange(attributes);
            return this;
        }
        public PropertySnippet WithAttribute(string name, params object[] attrParams)
        {
            Attributes.Add(new AttributeSnippet(name, attrParams));
            return this;
        }

        public  IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet();

            var signature = new StringBuilder()
                .Append($"{Scope.AsCSharp()}{Field.DataTypeName} {Field.Name} ")
                .Append("{get; set;}")
                .Append(Field.DefaultDeclaration).Append(Field.HasDefault? ";":"")
                .ToString();

            snippet
                .AddSnippets(Attributes)
                .AddLine(signature).BlankLine();
                
            return snippet.ToCSharp();

        }
        public string CSharp
        {
            get { return ToCSharp().ToText(); }
        }
    }
}