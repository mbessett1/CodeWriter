using System;
using System.Diagnostics;
using System.Linq;
using Bessett.SmartConsole;
using Bessett.CodeWriter;
using Bessett.CodeWriter.CodeSnippets;
using Microsoft.CodeAnalysis;
using Accessibility = Bessett.CodeWriter.CodeSnippets.Accessibility;

namespace Bessett.CodeWriter.Tests.Tasks
{
    public interface Itest
    {
        void Run();
    }
    public struct Address: Itest
    {
        public string Name { get; set; }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }

    [NoConfirmation]
    [TaskHelp("Provide Example")]
    public class Example: ConsoleTask
    {
        public override TaskResult StartTask()
        {
            System.Reflection.Assembly assembly;

            var targetNamespace = "HelloWorld";
            var personTypename = "Person";
            var targetMethod = "Hello";

            var personClass = new ClassSnippet(personTypename, Accessibility.Public)
                    .WithProperties(
                        new PropertySnippet(Accessibility.Public, typeof(string), "Name"))
                    .WithMethods(
                        new MethodSnippet(Accessibility.Public, "", targetMethod)
                            .WithParameter(new FieldDef(typeof(string), "name"))
                            .WithCodeStatement("Console.WriteLine($\"Hi {name} !!!\");"),
                        new MethodSnippet(Accessibility.Public, "", targetMethod)
                            .WithCodeStatement("Console.WriteLine($\"Hi {Name} !!!\");"),
                        new MethodSnippet(Accessibility.Public, "int", "NameLength")
                            .WithCodeStatement("return Name.Length;"))
                ;

            var code = new CodeEntity()
                .WithUsing("System")
                .AddNamespace(new NamespaceSnippet(targetNamespace)
                    .WithClasses(personClass)
                );

            var snippet = new StructSnippet(Accessibility.Public, "Address", "")
                    .WithProperties(
                        new PropertySnippet(Accessibility.Public, typeof(string), "Name"),
                        new PropertySnippet(Accessibility.Public, typeof(string), "StreetAddress"),
                        new PropertySnippet(Accessibility.Public, typeof(string), "City"),
                        new PropertySnippet(Accessibility.Public, typeof(string), "State"),
                        new PropertySnippet(Accessibility.Public, typeof(string), "Zip")
                        );

            Console.WriteLine(snippet.CSharp );

            Console.WriteLine(code.SourceCode());

            var clock = new Stopwatch();
            Console.Write("Compiling...");
            try
            {
                clock.Start();
                assembly = new DynamicAssembly()
                    .AddCodeEntitiy(code)
                    .CreateAssembly(OutputKind.DynamicallyLinkedLibrary);

                clock.Stop();
                Console.WriteLine($" Done. {clock.Elapsed.TotalMilliseconds / 1000:F3} sec");
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
                return TaskResult.Exception(ex);
            }
            catch (Exception ex)
            {
                clock.Stop();
                Console.WriteLine();
                return TaskResult.Exception(ex);
            }

            try
            {
                var personType = assembly.GetType($"{targetNamespace}.{personTypename}");
                var myType2 = assembly.GetType($"{targetNamespace}.{personTypename}");

                dynamic t = Activator.CreateInstance(personType);

                t.Name = "Sally";
                t.Hello();
                t.Hello("Luke");

                object? v = Activator.CreateInstance(personType);
                v.GetType().GetProperty("Name").SetValue(v, "Skywalker");
                //v.GetType().GetMethod("Hello").Invoke(v, new string[]);
                
                return TaskResult.Complete();

            }
            catch (Exception ex)
            {
                return TaskResult.Exception(ex);
            }
        }

    }
}
