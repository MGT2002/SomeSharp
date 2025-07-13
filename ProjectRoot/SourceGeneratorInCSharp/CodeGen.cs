using System.Linq;

namespace SourceGeneratorInCSharp;

internal class CodeGenerators
{
    [CodeGen.CodeGenMethod]
    public static string MyGeneratorMethod()
    {
        var consts = Enumerable.Range(0, 10)
        .Select(n => $$"""
            public class Fib{{n}}{
                public const int Value = {{Fib(n)}};
            }
        """);
        var code = string.Join("\n", consts);

        return $$"""
            namespace SourceGeneratorInCSharp;

            internal class Generated
            {
            {{code}}
            }            
            """;
    }

    public static int Fib(int n)
    {
        return n switch
        {
            <= 1 => 1,
            _ => Fib(n - 1) + Fib(n - 2)
        };
    }
}
