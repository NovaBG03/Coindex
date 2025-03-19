using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Infrastructure.Data;

public class DatabaseInitializer(ApplicationDbContext context)
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

        const int seed = 1234;
        var random = new Random(seed);
        var countries = new[]
        {
            "United States", "Canada", "United Kingdom", "Germany", "France", "Italy", "Spain", "Japan", "China",
            "Australia"
        };
        var mints = new[] { "US Mint", "Royal Mint", "Royal Canadian Mint", "Perth Mint", "Berlin Mint", "Paris Mint" };
        var materials = new[] { "Gold", "Silver", "Platinum", "Copper", "Nickel", "Bronze", "Brass" };

        var newCoins = new List<Coin>();
        for (var i = 1; i <= 25; i++)
        {
            var year = random.Next(1900, 2023);
            var country = countries[random.Next(countries.Length)];
            var faceValue = Convert.ToDecimal(Math.Pow(10, random.Next(0, 3)));
            var condition = (ItemCondition)random.Next(0, 7);
            newCoins.Add(new Coin
            {
                Name = $"Test Coin {i}",
                Description = $"Pagination test coin {i} from {country} ({year})",
                Year = year,
                Country = country,
                FaceValue = faceValue,
                Condition = condition,
                Mint = mints[random.Next(mints.Length)],
                Material = materials[random.Next(materials.Length)],
                WeightInGrams = (decimal)Math.Round(random.NextSingle() * 50, 2),
                DiameterInMM = (decimal)Math.Round(random.NextSingle() * 40 + 10, 2),
                Tags = GetRandomTags(tags, random)
            });
        }

        context.Coins.AddRange(newCoins);

        var newBills = new List<Bill>();
        for (var i = 1; i <= 10; i++)
        {
            var year = random.Next(1950, 2023);
            var country = countries[random.Next(countries.Length)];
            var faceValue = Convert.ToDecimal(Math.Pow(10, random.Next(0, 4)));
            var condition = (ItemCondition)random.Next(0, 7);
            newBills.Add(new Bill
            {
                Name = $"Test Bill {i}",
                Description = $"Pagination test bill {i} from {country} ({year})",
                Year = year,
                Country = country,
                FaceValue = faceValue,
                Condition = condition,
                Series = $"{year}-{(char)('A' + random.Next(0, 26))}",
                SerialNumber = GenerateRandomSerialNumber(random),
                SignatureType = "Finance Minister",
                BillType = "National Currency",
                WidthInMM = (decimal)Math.Round(random.NextSingle() * 50 + 120, 2),
                HeightInMM = (decimal)Math.Round(random.NextSingle() * 20 + 60, 2),
                Tags = GetRandomTags(tags, random)
            });
        }

        context.Bills.AddRange(newBills);
        context.SaveChanges();

        Console.WriteLine($"Database seeded with {newCoins.Count} coins and {newBills.Count} bills for testing.");
    }

    private static List<Tag> GetRandomTags(List<Tag> availableTags, Random random)
    {
        var count = random.Next(1, Math.Min(4, availableTags.Count));
        var shuffled = availableTags.OrderBy(_ => random.Next()).Take(count).ToList();
        return shuffled;
    }

    private static string GenerateRandomSerialNumber(Random random)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var prefix = chars[random.Next(chars.Length)];
        var suffix = chars[random.Next(chars.Length)];
        var digits = string.Join("", Enumerable.Range(0, 8).Select(_ => random.Next(10).ToString()));
        return $"{prefix}{digits}{suffix}";
    }
}
