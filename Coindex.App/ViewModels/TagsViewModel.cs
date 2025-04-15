using System.Collections.ObjectModel;
using Coindex.App.ViewModels.Base;
using Coindex.App.Views;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public partial class TagsViewModel(ITagService tagService) : BaseViewModel("All Tags")
{
    private const string allTagName = "All";

    [ObservableProperty] private ObservableCollection<Tag> _tags = [];

    public bool ShouldAddTagForAllNavigation { get; set; }

    [RelayCommand]
    private async Task Initialize()
    {
        try
        {
            IsBusy = true;
            Tags.Clear();

            var allTags = await tagService.GetAllTagsAsync();
            if (ShouldAddTagForAllNavigation) Tags.Add(new Tag { Name = allTagName, Color = "#000000" });
            foreach (var tag in allTags) Tags.Add(tag);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load tags: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task TagSelected(string? tagName)
    {
        var tagNameQueryParam = tagName == allTagName ? "" : tagName;
        await Shell.Current.GoToAsync($"//{nameof(CollectableItemsPage)}?tagName={tagNameQueryParam}");
    }

    [RelayCommand]
    private async Task CreateTag()
    {
        await Shell.Current.GoToAsync(nameof(TagEditPage));
    }

    [RelayCommand]
    private async Task EditTag(Tag? tag)
    {
        if (tag is null) return;
        await Shell.Current.GoToAsync($"{nameof(TagEditPage)}?tagId={tag.Id}");
    }
}
