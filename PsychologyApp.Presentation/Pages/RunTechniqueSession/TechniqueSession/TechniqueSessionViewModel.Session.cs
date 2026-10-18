using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Models.Practice.Techniques;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

public partial class TechniqueSessionViewModel
{
    /// <summary>The practice has its own "tension before" step (breathing, thought record): the separate scale above is not shown.</summary>
    public bool AsksPreIntensity => !Entries.Any(e => e.Role == EntryFieldRole.IntensityBefore);

    protected override void OnTechniqueContentChanged()
    {
        WatchEntries();
        ResetWizardStep();
        OnPropertyChanged(nameof(AsksPreIntensity));

        if (AppliedUiKind == TechniqueUiKind.Entry)
        {
            _entryDraftCoordinator.WireHandlers();
            _entryDraftCoordinator.LoadAsync(() =>
            {
                OnPropertyChanged(nameof(Entries));
                ResumeWhereLeft();
                Notify(nameof(NextStepText), nameof(CanFinish), nameof(IsCurrentEmpty), nameof(NextStepVariant));
            }).FireAndForget();
        }

        LoadLastNoteAsync();
        OfferRecentProblemAsync().FireAndForget();
    }

    public void SaveEntryDraftIfNeeded() => _entryDraftCoordinator.SaveIfNeeded();

    /// <summary>A practice left half-way opens again at its first unanswered step, not at the start.</summary>
    private void ResumeWhereLeft()
    {
        if (!IsWizardMode)
        {
            return;
        }

        int step = PracticeAnswers.ResumeStep(Entries.Select(e => (string?)e.Text).ToList());
        if (step > 0)
        {
            CurrentStepIndex = step;
        }
    }

    private async Task CompleteSessionAsync()
    {
        _entryDraftCoordinator.MarkSessionCompleted();
        await RememberProblemAsync();
        RememberTypedAnswers();

        await _sessionHelper.CompleteAsync(
            _techniqueId.ToString(),
            ModuleName,
            PageName,
            _sessionStartedAt,
            RatingOf(EntryFieldRole.IntensityBefore) ?? TryGetPreIntensity(),
            RatingOf(EntryFieldRole.IntensityAfter));
    }

    /// <summary>The 0..10 answer of the step with this role, when the practice has one and it was answered.</summary>
    private int? RatingOf(EntryFieldRole role) =>
        Entries.FirstOrDefault(e => e.Role == role) is { } entry && int.TryParse(entry.Text, out int value) && value is >= 0 and <= 10
            ? value
            : null;
}
