using Moq;
using PsychologyApp.Presentation.Pages.ManageProfile.AppLock;
using PsychologyApp.Presentation.Shared.Services.Security;
using Xunit;

namespace PsychologyApp.Presentation.Tests.ViewModels;

/// <summary>The lock screen opens the app only after the phone's unlock, and never shuts the person out of their own records.</summary>
public sealed class AppLockViewModelTests
{
    private readonly Mock<IDeviceAuthenticator> _authenticator = new();

    private AppLockViewModel Create(DeviceUnlockAvailability availability, DeviceUnlockResult result)
    {
        _authenticator.SetupGet(a => a.Availability).Returns(availability);
        _authenticator.Setup(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(result);
        return new AppLockViewModel(_authenticator.Object);
    }

    [Fact]
    public async Task The_phones_unlock_opens_the_app()
    {
        AppLockViewModel viewModel = Create(DeviceUnlockAvailability.Available, DeviceUnlockResult.Passed);

        await VmTestHelpers.RunAsync(viewModel.UnlockCommand);

        Assert.True(viewModel.Unlocked.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task A_cancelled_prompt_keeps_the_app_closed()
    {
        AppLockViewModel viewModel = Create(DeviceUnlockAvailability.Available, DeviceUnlockResult.NotPassed);

        await VmTestHelpers.RunAsync(viewModel.UnlockCommand);

        Assert.False(viewModel.Unlocked.IsCompleted);
    }

    [Fact]
    public async Task With_the_phones_screen_lock_removed_the_app_opens_instead_of_locking_the_person_out()
    {
        AppLockViewModel viewModel = Create(DeviceUnlockAvailability.NoScreenLock, DeviceUnlockResult.NotPassed);

        await VmTestHelpers.RunAsync(viewModel.UnlockCommand);

        Assert.True(viewModel.Unlocked.IsCompletedSuccessfully);
        _authenticator.Verify(a => a.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task When_the_system_cannot_ask_at_all_the_person_is_let_in()
    {
        AppLockViewModel viewModel = Create(DeviceUnlockAvailability.Available, DeviceUnlockResult.CannotAsk);

        await VmTestHelpers.RunAsync(viewModel.UnlockCommand);

        Assert.True(viewModel.Unlocked.IsCompletedSuccessfully);
    }
}
