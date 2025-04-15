using System.Diagnostics;
using Coindex.App.ViewModels.Base;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

[QueryProperty(nameof(OriginalTagId), "tagId")]
public partial class TagEditViewModel(ITagService tagService) : BaseViewModel("Edit Tag"), ITagEditViewModel
{
    [ObservableProperty] private bool _canSave = true;

    private Tag? _originalTag;

    [ObservableProperty] private string _originalTagId = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _tagColor = "#808080";

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _tagDescription = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _tagName = string.Empty;


    partial void OnOriginalTagIdChanged(string value)
    {
        if (!int.TryParse(value, out var intValue)) return;

        try
        {
            _originalTag = tagService.GetTagById(intValue);
            if (_originalTag is null) return;
            TagName = _originalTag.Name;
            TagDescription = _originalTag.Description;
            TagColor = _originalTag.Color;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading Tag: {ex.Message}");
            Shell.Current.DisplayAlert("Error", "Failed to load tag data.", "OK");
            Shell.Current.GoToAsync("..");
        }
    }


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
        if (!CanSave || _originalTag == null) return;

        IsBusy = true;
        try
        {
            // Update the original tag object with new values
            _originalTag.Name = TagName;
            _originalTag.Description = TagDescription;
            _originalTag.Color = TagColor;

            await tagService.UpdateTagAsync(_originalTag);
            await Shell.Current.GoToAsync(".."); // Navigate back
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to update tag: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await Shell.Current.GoToAsync(".."); // Navigate back
    }
}
