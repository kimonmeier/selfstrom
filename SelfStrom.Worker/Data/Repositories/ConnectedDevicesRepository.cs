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

internal class ConnectedDevicesRepository : GenericRepository<ConnectedDevices>
{
    public ConnectedDevicesRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<ConnectedDevices?> FindByMacAddress(string macAddress)
    {
        return Entities.Where(x => x.MacAdress == macAddress).Cacheable().SingleOrDefaultAsync();
    }

    public Task<List<Guid>> ListAllIds()
    {
        return Entities.Select(x => x.Id).Cacheable().ToListAsync();
    }

    public Task<List<ConnectedDevices>> ListAll()
    {
        return Entities.Cacheable().ToListAsync();
    }
}
