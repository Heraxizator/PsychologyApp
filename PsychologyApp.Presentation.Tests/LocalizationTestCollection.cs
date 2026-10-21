using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>Tests that read or switch the interface language share one collection, so they never run alongside each other.</summary>
public sealed class LocalizationTestFixture : IDisposable
{
    public LocalizationTestFixture()
    {
        UserPreferences.Persisted = UserPreferences.DefaultLanguage;
        AppStrings.LanguageOverride = UserPreferences.DefaultLanguage;
    }

    public void Dispose()
    {
        UserPreferences.Persisted = UserPreferences.DefaultLanguage;
        AppStrings.LanguageOverride = null;
    }
}

[CollectionDefinition("Localization", DisableParallelization = true)]
public sealed class LocalizationTestCollection : ICollectionFixture<LocalizationTestFixture>;
