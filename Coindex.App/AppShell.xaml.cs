using Coindex.App.Views;

namespace Coindex.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private static void RegisterRoutes()
    {
        // Routing.RegisterRoute(nameof(CollectableItemsPage), typeof(CollectableItemsPage));
        Routing.RegisterRoute(nameof(CollectableItemDetailsPage), typeof(CollectableItemDetailsPage));
        Routing.RegisterRoute(nameof(CollectableItemEditPage), typeof(CollectableItemEditPage));
        // Routing.RegisterRoute(nameof(TagsPage), typeof(TagsPage));
        Routing.RegisterRoute(nameof(TagEditPage), typeof(TagEditPage));
    }
}
