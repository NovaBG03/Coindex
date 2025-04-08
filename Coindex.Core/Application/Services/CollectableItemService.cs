using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Filters;

namespace Coindex.Core.Application.Services;

public class CollectableItemService(ICollectableItemRepository collectableItemRepository) : ICollectableItemService
{
    public async Task<IEnumerable<CollectableItem>> GetAllItemsAsync()
    {
        return await collectableItemRepository.GetAllAsync();
    }

    public async Task<IEnumerable<CollectableItem>> GetPagedItemsAsync(int pageNumber, int pageSize,
        CollectableItemFilter? filter = null)
    {
        return await collectableItemRepository.GetPagedItemsAsync(pageNumber, pageSize, filter);
    }

    public async Task<int> GetTotalCountAsync(CollectableItemFilter? filter = null)
    {
        return await collectableItemRepository.GetTotalCountAsync(filter);
    }

    public async Task<CollectableItem?> GetItemByIdAsync(int id)
    {
        return await collectableItemRepository.GetByIdAsync(id);
    }

    public async Task<CollectableItem?> GetItemByIdWithTagsAsync(int id)
    {
        return await collectableItemRepository.GetItemByIdWithTagsAsync(id);
    }

    public async Task<int> AddItemAsync(CollectableItem item)
    {
        await collectableItemRepository.AddAsync(item);
        return item.Id;
    }

    public async Task UpdateItemAsync(CollectableItem item)
    {
        await collectableItemRepository.UpdateAsync(item);
    }
}
