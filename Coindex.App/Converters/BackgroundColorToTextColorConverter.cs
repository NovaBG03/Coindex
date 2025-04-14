using System.Globalization;

namespace Coindex.App.Converters;

public class BackgroundColorToTextColorConverter : IValueConverter
{
    private static readonly Color DefaultTextColor = Colors.Black;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var backgroundColor = TryGetColorFromValue(value);

        if (backgroundColor is null || Equals(backgroundColor, Colors.Transparent))
            return DefaultTextColor;

        // Calculate the complementary color (opposite on the color wheel)
        var rComp = 1 - backgroundColor.Red;
        var gComp = 1 - backgroundColor.Green;
        var bComp = 1 - backgroundColor.Blue;

        // Calculate background luminance
        var bgLuminance = CalculateRelativeLuminance(backgroundColor);

        // Adjust the complementary color to maximize contrast
        if (bgLuminance > 0.5)
        {
            // Darken the complementary color for light backgrounds
            rComp = Math.Max(0, rComp - 0.2f);
            gComp = Math.Max(0, gComp - 0.2f);
            bComp = Math.Max(0, bComp - 0.2f);
        }
        else
        {
            // Lighten the complementary color for dark backgrounds
            rComp = Math.Min(1, rComp + 0.2f);
            gComp = Math.Min(1, gComp + 0.2f);
            bComp = Math.Min(1, bComp + 0.2f);
        }

        // Create the complementary color with adjusted brightness
        var textColor = new Color(backgroundColor.Alpha, rComp, gComp, bComp);

        // Calculate contrast ratio between background and text color
        var contrastRatio = CalculateContrastRatio(
            CalculateRelativeLuminance(backgroundColor),
            CalculateRelativeLuminance(textColor)
        );

        // If contrast is not sufficient, fall back to black or white
        if (contrastRatio < 4.5) return bgLuminance > 0.5 ? Colors.Black : Colors.White;

        return textColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Converting from text color back to background color is not supported.");
    }

    private static Color? TryGetColorFromValue(object? value)
    {
        switch (value)
        {
            case Color colorValue:
                return colorValue;
            case string colorString:
                try
                {
                    return Color.FromArgb(colorString);
                }
                catch (FormatException)
                {
                    return null;
                }
            default:
                return null;
        }
    }

    private static double CalculateRelativeLuminance(Color color)
    {
        var r = AdjustGamma(color.Red);
        var g = AdjustGamma(color.Green);
        var b = AdjustGamma(color.Blue);
        return 0.2126 * r + 0.7152 * g + 0.0722 * b;
    }

    private static double AdjustGamma(float channel)
    {
        var c = (double)channel;
        return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }

    private static double CalculateContrastRatio(double luminance1, double luminance2)
    {
        var lighter = Math.Max(luminance1, luminance2);
        var darker = Math.Min(luminance1, luminance2);
        return (lighter + 0.05) / (darker + 0.05);
    }
}
