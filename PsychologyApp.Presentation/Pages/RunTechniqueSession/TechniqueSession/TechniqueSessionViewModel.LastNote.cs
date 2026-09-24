using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

public partial class TechniqueSessionViewModel
{
    private string _lastNoteText = string.Empty;

    /// <summary>What the person wrote after the last completed session of this same technique, if anything —
    /// a small nudge that what they noticed before is still there, not just a fresh blank form every time.</summary>
    public string LastNoteText
    {
        get => _lastNoteText;
        private set
        {
            if (SetProperty(ref _lastNoteText, value))
            {
                OnPropertyChanged(nameof(HasLastNote));
            }
        }
    }

    public bool HasLastNote => LastNoteText.Length > 0;

    public string LastNoteTitle => AppStrings.PracticeLastNoteTitle;

    private void LoadLastNoteAsync() =>
        LoadLastNoteCoreAsync().FireAndForget();

    private async Task LoadLastNoteCoreAsync()
    {
        try
        {
            string? note = await _userProgressService.GetLastSessionNoteAsync(_techniqueId.ToString());
            LastNoteText = note ?? string.Empty;
        }
        catch
        {
            // The last note is a nice-to-have; the session works fine without it.
        }
    }
}
