using Coindex.App.Views;

namespace Coindex.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute(nameof(TagsPage), typeof(TagsPage));
        Routing.RegisterRoute(nameof(CollectableItemDetailsPage), typeof(CollectableItemDetailsPage));
        Routing.RegisterRoute(nameof(CollectableItemEditPage), typeof(CollectableItemEditPage));
    }
}
