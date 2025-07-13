#pragma warning disable IDE0005 // Using directive is unnecessary.
// Use explicit usings for Code Gen
using System;
using System.Linq;
using System.Collections.Generic;
#pragma warning restore IDE0005 // Using directive is unnecessary.

namespace SourceGeneratorInCSharp;

internal class CodeGenerators
{
    [CodeGen.CodeGenMethod]
    public static string GenerateHelloWorld()
    {
        return $$"""
            namespace CodeGenHello;

            internal class Hello
            {
             public const string Hi = "Hello From Generated Code!";
            }            
            """;
    }

    [CodeGen.CodeGenMethod]
    public static string FibonacciGen()
    {
        var consts = Enumerable.Range(0, 100)
        .Select(n => $$"""
                public const int Fib{{n}} = {{Fib(n)}};
        """);
        var code = string.Join("\n", consts);

        return $$"""
            namespace FibonacciGen;

            internal class Generated
            {
            {{code}}
            }            
            """;
    }

    private static Dictionary<int, int> memo = new();
    public static int Fib(int n)
    {
        if (memo.TryGetValue(n, out var res))
            return res;

        res = n switch
        {
            <= 1 => 1,
            _ => Fib(n - 1) + Fib(n - 2)
        };

        memo[n] = res;

        return res;
    }
}
