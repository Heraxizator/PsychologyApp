using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ProfileInfo;

public class InfoViewModel : BaseViewModel
{
    public string PageTitle => AppStrings.OptionsAboutTitle;
    public string AboutBody => AppStrings.InfoAboutBody;
    public string AppVersionText => AppStrings.InfoAppVersion(AppInfo.Current.VersionString);

    public InfoViewModel(INavigationService navigationService)
    {
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.OptionsAboutTitle;
        BindNavigation(navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
    }

    public ICommand BackCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(AboutBody), nameof(AppVersionText));
    }
}
