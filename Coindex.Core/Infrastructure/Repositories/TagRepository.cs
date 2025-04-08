using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Infrastructure.Data;

namespace Coindex.Core.Infrastructure.Repositories;

public class TagRepository(ApplicationDbContext context) : Repository<Tag>(context), ITagRepository
{
}
