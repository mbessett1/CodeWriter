using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bessett.CodeWriter.CodeSnippets
{
    public interface IStructMemberSnippet : ICodeSnippet {}

    public partial class NamespaceSnippet
    {
        public NamespaceSnippet AddStucts(params StructSnippet[] structSnuppets)
        {
            WithSnippets(structSnuppets);
            return this;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StructSnippet : SnippetContainer<IStructMemberSnippet>, INamespaceMemberSnippet, IClassMemberSnippet, IStructMemberSnippet
    {
        public string InterfaceSpec { get; protected set; }
        public Accessibility Scope { get; protected set; }
        public bool Partial { get; protected set; }

        private List<AttributeSnippet> Attributes { get; set; } = new List<AttributeSnippet>();
        private List<PropertySnippet> Properties { get; set; } = new List<PropertySnippet>();

        public StructSnippet WithAttributes(params AttributeSnippet[] attributes)
        {
            Attributes.AddRange(attributes);
            return this;
        }
        public StructSnippet WithSnippets<T>(params T[] snippets) where T : IStructMemberSnippet
        {
            Snippets.AddRange((IEnumerable<IStructMemberSnippet>)snippets.AsEnumerable());
            return this;
        }

        public StructSnippet IsPartial()
        {
            Partial = true;
            return this;
        }

        public StructSnippet(Accessibility scope, string name, string interfaceSpec)
        {
            Name = name;
            Scope = scope;
            InterfaceSpec = interfaceSpec;
        }

        public StructSnippet() {}

        public StructSnippet WithName(string name)
        {
            Name = name;
            return this;
        }
        public StructSnippet WithScope(Accessibility scope)
        {
            Scope = scope;
            return this;
        }
        public StructSnippet WithInterfaceSpec(string interfaceSpec)
        {
            InterfaceSpec = interfaceSpec;
            return this;
        }
        public StructSnippet WithProperties(params PropertySnippet[] propertySpecs)
        {
            Snippets.AddRange(propertySpecs);
            return this;
        }
        public StructSnippet WithProperty(Accessibility scope, Type dataType, string name)
        {
            Snippets.Add(new PropertySnippet(scope, dataType, name));
            return this;
        }
        public StructSnippet WithProperty(Accessibility scope, string dataType, string name)
        {
            Snippets.Add(new PropertySnippet(scope, dataType, name));
            return this;
        }

        public override IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet();
            var scopeAttrCode = Scope.AsCSharp();
            var isPartial = Partial ? " partial " : "";

            var baseClassDeclaration = string.IsNullOrEmpty(InterfaceSpec)
                ? ""
                : $" : {InterfaceSpec}";

            snippet
                .AddSnippets(Attributes)
                .AddSnippets(Properties)
                .AddLine($"{scopeAttrCode}{isPartial} struct {Name} {baseClassDeclaration}")
                //.AddSnippets(TypeParameters.ConstraintSnippet)
                .EncloseInScope(Snippets)
                ;

            return snippet.ToCSharp();

            //return new CodeSnippet()
            //    .AddSnippets(Attributes)
            //    .AddSnippets(Snippets)
            //    .ToCSharp();
        }

    }
}