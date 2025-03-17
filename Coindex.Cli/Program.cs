using Coindex.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Cli;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        var dbPath = Path.Combine(
            "/Users/nova/Library/Containers/com.companyname.coindex.app/Data/Documents/",
            "coindex.db3");
        // /Users/nova/Library/Containers/com.companyname.coindex.app/Data/Documents/coindex.db3
        Console.WriteLine($"db path: {dbPath}");
        SQLitePCL.Batteries_V2.Init();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
        var dbContext = new ApplicationDbContext(optionsBuilder.Options);

        Console.WriteLine($"Dropping database context");
        dbContext.Database.EnsureDeleted();
        // Console.WriteLine($"Creating database context");
        // dbContext.Database.EnsureCreated();
        new DatabaseInitializer(dbContext).Initialize();
    }
}
