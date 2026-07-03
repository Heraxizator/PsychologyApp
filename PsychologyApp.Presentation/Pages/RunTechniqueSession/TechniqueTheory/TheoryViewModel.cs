using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession.Index;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;
    private readonly string? _legacyContent;
    private readonly TechniqueCatalogGateway _techniqueCatalog;

    public TheoryViewModel() { }

    public TheoryViewModel(
        INavigationService navigationService,
        TechniqueCatalogGateway techniqueCatalog,
        string content,
        TechniqueId? techniqueId = null)
    {
        _techniqueId = techniqueId;
        _legacyContent = techniqueId is null ? content : null;
        _techniqueCatalog = techniqueCatalog;
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
            ApplyContent(_techniqueCatalog.Get(techniqueId).TheoryInfo, techniqueId);
            return;
        }

        if (!string.IsNullOrWhiteSpace(_legacyContent))
        {
            ApplyContent(_legacyContent, null);
        }
    }
}
