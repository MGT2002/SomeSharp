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

var copy = (Person)person.Clone();

Print(person, copy);
Console.WriteLine(new string('_', 50));
copy.Email = default!;
copy.Child.Name = "adsadsd";
Print(person, copy);

static void Print(Person person, Person copy)
{
    Console.WriteLine(person);
    Console.WriteLine(copy);
}

public class Person : ICloneable
{
    public string Name { get; set; } = "";
    [JsonIgnore]
    public int Age { get; set; }
    public string Email { get; set; } = "";
    [JsonIgnore]
    public Person? Child { get; set; }
    [JsonIgnore]
    public List<Person> people = [];

    public override string ToString()
    {
        return $"{{Name={Name}, Age={Age}, Email={Email}" +
            $"\nChild={Child?.ToString() ?? "null"}}}";
    }

    public object Clone()
    {
        var clone = this.DeepCopy();
        clone.Child = this.Child.DeepCopy();
        clone.people = this.people.DeepCopy();
        clone.Age = this.Age.DeepCopy();

        return clone;
    }
}

public static class DeepCopyExtension
{
    public static T DeepCopy<T>(this T source)
    {
        var json = JsonSerializer.Serialize(source);
        var copy = JsonSerializer.Deserialize<T>(json)!;

        return copy;
    }
}

