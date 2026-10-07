using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Technique;

public class TechniqueItem : System.ComponentModel.INotifyPropertyChanged
{
    private bool _active;

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    public long Id { get; set; }
    public string? Number { get; set; }
    public string? Date { get; set; }
    public string? Image { get; set; }
    public string? IconName { get; set; }
    public string? DurationText { get; set; }
    public string? MetaText { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Theme { get; set; }
    public string? Author { get; set; }

    /// <summary>A <c>TechniqueFlavor</c> name: the group of the practice, which colours its card.</summary>
    public string? Flavor { get; set; }
    /// <summary>Whether the card is shown; the group filter turns cards off and on without touching the list.</summary>
    public bool Active
    {
        get => _active;
        set
        {
            if (_active != value)
            {
                _active = value;
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Active)));
            }
        }
    }
    public ICommand? TapCommand { get; set; }

    /// <summary>True when both would render identically, so a refresh can keep the bound instance and skip a rebind.</summary>
    public static bool SameContent(TechniqueItem? a, TechniqueItem? b) =>
        ReferenceEquals(a, b)
        || (a is not null && b is not null
            && a.Id == b.Id && a.Title == b.Title && a.Subtitle == b.Subtitle && a.MetaText == b.MetaText
            && a.Date == b.Date && a.IconName == b.IconName && a.Image == b.Image && a.Active == b.Active
            && a.DurationText == b.DurationText && a.Flavor == b.Flavor && a.Number == b.Number && a.Theme == b.Theme && a.Author == b.Author);
}
