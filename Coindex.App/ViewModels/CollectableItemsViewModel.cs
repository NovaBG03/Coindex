using System.Collections.ObjectModel;
using Coindex.App.ViewModels.Base;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public partial class CollectableItemsViewModel : BaseViewModel
{
    private const int pageSize = 10;
    private const int firstPage = 1;

    private int _currentPage = firstPage;
    private bool _hasMoreItems = true;
    private bool _isLoadingMore;

    private readonly ICollectableItemService _collectableItemService;

    public CollectableItemsViewModel(ICollectableItemService collectableItemService)
    {
        _collectableItemService = collectableItemService;
        Title = "Collection";
    }

    public ObservableCollection<CollectableItem> Items { get; } = [];

    [ObservableProperty] private bool _isRefreshing;

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

            Items.Clear();
            var items = await _collectableItemService.GetPagedItemsAsync(_currentPage, pageSize);
            foreach (var item in items)
            {
                Items.Add(item);
            }

            // Check if we have more items
            var totalCount = await _collectableItemService.GetTotalCountAsync();
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
            _currentPage++;
            var items = await _collectableItemService.GetPagedItemsAsync(_currentPage, pageSize);
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

            // Check if we have more items
            var totalCount = await _collectableItemService.GetTotalCountAsync();
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
}
