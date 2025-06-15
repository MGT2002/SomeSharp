namespace WebApplication1;

public static class LogExtensions
{
    public static T Log<T>(this T obj, string start = "", string end = "")
    {
        Console.WriteLine(start + obj + end);

        return obj;
    }
}
