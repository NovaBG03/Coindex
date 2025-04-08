using System.Globalization;
using Coindex.App.ViewModels.Base;
using Coindex.Core.Application.Interfaces.Services;
using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Coindex.App.ViewModels;

[QueryProperty(nameof(ItemId), "id")]
public partial class CollectableItemEditViewModel(
    ICollectableItemService collectableItemService,
    ICollectableItemDataGeneratorService dataGenerator,
    ITagService tagService)
    : BaseViewModel("Item Editor")
{
    // Mode tracking
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsCreateMode))]
    private int _itemId;

    public bool IsCreateMode => ItemId == 0;

    // Item type properties
    [ObservableProperty] private List<string> _itemTypes = [nameof(Coin), nameof(Bill)];

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsCoin))] [NotifyPropertyChangedFor(nameof(IsBill))]
    private string _selectedItemType = nameof(Coin);

    public bool IsCoin => SelectedItemType == nameof(Coin);
    public bool IsBill => SelectedItemType == nameof(Bill);

    // Display properties
    [ObservableProperty] private bool _canSave = true;

    // Common properties
    [ObservableProperty] private string _name = "";
    [ObservableProperty] private string _description = "";
    [ObservableProperty] private string _year = "";
    [ObservableProperty] private string _country = "";
    [ObservableProperty] private string _faceValue = "0";
    [ObservableProperty] private List<ItemCondition> _conditions = Enum.GetValues<ItemCondition>().ToList();
    [ObservableProperty] private ItemCondition _selectedCondition = ItemCondition.Good;

    // Coin properties
    [ObservableProperty] private string _mint = "";
    [ObservableProperty] private string _material = "";
    [ObservableProperty] private string _weightInGrams = "0";
    [ObservableProperty] private string _diameterInMM = "0";

    // Bill properties
    [ObservableProperty] private string _series = "";
    [ObservableProperty] private string _serialNumber = "";
    [ObservableProperty] private string _signatureType = "";
    [ObservableProperty] private string _billType = "";
    [ObservableProperty] private string _widthInMM = "0";
    [ObservableProperty] private string _heightInMM = "0";

    // Tag properties
    [ObservableProperty] private string _tagInput = "";
    [ObservableProperty] private ObservableCollection<Tag> _selectedTags = [];
    [ObservableProperty] private ObservableCollection<Tag> _availableTags = [];
    [ObservableProperty] private ObservableCollection<Tag> _filteredTags = [];

    // Random colors for new tags
    private static readonly string[] TagColors =
    {
        "#FF5733", "#33FF57", "#3357FF", "#F033FF", "#FF33F0",
        "#33FFF0", "#F0FF33", "#5733FF", "#FF5733", "#33FF57"
    };

    [RelayCommand]
    private async Task Initialize()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await LoadAllTags();

            if (IsCreateMode)
            {
                Title = "Create New Item";
            }
            else
            {
                Title = "Edit Item";
                await LoadItem();
            }

            FilterAvailableTags();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to initialize: {ex.Message}", "OK");
            await NavigateBack();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadAllTags()
    {
        var tags = await tagService.GetAllTagsAsync();
        AvailableTags = new ObservableCollection<Tag>(tags);
        FilterAvailableTags();
    }

    partial void OnTagInputChanged(string value)
    {
        FilterAvailableTags();
    }

    private void FilterAvailableTags()
    {
        var filtered = AvailableTags
            .Where(t => SelectedTags.All(st => st.Id != t.Id))
            .Where(t => string.IsNullOrWhiteSpace(TagInput) ||
                        t.Name.Contains(TagInput, StringComparison.OrdinalIgnoreCase))
            .ToList();

        FilteredTags = new ObservableCollection<Tag>(filtered);
    }

    [RelayCommand]
    private void AddTag(Tag tag)
    {
        if (SelectedTags.All(t => t.Id != tag.Id))
        {
            SelectedTags.Add(tag);
            FilterAvailableTags();
        }
    }

    [RelayCommand]
    private void RemoveTag(Tag tag)
    {
        if (SelectedTags.FirstOrDefault(t => t.Id == tag.Id) is { } tagToRemove)
        {
            SelectedTags.Remove(tagToRemove);
            FilterAvailableTags();
        }
    }

    [RelayCommand]
    private void CreateTag()
    {
        if (string.IsNullOrWhiteSpace(TagInput)) return;

        // Check if tag with this name already exists
        var existingTag = AvailableTags.FirstOrDefault(t =>
            t.Name.Equals(TagInput, StringComparison.OrdinalIgnoreCase));

        if (existingTag is null)
        {
            var newTag = new Tag
            {
                Name = TagInput,
                Description = "",
                Color = GetColorForTag(TagInput)
            };

            AvailableTags.Add(newTag);
            SelectedTags.Add(newTag);
            FilterAvailableTags();
        }
        else
        {
            AddTag(existingTag);
        }

        TagInput = "";
    }

    private static string GetColorForTag(string tagName)
    {
        return TagColors[Math.Abs(tagName.GetHashCode()) % TagColors.Length];
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await NavigateBack();
    }

    [RelayCommand]
    private async Task Save()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Error", "Name is required", "OK");
                return;
            }

            CollectableItem item;

            if (IsCoin)
            {
                item = IsCreateMode
                    ? new Coin()
                    : (Coin)(await collectableItemService.GetItemByIdWithTagsAsync(ItemId) ??
                             throw new InvalidOperationException($"Cannot find item with id {ItemId} in the database"));

                FillCommonProperties(item);

                var coin = (Coin)item;
                coin.Mint = Mint;
                coin.Material = Material;
                coin.WeightInGrams = ParseDecimal(WeightInGrams);
                coin.DiameterInMM = ParseDecimal(DiameterInMM);
            }
            else // IsBill
            {
                item = IsCreateMode
                    ? new Bill()
                    : (Bill)(await collectableItemService.GetItemByIdWithTagsAsync(ItemId) ??
                             throw new InvalidOperationException($"Cannot find item with id {ItemId} in the database"));

                FillCommonProperties(item);

                var bill = (Bill)item;
                bill.Series = Series;
                bill.SerialNumber = SerialNumber;
                bill.SignatureType = SignatureType;
                bill.BillType = BillType;
                bill.WidthInMM = ParseDecimal(WidthInMM);
                bill.HeightInMM = ParseDecimal(HeightInMM);
            }

            item.Tags = SelectedTags.ToList();

            if (IsCreateMode)
            {
                await collectableItemService.AddItemAsync(item);
            }
            else
            {
                await collectableItemService.UpdateItemAsync(item);
            }

            await NavigateBack();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to save: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadItem()
    {
        var item = await collectableItemService.GetItemByIdWithTagsAsync(ItemId);
        if (item is null)
        {
            await Shell.Current.DisplayAlert("Error", $"Item with id {ItemId} not found", "OK");
            await NavigateBack();
            return;
        }

        Name = item.Name;
        Description = item.Description;
        Year = item.Year.ToString();
        Country = item.Country;
        FaceValue = item.FaceValue.ToString(CultureInfo.InvariantCulture);
        SelectedCondition = item.Condition;

        SelectedTags = new ObservableCollection<Tag>(item.Tags);
        FilterAvailableTags();

        switch (item)
        {
            case Coin coin:
                SelectedItemType = nameof(Coin);
                Mint = coin.Mint;
                Material = coin.Material;
                WeightInGrams = coin.WeightInGrams.ToString(CultureInfo.InvariantCulture);
                DiameterInMM = coin.DiameterInMM.ToString(CultureInfo.InvariantCulture);
                break;
            case Bill bill:
                SelectedItemType = nameof(Bill);
                Series = bill.Series;
                SerialNumber = bill.SerialNumber;
                SignatureType = bill.SignatureType;
                BillType = bill.BillType;
                WidthInMM = bill.WidthInMM.ToString(CultureInfo.InvariantCulture);
                HeightInMM = bill.HeightInMM.ToString(CultureInfo.InvariantCulture);
                break;
        }

        Title = $"Edit {item.Name}";
    }

    private void FillCommonProperties(CollectableItem item)
    {
        item.Name = Name;
        item.Description = Description;
        item.Year = ParseInt(Year);
        item.Country = Country;
        item.FaceValue = ParseDecimal(FaceValue);
        item.Condition = SelectedCondition;
    }

    // Helper methods for parsing input strings
    private static int ParseInt(string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }

    private static decimal ParseDecimal(string value)
    {
        return decimal.TryParse(value, out var result) ? result : 0m;
    }

    partial void OnNameChanged(string value)
    {
        ValidateCanSave();
    }

    private void ValidateCanSave()
    {
        CanSave = !string.IsNullOrWhiteSpace(Name);
    }

    [RelayCommand]
    private void FillRandomData()
    {
        var year = dataGenerator.GenerateRandomYear(IsCoin);
        var country = dataGenerator.GenerateRandomCountry();

        Name = dataGenerator.GenerateRandomName(IsCoin);
        Description = dataGenerator.GenerateRandomDescription(IsCoin, country, year);
        Year = year.ToString();
        Country = country;
        FaceValue = dataGenerator.GenerateRandomFaceValue(IsCoin).ToString(CultureInfo.InvariantCulture);
        SelectedCondition = dataGenerator.GenerateRandomCondition();

        if (IsCoin)
        {
            Mint = dataGenerator.GenerateRandomMint();
            Material = dataGenerator.GenerateRandomMaterial();
            WeightInGrams = dataGenerator.GenerateRandomWeight().ToString(CultureInfo.InvariantCulture);
            DiameterInMM = dataGenerator.GenerateRandomDiameter().ToString(CultureInfo.InvariantCulture);
        }
        else // IsBill
        {
            Series = dataGenerator.GenerateRandomSeries(year);
            SerialNumber = dataGenerator.GenerateRandomSerialNumber();
            SignatureType = dataGenerator.GenerateRandomSignatureType();
            BillType = dataGenerator.GenerateRandomBillType();
            WidthInMM = dataGenerator.GenerateRandomWidth().ToString(CultureInfo.InvariantCulture);
            HeightInMM = dataGenerator.GenerateRandomHeight().ToString(CultureInfo.InvariantCulture);
        }
    }

    private static async Task NavigateBack()
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            await Shell.Current.GoToAsync("//CollectableItemsPage");
        }
    }
}
