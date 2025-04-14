using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class HomePage : ContentPage
{
    private readonly TagsViewModel _viewModel;

    public HomePage(TagsViewModel viewModel)
    {
        viewModel.Title = "Home";
        viewModel.ShouldAddTagForAllNavigation = true;
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
