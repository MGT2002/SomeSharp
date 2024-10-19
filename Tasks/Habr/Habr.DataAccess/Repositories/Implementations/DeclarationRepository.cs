using Habr.DataAccess.Entities;
using Habr.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Habr.DataAccess.Repositories.Implementations;

public class DeclarationRepository(DataContext dataContext) : IDeclarationRepository
{
    private readonly DataContext _dataContext = dataContext;

    public async Task<Declaration?> GetDeclarationById(int id)
    {
        return await _dataContext.Declarations.FirstOrDefaultAsync(d => d.Id == id);
    }
}
