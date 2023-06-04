using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bessett.SmartConsole;
using Bessett.CodeWriter.CodeSnippets;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Reflection;

namespace Bessett.CodeWriter.Tests.Tasks
{
    [NoConfirmation]
    public abstract class CodeTest : ConsoleTask
    {
        protected static bool CompileCode(CodeEntity code, out Assembly assembly, out TaskResult taskResult)
        {
            assembly = null;
            var clock = new Stopwatch();
            Console.Write("Compiling...");

            try
            {
                clock.Start();
                assembly = new DynamicAssembly()
                    .AddCodeEntitiy(code)
                    .AddReferences(Assembly.GetExecutingAssembly())
                    .AddReferencesFromTypes(typeof(ConsoleTask))
                    .CreateAssembly(OutputKind.DynamicallyLinkedLibrary);

                clock.Stop();
                Console.WriteLine($" Done. {clock.Elapsed.TotalMilliseconds / 1000:F3} sec");
                taskResult = TaskResult.Complete();

                return true;

            }
            catch (CompileException ex)
            {
                clock.Stop();
                Console.WriteLine();
                int lineCount;

                Console.WriteLine(code.SourceCode(true, out lineCount));

                foreach (var diagnostic in ex.Failures)
                {
                    Console.WriteLine($"{diagnostic}");
                }

                taskResult = TaskResult.Exception(ex);
                return false;
            }
            catch (Exception ex)
            {
                clock.Stop();
                Console.WriteLine();
                {
                    taskResult = TaskResult.Exception(ex);
                    return false;
                }
            }

        }

    }

}
