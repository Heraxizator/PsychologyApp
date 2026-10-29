using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileSettings;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDonate;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInfo;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileDataBackup;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileUser;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfilePracticeHistory;
using PsychologyApp.Presentation.Pages.ManageProfile.ProfileInsights;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public interface IProfilePageFactory
{
    UserPage CreateUserPage();
    OptionsPage CreateOptionsPage();
    InfoPage CreateInfoPage();
    DonatePage CreateDonatePage();
    SettingsPage CreateSettingsPage();
    PracticeHistoryPage CreatePracticeHistoryPage();
    InsightsPage CreateInsightsPage();
    DataBackupPage CreateDataBackupPage();
}

public sealed class ProfilePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    IUserViewModelFactory userViewModelFactory,
    IOptionsViewModelFactory optionsViewModelFactory,
    IInfoViewModelFactory infoViewModelFactory,
    IDonateViewModelFactory donateViewModelFactory,
    ISettingsViewModelFactory settingsViewModelFactory,
    IPracticeHistoryViewModelFactory practiceHistoryViewModelFactory,
    IInsightsViewModelFactory insightsViewModelFactory,
    IDataBackupViewModelFactory dataBackupViewModelFactory) : IProfilePageFactory
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

    public PracticeHistoryPage CreatePracticeHistoryPage() =>
        new(practiceHistoryViewModelFactory);

    public InsightsPage CreateInsightsPage() =>
        new(insightsViewModelFactory);

    public DataBackupPage CreateDataBackupPage() =>
        new(dataBackupViewModelFactory);
}
