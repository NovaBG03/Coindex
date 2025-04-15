using Coindex.App.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public interface ITagEditViewModel : IBaseViewModel
{
    string TagName { get; set; }
    string TagDescription { get; set; }
    string TagColor { get; set; }

    bool CanSave { get; }

    IAsyncRelayCommand SaveCommand { get; }
    IAsyncRelayCommand CancelCommand { get; }
}
