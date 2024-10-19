using Habr.BusinessLogic.Extensions;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using Habr.BusinessLogic.Implementations.UserFeatures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using Habr.WebAPI.Endpoints;

namespace Habr.WebAPI.Helpers;

public static class Extensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }});
        });
    }

    public static void AddBusinessLogicWithJWTAuth(this IHostApplicationBuilder builder)
    {
        var jwtSection = builder.Configuration.GetSection(nameof(JwtSettings));
        var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new($"Section {nameof(JwtSettings)} wasn't found.");
        builder.Services.Configure<JwtSettings>(jwtSection);

        builder.Services.AddBusinessLogicServices();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new()
                {
                    OnTokenValidated = async context =>
                    {
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<IUserManager>();
                        var userId = int.Parse(context.Principal!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                        await userManager.ManageUserAsync(userId);
                    }
                };
            });
        builder.Services.AddAuthorization();
    }

    public static void AddSerilogLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSerilog();

        builder.Host.UseSerilog(Log.Logger);
    }

    public static void UseSerilogLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();
    }

    public static void MapEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapAuthenticationEndpoints();
        routes.MapPostEndpoints();
        routes.MapCommentEndpoints();
    }
}
