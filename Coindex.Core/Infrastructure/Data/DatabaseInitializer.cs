using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Infrastructure.Data;

public class DatabaseInitializer(ApplicationDbContext context)
{
    public void Initialize()
    {
        context.Database.EnsureCreated();

        if (context.Coins.Any() || context.Bills.Any())
            return;

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var tags = new List<Tag>
        {
            new() { Name = "Rare", Description = "Rare collectible items", Color = "#FF5733" },
            new() { Name = "Ancient", Description = "Ancient collectible items", Color = "#33FF57" },
            new() { Name = "Modern", Description = "Modern collectible items", Color = "#3357FF" },
            new() { Name = "Gold", Description = "Gold collectible items", Color = "#FFD700" },
            new() { Name = "Silver", Description = "Silver collectible items", Color = "#C0C0C0" }
        };
        context.Tags.AddRange(tags);
        context.SaveChanges();

        var coins = new List<Coin>
        {
            new()
            {
                Name = "American Gold Eagle",
                Description = "Official gold bullion coin of the United States",
                Year = 2021,
                Country = "United States",
                FaceValue = 50,
                Condition = ItemCondition.BrilliantUncirculated,
                Mint = "West Point",
                Material = "Gold",
                WeightInGrams = 31.104m,
                DiameterInMM = 32.7m,
                Tags = [tags[0], tags[3], tags[2]]
            },
            new()
            {
                Name = "Canadian Silver Maple Leaf",
                Description = "Official silver bullion coin of Canada",
                Year = 2022,
                Country = "Canada",
                FaceValue = 5,
                Condition = ItemCondition.Uncirculated,
                Mint = "Royal Canadian Mint",
                Material = "Silver",
                WeightInGrams = 31.10m,
                DiameterInMM = 38.0m,
                Tags = [tags[4], tags[2]]
            },
            new()
            {
                Name = "Ancient Roman Denarius",
                Description = "Silver coin from the Roman Republic",
                Year = -100, // 100 BCE
                Country = "Roman Republic",
                FaceValue = 1,
                Condition = ItemCondition.Fine,
                Mint = "Rome",
                Material = "Silver",
                WeightInGrams = 3.9m,
                DiameterInMM = 19.0m,
                Tags = [tags[0], tags[1], tags[4]]
            },
            new()
            {
                Name = "2020 Tokyo Olympics Coin",
                Description = "Special edition coin celebrating the 2020 Tokyo Olympics",
                Year = 2020,
                Country = "Japan",
                FaceValue = 1000,
                Condition = ItemCondition.Uncirculated,
                Mint = "Japan Mint",
                Material = "Silver",
                WeightInGrams = 31.1m,
                DiameterInMM = 40.0m,
                Tags = [tags[4], tags[2]]
            }
        };
        context.Coins.AddRange(coins);
        context.SaveChanges();

        var bills = new List<Bill>
        {
            new()
            {
                Name = "U.S. $2 Bill",
                Description = "Rare $2 bill featuring Thomas Jefferson",
                Year = 2003,
                Country = "United States",
                FaceValue = 2,
                Condition = ItemCondition.VeryFine,
                Series = "2003A",
                SerialNumber = "A12345678B",
                SignatureType = "Treasury Secretary",
                BillType = "Federal Reserve Note",
                WidthInMM = 156.0m,
                HeightInMM = 66.3m,
                Tags = [tags[0], tags[2]]
            },
            new()
            {
                Name = "Canadian $10 Polymer Note",
                Description = "Modern polymer note featuring Viola Desmond",
                Year = 2018,
                Country = "Canada",
                FaceValue = 10,
                Condition = ItemCondition.Uncirculated,
                Series = "2018",
                SerialNumber = "FDH1234567",
                SignatureType = "Governor",
                BillType = "Polymer Note",
                WidthInMM = 152.4m,
                HeightInMM = 69.85m,
                Tags = [tags[2]]
            },
            new()
            {
                Name = "Japanese 10,000 Yen Note",
                Description = "High-value Japanese currency featuring Yukichi Fukuzawa",
                Year = 2004,
                Country = "Japan",
                FaceValue = 10000,
                Condition = ItemCondition.VeryGood,
                Series = "E",
                SerialNumber = "HZ123456P",
                SignatureType = "Bank of Japan Governor",
                BillType = "National Currency",
                WidthInMM = 160.0m,
                HeightInMM = 76.0m,
                Tags = [tags[2]]
            }
        };
        context.Bills.AddRange(bills);
        context.SaveChanges();

        Console.WriteLine($"Database seeded with {coins.Count} coins, {bills.Count} bills, and {tags.Count} tags.");
    }
}
