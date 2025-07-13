using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace Generator;

[Generator]
public sealed class ExampleGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            var sourceText = $$"""
                using System;
                namespace Code
                {
                    public static class Gen
                    {
                        public static void SayHello()
                        {
                            Console.WriteLine("Hello From Generator");
                        }
                    }
                }
                """;

            ctx.AddSource("ExampleGenerator.g.cs", SourceText.From(sourceText, Encoding.UTF8));
        });
    }
}