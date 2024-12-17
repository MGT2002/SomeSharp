using DeepCopy;
using System.Text.Json.Serialization;

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
copy.Child!.Name = "adsadsd";
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


