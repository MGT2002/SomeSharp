using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Linq;
using System.Text.Json.Serialization;
using System.Net.Http.Json;

var person = new Person
{
    Name = "John Doe",
    Age = 30,
    Email = "johndoe@example.com",
    Child = new() { Name = "Child", Age = 12, Email = "Child@Cm.com" }
};

person.people.AddRange([200, 100]);

var copy = (Person)person.DeepCopy();

Print(person, copy);
Console.WriteLine(new string('_', 50));
copy.Email = default!;
copy.Child.Name = "adsadsd";
Print(person, copy);

static void Print(Person person, Person copy)
{
    Console.WriteLine("Person Start!");
    Console.WriteLine(person);
    Console.WriteLine("Person End!");
    Console.WriteLine("Copy Start!");
    Console.WriteLine(copy);
    Console.WriteLine("Copy End!");
}

public class Person
{
    public string Name { get; set; } = "";
    [JsonIgnore]
    public int Age { get; set; }
    public string Email { get; set; } = "";
    [JsonIgnore]
    public Person? Child { get; set; }
    [JsonIgnore]
    public List<int> people = [];

    public override string ToString()
    {
        string peopleText = "people: [\n";
        foreach (var i in people)
        {
            peopleText += i.ToString();
            peopleText += "\n";
        }
        peopleText += "]";

        return $"{{\nName={Name}, Age={Age}, Email={Email}" +
            $"\nChild={Child?.ToString() ?? "null"}\n{peopleText}\n}}";
    }
}

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

