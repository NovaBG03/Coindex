namespace Coindex.App.ViewModels.Base;

public interface IBaseViewModel
{
    string Title { get; }
    bool IsBusy { get; set; }
    bool IsNotBusy { get; }
}
