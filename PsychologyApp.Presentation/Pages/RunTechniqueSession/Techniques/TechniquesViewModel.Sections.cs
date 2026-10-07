using PsychologyApp.Presentation.Entities.Technique;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.Techniques;

/// <summary>
/// The catalog is the built-in practices in sections by group (body, thoughts, heart, action) and then the person's own practices. The sections are
/// replaced as a whole; the person's own group keeps one object for as long as the list lives, because loading more and inserting a new practice work on it.
/// </summary>
public partial class TechniquesViewModel
{
    /// <summary>Every built-in practice; inserting a new practice rebuilds the list from this, not from what is on screen.</summary>
    private List<TechniqueItem>? _staticItemsAll;

    private TechniqueGroup? _customGroup;

    /// <summary>The person has practices of their own: their group has a heading, and the "create" card sits right under it. Without any, the card brings its own heading.</summary>
    public bool HasCustomGroup => _customGroup is not null;

    public bool HasNoCustomGroup => !HasCustomGroup;

    public string CreateOwnLabel => AppStrings.PracticeCreateOwn;

    private void RebuildSections()
    {
        List<TechniqueGroup> groups = [.. TechniqueSections.Build(_staticItemsAll ?? [], SectionTitle)];
        if (_customGroup is not null)
        {
            groups.Add(_customGroup);
        }

        TechniqueGroups.ReplaceAll(groups);
        OnPropertyChanged(nameof(HasCustomGroup));
        OnPropertyChanged(nameof(HasNoCustomGroup));
    }

    private static string SectionTitle(TechniqueFlavor flavor) => flavor switch
    {
        TechniqueFlavor.Body => AppStrings.PracticeFilterBody,
        TechniqueFlavor.Mind => AppStrings.PracticeFilterMind,
        TechniqueFlavor.Heart => AppStrings.PracticeFilterHeart,
        TechniqueFlavor.Action => AppStrings.PracticeFilterAction,
        _ => string.Empty
    };
}
