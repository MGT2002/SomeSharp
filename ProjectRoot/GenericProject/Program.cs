using DeepCopy;
using System.Text.Json.Serialization;

var person = new Person(1, 2)
{
    Name = "John Doe",
    Age = 30,
    Email = "johndoe@example.com",
    Child = new(1, 2) { Name = "Child", Age = 12, Email = "Child@Cm.com" }
};
var person2 = new Person(1, 2)
{
    Name = "2",
    Age = 30,
    Email = "2",
    Child = new(1, 2) { Name = "Child2", Age = 12, Email = "Child2@Cm.com" }
};
var person3 = new Person(1, 2)
{
    Name = "3",
    Age = 30,
    Email = "3",
    Child = new(3, 4) { Name = "Child3", Age = 12, Email = "Child3@Cm.com" }
};

person.people.AddRange([person2, person3]);

var copy = DeepCopier.Copy(person);

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
    public List<Person> people = [];
    [JsonIgnore]
    private IEnumerable<object> some = [1, "asdsad", null, new List<int>[1, 3, 6, 1]];

    public Person(int a, int b)
    {
        
    }

    public override string ToString()
    {
        string some = string.Join(',', this.some);
        string peopleText = "people: [\n";
        foreach (var i in people)
        {
            peopleText += i.ToString();
            peopleText += "\n";
        }
        peopleText += "]";

        return $"{{\nName={Name}, Age={Age}, Email={Email}" +
            $"\nChild={Child?.ToString() ?? "null"}\n{peopleText}\nsome: {some}}}";
    }
}
