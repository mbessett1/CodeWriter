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
using Accessibility = Bessett.CodeWriter.CodeSnippets.Accessibility;

namespace Lookups
{
    public static class Lookup
    {
        public static string AgentType()
        {
            return "mapValue";
        }         
    }

}
namespace Bessett.CodeWriter.Tests.Tasks
{
    public interface ICanonical
    {
        object ToCanonical();
    }


    public class CanonicalAgent
    {
        public string AgentName { get; set; }
        public string AgentPhone { get; set; }
        public string AgentEmail { get; set; }
        public DateTime DateStarted { get; set; }
        public int YearsAsAgent { get; set; }
        public string AgentType { get; set; }
    }

    [TaskHelp("Test ICanonical self-referencing scenario")]
    public class Test2 : CodeTest
    {

        Dictionary<string, string> Transformations()
        {
            // sample transform description
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

            return result
                .AddLine(new StructDeclaration<int>("testInt"))
              //  .AddLine(new ObjectDeclaration<CanonicalAgent>("test"))
                .AddLine("return new CanonicalAgent()")
                .EncloseInScope(new CodeSnippet().WithLines(transformCodeLines))
                .AddLine(";");

        }

        CodeEntity GenerateCode(string targetNamespace, string targetType )
        {

            var code = new CodeEntity()
                .WithUsing("System")
                .WithUsing("Bessett.SmartConsole")
                .WithUsing("Lookups")
                .WithUsing("Bessett.CodeWriter.Tests.Tasks")
                .AddNamespace(new NamespaceSnippet(targetNamespace)

                    .WithSnippets(
                        new ClassSnippet(targetType, Accessibility.Public, typeof(ICanonical))
                            .WithSnippets(
                                new PropertySnippet(Accessibility.Public, typeof(string), "FirstName"),
                                new PropertySnippet(Accessibility.Public, typeof(string), "LastName"),
                                new PropertySnippet(Accessibility.Public, typeof(string), "Phone"),
                                new PropertySnippet(Accessibility.Public, typeof(string), "Email"),
                                new PropertySnippet(Accessibility.Public, typeof(DateTime), "StartDate"),
                                new PropertySnippet(Accessibility.Public, typeof(int), "ServiceYears")
                                )
                            .WithSnippets(
                                new MethodSnippet(Accessibility.Public, typeof(object), "ToCanonical")
                                    .WithBody(Transformer(Transformations()))
                            )
                    )
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
