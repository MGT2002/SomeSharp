// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;

Console.WriteLine("Hello, World!");
Code.Gen.SayHello();

RunCode();

static void RunCode()
{
    string code = @"
using System;

namespace SourceGeneratorInCSharp
{
    public class CodeGen
    {
        public string Fibonacci(int n)
        {
            if (n is 0 or 1)
            {
                return ""0"";
            }

            return n.ToString();
        }
    }
}
";

    var namespaceName = "SourceGeneratorInCSharp";
    var className = "CodeGen";
    var methodName = "Fibonacci";
    var methodParameters = new object[] { 48 };

    var output = CompileAndRun(code, namespaceName, className, methodName, methodParameters);

    Console.WriteLine($"Result: {output}");
}

static string CompileAndRun(
    string code,
    string namespaceName,
    string className,
    string methodName,
    object[] methodParameters)
{
    // Create syntax tree
    var syntaxTree = CSharpSyntaxTree.ParseText(code);

    // Reference necessary assemblies
    var references = new[]
    {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location)
        };

    // Create the compilation
    var compilation = CSharpCompilation.Create(
        "DynamicAssembly",
        new[] { syntaxTree },
        references,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    );

    using var ms = new MemoryStream();
    var result = compilation.Emit(ms);

    if (!result.Success)
    {
        var errors = "";
        foreach (var diagnostic in result.Diagnostics)
        {
            errors += diagnostic.ToString();
            Console.WriteLine(diagnostic.ToString());
        }
        throw new Exception(errors);
    }

    ms.Seek(0, SeekOrigin.Begin);
    var assembly = Assembly.Load(ms.ToArray());

    // Use reflection to invoke method    

    var type = assembly.GetType($"{namespaceName}.{className}");
    var instance = Activator.CreateInstance(type);
    var method = type.GetMethod(methodName);
    var output = method!.Invoke(instance, parameters: methodParameters);

    return output!.ToString()!;
}
