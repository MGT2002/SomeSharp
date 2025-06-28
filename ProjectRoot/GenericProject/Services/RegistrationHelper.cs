using System.Reflection;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace GenericProject.WebApi.Services;

public static class RegistrationHelper
{
    public static void AddSerilogLogging()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
         ?? throw new InvalidOperationException("Could not determine base path for configuration(Serilog).");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        Log.Information("Serilog logging configured successfully.\n"
        + " Base path: {BasePath}", basePath);
    }
}

