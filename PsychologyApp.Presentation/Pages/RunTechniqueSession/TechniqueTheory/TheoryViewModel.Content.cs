using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueTheory;

public partial class TheoryViewModel
{
    private string text = string.Empty;
    private string techniqueSubtitle = string.Empty;
    private IReadOnlyList<TheorySection> sections = [];

    public bool HasSections => sections.Count > 0;
    public bool HasLegacyText => !HasSections && !string.IsNullOrWhiteSpace(Text);
    public bool HasTechniqueSubtitle => !string.IsNullOrWhiteSpace(TechniqueSubtitle);
    public IReadOnlyList<TheorySection> Sections => sections;

    public string TechniqueSubtitle
    {
        get => techniqueSubtitle;
        private set
        {
            if (techniqueSubtitle == value)
            {
                return;
            }

            techniqueSubtitle = value;
            OnPropertyChanged(nameof(TechniqueSubtitle));
            OnPropertyChanged(nameof(HasTechniqueSubtitle));
        }
    }

    private void ApplyContent(string content, TechniqueId? techniqueId)
    {
        if (techniqueId is not null)
        {
            throw new InvalidOperationException("Use ApplyContent(TechniqueDefinition) when techniqueId is set.");
        }

        TechniqueSubtitle = string.Empty;
        sections = [];
        Text = content;
        NotifyContentChanged();
    }

    private void ApplyContent(TechniqueDefinition definition)
    {
        TechniqueSubtitle = definition.ListTitle;
        sections = definition.TheorySections ?? [];
        Text = sections.Count > 0 ? string.Empty : definition.TheoryInfo;
        NotifyContentChanged();
    }

    private void NotifyContentChanged()
    {
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
