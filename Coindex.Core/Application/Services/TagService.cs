using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        return await tagRepository.GetAllAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(int id)
    {
        return await tagRepository.GetByIdAsync(id);
    }
}
