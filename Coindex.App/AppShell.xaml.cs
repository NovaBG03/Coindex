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
        Routing.RegisterRoute("CollectableItemDetailsPage", typeof(CollectableItemDetailsPage));
    }
}
