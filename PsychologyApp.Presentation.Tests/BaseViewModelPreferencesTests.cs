using System.Reflection;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class BaseViewModelPreferencesTests
{
    [Fact]
    public void UserPreferencesChanged_InvokesRefreshLocalizedProperties()
    {
        var viewModel = new PreferencesStubViewModel();
        Assert.False(viewModel.Refreshed);

        RaiseUserPreferencesChanged();

        Assert.True(viewModel.Refreshed);
    }

    private static void RaiseUserPreferencesChanged()
    {
        FieldInfo? field = typeof(UserPreferences).GetField(
            "Changed",
            BindingFlags.Static | BindingFlags.NonPublic);

        if (field?.GetValue(null) is Action handler)
        {
            handler.Invoke();
        }
    }

    private sealed class PreferencesStubViewModel : BaseViewModel
    {
        public bool Refreshed { get; private set; }

        protected override void RefreshLocalizedProperties() => Refreshed = true;
    }
}
