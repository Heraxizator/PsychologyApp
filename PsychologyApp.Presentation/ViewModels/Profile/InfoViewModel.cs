using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public class InfoViewModel : BaseViewModel
{
    public string PageTitle => AppStrings.OptionsAboutTitle;
    public string AboutBody => AppStrings.InfoAboutBody;
    public string AppVersionText => AppStrings.InfoAppVersion(AppInfo.Current.VersionString);

    public InfoViewModel(INavigation navigation, INavigationService navigationService)
    {
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.OptionsAboutTitle;
        BindNavigation(navigation, navigationService);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
    }

    public ICommand BackCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(AboutBody), nameof(AppVersionText));
    }
}
