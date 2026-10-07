using System.Collections.ObjectModel;

namespace PsychologyApp.Presentation.Entities.Technique;

public sealed class TechniqueGroup : ObservableCollection<TechniqueItem>
{
    public TechniqueGroup(string title, IEnumerable<TechniqueItem> items)
        : base(items)
    {
        Title = title;
    }

    public string Title { get; }

    /// <summary>The person's own practices: the group that is paged and edited, as opposed to the built-in sections.</summary>
    public bool IsCustom { get; init; }

    public bool HasTitle => !string.IsNullOrEmpty(Title);
}
