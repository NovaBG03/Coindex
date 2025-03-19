using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class CollectableItemEditPage : ContentPage
{
    private readonly CollectableItemEditViewModel _viewModel;

    public CollectableItemEditPage(CollectableItemEditViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.InitializeCommand.Execute(null);
    }
}
