using CodeGen;

namespace SourceGeneratorInCSharp;

internal class CodeGen
{
    [CodeGenMethod]
    public string Fibonacci(int n)
    {
        if (n is 0 or 1)
        {
            return "0";
        }

        return n.ToString();
    }
}
