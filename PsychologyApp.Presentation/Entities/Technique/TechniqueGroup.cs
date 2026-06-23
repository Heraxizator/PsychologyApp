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

    public bool HasTitle => !string.IsNullOrEmpty(Title);
}
