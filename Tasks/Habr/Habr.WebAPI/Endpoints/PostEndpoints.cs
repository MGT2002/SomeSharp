using Habr.Common.DTOs.Requests;
using Habr.Common.DTOs.Views;
using Habr.BusinessLogic.Interfaces.PostFeatures;
using Habr.BusinessLogic.Interfaces.UserFeatures;

namespace Habr.WebAPI.Endpoints
{
    public static class PostEndpoints
    {
        public static void MapPostEndpoints(this IEndpointRouteBuilder routes)
        {
            routes = routes.MapGroup("/api/posts").WithTags("Post").RequireAuthorization();

            routes.MapGet("/", async (IPostService postService, IUserManager userManager) =>
            {
                var userPosts = await postService.GetByUserIdAsync(userManager.UserId);
                return Results.Ok(userPosts);
            })
            .Produces<IEnumerable<PostViewModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            routes.MapGet("/drafts", async (IPostService postService, IUserManager userManager) =>
            {
                var drafts = await postService.GetDraftListAsync(userManager.UserId);
                return Results.Ok(drafts);
            })
            .Produces<List<DraftListItem>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            routes.MapGet("/published", async (IPostService postService) =>
            {
                var publishedPosts = await postService.GetPublishedPostListAsync();
                return Results.Ok(publishedPosts);
            })
            .Produces<List<PostListItem>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            routes.MapGet("{id}", async (IPostService postService, int id) =>
            {
                var postViewModel = await postService.GetByIdAsync(id);
                return Results.Ok(postViewModel);
            })
            .Produces<PostViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

            routes.MapGet("{id}/publishedPostWithComments", async (IPostService postService, int id) =>
            {
                var publishedPostViewModel = await postService.GetPublishedPostAsync(id);
                return Results.Ok(publishedPostViewModel);
            })
            .Produces<PublishedPostViewModel>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

            routes.MapPost("/", async (IPostService postService, AddPostRequestModel model) =>
            {
                int id = await postService.AddAsync(model);
                return Results.Ok(id);
            })
            .Produces<int>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

            routes.MapPut("/", async (IPostService postService, UpdatePostRequestModel model) =>
            {
                await postService.UpdateAsync(model);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapPatch("{id}/moveToDrafts", async (IPostService postService, int id) =>
            {
                await postService.MoveToDraftsAsync(id);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapPatch("{id}/publish", async (IPostService postService, int id) =>
            {
                await postService.PublishPostAsync(id);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

            routes.MapDelete("{id}", async (IPostService postService, int id) =>
            {
                await postService.DeleteByIdAsync(id);
                return Results.Ok();
            })
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

        }
    }
}
