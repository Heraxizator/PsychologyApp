using PsychologyApp.Presentation.Infrastructure;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class UserPreferencesTests
{
    [Fact]
    public void DefaultState_MatchesBuiltInDefaults()
    {
        UserPreferencesState state = new();

        Assert.Equal(UserPreferences.DefaultTheme, state.Theme);
        Assert.Equal(UserPreferences.DefaultColor, state.Color);
        Assert.Equal(UserPreferences.DefaultForm, state.Form);
        Assert.Equal(UserPreferences.DefaultSize, state.Size);
        Assert.False(state.IsBold);
    }

    [Fact]
    public void PreferenceKeys_AreStable()
    {
        Assert.Equal("Theme", UserPreferences.ThemeKey);
        Assert.Equal("Color", UserPreferences.ColorKey);
        Assert.Equal("Form", UserPreferences.FormKey);
        Assert.Equal("Size", UserPreferences.SizeKey);
        Assert.Equal("IsBold", UserPreferences.IsBoldKey);
    }

    [Fact]
    public void State_RoundTripsThroughCopy()
    {
        UserPreferencesState original = new()
        {
            Theme = "Dark",
            Color = "Green",
            Form = "Italic",
            Size = "Large",
            IsBold = true
        };

        UserPreferencesState copy = new()
        {
            Theme = original.Theme,
            Color = original.Color,
            Form = original.Form,
            Size = original.Size,
            IsBold = original.IsBold
        };

        Assert.Equal(original.Theme, copy.Theme);
        Assert.Equal(original.Color, copy.Color);
        Assert.Equal(original.Form, copy.Form);
        Assert.Equal(original.Size, copy.Size);
        Assert.Equal(original.IsBold, copy.IsBold);
    }
}
