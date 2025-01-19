using Core.Interfaces;
using Core.Repositories;
using Core.Services;
using JsonInfrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Presentation;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ISceneRepository, SceneRepository>();
builder.Services.AddScoped<ICreatureRepository, CreatureRepository>();
builder.Services.AddScoped<IGameManager, DreamWorldGameManager>();

await builder.Build().RunAsync();
