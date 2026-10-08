using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Technique;

public sealed class TechniqueGroup : ObservableCollection<TechniqueItem>
{
    public TechniqueGroup(string title, IEnumerable<TechniqueItem> items)
        : base(items)
    {
        Title = title;
    }

    public string Title { get; }

    public bool HasTitle => !string.IsNullOrEmpty(Title);

    /// <summary>The person's own practices: the group that is paged and edited, as opposed to the built-in sections.</summary>
    public bool IsCustom { get; init; }

    /// <summary>The group of a built-in section ("Body"...); null for the others. Only a section with a key can be folded.</summary>
    public string? Key { get; init; }

    public bool CanCollapse => Key is not null;

    /// <summary>Folded: the heading stays, the cards are not in the list at all (hiding them left a blank page on Android).</summary>
    public bool IsCollapsed { get; init; }

    /// <summary>How many practices the section holds, also while it is folded.</summary>
    public int TotalCount { get; init; }

    /// <summary>The heading as shown: with the number of practices once the section is folded, so a closed section still says what is in it.</summary>
    public string HeaderText => IsCollapsed ? $"{Title} · {TotalCount}" : Title;

    /// <summary>Folds or opens the section; set by whoever builds the groups.</summary>
    public ICommand? ToggleCommand { get; init; }
}
