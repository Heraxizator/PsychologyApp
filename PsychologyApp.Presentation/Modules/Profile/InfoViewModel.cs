using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.About;

public class InfoViewModel : BaseViewModel
{
    public string PageTitle => AppStrings.OptionsAboutTitle;
    public string AboutBody => AppStrings.InfoAboutBody;

    public InfoViewModel(INavigation navigation, INavigationService navigationService)
    {
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.OptionsAboutTitle;
        BindNavigation(navigation);
        BackCommand = new AsyncCommand(() => navigationService.GoBackAsync());
    }

    public ICommand BackCommand { get; }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(AboutBody));
    }
}
