using System;
using System.Collections.Generic;
using System.Linq;
using Bessett.CodeWriter.Common;

namespace Bessett.CodeWriter.CodeSnippets
{
    public interface IClassMemberSnippet : ICodeSnippet
    { }

    public partial class NamespaceSnippet
    {
        public NamespaceSnippet WithClasses(params ClassSnippet[] classSnippets)
        {
            WithSnippets(classSnippets);
            return this;
        }
    }

    public partial class ClassSnippet : SnippetContainer<IClassMemberSnippet>, INamespaceMemberSnippet, IClassMemberSnippet
    {
        public Accessibility Scope { get; protected set; }
        public List<AttributeSnippet> Attributes { get; protected set; } = new List<AttributeSnippet>();
        public TypeParameterList TypeParameters { get; protected set; } = new TypeParameterList();
        public string BaseTypename { get; protected set; } = "";
        public bool Abstract { get; protected set; }
        public bool Partial { get; protected set; }
        protected List<ValueString> Interfaces { get; set; } = new List<ValueString>();
        public string InterfaceSpec
        {
            get
            {
                var interfaces = Interfaces.Count > 0 ? string.Join(",", Interfaces) : "";
                var comma = (BaseTypename.Any() && interfaces.Any() ? "," : "");
                return $"{BaseTypename}{comma}{interfaces}";
            }
        }
        public ClassSnippet WithProperties(params PropertySnippet[] propertySpecs)
        {
            Snippets.AddRange(propertySpecs);
            return this;
        }
        public ClassSnippet WithProperty(Accessibility scope, Type dataType, string name)
        {
            Snippets.Add(new PropertySnippet(scope, dataType, name));
            return this;
        }
        public ClassSnippet WithProperty(Accessibility scope, string dataType, string name)
        {
            Snippets.Add(new PropertySnippet(scope, dataType, name));
            return this;
        }

        #region Construction
        public ClassSnippet() { }
        public ClassSnippet(string name, Accessibility scope, Type baseType = null, params ValueString[] interfaces)
        {
            Name = name;
            Scope = scope;
            WithInterfaces(interfaces);
            if (baseType != null)
            {
                AddReferenceType(baseType);
                BaseTypename = baseType.ToCSharp();
            }
        }
        public ClassSnippet(Accessibility scope, string name, string baseType, params ValueString[] interfaces)
        {
            Name = name;
            Scope = scope;
            BaseTypename = baseType;
            WithInterfaces(interfaces);
        }
        #endregion

        #region Fluent
        public ClassSnippet WithClasses(params ClassSnippet[] classSnippets)
        {
            Snippets.AddRange(classSnippets);
            return this;
        }
        public ClassSnippet WithName(string name)
        {
            Name = name;
            return this;
        }

        public ClassSnippet WithBaseClass(string classname)
        {
            BaseTypename = classname;
            return this;
        }

        public ClassSnippet WithInterfaces(ValueString[] interfaces)
        {
            Interfaces.AddRange(interfaces);
            return this;
        }
        public ClassSnippet WithTypeParameter(params TypeParameter[] typeParameter)
        {
            TypeParameters.AddRange(typeParameter);
            return this;
        }

        public ClassSnippet WithScope(Accessibility scope)
        {
            Scope = scope;
            return this;
        }

        public ClassSnippet IsAbstract()
        {
            Abstract = true;
            return this;
        }

        public ClassSnippet IsPartial()
        {
            Partial = true;
            return this;
        }

        public ClassSnippet WithAttributes(params AttributeSnippet[] attributes)
        {
            Attributes.AddRange(attributes);
            return this;
        }

        public ClassSnippet WithSnippets<T>(params T[] snippets) where T : IClassMemberSnippet
        {
            Snippets.AddRange((IEnumerable<IClassMemberSnippet>)snippets.AsEnumerable());
            return this;
        }

        #endregion

        #region ICodeSnippet
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
                .AddLine($"{scopeAttrCode}{isPartial} class {Name} {TypeParameters.ParamList} {baseClassDeclaration}")
                .AddSnippets(TypeParameters.ConstraintSnippet)
                .EncloseInScope(Snippets)
                ;

            return snippet.ToCSharp();
        }
        #endregion
    }
}