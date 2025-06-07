using GenericProject;
using Serilog;

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug() // Set minimum log level
           .WriteTo.Console() // Log to console
           .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log to daily files
           .CreateLogger();

var scpecialZipCode = "AAAA";

var result = new ApiResult() { Users = [new() { Address = new() { ZipCode = "BBBB" } }, new() { Address = new()}] };

if (result.Log("1->").Users.First().Address.Log("2->").ZipCode.Log("Zip->")
    == scpecialZipCode.SeriLog("Special is: "))
{
    "IsSpecial".Log();
}
else
{
    0.Log();
}

class ApiResult
{
    public required User[] Users { get; set; }
}

class User
{
    public required Address Address { get; set; }
}

class Address
{
    public string ZipCode { get; set; } = "";
}