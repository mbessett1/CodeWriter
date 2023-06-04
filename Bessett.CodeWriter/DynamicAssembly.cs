using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Bessett.CodeWriter.CodeSnippets;
using System.Runtime;

namespace Bessett.CodeWriter
{
    public class DynamicAssembly
    {
        private List<CodeEntity> CodeEntities { get; set; } = new List<CodeEntity>();
        private List<PortableExecutableReference> ReferenceAssembies = new List<PortableExecutableReference>();

        public string SourceCode( bool includeLineNumbers)
        {
            var result = new StringBuilder();
            var lineNo = 0;

            foreach (var codeEntity in CodeEntities)
            {
                result.AppendLine(codeEntity.SourceCode(includeLineNumbers, out lineNo, lineNo ));
            }

            return result.ToString();
        }

        /// <summary>
        /// Dynamic Assembly Creator
        /// Note: System and System.Linq refernces included
        /// Any other required assembly references must be specified
        /// </summary>
        public DynamicAssembly()
        {
            ReferenceAssembies.Add(MetadataReference.CreateFromFile(typeof(Object).Assembly.Location));
            ReferenceAssembies.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location));
            ReferenceAssembies.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
            ReferenceAssembies.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
        }

        public DynamicAssembly AddReferences(params Assembly[] assemblies)
        {
            ReferenceAssembies
                .AddRange(assemblies.Select(
                    a=>  MetadataReference.CreateFromFile(a.Location)
                ));

            return this;
        }

        public DynamicAssembly AddReferencesFromTypes(params Type[] types)
        {
            ReferenceAssembies
                .AddRange(types.Select(
                    t => MetadataReference.CreateFromFile(t.Assembly.Location)
                ));

            return this;
        }

        public DynamicAssembly AddCodeEntitiy( CodeEntity codeEntity)
        {
            CodeEntities.Add(codeEntity);
            return this;
        }

        public Assembly CreateAssembly(OutputKind assemblyKind = OutputKind.DynamicallyLinkedLibrary)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (var codeEntity in CodeEntities)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(codeEntity.CSharp);
                syntaxTrees.Add(syntaxTree);
            }

            string assemblyName = Path.GetRandomFileName();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: syntaxTrees,
                references: ReferenceAssembies,
                options: new CSharpCompilationOptions(assemblyKind));

            var diagnostics = new List<Diagnostic>();

            using (var ms = new MemoryStream())
            {
                var complieResult = compilation.Emit(ms);

                if (!complieResult.Success)
                {
                    IEnumerable<Diagnostic> failures = complieResult.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    throw new CompileException(failures);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    Assembly assembly = Assembly.Load(ms.ToArray());
                    return assembly;
                }
            }
        }
    }
}