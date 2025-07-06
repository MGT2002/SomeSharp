using T4FileGenerator.Helpers;
using T4FileGenerator.RuntimeGenerators;

string outputDir = PathHelper.GetOutputFolder();

GenerateFile<InsertPeople>(fileExtension: ".sql");

void GenerateFile<T>(string fileExtension) where T : new()
{
    var type = typeof(T);
    T instance = new();

    string transformedText = type.GetMethod("TransformText")!.Invoke(instance, null)!.ToString()!;

    File.WriteAllText(
        Path.Combine(outputDir, type.Name + fileExtension),
        transformedText.ToString()
    );

    Console.WriteLine($"File generated: {type.Name}{fileExtension}");
}