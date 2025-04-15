using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Core.Infrastructure.Repositories;

public class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public T? GetById(int id)
    {
        return DbSet.Find(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null) return;

        DbSet.Remove(entity);
        await Context.SaveChangesAsync();
    }
}
