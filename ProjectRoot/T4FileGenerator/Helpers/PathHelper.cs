using System.Reflection;

namespace T4FileGenerator.Helpers;

internal static class PathHelper
{
    public static string GetOutputFolder()
    {
        // Get the directory of the executing assembly (the exe file)
        string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        // Move up to the solution folder
        string solutionFolder = Directory.GetParent(exeDirectory)!.Parent!.Parent!.FullName;

        string outputDir = Path.Combine(solutionFolder, "Output");

        return outputDir;
    }
}
