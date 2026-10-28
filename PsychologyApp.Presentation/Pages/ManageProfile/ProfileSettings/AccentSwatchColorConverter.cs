using System.Globalization;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;

/// <summary>An accent key ("blue") to the colour it gives the buttons, for its swatch in the settings.</summary>
public sealed class AccentSwatchColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        UserPreferences.AccentColor(value as string ?? string.Empty);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
