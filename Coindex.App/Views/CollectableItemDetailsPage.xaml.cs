using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class CollectableItemDetailsPage : ContentPage
{
    private readonly CollectableItemDetailsViewModel _viewModel;

    public CollectableItemDetailsPage(CollectableItemDetailsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadItemCommand.Execute(null);
    }
}
