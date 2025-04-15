using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllTagsAsync();
    Task<Tag?> GetTagByIdAsync(int id);
    Tag? GetTagById(int id);
    Task<Tag> CreateTagAsync(string name, string description, string color);
    Task UpdateTagAsync(Tag tag);
}
