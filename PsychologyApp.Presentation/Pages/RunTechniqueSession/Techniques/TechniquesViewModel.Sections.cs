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

    private const string CollapsedPreferenceKey = "practice.collapsedSections";

    /// <summary>The sections the person has folded; all open until they fold one. Remembered between launches.</summary>
    private IReadOnlySet<string> _collapsedSections = LoadCollapsedSections();

    private static IReadOnlySet<string> LoadCollapsedSections()
    {
        try
        {
            return CollapsedSections.Parse(Microsoft.Maui.Storage.Preferences.Default.Get(CollapsedPreferenceKey, string.Empty));
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException)
        {
            return new HashSet<string>();
        }
    }

    private void ToggleSection(string key)
    {
        _collapsedSections = CollapsedSections.Toggle(_collapsedSections, key);
        try
        {
            Microsoft.Maui.Storage.Preferences.Default.Set(CollapsedPreferenceKey, CollapsedSections.Format(_collapsedSections));
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException)
        {
            // Not remembered this time; the section is still folded for now.
        }

        RebuildSections();
    }

    private void RebuildSections()
    {
        List<TechniqueGroup> groups = [.. TechniqueSections.Build(_staticItemsAll ?? [], SectionTitle, _collapsedSections, key => new Command(() => ToggleSection(key)))];
        if (_customGroup is not null)
        {
            groups.Add(_customGroup);
        }

        TechniqueGroups.ReplaceAll(groups);
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
