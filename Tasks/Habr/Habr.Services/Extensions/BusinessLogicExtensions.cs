using Habr.BusinessLogic.Implementations.CommentFeatures;
using Habr.BusinessLogic.Implementations.PostFeatures;
using Habr.BusinessLogic.Implementations.UserFeatures;
using Habr.BusinessLogic.Interfaces.CommentFeatures;
using Habr.BusinessLogic.Interfaces.PostFeatures;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using Habr.BusinessLogic.MapProfiles;
using Habr.DataAccess.Repositories.Implementations;
using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Habr.BusinessLogic.Extensions;

public static class BusinessLogicExtensions
{
    public static void AddBusinessLogicServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(CommentProfile).Assembly);

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDeclarationRepository, DeclarationRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();

        services.AddSingleton<IJwtGenerator, JwtGenerator>();
        services.AddScoped<IUserManager, UserManager>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<ICommentService, CommentService>();
    }
}
