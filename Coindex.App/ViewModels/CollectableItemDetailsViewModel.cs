using Coindex.App.ViewModels.Base;
using Coindex.App.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

[QueryProperty(nameof(ItemId), "id")]
public partial class CollectableItemDetailsViewModel(ICollectableItemService collectableItemService)
    : BaseViewModel("Item Details")
{
    [ObservableProperty] private CollectableItem? _item;
    [ObservableProperty] private int _itemId;

    public bool IsCoin => Item is Coin;
    public bool IsBill => Item is Bill;
    public Coin? Coin => Item as Coin;
    public Bill? Bill => Item as Bill;

    partial void OnItemIdChanged(int value)
    {
        LoadItemCommand.Execute(null);
    }

    partial void OnItemChanged(CollectableItem? value)
    {
        if (value is not null)
        {
            Title = value.Name;
            OnPropertyChanged(nameof(IsCoin));
            OnPropertyChanged(nameof(IsBill));
            OnPropertyChanged(nameof(Coin));
            OnPropertyChanged(nameof(Bill));
        }
    }

    [RelayCommand]
    private async Task LoadItem()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Item = null;

            var item = await collectableItemService.GetItemByIdWithTagsAsync(ItemId);
            if (item is null)
            {
                await Shell.Current.DisplayAlert("Error", "Item not found", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            Item = item;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load item: {ex.Message}", "OK");
            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditItem()
    {
        if (Item is null) return;

        await Shell.Current.GoToAsync(nameof(CollectableItemEditPage),
            new Dictionary<string, object> { { "id", ItemId } });
    }
}
