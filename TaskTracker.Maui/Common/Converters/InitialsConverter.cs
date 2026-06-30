using System.Globalization;

namespace TaskTracker.Maui.Common.Converters;

public class InitialsConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string fullName || string.IsNullOrWhiteSpace(fullName))
            return "?";

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length switch
        {
            0 => "?",
            1 => parts[0][..1].ToUpper(),
            _ => $"{parts[0][0]}{parts[1][0]}".ToUpper()
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}