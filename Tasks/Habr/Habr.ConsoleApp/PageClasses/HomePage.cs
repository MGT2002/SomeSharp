using Habr.Common.Enums;
using Habr.BusinessLogic.Implementations.UserFeatures;
using Habr.BusinessLogic.Interfaces.PostFeatures;
using Habr.ConsoleApp.Enums;
using Habr.ConsoleApp.Helpers;
using Habr.Common.Models;
using Habr.Common.DTOs;

namespace Habr.ConsoleApp.PageClasses;

public class HomePage(UserManager userManager, IPostService postService)
{
    private readonly UserManager _userManager = userManager;
    private readonly IPostService _postService = postService;

    public async Task<Pages> RunAsync()
    {
        ShowHomePagePresentation();

        string input = Console.ReadLine() ?? "";
        Pages newPage = await ProcessInputInHomePageAsync(input);

        return newPage;
    }

    private void ShowHomePagePresentation()
    {
        ($"Welcome {_userManager.UserName}, input - p[my posts], d[my drafts],\n" +
        "v[view posts], q[logout], e[exit].").Log();
    }

    private async Task<Pages> ProcessInputInHomePageAsync(string input)
    {
        switch (input)
        {
            case "e":
                return Pages.None;
            case "p":
                await ProcessCurrentUserPostsAsync();
                break;
            case "d":
                await ShowDraftsAsync();
                break;
            case "v":
                return Pages.PublishedArticles;
            case "q":
                _userManager.LogOutUser();
                return Pages.Authentication;
            default:
                ($"Wrong input.").Log();
                break;
        }

        return Pages.Home;
    }

    private async Task ShowDraftsAsync()
    {
        foreach (var draft in await _postService.GetDraftListAsync(_userManager.UserId!.Value))
        {
            Console.WriteLine(draft);
        }
    }

    private async Task ProcessCurrentUserPostsAsync()
    {
        bool endProcess = false;
        while (!endProcess)
        {
            await ShowUserPostsSectionPresentationAsync();

            string input = Console.ReadLine() ?? "";
            endProcess = await ProcessInputInUserPostsSectionAsync(input);
        }
    }

    private async Task ShowUserPostsSectionPresentationAsync()
    {
        await ShowPostsAsync();

        ("\ta[Create new post], m{id}[Move post to drafts] " +
        "p{id}[Publish post]\n\td{id}[delete post], e{id}[edit post], b[back].").Log();
    }

    private async Task ShowPostsAsync()
    {
        var posts = await _postService.GetByUserIdAsync(_userManager.UserId!.Value);

        Console.WriteLine("Your posts.");
        foreach (var p in posts)
        {
            Console.WriteLine(p);
        }
    }

    private async Task<bool> ProcessInputInUserPostsSectionAsync(string input)
    {
        if (input.StartsWith('e') && int.TryParse(input.AsSpan(1), out int id))
        {
            await EditAsync(id);

            return false;
        }
        if (input.StartsWith('d') && int.TryParse(input.AsSpan(1), out id))
        {
            await DeleteAsync(id);

            return false;
        }
        if (input.StartsWith('m') && int.TryParse(input.AsSpan(1), out id))
        {
            await MoveToDraftsAsync(id);

            return false;
        }
        if (input.StartsWith('p') && int.TryParse(input.AsSpan(1), out id))
        {
            await PublishPostAsync(id);

            return false;
        }
        if (input == "a")
        {
            await AddNewPostAsync();

            return false;
        }
        if (input == "b")
        {
            return true;
        }

        ("Wrong input.").Log();
        return false;
    }

    private async Task PublishPostAsync(int id)
    {
        var result = await _postService.PublishPostAsync(id);

        if (!result.IsCompletedSuccessfully)
        {
            result.ErrorMessages.Log();
        }
    }

    private async Task MoveToDraftsAsync(int id)
    {
        ServiceActionResult result = await _postService.GetByIdAsync(id);

        if (!result.IsCompletedSuccessfully)
        {
            (result.ErrorMessages).Log();
            return;
        }

        result = await _postService.MoveToDraftsAsync(((ServiceActionResult<PostViewModel>)result).Result!);

        if (!result.IsCompletedSuccessfully)
        {
            (result.ErrorMessages).Log();
        }
    }

    private async Task EditAsync(int id)
    {
        ServiceActionResult result = await _postService.GetByIdAsync(id, GetPostOptions.IncludeCreator);

        if (!result.IsCompletedSuccessfully)
        {
            (result.ErrorMessages).Log();
            return;
        }

        PostViewModel post = ((ServiceActionResult<PostViewModel>)result).Result!;
        ModelBuilder.AssignChangeablePropertiesViaInput(post);
        result = await _postService.UpdateAsync(post);

        if (!result.IsCompletedSuccessfully)
        {
            (result.ErrorMessages).Log();
        }
    }

    private async Task DeleteAsync(int id)
    {
        var result = await _postService.DeleteByIdAsync(id);

        if (!result.IsCompletedSuccessfully)
        {
            (result.ErrorMessages).Log();
        }
    }

    private async Task AddNewPostAsync()
    {
        var model = ModelBuilder.CreatePostViewModelViaInput(_userManager);
        await _postService.AddAsync(model);
    }
}
