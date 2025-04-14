using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Application.Services;

public class CollectableItemDataGeneratorService(int? seed = null) : ICollectableItemDataGeneratorService
{
    private static readonly string[] Countries =
    [
        "United States", "Canada", "United Kingdom", "Germany", "France", "Japan", "Australia",
        "China", "Brazil", "Mexico", "India", "Russia", "Italy", "Spain"
    ];

    private static readonly string[] Mints =
    [
        "Philadelphia", "Denver", "San Francisco", "West Point", "Royal Mint",
        "Royal Canadian Mint", "Perth Mint", "Berlin", "Paris", "Tokyo"
    ];

    private static readonly string[] Materials =
    [
        "Gold", "Silver", "Copper", "Nickel", "Brass", "Bronze", "Zinc",
        "Steel", "Platinum", "Aluminum"
    ];

    private static readonly string[] SignatureTypes =
    [
        "Treasury Secretary", "Federal Reserve", "Bank President",
        "Finance Minister", "Governor"
    ];

    private static readonly string[] BillTypes =
    [
        "Federal Reserve Note", "Treasury Note", "Silver Certificate",
        "Gold Certificate", "National Bank Note", "National Currency"
    ];

    private readonly Random _random = seed.HasValue ? new Random(seed.Value) : new Random();

    public string GenerateRandomName(bool isCoin, int? number = null)
    {
        if (number.HasValue)
            return $"Test {(isCoin ? "Coin" : "Bill")} {number}";

        var randomSuffix = _random.Next(1000, 10000);
        var randomLetter = (char)('A' + _random.Next(0, 26));
        return $"{(isCoin ? "Coin" : "Bill")} {randomLetter}-{randomSuffix}";
    }

    public string GenerateRandomDescription(bool isCoin, string country, int year)
    {
        return $"This is a sample {(isCoin ? "coin" : "bill")} from {country} ({year})";
    }

    public int GenerateRandomYear(bool isCoin)
    {
        return _random.Next(isCoin ? 1800 : 1950, 2023);
    }

    public string GenerateRandomCountry()
    {
        return Countries[_random.Next(Countries.Length)];
    }

    public decimal GenerateRandomFaceValue(bool isCoin)
    {
        return Convert.ToDecimal(Math.Pow(10, _random.Next(0, isCoin ? 3 : 4)));
    }

    public ItemCondition GenerateRandomCondition()
    {
        var conditions = Enum.GetValues<ItemCondition>();
        return conditions[_random.Next(conditions.Length)];
    }

    public string GenerateRandomMint()
    {
        return Mints[_random.Next(Mints.Length)];
    }

    public string GenerateRandomMaterial()
    {
        return Materials[_random.Next(Materials.Length)];
    }

    public decimal GenerateRandomWeight()
    {
        return (decimal)Math.Round(_random.NextDouble() * 49 + 1, 2);
    }

    public decimal GenerateRandomDiameter()
    {
        return (decimal)Math.Round(_random.NextDouble() * 30 + 10, 2);
    }

    public string GenerateRandomSeries(int year)
    {
        return $"{year}-{(char)('A' + _random.Next(0, 26))}";
    }

    public string GenerateRandomSerialNumber()
    {
        return $"{(char)('A' + _random.Next(0, 26))}{_random.Next(10000000, 99999999)}";
    }

    public string GenerateRandomSignatureType()
    {
        return SignatureTypes[_random.Next(SignatureTypes.Length)];
    }

    public string GenerateRandomBillType()
    {
        return BillTypes[_random.Next(BillTypes.Length)];
    }

    public decimal GenerateRandomWidth()
    {
        return (decimal)Math.Round(_random.NextDouble() * 60 + 120, 2);
    }

    public decimal GenerateRandomHeight()
    {
        return (decimal)Math.Round(_random.NextDouble() * 30 + 60, 2);
    }

    public List<Tag> GetRandomTags(List<Tag> availableTags)
    {
        var count = _random.Next(0, Math.Min(4, availableTags.Count));
        return count == 0 ? [] : availableTags.OrderBy(_ => _random.Next()).Take(count).ToList();
    }
}
