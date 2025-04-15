using Coindex.App.ViewModels.Base;
using Coindex.Core.Application.Interfaces.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public partial class TagCreateViewModel(ITagService tagService) : BaseViewModel("Create New Tag"), ITagEditViewModel
{
    private static readonly Random Random = new();

    [ObservableProperty] private bool _canSave;

    [ObservableProperty] private string _tagColor = GetRandomHexColor();

    [ObservableProperty] private string _tagDescription = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _tagName = string.Empty;

    partial void OnTagNameChanged(string value)
    {
        UpdateCanSave();
    }

    private void UpdateCanSave()
    {
        CanSave = !string.IsNullOrWhiteSpace(TagName);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!CanSave) return;

        IsBusy = true;
        try
        {
            Console.WriteLine($"{TagName}, {TagDescription}, {TagColor}");
            await tagService.CreateTagAsync(TagName, TagDescription, TagColor);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to create tag: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }

    private static string GetRandomHexColor()
    {
        return $"#{Random.Next(0x1000000):X6}";
    }
}
