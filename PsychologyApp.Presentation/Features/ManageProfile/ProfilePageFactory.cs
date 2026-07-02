using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public interface IProfilePageFactory
{
    UserPage CreateUserPage();
    OptionsPage CreateOptionsPage();
    InfoPage CreateInfoPage();
    DonatePage CreateDonatePage();
    SettingsPage CreateSettingsPage();
}

public sealed class ProfilePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    IUserViewModelFactory userViewModelFactory,
    IOptionsViewModelFactory optionsViewModelFactory,
    IInfoViewModelFactory infoViewModelFactory,
    IDonateViewModelFactory donateViewModelFactory,
    ISettingsViewModelFactory settingsViewModelFactory) : IProfilePageFactory
{
    public UserPage CreateUserPage() =>
        new(pageViewModelActivator, userViewModelFactory);

    public OptionsPage CreateOptionsPage() =>
        new(pageViewModelActivator, optionsViewModelFactory);

    public InfoPage CreateInfoPage() =>
        new(pageViewModelActivator, infoViewModelFactory);

    public DonatePage CreateDonatePage() =>
        new(donateViewModelFactory);

    public SettingsPage CreateSettingsPage() =>
        new(pageViewModelActivator, settingsViewModelFactory);
}
