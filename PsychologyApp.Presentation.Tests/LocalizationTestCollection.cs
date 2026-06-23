using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class LocalizationTestFixture : IDisposable
{
    public LocalizationTestFixture()
    {
        UserPreferences.UseInMemoryStorage(new UserPreferencesState
        {
            Language = UserPreferences.DefaultLanguage,
            Theme = UserPreferences.DefaultTheme,
            Color = UserPreferences.DefaultColor,
            Form = UserPreferences.DefaultForm,
            Size = UserPreferences.DefaultSize,
            IsBold = false,
            HasCompletedOnboarding = true,
            OnboardingConcern = "explore"
        });
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    public void Dispose()
    {
        UserPreferences.ResetInMemoryStorage();
        AppStrings.LanguageOverride = null;
    }
}

[CollectionDefinition("Localization", DisableParallelization = true)]
public sealed class LocalizationTestCollection : ICollectionFixture<LocalizationTestFixture>;
