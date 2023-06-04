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

    [NoConfirmation]
    public class Test : ConsoleTask
    {

        public override TaskResult StartTask()
        {
            System.Reflection.Assembly assembly;

            var targetNamespace = "HelloWorld";
            var targetType = "CoolType";
            var targetType2 = "CoolType2";
            var targetMethod = "Hello";

            var myEnum = new EnumSnippet(Accessibility.Public, "MyEnum")
                        .WithValue("True", 1)
                        .WithValue("False", 0);

            var myInterface = new InterfaceSnippet()
                    .WithName("ISpecial")
                    .WithScope(Accessibility.Public)
                    .WithSnippets(new InterfaceMethodSnippet(null, "Method")
                                    .WithParameter(FieldDef.Simple(typeof(double), "Wingspan")))
                    .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Title"))
                ;

            var myClassSnippet = new ClassSnippet(targetType2, Accessibility.Public)
                    .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Nombre"))
                    .WithSnippets(new MethodSnippet(Accessibility.Public, "", targetMethod)
                    .WithParameter(new FieldDef(typeof(string), "Name", "Han"))
                    .WithCodeStatement("Console.WriteLine($\"Hi {Name} !!!\");"))
                ;

            var typeParamClass = new ClassSnippet("TypeParamDemo", Accessibility.Public)
                    .IsAbstract().IsPartial()
                    .WithTypeParameter(new TypeParameter("T", "class", "new()") )
                    .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Name"))
                    .WithSnippets(new MethodSnippet(Accessibility.Public, null, targetMethod)
                        .WithParameter(new FieldDef(typeof(string), "Name", "Han"))
                        .WithCodeStatement("Console.WriteLine($\"Hi {Name} !!!\");"))
                ;

            //myClassSnippet
            //        .WithSnippets(QuickSnippets.ConvertSnippet("Transform", targetType2, targetType,
            //            new CodeSnippet().AddLine($"Nombre = \"No One\"")))
            //        .WithSnippets(QuickSnippets.ImplicitOperatorSnippet(targetType2, targetType,
            //            new CodeSnippet().AddLine($"Nombre = \"No One\"")))
            //    ;

            var code = new CodeEntity()
                .WithUsing("System")
                .AddNamespace(new NamespaceSnippet(targetNamespace)
                    .WithClasses(typeParamClass, myClassSnippet)
                    .WithSnippets(new ClassSnippet(targetType, Accessibility.Public)
                        .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Name"))
                        .WithSnippets(new MethodSnippet(Accessibility.Public, "void", targetMethod)
                            .WithParameter(new FieldDef(typeof(string), "Name", "Darth"))
                            .WithCodeStatement("Console.WriteLine($\"Hi {Name}!\");")))
                );

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
                var myType = assembly.GetType($"{targetNamespace}.{targetType}");
                var myType2 = assembly.GetType($"{targetNamespace}.{targetType2}");

                dynamic t = Activator.CreateInstance(myType);
                dynamic t2 = Activator.CreateInstance(myType2);

                var v = Activator.CreateInstance(myType);
                var v2 = Activator.CreateInstance(myType2);
                v.GetType().GetProperty("Name").SetValue(v, "Skywalker");
                v2.GetType().GetProperty("Nombre").SetValue(v2, "Vader");

                //var v5 = v2.GetType().GetMethod("Transform").Invoke(v2, new [] { v } );

                t2.Nombre = "Fred";

                t.Hello();
                t.Hello("Luke");

                t2.Hello();
                t2.Hello("Luke");
                t2.Hello(t2.Nombre);

                //v2 = t2.Transform(t);       // dynamic works much better than var

                t2 = t;

                //var v3 = t2.Transform(v);   // throws

                return TaskResult.Complete();

                /*
                 
                 dynamic is not interpreted as a target type, so equals override does not
                 really work. It does a dynamic operation.

                 To make this work, need to implement an interface that includes ICanonical.
                 ICanonical should have ToCanonical() support for a conversion.
                                  
                */



            }
            catch (Exception ex)
            {
                return TaskResult.Exception(ex);
            }
        }

    }
}
