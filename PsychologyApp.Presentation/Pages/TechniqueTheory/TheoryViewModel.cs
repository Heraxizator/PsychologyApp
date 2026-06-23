using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.TechniqueTheory;

public partial class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;
    private readonly string? _legacyContent;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigationService navigationService, string content, TechniqueId? techniqueId = null)
    {
        _techniqueId = techniqueId;
        _legacyContent = techniqueId is null ? content : null;
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
            return;
        }

        if (!string.IsNullOrWhiteSpace(_legacyContent))
        {
            ApplyContent(_legacyContent, null);
        }
    }
}
