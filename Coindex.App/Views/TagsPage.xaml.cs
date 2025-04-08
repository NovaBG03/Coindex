using Coindex.App.ViewModels;

namespace Coindex.App.Views;

public partial class TagsPage : ContentPage
{
    private readonly TagsViewModel _viewModel;

    public TagsPage(TagsViewModel viewModel)
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
