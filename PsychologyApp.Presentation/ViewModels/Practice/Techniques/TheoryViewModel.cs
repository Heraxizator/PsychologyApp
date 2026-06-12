using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.ViewModels;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public class TheoryViewModel : BaseViewModel
{
    private readonly TechniqueId? _techniqueId;
    private string text = string.Empty;
    private IReadOnlyList<TheorySection> sections = [];

    public string PageTitle => AppStrings.TechniqueTheory;
    public string BackText => AppStrings.Back;
    public bool HasSections => sections.Count > 0;
    public bool HasLegacyText => !HasSections && !string.IsNullOrWhiteSpace(Text);
    public IReadOnlyList<TheorySection> Sections => sections;

    public TheoryViewModel() { }

    public TheoryViewModel(INavigation navigation, INavigationService navigationService, string content, TechniqueId? techniqueId = null)
    {
        _techniqueId = techniqueId;
        ModuleName = AppStrings.ShellTabPractice;
        PageName = AppStrings.TechniqueTheory;

        BindNavigation(navigation, navigationService);
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

    private void ApplyContent(string content, TechniqueId? techniqueId)
    {
        if (techniqueId is TechniqueId id)
        {
            TechniqueDefinition definition = TechniqueCatalog.Get(id);
            sections = definition.TheorySections ?? [];
            Text = sections.Count > 0 ? string.Empty : definition.TheoryInfo;
        }
        else
        {
            sections = [];
            Text = content;
        }

        OnPropertyChanged(nameof(Sections));
        OnPropertyChanged(nameof(HasSections));
        OnPropertyChanged(nameof(HasLegacyText));
        OnPropertyChanged(nameof(Text));
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
                OnPropertyChanged(nameof(HasLegacyText));
            }
        }
    }
}
