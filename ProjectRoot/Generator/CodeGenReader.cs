using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using static Generator.Names;

namespace Generator;

[Generator]
public class CodeGenReader : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            //System.Diagnostics.Debugger.Launch();
        }

        AddCodeGenAttribute(context);

        var pipeline = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: $"{CodeGenNameSpace}.{CodeGenMethodAttribute}",
            predicate: static (syntaxNode, cancellationToken) => syntaxNode is MethodDeclarationSyntax or BaseMethodDeclarationSyntax,
            transform: static (context, cancellationToken) =>
            {
                var containingClass = context.TargetSymbol.ContainingType;

                return new Model(
                    // Note: this is a simplified example. You will also need to handle the case where the type is in a global namespace, nested, etc.
                    Namespace: containingClass.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)),
                    ClassName: containingClass.Name,
                    MethodName: context.TargetSymbol.Name);
            }
        );

        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            var sourceText = SourceText.From($$"""
                //Data man {{model}}
                """, Encoding.UTF8);

            context.AddSource($"CodeGen_{model.MethodName}.g.cs", sourceText);
        });

        AddViaUserClass(context);
    }

    private static void AddViaUserClass(IncrementalGeneratorInitializationContext context)
    {
        //TODO: read from XML
#pragma warning disable RS1035 // Do not use APIs banned for analyzers
        string codeGenUser = File.ReadAllText(
            "D:\\Garik\\MyProjects\\SomeSharp\\ProjectRoot\\SourceGeneratorInCSharp\\CodeGen.cs",
        Encoding.UTF8);
#pragma warning restore RS1035 // Do not use APIs banned for analyzers

    context.RegisterPostInitializationOutput(ctx =>
        {
            var sourceText = "//empty";

            ctx.AddSource($"UserGenerator.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        });
    }

    private static void AddCodeGenAttribute(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            var sourceText = $$"""
                namespace {{CodeGenNameSpace}}
                {
                    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
                    sealed class {{CodeGenMethodAttribute}} : System.Attribute
                    {

                    }   
                }
                """;

            ctx.AddSource($"{nameof(CodeGenMethodAttribute)}.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        });
    }

    private record Model(string Namespace, string ClassName, string MethodName);
}

