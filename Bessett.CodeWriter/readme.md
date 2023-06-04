# CodeWriter
Codewriter is a essentially a wrapper around the Roslyn compiler and provides a fluent-coding experience for libraries. The files are then emitted for viewing or compilation as a Dynamic assembly. As a dynamic assembly, the code can be executed directly from the running assembly.

## Dynamic Assembly
A dynamic assembly is one that is created, compiled and even executed all from memory and at runtime. The assembly can also reference the calling assembly if desired. 

Consider a case where you have an interface defined, and you emit and compile an assembly that contains a class that also implements that interface. You can invloke the class via reflection and use it as any implementation.

Alternatively, you can execute the code in the dynamic assembly as you would any dynamic class.

## Why is this Valuable?
Having a repeatable mecahnism to define code programatically is essential for software that builds code on the fly. This technique allows code to be defined in segments from a database or other store in a very abstract way. These components which would otherwise make up a program can be individually and discretely maintained and the projected code will have a high level of robustness once the foundation model is proven.

This model has been shown to bring low cycle time for updates based on business demands and changing requirements. This is largely due to the ability to bring the solution maintenance and control to a technical operations group closer to the changing busuness and freeing developers from costly build-test-release cycles.

Note that this approach also allows your assembly to be refrenced by the dynamic assembly for interfaces, classes or any other code you wish it to inherit or execute. For example, this might include tests that are dynamically written based on the current state or the referenced code.

## Simple example

Consider the class:
```C#
    using System;
    namespace HelloWorld
    {
        public  class Person
        {
            public String Name {get; set;}

            public void Hello (String Name  = "Han")
            {
                Console.WriteLine($"Hi {Name} !!!");
            }
        }
    }

```
This class is represented using fluent snippets as follows:
```C#
    var targetNamespace = "HelloWorld";
    var personTypename = "Person";
    var targetMethod = "Hello";

    var personClass = new ClassSnippet(personTypename, Accessibility.Public)
            .WithSnippets(new PropertySnippet(Accessibility.Public, typeof(string), "Name"))
            .WithSnippets(
                new MethodSnippet(Accessibility.Public, "", targetMethod)
                    .WithParameter(new FieldDef(typeof(string), "name"))
                    .WithCodeStatement("Console.WriteLine($\"Hi {name} !!!\");"))                    
            .WithSnippets(
                new MethodSnippet(Accessibility.Public, "", targetMethod)
                    .WithCodeStatement("Console.WriteLine($\"Hi {Name} !!!\");"))
        ;

    var code = new CodeEntity()
        .WithUsing("System")
        .AddNamespace(new NamespaceSnippet(targetNamespace)
            .WithClasses(personClass)
        );

    Console.WriteLine(code.SourceCode());

```
The code object created above is compiled to memory like this
```C#
    System.Reflection.Assembly assembly;

    try
    {
        assembly = new DynamicAssembly()
            .AddCodeEntitiy(code)
            .CreateAssembly(OutputKind.DynamicallyLinkedLibrary);
    }
    catch(CompileException ex)
    {
        foreach (var diagnostic in ex.Failures)
            {
                Console.WriteLine($"{diagnostic}");
            }
    }

```
And can be executed immediately from the same code
```C#
    var personType = assembly.GetType($"HelloWorld.Person");

    dynamic t = Activator.CreateInstance(personType);
    t.Name = "John";
    t.Hello();

```
## **Snippets**
The building blocks of the CodeWriter fluent library are code snippets. A snippet is simply a construct defined that implents ```ICodeSnippet``` soe it can be projected to CSharp (in this release). All snippets are essentially code snippets with their respoctive structural requirements that are rendered using the ```ICodeSnippet.ToCSharp()``` method.

### **CodeSnippet**
```CodeSnippet.FromLines(IEnumerable<string> lines)```
* This is the simplest implementation that is used to create a block of any code. Ultimately, this is simply a list of lines of code.

### **EnumSnippet**
`public EnumSnippet(Accessibility scope, string name, params AttributeSnippet[] attributes)`
* As the name implies, this will render an enumeration

