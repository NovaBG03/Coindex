using CommunityToolkit.Mvvm.ComponentModel;

namespace Coindex.App.ViewModels.Base;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;

    [ObservableProperty] private string _title = "Coindex";

    public bool IsNotBusy => !IsBusy;
}
