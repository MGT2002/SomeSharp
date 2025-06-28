using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using GenericProject.WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logging
RegistrationHelper.AddSerilogLogging();
Log.Information("Starting application[Registering services]");

// Add EF Core with SQL Server
builder.Services.AddDbContext<GenericProject.WebApi.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers (MVC)
builder.Services.AddControllers();

// Add application and infrastructure services
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();
Log.Information("Application built successfully!");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapGet("/", () =>
{
    Log.Information("Root endpoint hit!");
    return "Hello World!";
});
app.MapGet("/L", () =>
 {
     Log.Information("Log endpoint hit!");
 });

app.MapControllers();

Log.Information("Application Start!");
app.Run();
Log.Information("Application End!");
Log.CloseAndFlush();
public partial class Program { }