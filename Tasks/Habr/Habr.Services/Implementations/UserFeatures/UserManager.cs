using Habr.Common.DTOs.Requests;
using Habr.Common.Validators;
using Habr.Common.Exceptions;
using Habr.Common.Resources;
using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Habr.BusinessLogic.Helpers;
using Habr.BusinessLogic.Interfaces.UserFeatures;
using Serilog;

namespace Habr.BusinessLogic.Implementations.UserFeatures;
public class UserManager(IUserRepository userRepository, IJwtGenerator jwtGenerator) : IUserManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtGenerator _jwtGenerator = jwtGenerator;

    private User? CurrentAuthenticatedUser { get; set; } = null;

    public int UserId => CurrentAuthenticatedUser?.Id ?? throw DomainException.NotFound;

    public string UserName => CurrentAuthenticatedUser?.Name ?? throw DomainException.NotFound;

    public string UserEmail => CurrentAuthenticatedUser?.Email ?? throw DomainException.NotFound;

    public async Task RegistrateUserAsync(RegisterRequestModel model)
    {
        var errorMessages = model.ValidateModelWithAnnotations();

        if (errorMessages is null)
        {
            if (await IsEmailAlreadyRegisteredAsync(model.Email!))
            {
                throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.EmailAlreadyTaken);
            }
        }
        else
        {
            throw DomainException.GenerateUnprocessableAction(errorMessages);
        }

        Log.Information("Registration success.");
        await AddUserAsync(model);
    }

    public async Task<string> AuthenticateUserAsync(LoginRequestModel model)
    {
        var user = await _userRepository.GetUserByEmailAsync(model.Email!)
            ?? throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.EmailIncorrect);
        
        if (!PasswordHelper.VerifyPassword(model.Password!, user.PasswordHash, user.PasswordSalt))
        {
            throw DomainException.GenerateUnprocessableAction(ExceptionMessageResource.PasswordWrong);
        }

        Log.Information("User logged in successfuly.");
        return _jwtGenerator.GenerateToken(user.Id);
    }

    public async Task<bool> ManageUserAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return false;
        }

        CurrentAuthenticatedUser = user;
        return true;
    }

    private async Task<bool> IsEmailAlreadyRegisteredAsync(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email) is not null;
    }

    private async Task AddUserAsync(RegisterRequestModel model)
    {
        var (hash, salt) = PasswordHelper.CreatePasswordHash(model.Password!);

        await _userRepository.AddAsync(new()
        {
            Name = model.Email!.Split('@')[0],
            Email = model.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        });
    }
}
