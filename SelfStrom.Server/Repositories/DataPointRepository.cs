using Microsoft.EntityFrameworkCore;
using SelfStrom.Server.Data;
using SelfStrom.Server.Data.Entities;
using SelfStrom.Shared.Data.Repositories;

namespace SelfStrom.Server.Repositories;

public class DataPointRepository : GenericRepository<DataPoint>
{
    public DataPointRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
