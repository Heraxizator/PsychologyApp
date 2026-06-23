using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ProfileOptions;

public class OptionsViewModel : BaseViewModel
{
    public ICommand OpenAboutPageCommand { get; private set; } = default!;
    public ICommand OpenDonatePageCommand { get; private set; } = default!;
    public ICommand OpenFeedbackPageCommand { get; private set; } = default!;
    public ICommand OpenSettingsPageCommand { get; private set; } = default!;
    public ICommand BackCommand { get; private set; } = default!;

    public string PageTitle => AppStrings.OptionsTitle;
    public string SettingsTitle => AppStrings.OptionsSettingsTitle;
    public string SettingsSubtitle => AppStrings.OptionsSettingsSubtitle;
    public string AboutTitle => AppStrings.OptionsAboutTitle;
    public string AboutSubtitle => AppStrings.OptionsAboutSubtitle;
    public string FeedbackTitle => AppStrings.OptionsFeedbackTitle;
    public string FeedbackSubtitle => AppStrings.OptionsFeedbackSubtitle;
    public string DonateTitle => AppStrings.OptionsDonateTitle;
    public string DonateSubtitle => AppStrings.OptionsDonateSubtitle;

    public OptionsViewModel(INavigationService navigationService)
    {
        BindNavigation(navigationService);

        OpenAboutPageCommand = new AsyncCommand(() => navigationService.GoToInfoAsync());
        OpenDonatePageCommand = new AsyncCommand(() => navigationService.GoToDonateAsync());
        OpenFeedbackPageCommand = new AsyncCommand(() => navigationService.GoToFormAsync());
        OpenSettingsPageCommand = new AsyncCommand(() => navigationService.GoToSettingsAsync());
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());

    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(SettingsTitle),
            nameof(SettingsSubtitle),
            nameof(AboutTitle),
            nameof(AboutSubtitle),
            nameof(FeedbackTitle),
            nameof(FeedbackSubtitle),
            nameof(DonateTitle),
            nameof(DonateSubtitle));
    }
}
