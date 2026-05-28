using Microsoft.EntityFrameworkCore;
using SelfStrom.Shared.Data.Entities;

namespace SelfStrom.Shared.Data.Repositories;

public abstract class GenericRepository<TEntity> where TEntity : class, IEntity 
{
    private readonly DbContext _dbContext;

    protected DbSet<TEntity> Entities => _dbContext.Set<TEntity>();

    protected DbContext Context => _dbContext;

    protected GenericRepository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        return (await _dbContext.Set<TEntity>().AddAsync(entity)).Entity;
    }

    public Task UpdateAsync(TEntity entity) { 
        _dbContext.Set<TEntity>().Update(entity);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(TEntity entity) {
        if (entity is IPersistenEntity persistenEntity) {
            persistenEntity.IsDeleted = true;

            return UpdateAsync(entity);
        }

        _dbContext.Set<TEntity>().Remove(entity);

        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id)
    {
        TEntity entity = await _dbContext.Set<TEntity>().Where(x => x.Id.Equals(id)).SingleAsync();

        await RemoveAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    public Task<TEntity?> FindByEntityAsync(Guid id)
    {
        return _dbContext.Set<TEntity>().SingleOrDefaultAsync(x => x.Id.Equals(id));
    }
}
