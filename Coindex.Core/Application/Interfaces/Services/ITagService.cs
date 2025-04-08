using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
}