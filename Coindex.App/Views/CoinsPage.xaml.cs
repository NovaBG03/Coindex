using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class CoinsPage : ContentPage
{
    private readonly CoinsViewModel _viewModel;

    public CoinsPage(CoinsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCoinsCommand.Execute(null);
    }
}
