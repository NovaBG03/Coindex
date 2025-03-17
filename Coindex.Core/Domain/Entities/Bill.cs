namespace Coindex.Core.Domain.Entities;

public class Bill : CollectableItem
{
    public string Series { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string SignatureType { get; set; } = "";
    public string BillType { get; set; } = "";
    public decimal WidthInMM { get; set; }
    public decimal HeightInMM { get; set; }
}
