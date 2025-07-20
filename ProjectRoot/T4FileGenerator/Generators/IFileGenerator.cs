namespace T4FileGenerator.Generators;

public interface IFileGenerator
{
    void CreateTableFromClass(Type type, string? generatedFilePath = null);
}
