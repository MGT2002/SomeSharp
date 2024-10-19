using Habr.Common.Resources;
using Habr.DataAccess;
using Habr.WebAPI.Helpers;
using Habr.WebAPI.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(
     builder.Configuration.GetConnectionString("Default")
     ?? throw new Exception(ExceptionMessageResource.ConfigConnectionStringNotFound)));

builder.AddBusinessLogicWithJWTAuth();
builder.Services.AddExceptionHandler<DefaultGlobalExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogLogging();

app.UseExceptionHandler("/Error");

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
