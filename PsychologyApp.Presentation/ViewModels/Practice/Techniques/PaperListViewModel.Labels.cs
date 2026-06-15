using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Practice.Techniques;

public partial class PaperListViewModel
{
    public string ThoughtFieldLabel => Entries.Count > 0 ? Entries[0].Title ?? string.Empty : string.Empty;
    public string ThoughtPlaceholder => Entries.Count > 0 ? Entries[0].Placeholder ?? string.Empty : string.Empty;
    public string RepeatButtonText => AppStrings.Repeat;
    public string ConcernFieldLabel => Entries.Count > 0 && !string.IsNullOrWhiteSpace(Entries[0].Title)
        ? Entries[0].Title
        : AppStrings.ConcernLabel;

    protected override void OnTechniqueContentChanged()
    {
        OnPropertyChanged(nameof(ThoughtFieldLabel));
        OnPropertyChanged(nameof(ThoughtPlaceholder));
        OnPropertyChanged(nameof(RepeatButtonText));
        OnPropertyChanged(nameof(ConcernFieldLabel));
    }
}
