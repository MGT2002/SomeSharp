using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Configuration;
using GenericProject.WebApi.Services;
using Microsoft.AspNetCore.Hosting; // <-- Added using directive

var builder = WebApplication.CreateBuilder(args);

// Add Serilog;
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add EF Core with SQL Server
builder.Services.AddDbContext<GenericProject.WebApi.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add controllers (MVC)
builder.Services.AddControllers();

// Add application and infrastructure services
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapGet("/L", () =>
 {
     Log.Information("Log endpoint hit!");
 });

app.MapControllers();

app.Run();

public partial class Program { }