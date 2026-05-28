using Microsoft.EntityFrameworkCore;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Data.Repositories;

namespace SelfStrom.Server.Repositories;

public class WorkerRepository : GenericRepository<Worker>
{
    public WorkerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Worker>> ListByUserAsync(string userId)
    {
        return Entities.Where(x => x.Home.UserId == userId).Include(x => x.Home).ToListAsync();
    }

    public Task<int> CountByUserAsync(string userId)
    {
        if (!Entities.Where(x => x.Home.UserId == userId).Any())
        {
            return Task.FromResult(0);
        }

        return Entities.Where(x => x.Home.UserId == userId).MaxAsync(x => x.Number);
    }

    public Task<bool> IsDeviceAssignedToWorker(Guid deviceId, Guid workerId)
    {
        Guid deviceHomeId = Context.Set<Device>().Include(x => x.Room).Where(x => x.Id.Equals(deviceId)).Select(x => x.Room.HomeId).Single();
        Guid workerHomeId = Context.Set<Worker>().Where(x => x.Id.Equals(workerId)).Select(x => x.HomeId).Single();

        return Task.FromResult(workerHomeId.Equals(deviceHomeId));
    }
}
