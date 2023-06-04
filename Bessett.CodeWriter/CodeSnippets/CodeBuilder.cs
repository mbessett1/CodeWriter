using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Xml;
using Bessett.CodeWriter.CodeSnippets;

/*
    c:\nuget\nuget.exe pack $(ProjectPath) -Symbols
    xcopy.exe $(ProjectDir)*.nupkg Z:\TFS\Packages /y
*/

namespace Bessett.CodeWriter.CodeSnippets
{
    public enum Accessibility
    {
        None,
        Private,
        Public,
        Internal,
        Protected
    }

    public interface ICodeAtom
    {
        string CSharp { get; }
    }

    /// <summary>
    /// CodeAtoms are simple strings that compose to generate a line of code
    /// </summary>
    public class CodeAtom : ICodeAtom
    {
        protected string SourceLine { get; set; } = "";

        public string CSharp
        {
            get { return SourceLine; }
        }

        public static implicit operator string(CodeAtom atom)
        {
            return atom.CSharp;
        }
    }

    public class ObjectDeclaration : CodeAtom 
    {
        public ObjectDeclaration(string name, Type target)
        {
            SourceLine = $"var {name} = new {target.FullName}();";

        }
        public ObjectDeclaration(string name, string dataType)
        {
            SourceLine = $"var {name} = new {dataType}();";

        }

    }
    public class StructDeclaration<T> : CodeAtom where T : struct
    {
        public StructDeclaration(string name)
        {
            SourceLine = $"{typeof(T).FullName} {name};";

        }

    }


}