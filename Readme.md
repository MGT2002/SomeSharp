🔧 Source Generator in C# – Experimental Playground

This is an experimental project that demonstrates compile-time code generation in C# using Roslyn's IIncrementalGenerator.

It introduces a custom [CodeGenMethod] attribute that allows you to inject C# source code into your project by simply writing methods that return code as strings.

<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/18eaaa27-a736-4c0d-9588-d907b757f3fb" />

⚠️ Note: This project uses File and Assembly access inside IIncrementalGenerator, which is explicitly discouraged and unsupported by Microsoft. It is not intended for production use, but rather as a proof-of-concept and learning tool.

✨ What It Does

    ✅ Generates a simple Hello class with a constant message.

    ✅ Generates a FibonacciGen.Generated class with const int Fib0 to Fib99.

    🧪 Enables writing generator methods like this:

[CodeGenMethod]
public static string GenerateHelloWorld()
{
    return """
        namespace CodeGenHello;

        internal class Hello
        {
            public const string Hi = "Hello From Generated Code!";
        }
        """;
}

🔍 Example Output

Console.WriteLine(Hello.Hi);         // Hello From Generated Code!
Console.WriteLine(Generated.Fib9);   // 55
Console.WriteLine(Generated.Fib86);  // 420196140727489673

📦 Technologies

    C# 12 / .NET SDK

    Roslyn IIncrementalGenerator

    Source Generators

    Compile-time metaprogramming

🚫 Disclaimer

This repo is for exploration and educational purposes only. Using System.IO or reflection APIs inside source generators breaks compatibility and is unsupported.

Use responsibly and do not copy this pattern into production software.
