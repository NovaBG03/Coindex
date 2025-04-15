using Coindex.App.ViewModels;

namespace Coindex.App.Views;

[QueryProperty(nameof(TagId), "tagId")]
public partial class TagEditPage : ContentPage
{
    private readonly IServiceProvider _serviceProvider;

    public TagEditPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;
        TagId = null;
    }

    public string? TagId
    {
        set => UpdateViewModel(value);
    }

    private void UpdateViewModel(string? value)
    {
        if (int.TryParse(value, out _))
        {
            var vm = _serviceProvider.GetRequiredService<TagEditViewModel>();
            vm.OriginalTagId = value;
            BindingContext = vm;
        }
        else
        {
            BindingContext = _serviceProvider.GetRequiredService<TagCreateViewModel>();
        }
    }
}
