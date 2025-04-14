using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Application.Services;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;
using Coindex.Core.Domain.Filters;
using Coindex.Core.Infrastructure.Data;
using Coindex.Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SQLitePCL;

namespace Coindex.Cli;

internal abstract class Program
{
    private static async Task Main(string[] args)
    {
        string? customDbPath = null;
        var remainingArgs = new List<string>();

        for (var i = 0; i < args.Length; i++)
            if ((args[i] == "--db" || args[i] == "--database") && i + 1 < args.Length)
            {
                customDbPath = args[i + 1];
                i++; // Skip the next argument since it's the path
            }
            else
            {
                remainingArgs.Add(args[i]);
            }

        var dbPath = customDbPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "coindex.db3");

        var serviceProvider = ConfigureServices(dbPath);
        EnsureDatabaseCreated(serviceProvider, dbPath);

        //  if no arguments provided default to help
        if (remainingArgs.Count == 0)
        {
            ShowHelp();
            return;
        }

        var command = remainingArgs[0].ToLower();
        switch (command)
        {
            case "list":
                await ListCollectables(serviceProvider, remainingArgs.Skip(1).ToArray());
                break;
            case "show":
                await ShowCollectable(serviceProvider, remainingArgs.Skip(1).ToArray());
                break;
            case "tags":
                await ListTags(serviceProvider);
                break;
            case "search":
                await SearchCollectables(serviceProvider, remainingArgs.Skip(1).ToArray());
                break;
            case "stats":
                await ShowStats(serviceProvider);
                break;
            case "help":
            case "--help":
            case "-h":
                ShowHelp();
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowHelp();
                break;
        }
    }

    private static ServiceProvider ConfigureServices(string dbPath)
    {
        var services = new ServiceCollection();

        Console.WriteLine($"Using database at: {dbPath}");

        // Initialize SQLite
        Batteries_V2.Init();

        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        // Repositories
        services.AddTransient<ICollectableItemRepository, CollectableItemRepository>();
        services.AddTransient<ITagRepository, TagRepository>();

        // Services
        services.AddTransient<ICollectableItemService, CollectableItemService>();
        services.AddTransient<ITagService, TagService>();
        services.AddTransient<ICollectableItemDataGeneratorService, CollectableItemDataGeneratorService>();
        services.AddTransient<DatabaseInitializer>();

        return services.BuildServiceProvider();
    }

    private static void EnsureDatabaseCreated(ServiceProvider serviceProvider, string dbPath)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            // Try to connect to validate schema
            if (dbContext.Database.CanConnect())
                // Verify schema by executing a simple query
                try
                {
                    dbContext.Tags.FirstOrDefault();
                    Console.WriteLine("Database exists and schema is valid.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database schema is invalid: {ex.Message}");
                    Console.WriteLine("Recreating database...");

                    // Delete existing database
                    dbContext.Database.EnsureDeleted();
                }

            // Create and initialize the database
            Console.WriteLine("Creating database and seeding data...");
            dbContext.Database.EnsureCreated();
            var dataGenerator = serviceProvider.GetRequiredService<ICollectableItemDataGeneratorService>();
            var initializer = new DatabaseInitializer(dbContext, dataGenerator);
            initializer.Initialize();
            Console.WriteLine("Database initialized with seed data.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");

            // Try to delete the file directly as a last resort
            try
            {
                if (File.Exists(dbPath))
                {
                    File.Delete(dbPath);
                    Console.WriteLine("Deleted existing database file.");

                    // Try again with a fresh database
                    dbContext.Database.EnsureCreated();
                    var dataGenerator = serviceProvider.GetRequiredService<ICollectableItemDataGeneratorService>();
                    var initializer = new DatabaseInitializer(dbContext, dataGenerator);
                    initializer.Initialize();
                    Console.WriteLine("Database initialized with seed data.");
                }
            }
            catch (Exception innerEx)
            {
                Console.WriteLine($"Fatal error: Could not recreate database: {innerEx.Message}");
                Environment.Exit(1);
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine("Coindex CLI - Manage your collectables from the command line");
        Console.WriteLine();
        Console.WriteLine("Global options:");
        Console.WriteLine("  --db <path>, --database <path>");
        Console.WriteLine("      Specify a custom database file path");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        Console.WriteLine("  list [--tag <tagName>] [--condition <condition>] [--page <pageNumber>]");
        Console.WriteLine("      List collectables with optional filtering");
        Console.WriteLine();
        Console.WriteLine("  show <id>");
        Console.WriteLine("      Show details for a specific collectable");
        Console.WriteLine();
        Console.WriteLine("  tags");
        Console.WriteLine("      List all available tags");
        Console.WriteLine();
        Console.WriteLine("  search <query>");
        Console.WriteLine("      Search for collectables matching the query");
        Console.WriteLine();
        Console.WriteLine("  stats");
        Console.WriteLine("      Show statistics about your collection");
        Console.WriteLine();
        Console.WriteLine("  help");
        Console.WriteLine("      Show this help message");
    }

    private static async Task ListCollectables(ServiceProvider serviceProvider, string[] args)
    {
        var collectableService = serviceProvider.GetRequiredService<ICollectableItemService>();
        var tagService = serviceProvider.GetRequiredService<ITagService>();

        // Parse arguments
        var pageNumber = 1;
        string? tagName = null;
        ItemCondition? condition = null;

        for (var i = 0; i < args.Length; i++)
            switch (args[i])
            {
                case "--page" when i + 1 < args.Length:
                {
                    if (int.TryParse(args[i + 1], out var page)) pageNumber = page;

                    i++;
                    break;
                }
                case "--tag" when i + 1 < args.Length:
                    tagName = args[i + 1];
                    i++;
                    break;
                case "--condition" when i + 1 < args.Length:
                {
                    if (Enum.TryParse<ItemCondition>(args[i + 1], true, out var parsedCondition))
                        condition = parsedCondition;

                    i++;
                    break;
                }
            }

        // Build filter
        var filter = new CollectableItemFilter();

        // Add tag filter if specified
        if (!string.IsNullOrWhiteSpace(tagName))
        {
            var tags = await tagService.GetAllTagsAsync();
            var tag = tags.FirstOrDefault(t =>
                t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (tag != null)
            {
                filter.TagId = tag.Id;
                Console.WriteLine($"Filtering by tag: {tag.Name}");
            }
            else
            {
                Console.WriteLine($"Tag '{tagName}' not found.");
            }
        }

        // Add condition filter if specified
        if (condition.HasValue)
        {
            filter.Condition = condition.Value;
            Console.WriteLine($"Filtering by condition: {condition.Value}");
        }

        // Get collectables
        var collectables = await collectableService.GetPagedItemsAsync(pageNumber, 10, filter);
        var totalCount = await collectableService.GetTotalCountAsync(filter);
        var totalPages = (int)Math.Ceiling(totalCount / 10.0);

        Console.WriteLine($"Page {pageNumber} of {totalPages} (Total items: {totalCount})");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"ID",-5} {"Type",-6} {"Name",-30} {"Year",-6} {"Condition",-20} {"Tags",-20}");
        Console.WriteLine(new string('-', 80));

        foreach (var item in collectables)
        {
            var type = item is Coin ? "Coin" : "Bill";
            var tags = string.Join(", ", item.Tags.Select(t => t.Name));
            Console.WriteLine(
                $"{item.Id,-5} {type,-6} {TruncateString(item.Name, 30),-30} {item.Year,-6} {item.Condition,-20} {TruncateString(tags, 20),-20}");
        }

        Console.WriteLine(new string('-', 80));
    }

    private static async Task ShowCollectable(ServiceProvider serviceProvider, string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please specify a collectable ID.");
            return;
        }

        if (!int.TryParse(args[0], out var id))
        {
            Console.WriteLine("Invalid ID format. Please provide a number.");
            return;
        }

        var collectableService = serviceProvider.GetRequiredService<ICollectableItemService>();
        var item = await collectableService.GetItemByIdWithTagsAsync(id);

        if (item == null)
        {
            Console.WriteLine($"Collectable with ID {id} not found.");
            return;
        }

        Console.WriteLine(new string('=', 80));
        Console.WriteLine($"ID: {item.Id}");
        Console.WriteLine($"Name: {item.Name}");
        Console.WriteLine($"Description: {item.Description}");
        Console.WriteLine($"Year: {item.Year}");
        Console.WriteLine($"Country: {item.Country}");
        Console.WriteLine($"Face Value: {item.FaceValue}");
        Console.WriteLine($"Condition: {item.Condition}");

        Console.WriteLine(item.Tags.Count != 0
            ? $"Tags: {string.Join(", ", item.Tags.Select(t => t.Name))}"
            : "Tags: None");

        switch (item)
        {
            case Coin coin:
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Coin Properties:");
                Console.WriteLine($"Mint: {coin.Mint}");
                Console.WriteLine($"Material: {coin.Material}");
                Console.WriteLine($"Weight: {coin.WeightInGrams}g");
                Console.WriteLine($"Diameter: {coin.DiameterInMM}mm");
                break;
            case Bill bill:
                Console.WriteLine(new string('-', 40));
                Console.WriteLine("Bill Properties:");
                Console.WriteLine($"Series: {bill.Series}");
                Console.WriteLine($"Serial Number: {bill.SerialNumber}");
                Console.WriteLine($"Signature Type: {bill.SignatureType}");
                Console.WriteLine($"Bill Type: {bill.BillType}");
                Console.WriteLine($"Dimensions: {bill.WidthInMM}mm x {bill.HeightInMM}mm");
                break;
        }

        Console.WriteLine(new string('=', 80));
    }

    private static async Task ListTags(ServiceProvider serviceProvider)
    {
        var tagService = serviceProvider.GetRequiredService<ITagService>();
        var tags = await tagService.GetAllTagsAsync();

        Console.WriteLine("Available Tags:");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Description",-25}");
        Console.WriteLine(new string('-', 50));

        foreach (var tag in tags)
            Console.WriteLine(
                $"{tag.Id,-5} {TruncateString(tag.Name, 20),-20} {TruncateString(tag.Description, 25),-25}");
    }

    private static async Task SearchCollectables(ServiceProvider serviceProvider, string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide a search term.");
            return;
        }

        var searchTerm = string.Join(" ", args);
        Console.WriteLine($"Searching for: {searchTerm}");

        var collectableService = serviceProvider.GetRequiredService<ICollectableItemService>();
        var filter = new CollectableItemFilter { Name = searchTerm };

        var collectables = await collectableService.GetPagedItemsAsync(1, 20, filter);
        var totalCount = await collectableService.GetTotalCountAsync(filter);

        Console.WriteLine($"Found {totalCount} item(s)");
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"{"ID",-5} {"Type",-6} {"Name",-30} {"Year",-6} {"Condition",-20} {"Country",-10}");
        Console.WriteLine(new string('-', 80));

        foreach (var item in collectables)
        {
            var type = item is Coin ? "Coin" : "Bill";
            Console.WriteLine(
                $"{item.Id,-5} {type,-6} {TruncateString(item.Name, 30),-30} {item.Year,-6} {item.Condition,-20} {TruncateString(item.Country, 10),-10}");
        }
    }

    private static async Task ShowStats(ServiceProvider serviceProvider)
    {
        var collectableService = serviceProvider.GetRequiredService<ICollectableItemService>();
        var tagService = serviceProvider.GetRequiredService<ITagService>();

        // Use GetPagedItemsAsync with a large number to get all items instead of GetAllItemsAsync
        var allItems = await collectableService.GetPagedItemsAsync(1, int.MaxValue);
        var tags = await tagService.GetAllTagsAsync();

        var collectableItems = allItems.ToList();
        var totalItems = collectableItems.Count;
        var coinCount = collectableItems.Count(i => i is Coin);
        var billCount = collectableItems.Count(i => i is Bill);

        var countries = collectableItems.Select(i => i.Country).Distinct().ToList();
        var years = collectableItems.Select(i => i.Year).Distinct().OrderBy(y => y).ToList();
        var conditions = collectableItems.Select(i => i.Condition).Distinct().ToList();

        Console.WriteLine("Collection Statistics");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"Total items: {totalItems}");
        Console.WriteLine($"Coins: {coinCount}");
        Console.WriteLine($"Bills: {billCount}");
        Console.WriteLine($"Total tags: {tags.Count()}");
        Console.WriteLine($"Countries represented: {countries.Count}");
        Console.WriteLine($"Year range: {(years.Any() ? $"{years.First()} - {years.Last()}" : "N/A")}");

        Console.WriteLine("\nCondition Breakdown:");
        foreach (var condition in conditions.OrderBy(c => (int)c))
        {
            var count = collectableItems.Count(i => i.Condition == condition);
            Console.WriteLine($"  {condition}: {count} item(s)");
        }

        Console.WriteLine("\nTop Countries:");
        var topCountries = collectableItems
            .GroupBy(i => i.Country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5);

        foreach (var country in topCountries) Console.WriteLine($"  {country.Country}: {country.Count} item(s)");
    }

    private static string TruncateString(string str, int maxLength)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.Length <= maxLength ? str : str.Substring(0, maxLength - 3) + "...";
    }
}
