using Habr.DataAccess;
using Habr.DataAccess.Repositories.Implementations;
using Habr.DataAccess.Repositories.Interfaces;
using Habr.Services.CommentFeatures;
using Habr.Services.PostFeatures;
using Habr.Services.PostFeaturesst;
using Habr.Services.UserFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IHost host = CreateHostBuilder(args).Build();

await host.RunAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration(config =>
            config.AddJsonFile("appsettings.json"))
        .ConfigureServices((context, services) =>
        {
            string cs = context.Configuration.GetConnectionString("Default") ??
                    throw new Exception("Connection string wasn't found");

            services.AddDbContext<DataContext>(options => options.UseSqlServer(cs));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<UserManager>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddScoped<PageController>();
            services.AddHostedService<ConsoleAppService>();
        });