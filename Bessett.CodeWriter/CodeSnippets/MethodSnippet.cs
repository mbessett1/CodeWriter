using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public partial class ClassSnippet 
    {
        public ClassSnippet WithMethods(params MethodSnippetBase[] methods)
        {
            Snippets.AddRange(methods);
            return this;
        }
        public ClassSnippet WithConstructors(params ConstructorSnippet[] constructors)
        {
            constructors.ToList().ForEach(c=> c.Name = this.Name);
            Snippets.AddRange(constructors);
            return this;
        }
    }

    public class ConstructorSnippet : MethodSnippetBase<ConstructorSnippet>
    {
        public enum InitializerTypes
        {
            None,
            This,
            Base
        }

        public InitializerTypes InitializerType { get; protected set; }= InitializerTypes.None;
        public string[] InitializerParamList { get; protected set; }

        /// <summary>
        /// simpler model, but must use the .WithConstructors member on classes to inject 
        /// the class typename
        /// </summary>
        /// <param name="typeAttribute"></param>
        public ConstructorSnippet(Accessibility typeAttribute)
        {
            TypeAttribute = typeAttribute;
        }

        public ConstructorSnippet(Accessibility typeAttribute, string name)
        {
            Name = name;
            TypeAttribute = typeAttribute;
        }

        public ConstructorSnippet WithBaseInitializer(params string[] initializerParams)
        {
            InitializerType = InitializerTypes.Base;
            InitializerParamList = initializerParams;
            return this;
        }
        public ConstructorSnippet WithInitializer(params string[] initializerParams)
        {
            InitializerType = InitializerTypes.This;
            InitializerParamList = initializerParams;
            return this;
        }

        protected override ConstructorSnippet Instance { get { return this; } }

        protected override string Signature
        {
            get
            {
                var constructorInitializer =
                    InitializerType == InitializerTypes.Base
                        ? $": base({string.Join(",", InitializerParamList)})"
                        : InitializerType == InitializerTypes.This
                            ? $": this({string.Join(",", InitializerParamList)})"
                            : "";

                return $"{TypeAttribute.AsCSharp()} {Name}({ParameterList}) {constructorInitializer}";

            }
        }
    }

    public class ImplicitOperatorMethodSnippet : MethodSnippetBase<ImplicitOperatorMethodSnippet>
    {
        public string ResultTypeName { get; protected set; }

        protected override ImplicitOperatorMethodSnippet Instance { get { return this; } }

        protected override string Signature
        {
            get { return $"public static implicit operator {ResultTypeName}({ParameterList})"; }
        }

        public ImplicitOperatorMethodSnippet(string resultTypeName, string sourceTypeName, string sourceParameterName)
        {
            Parameters.Add(FieldDef.Simple(sourceTypeName, sourceParameterName));
            ResultTypeName = resultTypeName;
        }

    }

    public abstract class MethodSnippetBase : IClassMemberSnippet
    {
        public List<AttributeSnippet> Attributes { get; protected set; } = new List<AttributeSnippet>();
        public Accessibility TypeAttribute { get; protected set; }
        public string Name { get; protected internal set; }
        public List<FieldDef> Parameters { get; protected set; } = new List<FieldDef>();
        public CodeSnippet BodyBlock { get; protected set; } = new CodeSnippet();
        protected bool IsAbstract { get; set; }

        public string ParameterList
        {
            get { return string.Join(",", Parameters.Select(p => p.CSharp)); }
        }

        public virtual IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet()
                .AddSnippets(Attributes);

            if (IsAbstract)
            {
                // help out the caller. We could check that the 
                //class is abstract, because it needs to be for this to compile...
                snippet.AddLine($"{Signature};");
            }
            else
            {
                snippet.AddLine(Signature)
                    .EncloseInScope(BodyBlock);
            }

            return snippet.ToCSharp();
        }

        public string CSharp
        {
            get { return ToCSharp().ToText(); }
        }
        protected abstract string Signature { get; }

    }

    public abstract class MethodSnippetBase<T> : MethodSnippetBase
    {

        public T WithBody(CodeSnippet codeBlock)
        {
            BodyBlock = BodyBlock ?? new CodeSnippet();
            BodyBlock.AddSnippets(codeBlock);
            return Instance;
        }
        public T WithParameter(FieldDef parameter)
        {
            if (!string.IsNullOrEmpty(parameter.Name))
                Parameters.Add(parameter);
            return Instance;
        }

        public T WithAttributes(params AttributeSnippet[] attributes)
        {
            Attributes.AddRange(attributes);
            return Instance;
        }

        public T WithCodeStatement(string codeLine)
        {
            BodyBlock = BodyBlock ?? new CodeSnippet();
            BodyBlock.AddLine(codeLine);
            return Instance;
        }

        protected abstract T Instance { get;}

    }

    /// <summary>
    /// ValueString is a string that cannot be null
    /// and is a struct
    /// </summary>
    public struct ValueString
    {
        private string objectValue;

        public ValueString(string value)
        {
            objectValue = value ?? "";
        }

        public override string ToString()
        {
            return objectValue;
        }

        public static implicit operator string(ValueString value)
        {
            return value.objectValue;
        }
        public static implicit operator ValueString(string value)
        {
            return new ValueString(value);
        }
    }

    public class MethodSnippet : MethodSnippetBase<MethodSnippet>
    {
        public string ReturnTypeName { get; protected set; }
        public bool IsOverride { get; protected set; }

        protected override MethodSnippet Instance { get { return this;}}

        public MethodSnippet() {}

        public MethodSnippet(Accessibility typeAttribute, Type returnType, string name, bool isOverride = false)
        {
            Name = name;
            ReturnTypeName = returnType?.FullName;
            TypeAttribute = typeAttribute;
            IsOverride = false;
        }

        public MethodSnippet(Accessibility typeAttribute, string returnType, ValueString name, bool isOverride = false)
        {
            Name = name;
            ReturnTypeName = string.IsNullOrWhiteSpace(returnType) ? null: returnType;
            TypeAttribute = typeAttribute;
            IsOverride = isOverride;
        }
        
        protected override string Signature {
            get
            {
                var scope = TypeAttribute.AsCSharp();
                var isAbstract = IsAbstract ? " abstract " : "";
                string returnTypeCode = ReturnTypeName ?? "void";
                string isOverride = (IsOverride ? " override " : "");
                return $"{scope}{isAbstract}{isOverride}{returnTypeCode} {Name} ({ParameterList})";
            }
        }
    }

}