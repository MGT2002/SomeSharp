namespace GenericProject;

public static class LogExtensions
{
    public static T Log<T>(this T obj, string start = "", string end = "")
    {
        Console.WriteLine(start + obj + end);

        return obj;
    }

    public static T SeriLog<T>(this T obj, string start = "", string end = "")
    {
        try
        {
            Serilog.Log.Information(start + obj + end);
        }
        finally
        {
            Serilog.Log.CloseAndFlush(); // Ensure all logs are written
        }

        return obj;
    }
}
