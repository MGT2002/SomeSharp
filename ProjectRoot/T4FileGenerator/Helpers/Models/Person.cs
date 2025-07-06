namespace T4FileGenerator.Helpers.Models;
public class Person(string name, int age)
{
    public string Name { get; set; } = name;
    public int Age { get; set; } = age;

    public string Greet()
    {
        return $"Hello, my name is {Name} and I am {Age} years old.";
    }
}