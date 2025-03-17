namespace Coindex.Core.Domain.Entities;

public class Coin : CollectableItem
{
    public string Mint { get; set; } = "";
    public string Material { get; set; } = ""; // Gold, Silver, etc.
    public decimal WeightInGrams { get; set; }
    public decimal DiameterInMM { get; set; }
}
