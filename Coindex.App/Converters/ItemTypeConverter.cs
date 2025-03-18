using System.Globalization;
using Coindex.Core.Domain.Entities;

namespace Coindex.App.Converters;

public class ItemTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            Coin => "coin.jpeg",
            Bill => "bill.jpeg",
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
