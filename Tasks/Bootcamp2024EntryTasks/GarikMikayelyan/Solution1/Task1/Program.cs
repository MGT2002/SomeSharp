using System.Globalization;
using Task1.Data;
using Task1.Interfaces;
using Task1.Models;
using Task1.Repositories;

string path = GetUserInput();
Console.WriteLine("Validation passed! -> " + path);
//string path = @"D:\Garik\VS\Solution1\Task1_loans.json";
ICreditRepository repository = new CreditRepository(path);

List<Credit> credits = repository.GetCreditModels();

while (true)
{
    Console.WriteLine("Press '1' to print all credits");
    Console.WriteLine("Press '2' to print all banks");
    Console.WriteLine("Press '3' to print all borrowers");
    Console.WriteLine("Press '4' to add new credit");
    Console.WriteLine("Press '5' to get specific borrower's" +
        " list of credits by his lastname");
    Console.WriteLine("Press '6' to Calculate the amount of the monthly" +
        " annuity payment for a given loan(by loan ID).");
    Console.WriteLine("Press '7' to get list of credits by the credit type");
    Console.WriteLine("Press 'q' to exit the app");

    switch (Console.ReadKey().Key)
    {
        case ConsoleKey.D1:
            Console.WriteLine("\n");
            PrintAllCredits();
            break;
        case ConsoleKey.D2:
            Console.WriteLine("\n");
            PrintAllBanks();
            break;
        case ConsoleKey.D3:
            Console.WriteLine("\n");
            PrintAllBorrowers();
            break;
        case ConsoleKey.D4:
            Console.WriteLine("\n");
            AddCredit();
            break;
        case ConsoleKey.D5:
            Console.WriteLine("\n");
            PrintCreditsByBorrowerLastName();
            break;
        case ConsoleKey.D6:
            Console.WriteLine("\n");
            PrintMonthlyAnnuityByLoanId();
            break;
        case ConsoleKey.D7:
            Console.WriteLine("\n");
            PrintCreditsByCreditType();
            break;
        case ConsoleKey.Q:
            Console.WriteLine("\nGoodbye!");
            return;
        default:
            Console.WriteLine("Wrong input!");
            continue;
    }
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}

void PrintCreditsByCreditType()
{
    while (true)
    {
        Console.Write($"Enter the credit type.\n" +
            $"Press 'a' for {CreditType.Auto}, 'm' for {CreditType.Mortgage}," +
            $" 'e' for {CreditType.Education}");

        CreditType type;
        switch (Console.ReadKey().Key)
        {
            case ConsoleKey.M:
                type = CreditType.Mortgage;
                break;
            case ConsoleKey.A:
                type = CreditType.Auto;
                break;
            case ConsoleKey.E:
                type = CreditType.Education;
                break;
            default:
                Console.WriteLine("\nWrong input!");
                continue;
        }

        Console.WriteLine($"\n\nThe list of credits of type {type}:");
        var creditsByType = credits.Where(c => c.CreditType == type);
        PrintCredits(creditsByType);
        return;
    }
}

void PrintMonthlyAnnuityByLoanId()
{
    Console.Write("Enter credit id: ");
    int creditId = int.Parse(Console.ReadLine()!);

    var credit = credits.FirstOrDefault(c => c.ID == creditId);
    if (credit == null)
    {
        Console.WriteLine($"Credit with credit id {creditId} not found.");
        return;
    }

    double monthlyInterestRate = (double)credit.Percent / 1200;

    double annuityCoefficient = monthlyInterestRate * (
        Math.Pow(1 + monthlyInterestRate, credit.CountOfMonth) /
        (Math.Pow(1 + monthlyInterestRate, credit.CountOfMonth) - 1)
        );

    decimal monthlyPayment = credit.Amount * (decimal)annuityCoefficient;

    Console.WriteLine($"Monthly annuity payment : {monthlyPayment}");
}

void PrintCreditsByBorrowerLastName()
{
    Console.Write("Enter the borrowers last name: ");
    string lastName = Console.ReadLine()!;

    var creditsByLastName = credits.Where(c =>
    c.Borrower.LastName.ToLower() == lastName.ToLower()).ToList();

    if (creditsByLastName.Count == 0)
    {
        Console.WriteLine($"Credits with the last name '{lastName}' are not found!");
        return;
    }

    Console.WriteLine($"The list of credits for '{lastName}':");
    PrintCredits(creditsByLastName);
}

