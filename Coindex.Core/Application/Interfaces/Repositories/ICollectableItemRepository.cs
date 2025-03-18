using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Interfaces.Repositories;

public interface ICollectableItemRepository : IRepository<CollectableItem>
{
    Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
}
