using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigationService navigationService, string content, TechniqueId? techniqueId = null)
    {
        _techniqueId = techniqueId;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.TechniqueTheory;

        BindNavigation(navigationService);
        ApplyContent(content, techniqueId);
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(nameof(PageTitle), nameof(BackText));

        if (_techniqueId is TechniqueId techniqueId)
        {
            ApplyContent(TechniqueCatalog.Get(techniqueId).TheoryInfo, techniqueId);
        }
    }
}
