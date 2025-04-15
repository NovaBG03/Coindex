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

    public async Task<Tag> CreateTagAsync(string name, string description, string color)
    {
        var newTag = new Tag
        {
            Name = name,
            Description = description,
            Color = color
        };
        await tagRepository.AddAsync(newTag);
        return newTag;
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        await tagRepository.UpdateAsync(tag);
    }

    public Tag? GetTagById(int id)
    {
        return tagRepository.GetById(id);
    }
}
