using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Filters;
using Coindex.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Core.Infrastructure.Repositories;

public class CollectableItemRepository(ApplicationDbContext context)
    : Repository<CollectableItem>(context), ICollectableItemRepository
{
    public async Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize,
        CollectableItemFilter? filter = null)
    {
        var query = DbSet.Include(i => i.Tags).AsNoTracking();

        if (filter is not null)
        {
            query = ApplyFilter(query, filter);
        }

        return await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync(CollectableItemFilter? filter = null)
    {
        var query = DbSet.AsQueryable();

        if (filter is not null)
        {
            query = ApplyFilter(query, filter);
        }

        return await query.CountAsync();
    }

    public async Task<CollectableItem?> GetItemByIdWithTagsAsync(int id)
    {
        return await DbSet.Include(x => x.Tags)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    private static IQueryable<CollectableItem> ApplyFilter(IQueryable<CollectableItem> query,
        CollectableItemFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            query = query.Where(i => i.Name.Contains(filter.Name));
        }

        if (filter.TagId.HasValue)
        {
            query = query.Where(i => i.Tags.Any(t => t.Id == filter.TagId.Value));
        }

        if (filter.Condition.HasValue)
        {
            query = query.Where(i => i.Condition == filter.Condition.Value);
        }

        return query;
    }
}
