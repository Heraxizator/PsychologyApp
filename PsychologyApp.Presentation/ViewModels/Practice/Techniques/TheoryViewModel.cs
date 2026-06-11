using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;
    private string text = string.Empty;

    public string PageTitle => AppStrings.TechniqueTheory;
    public string BackText => AppStrings.Back;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigation navigation, INavigationService navigationService, string content, TechniqueId? techniqueId = null)
    {
        _techniqueId = techniqueId;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.TechniqueTheory;

        BindNavigation(navigation, navigationService);
        Text = content;
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(BackText));

        if (_techniqueId is TechniqueId techniqueId)
        {
            Text = TechniqueCatalog.Get(techniqueId).TheoryInfo;
        }
    }

    public string Text
    {
        get => text;
        set
        {
            if (text != value)
            {
                text = value;
                OnPropertyChanged(nameof(Text));
            }
        }
    }
}
