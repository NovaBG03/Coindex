using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;

namespace Coindex.Core.Infrastructure.Data;

public class DatabaseInitializer(ApplicationDbContext context, ICollectableItemDataGeneratorService dataGenerator)
{
    public void Initialize()
    {
        context.Database.EnsureCreated();

        if (!context.Coins.Any() || !context.Bills.Any())
        {
            SeedTestData();
        }
    }

    private void SeedTestData()
    {
        Console.WriteLine("Seeding database for testing...");

        var tags = new List<Tag>
        {
            new() { Name = "Rare", Description = "Rare collectible items", Color = "#FF5733" },
            new() { Name = "Ancient", Description = "Ancient collectible items", Color = "#33FF57" },
            new() { Name = "Modern", Description = "Modern collectible items", Color = "#BBC6FA" },
            new() { Name = "Gold", Description = "Gold collectible items", Color = "#FFD700" },
            new() { Name = "Silver", Description = "Silver collectible items", Color = "#C0C0C0" }
        };
        context.Tags.AddRange(tags);
        context.SaveChanges();

        var newCoins = new List<Coin>();
        for (var i = 1; i <= 25; i++)
        {
            var year = dataGenerator.GenerateRandomYear(true);
            var country = dataGenerator.GenerateRandomCountry();

            newCoins.Add(new Coin
            {
                Name = dataGenerator.GenerateRandomName(true, i),
                Description = dataGenerator.GenerateRandomDescription(true, country, year),
                Year = year,
                Country = country,
                FaceValue = dataGenerator.GenerateRandomFaceValue(true),
                Condition = dataGenerator.GenerateRandomCondition(),
                Mint = dataGenerator.GenerateRandomMint(),
                Material = dataGenerator.GenerateRandomMaterial(),
                WeightInGrams = dataGenerator.GenerateRandomWeight(),
                DiameterInMM = dataGenerator.GenerateRandomDiameter(),
                Tags = dataGenerator.GetRandomTags(tags)
            });
        }

        context.Coins.AddRange(newCoins);

        var newBills = new List<Bill>();
        for (var i = 1; i <= 10; i++)
        {
            var year = dataGenerator.GenerateRandomYear(false);
            var country = dataGenerator.GenerateRandomCountry();

            newBills.Add(new Bill
            {
                Name = dataGenerator.GenerateRandomName(false, i),
                Description = dataGenerator.GenerateRandomDescription(false, country, year),
                Year = year,
                Country = country,
                FaceValue = dataGenerator.GenerateRandomFaceValue(false),
                Condition = dataGenerator.GenerateRandomCondition(),
                Series = dataGenerator.GenerateRandomSeries(year),
                SerialNumber = dataGenerator.GenerateRandomSerialNumber(),
                SignatureType = dataGenerator.GenerateRandomSignatureType(),
                BillType = dataGenerator.GenerateRandomBillType(),
                WidthInMM = dataGenerator.GenerateRandomWidth(),
                HeightInMM = dataGenerator.GenerateRandomHeight(),
                Tags = dataGenerator.GetRandomTags(tags)
            });
        }

        context.Bills.AddRange(newBills);
        context.SaveChanges();

        Console.WriteLine($"Database seeded with {newCoins.Count} coins and {newBills.Count} bills for testing.");
    }
}
