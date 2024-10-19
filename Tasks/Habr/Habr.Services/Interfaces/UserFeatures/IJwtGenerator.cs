namespace Habr.BusinessLogic.Interfaces.UserFeatures;

public interface IJwtGenerator
{
    public string GenerateToken(int userId);
}
