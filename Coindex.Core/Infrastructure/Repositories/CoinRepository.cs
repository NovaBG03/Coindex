using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Infrastructure.Data;

namespace Coindex.Core.Infrastructure.Repositories;

public class CoinRepository(ApplicationDbContext context) : Repository<Coin>(context), ICoinRepository
{
}
