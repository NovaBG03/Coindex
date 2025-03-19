using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Core.Infrastructure.Repositories;

public class CollectableItemRepository(ApplicationDbContext context)
    : Repository<CollectableItem>(context), ICollectableItemRepository
{
    public async Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize)
    {
        return await DbSet
            .Include(i => i.Tags)
            .AsNoTracking()
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await DbSet.CountAsync();
    }

    public async Task<CollectableItem?> GetItemByIdWithTagsAsync(int id)
    {
        return await DbSet.Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}