### **AttributeSnippet**
`public AttributeSnippet(string name, params object[] attrParams)`

* Attributes can be added to most snippets just as that can with actual code. Snippets that can have attributes have a ```.WithAttribute()``` fluent method.

### **NamespaceSnippet**
`public NamespaceSnippet(string namespaceName)`
* Specifies a namespace to use
* A namespace is a special container that can include classes, enums, interfaces and other namespace level constructs  
  
### **ClassSnippet**
`public ClassSnippet(string name, Accessibility scope, Type baseType = null, params ValueString[] interfaces)`

`public ClassSnippet(string name, Accessibility scope, string baseType = null, params ValueString[] interfaces)`
* Class snippets are also containers for class elements like properties, fields, methods, and even other classes.
* Classes can be marked Abstract
* Classes can have a base class and any number of interfaces
* Support Partial keywork

### **InterfaceSnippet**
`InterfaceSnippet(Accessibility scope, string name)`
* Support to declare interfaces
* Support Type arguments
* Support Partial

### **ConstructorSnippet**
`public ConstructorSnippet(Accessibility typeAttribute, string name)`
* Provides fluent mecahnism for multiple constructors, including `base` and `this` implementations
 
### ImplicitOperatorMethodSnippet
`public ImplicitOperatorMethodSnippet(string resultTypeName, string sourceTypeName, string sourceParameterName)`
* Provides a simple construct for a class to support implicit operator overrides.
  
### MethodSnippet
`public MethodSnippet(Accessibility typeAttribute, string returnType, ValueString name, bool isOverride = false)`
* Basic support to descibe methods for a class
* provides parameter lists 
* Can mark as override
  
### PropertySnippet
`public PropertySnippet(Accessibility scope, string dataType, string name)`
`public PropertySnippet(Accessibility scope, Type dataType, string name)`
* Property definition construct
* Attributes
* Datatypes
* supports default initial value
* Definable inside Class and Interface

### StructSnippet
````C#
public StructSnippet(Accessibility scope, string name, string interfaceSpec)
````
Example Struct:
````C#
var snippet = new StructSnippet(Accessibility.Public, "Address", "")
    .WithProperties(
        new PropertySnippet(Accessibility.Public, typeof(string), "Name"),
        new PropertySnippet(Accessibility.Public, typeof(string), "StreetAddress"),
        new PropertySnippet(Accessibility.Public, typeof(string), "City"),
        new PropertySnippet(Accessibility.Public, typeof(string), "State"),
        new PropertySnippet(Accessibility.Public, typeof(string), "Zip")
        );
```` 
result:
````C#
public struct Address
{
   public String Name {get; set;}
   public String StreetAddress {get; set;}
   public String City {get; set;}
   public String State {get; set;}
   public String Zip {get; set;}
}
````

* Supports interface spec & partial declarations
* Definable in Namespace, Class and Interface

## Exceptions
The `CompileException` class contains any informatoin about compilation errors which allow you to provide feedback for models that cause emmitted code to fail. Below is some sample code that compiles code, and when necessary reports compilation errors as an exception handler

```c#
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

````
And here is an example of the compilation errors output

```
00001 using System;
00002 namespace HelloWorld
00003 {
00004   public  class Person
00005   {
00006      public String Name {get; set;}
00007
00008      public String Name {get; set;}
00009
00010      public void Hello (String name )
00011      {
00012         Console.WriteLine($"Hi {name} !!!");
00013      }
00014      public void Hello ()
00015      {
00016         Console.WriteLine($"Hi {Name} !!!");
00017      }
00018      public int NameLength ()
00019      {
00020         return Name.Length;
00021      }
00022   }
00023 }

(8,21): error CS0102: The type 'Person' already contains a definition for 'Name'
(16,34): error CS0229: Ambiguity between 'Person.Name' and 'Person.Name'
(20,17): error CS0229: Ambiguity between 'Person.Name' and 'Person.Name'
```
