using Microsoft.EntityFrameworkCore;

namespace PVScan.Shared.Data
{
    public abstract class EntityFrameworkRepository<TEntity, TContext> : IRepository<TEntity>, IUnitOfWork
        where TEntity : class 
        where TContext : DbContext
    {
        private readonly TContext _context;
        private  DbSet<TEntity> _dbSet => _context.Set<TEntity>();

        public EntityFrameworkRepository(TContext context)
        {
            _context = context;
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);

            return entity;
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);

            return entity;
        }

        public async Task<TEntity> DeleteAsync(TEntity entity)
        {
            _dbSet.Remove(entity);

            return entity;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
