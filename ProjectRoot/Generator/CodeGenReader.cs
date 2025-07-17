using Generator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.IO;
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
            System.Diagnostics.Debugger.Launch();
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
                    MethodName: context.TargetSymbol.Name,
                    FilePath: context.TargetNode.SyntaxTree.FilePath);
            }
        );

        context.RegisterSourceOutput(pipeline, static (context, model) =>
        {
            string codeGenUser = File.ReadAllText(model.FilePath, Encoding.UTF8);

            string cleanCodeUser = codeGenUser.Replace($"[{CodeGenNameSpace}.{CodeGenMethod}]", "");

            var sourceText = GeneratorHelper.CompileAndRunMethod(
                code: cleanCodeUser,
                namespaceName: model.Namespace,
                className: model.ClassName,
                methodName: model.MethodName,
                null);

            context.AddSource($"CodeGen_{model.ClassName}_{model.MethodName}.g.cs", sourceText);
        });

        AddViaUserClass(context);
    }

    private static void AddViaUserClass(IncrementalGeneratorInitializationContext context)
    {
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

    private record Model(string Namespace, string ClassName, string MethodName, string FilePath);
}

