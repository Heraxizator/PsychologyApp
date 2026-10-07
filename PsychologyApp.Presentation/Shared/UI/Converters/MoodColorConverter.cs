using System.Globalization;
using PsychologyApp.Presentation.Core.Charts;

namespace PsychologyApp.Presentation.Shared.UI.Converters;

/// <summary>Turns a mood level into the colour of its calendar day: parameter "fill" for the cell, "text" for the day number. Follows the current theme.</summary>
public sealed class MoodColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        string hex = parameter as string == "text"
            ? MoodPalette.Text(dark)
            : MoodPalette.Fill(value as int?, dark);
        return Color.FromArgb(hex);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
