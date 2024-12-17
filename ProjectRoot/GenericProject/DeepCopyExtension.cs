using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace GenericProject;

public static class DeepCopyExtension
{
    public static object DeepCopy(this object source)
    {
        var copy = source.CallMemberwiseClone();

        var refFields = copy.GetReferenceFieldsWithoutStrings();
        for (int i = 0; i < refFields.Length; i++)
        {
            var value = refFields[i].GetValue(copy);
            if (value is null)
            {
                continue;
            }
            if (value is IEnumerable)
            {
                value = value.CopyWithJsonSerializer();
            }
            else
            {
                value = value.DeepCopy();
            }

            refFields[i].SetValue(copy, value);
        }

        return copy;
    }

    private static FieldInfo[] GetReferenceFieldsWithoutStrings(this object obj)
    {
        Type type = obj.GetType();

        FieldInfo[] allFields = type.GetFields(BindingFlags.Instance
            | BindingFlags.NonPublic
            | BindingFlags.Public);

        return Array.FindAll(allFields, field => !field.FieldType.IsValueType && field.FieldType != typeof(string));
    }

    private static object CallMemberwiseClone(this object obj)
    {
        Type type = obj.GetType();

        MethodInfo? memberwiseCloneMethod = type.GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);

        if (memberwiseCloneMethod == null)
            throw new InvalidOperationException("The method MemberwiseClone is not available.");

        // Invoke the method on the object and return the result
        return memberwiseCloneMethod.Invoke(obj, null)!;
    }

    private static object CopyWithJsonSerializer(this object obj)
    {
        return JsonSerializer.Deserialize(JsonSerializer.Serialize(obj), returnType: obj.GetType())!;
    }
}
