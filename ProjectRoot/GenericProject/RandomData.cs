namespace GenericProject;

public static class RandomDataGenerator
{
    private static readonly Random rand = new();

    public static int GetRandomId() => rand.Next(1, 10000);
    public static string GetRandomName() => "Name" + rand.Next(1, 10000);
    public static DateTime GetRandomStartDate() => DateTime.Now.AddDays(rand.Next(-1000, 1000));
    public static DateTime GetRandomEndDate(DateTime startDate) => startDate.AddDays(rand.Next(1, 365));
    public static int GetRandomCreatedBy() => rand.Next(1, 1000);
    public static DateTime GetRandomCreatedDate() => DateTime.Now.AddMinutes(-rand.Next(1, 100000));
    public static int GetRandomModifiedBy() => rand.Next(1, 1000);
    public static DateTime GetRandomModifiedDate(DateTime createdDate) => createdDate.AddMinutes(rand.Next(1, 100000));
}

