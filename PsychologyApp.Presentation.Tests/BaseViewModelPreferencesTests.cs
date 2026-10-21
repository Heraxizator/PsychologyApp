using Moq;
using PsychologyApp.Presentation.Shared.Services.Preferences;
using PsychologyApp.Presentation.Shared.ViewModels;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>A language or theme switch reaches every open screen: its texts are read again.</summary>
public class BaseViewModelPreferencesTests
{
    [Fact]
    public void A_settings_change_makes_the_screen_read_its_texts_again()
    {
        Mock<IUserPreferencesStore> store = new();
        PreferencesStubViewModel viewModel = new(store.Object);
        Assert.False(viewModel.Refreshed);

        store.Raise(s => s.Changed += null);

        Assert.True(viewModel.Refreshed);
    }

    private sealed class PreferencesStubViewModel : BaseViewModel
    {
        public PreferencesStubViewModel(IUserPreferencesStore store) => BindPreferences(store);

        public bool Refreshed { get; private set; }

        protected override void RefreshLocalizedProperties() => Refreshed = true;
    }
}
