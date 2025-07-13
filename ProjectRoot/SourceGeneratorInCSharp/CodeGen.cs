namespace SourceGeneratorInCSharp;

internal class CodeGenerators
{
    [CodeGen.CodeGenMethod]
    public string MyGeneratorMethod()
    {
        //Do all calls Here

        return "//Hello Guys";
    }
}
