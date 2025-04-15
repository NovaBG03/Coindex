using CommunityToolkit.Mvvm.ComponentModel;

namespace Coindex.App.ViewModels.Base;

public partial class BaseViewModel(string title = "Coindex") : ObservableObject, IBaseViewModel
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty] private string _title = title;

    public bool IsNotBusy => !IsBusy;
}
