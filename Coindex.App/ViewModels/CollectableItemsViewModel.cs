using System.Collections.ObjectModel;
using Coindex.App.ViewModels.Base;
using Coindex.App.Views;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;
using Coindex.Core.Domain.Filters;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public partial class CollectableItemsViewModel : BaseViewModel
{
    private const string tagNameAll = "All tags";
    private const string conditionNameAll = "All conditions";
    private const int filterDebounceMilliseconds = 500;

    private const int pageSize = 10;
    private const int firstPage = 1;

    private readonly Dictionary<string, Tag> _tagsByNameDictionary = [];
    private readonly Dictionary<string, ItemCondition> _conditionsByNameDictionary = [];
    private readonly ICollectableItemService _collectableItemService;
    private readonly ITagService _tagService;
    private readonly IDispatcherTimer _filterChangedDebounceTimer;
    private bool _isFilterChangedPending;

    private bool _isInitialized = false;
    private int _currentPage = firstPage;
    private bool _hasMoreItems = true;
    private bool _isLoadingMore;

    [ObservableProperty] private bool _isRefreshing;
    [ObservableProperty] private string _itemNameInputFilter = string.Empty;
    [ObservableProperty] private string _tagNameInputFilter = tagNameAll;
    [ObservableProperty] private string _conditionNameInputFilter = conditionNameAll;

    public CollectableItemsViewModel(ICollectableItemService collectableItemService, ITagService tagService) :
        base("Collection")
    {
        _collectableItemService = collectableItemService;
        _tagService = tagService;

        _filterChangedDebounceTimer = Application.Current!.Dispatcher.CreateTimer();
        _filterChangedDebounceTimer.Interval = TimeSpan.FromMilliseconds(filterDebounceMilliseconds);
        _filterChangedDebounceTimer.Tick += (_, _) =>
        {
            _filterChangedDebounceTimer.Stop();
            if (_isFilterChangedPending)
            {
                _isFilterChangedPending = false;
                OnFilterChanged();
            }
        };
    }

    public ObservableCollection<string> TagNames { get; } = [];
    public ObservableCollection<string> ConditionNames { get; } = [];
    public ObservableCollection<CollectableItem> Items { get; } = [];

    [RelayCommand]
    private async Task Initialize()
    {
        if (!_isInitialized)
        {
            _isInitialized = true;

            _tagsByNameDictionary.Clear();
            TagNames.Clear();
            TagNames.Add(tagNameAll);
            (await _tagService.GetAllTagsAsync()).ToList().ForEach(t =>
            {
                _tagsByNameDictionary[t.Name] = t;
                TagNames.Add(t.Name);
            });
            TagNameInputFilter = tagNameAll;

            _conditionsByNameDictionary.Clear();
            ConditionNames.Clear();
            ConditionNames.Add(conditionNameAll);
            Enum.GetValues<ItemCondition>().ToList().ForEach(c =>
            {
                var conditionName = c.ToString();
                _conditionsByNameDictionary[conditionName] = c;
                ConditionNames.Add(conditionName);
            });
            ConditionNameInputFilter = conditionNameAll;
        }

        await LoadItems();
    }

    private void OnFilterChanged()
    {
        LoadItemsCommand.Execute(null);
    }

    partial void OnItemNameInputFilterChanged(string value)
    {
        _isFilterChangedPending = true;
        _filterChangedDebounceTimer.Stop();
        _filterChangedDebounceTimer.Start();
    }

    partial void OnTagNameInputFilterChanged(string value)
    {
        OnFilterChanged();
    }

    partial void OnConditionNameInputFilterChanged(string value)
    {
        OnFilterChanged();
    }

    [RelayCommand]
    private async Task ItemTapped(CollectableItem? item)
    {
        if (item is null) return;

        await Shell.Current.GoToAsync(nameof(CollectableItemDetailsPage),
            new Dictionary<string, object> { { "id", item.Id } });
    }

    [RelayCommand]
    private async Task AddItem()
    {
        await Shell.Current.GoToAsync(nameof(CollectableItemEditPage));
    }

    [RelayCommand]
    private async Task LoadItems()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsRefreshing = true;
        try
        {
            _currentPage = firstPage;
            _hasMoreItems = true;

            var filter = GetItemFilter();

            Items.Clear();
            var items = await _collectableItemService.GetPagedItemsAsync(_currentPage, pageSize, filter);
            foreach (var item in items)
            {
                Items.Add(item);
            }

            // Check if we have more items
            var totalCount = await _collectableItemService.GetTotalCountAsync(filter);
            _hasMoreItems = Items.Count < totalCount;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load items: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task LoadMoreItems()
    {
        if (IsBusy || _isLoadingMore || !_hasMoreItems) return;

        _isLoadingMore = true;
        try
        {
            var filter = GetItemFilter();

            _currentPage++;
            var items = await _collectableItemService.GetPagedItemsAsync(
                _currentPage,
                pageSize,
                filter);

            var newItems = items.ToList();

            if (newItems.Count == 0)
            {
                _hasMoreItems = false;
                return;
            }

            foreach (var item in newItems)
            {
                Items.Add(item);
            }

            var totalCount = await _collectableItemService.GetTotalCountAsync(filter);
            _hasMoreItems = Items.Count < totalCount;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load more items: {ex.Message}", "OK");
            _currentPage--; // Revert page increment on failure
        }
        finally
        {
            _isLoadingMore = false;
        }
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadItemsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task ClearFilters()
    {
        ItemNameInputFilter = string.Empty;
        TagNameInputFilter = tagNameAll;
        ConditionNameInputFilter = conditionNameAll;

        await LoadItemsCommand.ExecuteAsync(null);
    }

    private CollectableItemFilter GetItemFilter()
    {
        var filter = new CollectableItemFilter();
        var name = GetItemNameFilter();
        if (name is not null)
        {
            filter.Name = name;
        }

        var tagId = GetTagIdFilter();
        if (tagId is not null)
        {
            filter.TagId = tagId;
        }

        var condition = GetConditionFilter();
        if (condition is not null)
        {
            filter.Condition = condition;
        }

        return filter;
    }

    private string? GetItemNameFilter()
    {
        var trimmedName = ItemNameInputFilter.Trim();
        return string.IsNullOrWhiteSpace(trimmedName) ? null : trimmedName;
    }

    private int? GetTagIdFilter()
    {
        if (!string.IsNullOrWhiteSpace(TagNameInputFilter)
            && TagNameInputFilter != tagNameAll
            && _tagsByNameDictionary.TryGetValue(TagNameInputFilter, out var tag))
        {
            return tag.Id;
        }

        return null;
    }

    private ItemCondition? GetConditionFilter()
    {
        if (!string.IsNullOrWhiteSpace(ConditionNameInputFilter)
            && ConditionNameInputFilter != conditionNameAll
            && _conditionsByNameDictionary.TryGetValue(ConditionNameInputFilter, out var condition))
        {
            return condition;
        }

        return null;
    }
}
