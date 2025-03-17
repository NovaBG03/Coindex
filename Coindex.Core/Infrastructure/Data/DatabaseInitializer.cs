using Coindex.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Core.Infrastructure.Data;

public class DatabaseInitializer(ApplicationDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.EnsureCreatedAsync();

        return;
        if (!await context.Coins.AnyAsync())
        {
            var coins = new List<Coin>
            {
                new()
                {
                    Name = "1921 Morgan Silver Dollar",
                    Description = "Last year of the Morgan dollar series, produced at Philadelphia mint"
                },
                new()
                {
                    Name = "1909-S VDB Lincoln Cent",
                    Description = "Rare first-year Lincoln cent with designer's initials, from San Francisco mint"
                },
                new()
                {
                    Name = "1856 Flying Eagle Cent",
                    Description = "Rare pattern coin that preceded the Indian Head cent"
                },
                new()
                {
                    Name = "2009 Ultra High Relief Double Eagle",
                    Description = "Modern recreation of Saint-Gaudens' classic design in high relief"
                },
                new()
                {
                    Name = "1794 Flowing Hair Dollar",
                    Description = "First silver dollar minted by the United States"
                }
            };

            await context.Coins.AddRangeAsync(coins);
            await context.SaveChangesAsync();

            Console.WriteLine($"Database seeded with {coins.Count} sample coins.");
        }
    }
}
