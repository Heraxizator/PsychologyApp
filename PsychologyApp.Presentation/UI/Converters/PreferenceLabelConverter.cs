using System.Globalization;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.UI.Converters;

public sealed class PreferenceLabelConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string key || string.IsNullOrWhiteSpace(key))
        {
            return value;
        }

        string uiLanguage = AppStrings.LanguageOverride ?? UserPreferences.GetPersistedLanguage();
        PreferenceLabelKind kind = ResolveKind(parameter);

        return kind switch
        {
            PreferenceLabelKind.Language => UserPreferences.GetLanguageLabel(key, uiLanguage),
            PreferenceLabelKind.Theme => UserPreferences.GetThemeLabel(key, uiLanguage),
            PreferenceLabelKind.Color => UserPreferences.GetColorLabel(key, uiLanguage),
            PreferenceLabelKind.Form => UserPreferences.GetFormLabel(key, uiLanguage),
            PreferenceLabelKind.Size => UserPreferences.GetSizeLabel(key, uiLanguage),
            _ => value
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();

    private static PreferenceLabelKind ResolveKind(object? parameter) =>
        parameter switch
        {
            PreferenceLabelKind kind => kind,
            string text when Enum.TryParse(text, ignoreCase: true, out PreferenceLabelKind parsed) => parsed,
            _ => PreferenceLabelKind.Language
        };
}
