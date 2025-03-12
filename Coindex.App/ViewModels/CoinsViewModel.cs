using System.Collections.ObjectModel;
using Coindex.App.ViewModels.Base;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Coindex.App.ViewModels;

public partial class CoinsViewModel : BaseViewModel
{
    private readonly ICoinService _coinService;

    [ObservableProperty] private string _newCoinName = string.Empty;

    [ObservableProperty] private string _newCoinDescription = string.Empty;

    public CoinsViewModel(ICoinService coinService)
    {
        _coinService = coinService;
        Title = "Coin Collection";
    }

    public ObservableCollection<Coin> Coins { get; } = [];

    [RelayCommand]
    private async Task LoadCoins()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            Coins.Clear();
            var coins = await _coinService.GetAllCoinsAsync();
            foreach (var coin in coins)
            {
                Coins.Add(coin);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to load coins: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddCoin()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(NewCoinName)) return;

        IsBusy = true;
        try
        {
            var newCoin = new Coin
            {
                Name = NewCoinName,
                Description = NewCoinDescription
            };

            await _coinService.AddCoinAsync(newCoin);

            NewCoinName = string.Empty;
            NewCoinDescription = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Unable to add coin: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }

        await LoadCoinsCommand.ExecuteAsync(null);
    }
}
