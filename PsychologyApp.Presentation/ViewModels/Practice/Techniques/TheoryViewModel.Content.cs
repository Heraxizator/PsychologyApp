using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class TheoryViewModel
{
    private string text = string.Empty;
    private IReadOnlyList<TheorySection> sections = [];

    public bool HasSections => sections.Count > 0;
    public bool HasLegacyText => !HasSections && !string.IsNullOrWhiteSpace(Text);
    public IReadOnlyList<TheorySection> Sections => sections;

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
