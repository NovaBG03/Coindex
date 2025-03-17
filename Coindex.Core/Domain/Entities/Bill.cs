namespace Coindex.Core.Domain.Entities;

public class Bill : CollectableItem
{
    public int Year { get; set; }
    public string Country { get; set; } = "";
    public decimal FaceValue { get; set; }
    public string Series { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string Condition { get; set; } = ""; // PMG or PCGS grade
    public string SignatureType { get; set; } = "";
    public string Type { get; set; } = ""; // Federal Reserve Note, Silver Certificate, etc.
    public decimal Width { get; set; } // In mm
    public decimal Height { get; set; } // In mm
}
