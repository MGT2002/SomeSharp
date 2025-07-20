namespace MGTFileGenerator.Generators;

public interface IFileGenerator
{
    void CreateTableFromClass(Type type, string? generatedFilePath = null);
}
