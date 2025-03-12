using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ICoinService
{
    Task<Coin?> GetCoinByIdAsync(int id);
    Task<IEnumerable<Coin>> GetAllCoinsAsync();
    Task AddCoinAsync(Coin coin);
    Task UpdateCoinAsync(Coin coin);
    Task DeleteCoinAsync(int id);
}
