using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public interface IInterfaceMemberSnippet : ICodeSnippet
    {}

    public partial class NamespaceSnippet
    {
        public NamespaceSnippet WithInterfaces(params InterfaceSnippet[] interfaceSnippets)
        {
            WithSnippets(interfaceSnippets);
            return this;
        }
    }

    public partial class InterfaceSnippet : SnippetContainer<IInterfaceMemberSnippet>, INamespaceMemberSnippet
    {
        public Accessibility Scope { get; set; }
        public List<AttributeSnippet> Attributes { get; protected set; } = new List<AttributeSnippet>();
        public TypeParameterList TypeParameters { get; protected set; } = new TypeParameterList();
        protected List<ValueString> Interfaces { get; set; } = new List<ValueString>();

        public bool Partial { get; protected set; }


        public string InterfaceSpec
        {
            get
            {
                var interfaces = Interfaces.Count > 0 ? string.Join(",", Interfaces) : "";
                return interfaces;
            }
        }


        #region Construction
        public InterfaceSnippet()
        { }

        public InterfaceSnippet(Accessibility scope, string name)
        {
            Name = name;
            Scope = scope;
        }

        #endregion
        #region Fluent
        public InterfaceSnippet WithScope(Accessibility scope)
        {
            Scope = scope;
            return this;
        }

        public InterfaceSnippet WithName(string name)
        {
            Name = name;
            return this;
        }
        public InterfaceSnippet IsPartial()
        {
            Partial = true;
            return this;
        }

        public InterfaceSnippet WithSnippets<T>(params T[] snippets) where T : IInterfaceMemberSnippet
        {
            Snippets.AddRange((IEnumerable<IInterfaceMemberSnippet>)snippets.AsEnumerable());
            return this;
        }

        #endregion
        #region ICodeSnippet
        #endregion

        public override IEnumerable<string> ToCSharp()
        {
            var isPartial = Partial ? " partial " : "";

            var baseClassDeclaration = string.IsNullOrEmpty(InterfaceSpec)
                ? ""
                : $" : {InterfaceSpec}";

            return new CodeSnippet()
                .AddLine($"{Scope.AsCSharp()}{isPartial} interface {Name} {baseClassDeclaration}")
                .OpenScope()
                .InsertSnippets(Snippets)
                .CloseScope().BlankLine()
                .ToCSharp();
        }

    }
}