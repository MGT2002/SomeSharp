using Habr.BusinessLogic.Interfaces.CommentFeatures;
using Habr.Common.DTOs.Requests;

namespace Habr.WebAPI.Endpoints
{
    public static class CommentEndpoints
    {
        public static void MapCommentEndpoints(this IEndpointRouteBuilder routes)
        {
            routes = routes.MapGroup("/api/comments").WithTags("Comment").RequireAuthorization();

            routes.MapPost("/", async (ICommentService commentService, WriteCommentRequestModel model) =>
            {
                await commentService.AddCommentAsync(model);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapDelete("{id}", async (ICommentService commentService, int id) =>
            {
                await commentService.DeleteCommentAsync(id);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
        }
    }
}
