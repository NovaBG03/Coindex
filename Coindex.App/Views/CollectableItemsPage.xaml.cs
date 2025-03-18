using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class CollectableItemsPage : ContentPage
{
    private readonly CollectableItemsViewModel _viewModel;

    public CollectableItemsPage(CollectableItemsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadItemsCommand.Execute(null);
    }
}
