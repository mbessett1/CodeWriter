using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bessett.CodeWriter.CodeSnippets
{
    public partial class InterfaceSnippet
    {
        public InterfaceSnippet WithMthods(params InterfaceMethodSnippet[] methods)
        {
            Snippets.AddRange(methods);
            return this;
        }
    }

    public class InterfaceMethodSnippet : MethodSnippetBase<InterfaceMethodSnippet>, IInterfaceMemberSnippet
    {
        public string ReturnTypeName { get; protected set; }
        public bool IsOverride { get; protected set; }

        protected override InterfaceMethodSnippet Instance { get { return this; } }

        protected override string Signature
        {
            get
            {
                BodyBlock = null;

                string returnTypeCode = ReturnTypeName ?? "void";
                string overrideSpec = (IsOverride ? " override " : "");

                return $"{overrideSpec}{returnTypeCode} {Name} ({ParameterList})";

            }
        }
        public InterfaceMethodSnippet(Type returnType, string name, bool isOverride = false)
        {
            Name = name;
            ReturnTypeName = returnType?.FullName;
            IsOverride = false;
        }

        public InterfaceMethodSnippet(string returnType, ValueString name, bool isOverride = false)
        {
            Name = name;
            ReturnTypeName = returnType;
            IsOverride = isOverride;
        }

    }
}