using System;
using System.Collections.Generic;

namespace Bessett.CodeWriter.CodeSnippets
{
    public interface ICodeSnippet
    {
        // TODO: implement the ReferenceTypes collection
        //       to allow the automatic type referencing system
        //       this will cause all implementations referencing types
        //       to fail until completed.
        // IEnumerable<Type> ReferenceTypes { get; }

        IEnumerable<string> ToCSharp();
        string CSharp { get; }
    }
}