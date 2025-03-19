using CommunityToolkit.Mvvm.ComponentModel;

namespace Coindex.App.ViewModels.Base;

public partial class BaseViewModel(string title = "Coindex") : ObservableObject
{
    [ObservableProperty] private string _title = title;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    public bool IsNotBusy => !IsBusy;
}
