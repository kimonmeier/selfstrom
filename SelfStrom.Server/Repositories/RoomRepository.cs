using Microsoft.EntityFrameworkCore;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Data.Repositories;

namespace SelfStrom.Server.Repositories;

public class RoomRepository : GenericRepository<Room>
{
    public RoomRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Room>> ListByHome(Guid homeId)
    {
        return Entities.Where(x => x.HomeId == homeId).Where(x => !x.IsDeleted).ToListAsync();
    }
    public Task<int> CountByHomeAsync(Guid homeId)
    {
        if (!Entities.Where(x => x.HomeId == homeId).Any())
        {
            return Task.FromResult(0);
        }

        return Entities.Where(x => x.HomeId == homeId).MaxAsync(x => x.Number);
    }
}
