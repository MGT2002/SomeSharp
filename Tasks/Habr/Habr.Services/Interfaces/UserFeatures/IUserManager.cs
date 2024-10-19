using Habr.Common.DTOs.Requests;

namespace Habr.BusinessLogic.Interfaces.UserFeatures;

public interface IUserManager
{
    public int UserId { get; }

    public string UserName { get; }

    public string UserEmail { get; }

    public Task RegistrateUserAsync(RegisterRequestModel model);

    public Task<string> AuthenticateUserAsync(LoginRequestModel model);

    public Task<bool> ManageUserAsync(int userId);
}
