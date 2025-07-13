using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace Generator.Helpers;

internal static class GeneratorHelper
{
    public static string CompileAndRunMethod(
    string code,
    string namespaceName,
    string className,
    string methodName,
    object[] methodParameters)
    {
        // Create syntax tree
        var syntaxTree = CSharpSyntaxTree.ParseText(code);

        // Reference necessary assemblies
        // Dynamically resolve all loaded references (for .NET 5/6/7/8)
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        // Create the compilation
        var compilation = CSharpCompilation.Create(
            "DynamicAssembly",
            [syntaxTree],
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
            return "//" + errors;
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        // Use reflection to invoke method    

        var type = assembly.GetType($"{namespaceName}.{className}");
        var instance = Activator.CreateInstance(type!);
        var method = type!.GetMethod(methodName);
        var output = method!.Invoke(instance, parameters: methodParameters);

        return output!.ToString()!;
    }
}
