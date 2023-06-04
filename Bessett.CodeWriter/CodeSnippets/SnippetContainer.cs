using System;
using System.Collections.Generic;
using System.Linq;

namespace Bessett.CodeWriter.CodeSnippets
{
    public abstract class SnippetContainer<T>: ICodeSnippet 
        where T: ICodeSnippet
    {
        public string Name { get; protected set; }
        protected List<T> Snippets { get; set; } = new List<T>();
        protected List<Type> LocalReferenceTypes { get; private set; } = new List<Type>();
        public IEnumerable<Type> ReferenceTypes => LocalReferenceTypes.AsEnumerable();

        public void AddReferenceType(Type type)
        {
            LocalReferenceTypes.Add(type);
        }
        public abstract IEnumerable<string> ToCSharp();

        public virtual string CSharp
        {
            get { return ToCSharp().ToText(); }
        }
    }
}