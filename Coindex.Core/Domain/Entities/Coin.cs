namespace Coindex.Core.Domain.Entities;

public class Coin : CollectableItem
{
    public int MintYear { get; set; }
    public string Mint { get; set; } = ""; // Mint mark or location
    public string Material { get; set; } = ""; // Gold, Silver, etc.
    public decimal FaceValue { get; set; } // Denomination
    public string Country { get; set; } = "";
    public string Condition { get; set; } = ""; // MS70, XF, VF, etc.
    public decimal Weight { get; set; } // In grams
    public decimal Diameter { get; set; } // In mm
}
