using Microsoft.EntityFrameworkCore;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Data.Repositories;

namespace SelfStrom.Server.Repositories;

public class DeviceRepository : GenericRepository<Device>
{
    public DeviceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Device>> GetDevicesByRoomAsync(Guid roomId)
    {
        return Entities.Where(x => x.RoomId == roomId).ToListAsync();
    }

    public Task<List<Device>> ListAllDevices()
    {
        return Entities.Include(x => x.Room).ToListAsync();
    }
}
