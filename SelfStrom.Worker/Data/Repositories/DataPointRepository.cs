using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using SelfStrom.Shared.Data.Repositories;
using SelfStrom.Worker.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfStrom.Worker.Data.Repositories;

internal class DataPointRepository : GenericRepository<DataPoint>
{
    public DataPointRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<bool> HasDataPoints(Guid deviceId)
    {
        return Entities.Cacheable().AnyAsync(x => x.DeviceId == deviceId);
    }

    public Task<List<DataPoint>> ListUnsynchedPoints()
    {
        return Entities.Where(x => !x.IsSynced).ToListAsync();
    }
}
