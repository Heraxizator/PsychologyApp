namespace PsychologyApp.Presentation.Infrastructure;

public static class UserPreferences
{
    public const string ThemeKey = "Theme";
    public const string ColorKey = "Color";
    public const string FormKey = "Form";
    public const string SizeKey = "Size";
    public const string IsBoldKey = "IsBold";

    public const string DefaultTheme = "Light";
    public const string DefaultColor = "Blue";
    public const string DefaultForm = "Normal";
    public const string DefaultSize = "Medium";

    public static UserPreferencesState Load()
    {
        return new UserPreferencesState
        {
            Theme = Preferences.Get(ThemeKey, DefaultTheme),
            Color = Preferences.Get(ColorKey, DefaultColor),
            Form = Preferences.Get(FormKey, DefaultForm),
            Size = Preferences.Get(SizeKey, DefaultSize),
            IsBold = Preferences.Get(IsBoldKey, false)
        };
    }

    public static void Save(UserPreferencesState state)
    {
        Preferences.Set(ThemeKey, state.Theme);
        Preferences.Set(ColorKey, state.Color);
        Preferences.Set(FormKey, state.Form);
        Preferences.Set(SizeKey, state.Size);
        Preferences.Set(IsBoldKey, state.IsBold);
    }

    public static void ApplyTheme()
    {
        if (Microsoft.Maui.Controls.Application.Current is null)
        {
            return;
        }

        string theme = Preferences.Get(ThemeKey, DefaultTheme);
        Microsoft.Maui.Controls.Application.Current.UserAppTheme = theme.Equals("Dark", StringComparison.OrdinalIgnoreCase)
            ? AppTheme.Dark
            : AppTheme.Light;
    }
}

public sealed class UserPreferencesState
{
    public string Theme { get; init; } = UserPreferences.DefaultTheme;
    public string Color { get; init; } = UserPreferences.DefaultColor;
    public string Form { get; init; } = UserPreferences.DefaultForm;
    public string Size { get; init; } = UserPreferences.DefaultSize;
    public bool IsBold { get; init; }
}
