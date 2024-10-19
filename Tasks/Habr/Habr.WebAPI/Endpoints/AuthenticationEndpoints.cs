using Habr.BusinessLogic.Interfaces.UserFeatures;
using Habr.Common.DTOs.Requests;

namespace Habr.WebAPI.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
        {
            routes = routes.MapGroup("/api/authentication").WithTags("Authentication");

            routes.MapPost("/register", async (IUserManager userManager, RegisterRequestModel model) =>
            {
                await userManager.RegistrateUserAsync(model);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapPost("/login", async (IUserManager userManager, LoginRequestModel model) =>
            {
                var token = await userManager.AuthenticateUserAsync(model);
                return Results.Ok(token);
            })
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapGet("/name", (IUserManager userManager) =>
            {
                var name = userManager.UserName;
                return Results.Ok(name);
            })
            .RequireAuthorization()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
        }
    }
}
