using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Services;

public class CollectableItemService(ICollectableItemRepository collectableItemRepository) : ICollectableItemService
{
    public async Task<IEnumerable<CollectableItem>> GetAllItemsAsync()
    {
        return await collectableItemRepository.GetAllAsync();
    }

    public async Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize)
    {
        return await collectableItemRepository.GetPagedItemsAsync(pageNumber, pageSize);
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await collectableItemRepository.GetTotalCountAsync();
    }

    public async Task<CollectableItem?> GetItemByIdAsync(int id)
    {
        return await collectableItemRepository.GetByIdAsync(id);
    }
}
