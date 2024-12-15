using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var person = new Person
{
    Name = "John Doe",
    Age = 30,
    Email = "johndoe@example.com",
    Child = new() { Name = "Child", Age = 12, Email = "Child@Cm.com" }
};

// Serialize the person object to a JSON string
string jsonString = JsonSerializer.Serialize(person, IgnoreJsonIgnoreConverter<Person>.CreateSerializerOptions());
Console.WriteLine("Serialized JSON:");
Console.WriteLine(jsonString);

// Deserialize the JSON string back into a Person object
var deserializedPerson = JsonSerializer.Deserialize<Person>(jsonString,
    IgnoreJsonIgnoreConverter<Person>.CreateSerializerOptions())!;
Console.WriteLine("\nDeserialized Object:");
Console.WriteLine(deserializedPerson);

public class Person
{
    public string Name { get; set; } = "";
    [JsonIgnore]
    public int Age { get; set; }
    public string Email { get; set; } = "";
    [JsonIgnore]
    public Person? Child { get; set; }

    public override string ToString()
    {
        return $"{{Name={Name}, Age={Age}, Email={Email}" +
            $"\nChild={Child?.ToString() ?? "null"}}}";
    }
}

public static class DeepCopyExtension
{
    public static T DeepCopy<T>(this T source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return (T)DeepCopyObject(source);
    }

    private static object DeepCopyObject(object source)
    {
        var sourceType = source.GetType();

        if (sourceType.IsPrimitive || sourceType == typeof(string))
            return source;

        // Handle collections (IEnumerable, excluding strings)
        if (source is IEnumerable enumerable && !(source is string))
        {
            var elementType = source.GetType().GetElementType() ?? sourceType.GetGenericArguments().First();
            var copyList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            foreach (var item in enumerable)
            {
                copyList.Add(DeepCopyObject(item));
            }

            return copyList;
        }

        // Handle creating a new instance of the source type
        var target = Activator.CreateInstance(sourceType);

        // Get all properties and fields of the source type
        var members = sourceType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
            .ToList();

        foreach (var member in members)
        {
            if (member is PropertyInfo propertyInfo)
            {
                // If the property has a getter, copy its value
                if (propertyInfo.CanRead)
                {
                    var value = propertyInfo.GetValue(source);
                    propertyInfo.SetValue(target, DeepCopyObject(value));
                }
            }
            else if (member is FieldInfo fieldInfo)
            {
                // For fields, directly get and set the value
                var value = fieldInfo.GetValue(source);
                fieldInfo.SetValue(target, DeepCopyObject(value));
            }
        }

        return target;
    }
}



