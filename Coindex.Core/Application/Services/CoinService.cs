using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Application.Services;

public class CoinService(ICoinRepository coinRepository) : ICoinService
{
    public async Task<Coin?> GetCoinByIdAsync(int id)
    {
        return await coinRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Coin>> GetAllCoinsAsync()
    {
        return await coinRepository.GetAllAsync();
    }

    public async Task AddCoinAsync(Coin coin)
    {
        await coinRepository.AddAsync(coin);
    }

    public async Task UpdateCoinAsync(Coin coin)
    {
        await coinRepository.UpdateAsync(coin);
    }

    public async Task DeleteCoinAsync(int id)
    {
        await coinRepository.DeleteAsync(id);
    }
}
