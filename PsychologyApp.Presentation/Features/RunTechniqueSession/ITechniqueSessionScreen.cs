using System.Windows.Input;
using MvvmHelpers;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

/// <summary>
/// What TechniqueSessionPage and its shell bind to. The page hosts three unrelated view models (Paper/Copied,
/// Polarity, and every entry-based technique), so this is the type its compiled bindings are checked against.
/// Defaults cover the list-style techniques, which have no wizard steps and no remembered session note.
/// </summary>
public interface ITechniqueSessionScreen
{
    string PageName { get; }
    string TechniquePageTitle { get; }
    ObservableRangeCollection<string> Algorithm { get; }
    ICommand Theory { get; }
    ICommand BackCommand { get; }
    ICommand CompleteCommand { get; }
    string PreIntensityLabel { get; }
    string PreIntensityText { get; set; }

    bool CanFinish => true;
    bool HasLastNote => false;
    string LastNoteTitle => string.Empty;
    string LastNoteText => string.Empty;
}
