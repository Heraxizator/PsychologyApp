using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.Views.Profile;
using PsychologyApp.Presentation.Views.Review;

namespace PsychologyApp.Presentation.Services;

public interface IProfilePageFactory
{
    UserPage CreateUserPage();
    OptionsPage CreateOptionsPage();
    InfoPage CreateInfoPage();
    DonatePage CreateDonatePage(INavigation navigation);
    FormPage CreateFormPage();
    SettingsPage CreateSettingsPage();
}

public sealed class ProfilePageFactory(
    IPageViewModelActivator pageViewModelActivator,
    IUserViewModelFactory userViewModelFactory,
    IOptionsViewModelFactory optionsViewModelFactory,
    IInfoViewModelFactory infoViewModelFactory,
    IDonateViewModelFactory donateViewModelFactory,
    IFormViewModelFactory formViewModelFactory,
    ISettingsViewModelFactory settingsViewModelFactory) : IProfilePageFactory
{
    public UserPage CreateUserPage() =>
        new(pageViewModelActivator, userViewModelFactory);

    public OptionsPage CreateOptionsPage() =>
        new(pageViewModelActivator, optionsViewModelFactory);

    public InfoPage CreateInfoPage() =>
        new(pageViewModelActivator, infoViewModelFactory);

    public DonatePage CreateDonatePage(INavigation navigation) =>
        new(donateViewModelFactory.Create(navigation));

    public FormPage CreateFormPage() =>
        new(formViewModelFactory.Create());

    public SettingsPage CreateSettingsPage() =>
        new(pageViewModelActivator, settingsViewModelFactory);
}
