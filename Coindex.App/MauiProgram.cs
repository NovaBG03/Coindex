﻿using Coindex.App.ViewModels;
using Coindex.App.Views;
using Coindex.Core.Application.Interfaces.Repositories;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Application.Services;
using Coindex.Core.Infrastructure.Data;
using Coindex.Core.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace Coindex.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "coindex.db3");
        // /Users/nova/Library/Containers/com.companyname.coindex.app/Data/Documents/coindex.db3
        Console.WriteLine($"db path: {dbPath}");

        // Initialize SQLite only once at application startup
        Batteries_V2.Init();
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        builder.Services.AddSingleton<DatabaseInitializer>();

        // Repositories
        builder.Services.AddScoped<ICollectableItemRepository, CollectableItemRepository>();
        builder.Services.AddScoped<ITagRepository, TagRepository>();

        // Services
        builder.Services.AddScoped<ICollectableItemService, CollectableItemService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddSingleton<ICollectableItemDataGeneratorService, CollectableItemDataGeneratorService>();
        // builder.Services.AddSingleton<ICollectableItemDataGeneratorService>(
        //     new CollectableItemDataGeneratorService(1234));

        // ViewModels
        builder.Services.AddTransient<TagsViewModel>();
        builder.Services.AddSingleton<CollectableItemsViewModel>();
        builder.Services.AddTransient<CollectableItemDetailsViewModel>();
        builder.Services.AddTransient<CollectableItemEditViewModel>();
        builder.Services.AddTransient<TagCreateViewModel>();
        builder.Services.AddTransient<TagEditViewModel>();

        // Pages
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<CollectableItemsPage>();
        builder.Services.AddTransient<CollectableItemDetailsPage>();
        builder.Services.AddTransient<CollectableItemEditPage>();
        builder.Services.AddSingleton<TagsPage>();
        builder.Services.AddTransient<TagEditPage>();

        builder.Services.AddSingleton<AppShell>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        app.Services
            .GetRequiredService<DatabaseInitializer>()
            .Initialize();

        return app;
    }
}