void AddCredit()
{
    Credit credit;
    while (true)
    {
        Console.WriteLine("Choose the credit type: Press...");
        Console.WriteLine($"  'a' - {CreditType.Auto}");
        Console.WriteLine($"  'm' - {CreditType.Mortgage}");
        Console.WriteLine($"  'e' - {CreditType.Education}");

        var key = Console.ReadKey().Key;
        Console.WriteLine();

        switch (key)
        {
            case ConsoleKey.M:
                Console.Write("  Address: ");
                string address = Console.ReadLine()!;
                Console.Write("  Square: ");
                decimal square = decimal.Parse(Console.ReadLine()!);
                credit = new MortgageCredit()
                {
                    CreditType = CreditType.Mortgage,
                    AddressOfObject = address,
                    Square = square,
                };
                break;
            case ConsoleKey.A:
                Console.Write("  Car model: ");
                string carModel = Console.ReadLine()!;
                Console.Write("  Car brand: ");
                string carBrand = Console.ReadLine()!;
                Console.Write("  VIN: ");
                string vin = Console.ReadLine()!;
                credit = new AutoCredit()
                {
                    CreditType = CreditType.Auto,
                    CarModel = carModel,
                    CarBrand = carBrand,
                    VIN = vin,
                };
                break;
            case ConsoleKey.E:
                Console.Write("  University address: ");
                string uAddress = Console.ReadLine()!;
                Console.Write("  University name: ");
                string uName = Console.ReadLine()!;
                credit = new EducationCredit()
                {
                    CreditType = CreditType.Education,
                    UniversityAddress = uAddress,
                    UniversityName = uName,
                };
                break;
            default:
                Console.WriteLine("\nWrong input!");
                continue;
        }

        break;
    }

    Console.Write("  Id: ");
    credit.ID = int.Parse(Console.ReadLine()!);
    Console.Write("Amount: ");
    credit.Amount = decimal.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);
    Console.Write("Count of month: ");
    credit.CountOfMonth = int.Parse(Console.ReadLine()!);
    Console.Write("Percent: ");
    credit.Percent = decimal.Parse(Console.ReadLine()!, CultureInfo.InvariantCulture);

    credit.Borrower = new Borrower();
    Console.WriteLine("Borrower information:");
    Console.Write("  Id: ");
    credit.Borrower.Id = int.Parse(Console.ReadLine()!);
    Console.Write("  Name: ");
    credit.Borrower.FirstName = Console.ReadLine()!;
    Console.Write("  Last name: ");
    credit.Borrower.LastName = Console.ReadLine()!;
    Console.Write("  Date of birth (format dd/MM/yyyy): ");
    credit.Borrower.DateOfBirth = DateTime.ParseExact(Console.ReadLine()!, "dd/MM/yyyy", CultureInfo.InvariantCulture);
    Console.Write("  Passport number: ");
    credit.Borrower.PassportNumber = Console.ReadLine()!;

    credit.Bank = new Bank();
    Console.WriteLine("Bank information:");
    Console.Write("  Id: ");
    credit.Bank.Id = int.Parse(Console.ReadLine()!);
    Console.Write("  Name: ");
    credit.Bank.Name = Console.ReadLine()!;
    Console.Write("  Address: ");
    credit.Bank.Address = Console.ReadLine()!;

    credits.Add(credit);
}

void PrintAllBorrowers()
{
    Console.WriteLine("List of all borrowers:");
    var borrowers = credits.Select(c => c.Borrower).Distinct();
    foreach (var borrower in borrowers)
    {
        Console.WriteLine($"id: {borrower.Id} | {borrower.LastName} " +
            $"{borrower.FirstName} | {borrower.DateOfBirth} |" +
            $" {borrower.PassportNumber}");
    }
    Console.WriteLine();
}

void PrintAllBanks()
{
    Console.WriteLine("List of all banks:");
    var banks = credits.Select(c => c.Bank).Distinct();
    foreach (var bank in banks)
    {
        Console.WriteLine($"id: {bank.Id} | {bank.Name} | {bank.Address}");
    }
    Console.WriteLine();
}

void PrintAllCredits()
{
    Console.WriteLine("List of all credits:");

    PrintCredits(credits);
}

static string GetUserInput()
{
    string path;
    while (true)
    {
        Console.Write("Path = ");
        path = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(path) || Path.GetExtension(path) != ".json"
            || !File.Exists(path))
        {
            Console.WriteLine("Wrong input!");
            continue;
        }

        return path;
    }
}

static void PrintCredits(IEnumerable<Credit> credits)
{
    foreach (var credit in credits)
    {
        Console.WriteLine($"ID : {credit.ID} | Amount : {credit.Amount} |" +
            $" Percent : {credit.Percent}% |Months : {credit.CountOfMonth} |" +
            $" Credit type : {credit.CreditType} | Bank name : {credit.Bank.Name} |" +
            $" Borrower : {credit.Borrower.LastName} {credit.Borrower.FirstName}\n");
    }
}