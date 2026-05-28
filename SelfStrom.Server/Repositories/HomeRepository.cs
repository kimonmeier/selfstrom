using Microsoft.EntityFrameworkCore;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Data.Repositories;

namespace SelfStrom.Server.Repositories;

public class HomeRepository : GenericRepository<Home>
{
    public HomeRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Home>> ListByUserAsync(string userId)
    {
        return Entities.Where(x => x.UserId == userId).Where(x => !x.IsDeleted).ToListAsync();
    }

    public Task<int> CountByUserAsync(string userId) {
        if (!Entities.Where(x => x.UserId == userId).Any())
        {
            return Task.FromResult(0);
        }

        return Entities.Where(x => x.UserId == userId).MaxAsync(x => x.Number);
    }
}
