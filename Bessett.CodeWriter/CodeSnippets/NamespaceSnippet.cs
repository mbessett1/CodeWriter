using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CSharp.RuntimeBinder;

namespace Bessett.CodeWriter.CodeSnippets
{
    public interface INamespaceMemberSnippet : ICodeSnippet
    {
    }

    public partial class NamespaceSnippet : SnippetContainer<INamespaceMemberSnippet> 
    {

        public NamespaceSnippet() {}
            
        public NamespaceSnippet(string namespaceName)
        {
            Name = namespaceName;
        }

        /// <summary>
        /// include the specified namespace level snippet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="codeSnippet"></param>
        /// <returns></returns>
        public NamespaceSnippet WithSnippets<T>(params T[] snippets) where T : INamespaceMemberSnippet
        {
            Snippets.AddRange((IEnumerable<INamespaceMemberSnippet>)snippets.AsEnumerable());
            return this;
        }
        public NamespaceSnippet WithName(string name)
        {
            Name = name;
            return this;
        }


        #region ICodeSnippet Interface
        public override IEnumerable<string> ToCSharp()
        {
            var snippet = new CodeSnippet()
                .AddLine($"namespace {Name}")
                .OpenScope()
                .InsertSnippets(Snippets)
                .CloseScope();

            return snippet.ToCSharp();
        }

        #endregion
    }
}