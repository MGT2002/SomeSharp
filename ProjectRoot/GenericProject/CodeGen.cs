namespace GenericProject;

public class CodeGen
{
    public static string ValueGen<T>(T value)
    {
        var type = typeof(T);
        if (type == typeof(int))
        {
            return "100";
        }

        return value!.ToString()!;
    }
}
