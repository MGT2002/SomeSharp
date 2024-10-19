using Habr.DataAccess.Entities;

namespace Habr.DataAccess.Repositories.Interfaces;

public interface IDeclarationRepository
{
    public Task<Declaration?> GetDeclarationById(int id);
}
