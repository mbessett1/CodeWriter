using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bessett.CodeWriter.CodeSnippets;
using Bessett.SmartConsole;

namespace Bessett.CodeWriter.Tests.Tasks
{
    public class Test3 : CodeTest
    {
        Dictionary<string, string> Transformations()
        {
            // No equals sign, no semicolons

            return new Dictionary<string, string>()
            {
                { "AgentName", "FirstName + \" \" + LastName" },
                { "AgentPhone", "Phone" },
                { "AgentEmail", "Email" },
                { "DateStarted", "StartDate" },
                { "YearsAsAgent", "ServiceYears" },
                { "AgentType" ,  "Lookup.AgentType()"}
            };
        }

        private CodeSnippet Transformer(Dictionary<string, string> transform)
        {
            var result = new CodeSnippet();
            var transformCodeLines = new List<string>();
            foreach (var xform in transform)
            {
                transformCodeLines.Add($"{xform.Key} = {xform.Value},");
            }

            return result.AddLine("return new CanonicalAgent()")
                .EncloseInScope(new CodeSnippet().WithLines(transformCodeLines))
                .AddLine(";");

        }

        CodeEntity GenerateCode(string targetNamespace, string targetType)
        {
            var typeParamClass = new ClassSnippet("TypeParamDemo", Accessibility.Public)
                    .IsAbstract().IsPartial()
                    .WithTypeParameter(
                        new TypeParameter("T1", "class", "new()"),
                        new TypeParameter("T2", "struct")
                    )
                    .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Name"))
                    .WithSnippets(new MethodSnippet(Accessibility.Public, null, "DoVoidMethod" ))  // this selects the void
                    .WithSnippets(new MethodSnippet(Accessibility.Public, "void", "DoMethod")
                        .WithParameter(new FieldDef(typeof(string), "Name", "Han"))
                        .WithCodeStatement("Console.WriteLine($\"Hi {Name} !!!\");"))
                ;

            var classSnippet = new ClassSnippet(targetType, Accessibility.Public, typeof(ICanonical)).IsAbstract()
                .WithSnippets(
                    new PropertySnippet(Accessibility.Public, typeof(string), "FirstName" ),
                    new PropertySnippet(Accessibility.Public, typeof(string), "LastName"),
                    new PropertySnippet(Accessibility.Public, typeof(string), "Phone"),
                    new PropertySnippet(Accessibility.Public, typeof(string), "Email"),
                    new PropertySnippet(Accessibility.Public, typeof(DateTime), "StartDate"),
                    new PropertySnippet(Accessibility.Public, typeof(int), "ServiceYears"))
                .WithMethods(
                    new ImplicitOperatorMethodSnippet("string", targetType, "x")
                        .WithCodeStatement("return string.Empty;"),
                    new MethodSnippet(Accessibility.Public, typeof(object), "ToCanonical")
                        .WithBody(Transformer(Transformations())),
                    new ConstructorSnippet(Accessibility.Public, targetType)
                        .WithParameter(FieldDef.Simple("int", "foo"))  )
                .WithConstructors(
                    new ConstructorSnippet(Accessibility.Public)
                    .WithParameter(FieldDef.Simple("string", "foo"))
                    .WithInitializer(),
                    new ConstructorSnippet(Accessibility.Public)
                    )
                ;

            var snippet = new CodeSnippet();

            var code = new CodeEntity()
                .WithUsing("System")
                .WithUsing("Bessett.SmartConsole")
                .WithUsing("Lookups")
                .WithUsing("Bessett.CodeWriter.Tests.Tasks")
                .AddNamespace(
                    new NamespaceSnippet(targetNamespace).WithSnippets(classSnippet, typeParamClass)
                );

            return code;
        }

        public override TaskResult StartTask()
        {
            var targetNamespace = "Sand.Agent";
            var targetType = "Agent";
            var code = GenerateCode(targetNamespace, targetType);

            Console.WriteLine(code.SourceCode());

            System.Reflection.Assembly assembly = null;
            TaskResult taskResult = null;

            if (!CompileCode(code, out assembly, out taskResult)) return taskResult;

            try
            {
                dynamic tester = Activator.CreateInstance(assembly.GetType($"{targetNamespace}.{targetType}"));

                tester.FirstName = "Jester";
                tester.LastName = "Kirby";
                tester.Phone = "555-112-2211";
                tester.ServiceYears = 15;
                tester.StartDate = DateTime.Now.AddYears(-tester.ServiceYears);
                var agent = tester.ToCanonical();

                Console.WriteLine($"Name:           {agent.AgentName}");
                Console.WriteLine($"Phone:          {agent.AgentPhone}");
                Console.WriteLine($"Email:          {agent.AgentEmail}");
                Console.WriteLine($"Type:           {agent.AgentType}");
                Console.WriteLine($"DateStarted:    {agent.DateStarted}");
                Console.WriteLine($"YearsInService: {agent.YearsAsAgent}");
                return TaskResult.Complete();

                //                return TaskResult.Complete();
            }
            catch (Exception ex)
            {
                return TaskResult.Exception(ex);
            }
        }

    }
}
