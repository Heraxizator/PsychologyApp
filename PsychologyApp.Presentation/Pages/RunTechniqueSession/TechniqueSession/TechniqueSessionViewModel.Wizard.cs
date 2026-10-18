using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.UI.Components;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueSession;

public partial class TechniqueSessionViewModel
{
    private int _currentStepIndex;

    public int CurrentStepIndex
    {
        get => _currentStepIndex;
        private set
        {
            if (SetProperty(ref _currentStepIndex, value))
            {
                NotifyStepChanged();
            }
        }
    }

    /// <summary>Shows step-by-step navigation only when there is more than one field to fill in.</summary>
    public bool IsWizardMode => Entries.Count > 1;

    public EntryItem? CurrentEntry => Entries.Count == 0
        ? null
        : Entries[Math.Clamp(CurrentStepIndex, 0, Entries.Count - 1)];

    public bool CanGoToPreviousStep => CurrentStepIndex > 0;

    public bool CanGoToNextStep => IsWizardMode && CurrentStepIndex < Entries.Count - 1;

    public string StepProgressText => IsWizardMode
        ? AppStrings.TechniqueStepProgress(CurrentStepIndex + 1, Entries.Count)
        : string.Empty;

    /// <summary>
    /// A multi-step session can be finished on the last step, or earlier once something has been written: every field is optional,
    /// and a short practice is better than an abandoned one.
    /// </summary>
    public bool CanFinish => !IsWizardMode || !CanGoToNextStep || Entries.Any(e => !string.IsNullOrWhiteSpace(e.Text));

    public string PreviousStepText => AppStrings.TechniqueStepBack;

    /// <summary>"Skip" while the current step is empty, so moving on without an answer is an obvious, allowed choice.</summary>
    public string NextStepText => string.IsNullOrWhiteSpace(CurrentEntry?.Text) ? AppStrings.TechniqueStepSkip : AppStrings.TechniqueStepNext;

    public bool IsSingleStep => !IsWizardMode;

    public bool IsCurrentEmpty => string.IsNullOrWhiteSpace(CurrentEntry?.Text);

    /// <summary>The look of the forward button: filled once the step has an answer, outlined while it would only skip.</summary>
    public string NextStepVariant => IsCurrentEmpty ? "Secondary" : "Primary";

    public ICommand PreviousStepCommand { get; private set; } = default!;
    public ICommand NextStepCommand { get; private set; } = default!;

    private void InitializeWizardCommands()
    {
        PreviousStepCommand = new DebouncedCommand(() =>
        {
            if (CanGoToPreviousStep)
            {
                CurrentStepIndex--;
            }
        });
        NextStepCommand = new DebouncedCommand(() =>
        {
            if (!CanGoToNextStep)
            {
                return;
            }

            CurrentStepIndex++;
            TechniqueEntryFeedback.PlayAddFeedback();
        });
    }

    private void ResetWizardStep()
    {
        _currentStepIndex = 0;
        OnPropertyChanged(nameof(CurrentStepIndex));
        NotifyStepChanged();
    }

    private void NotifyStepChanged()
    {
        OnPropertyChanged(nameof(CurrentEntry));
        OnPropertyChanged(nameof(IsWizardMode));
        OnPropertyChanged(nameof(CanGoToPreviousStep));
        OnPropertyChanged(nameof(CanGoToNextStep));
        OnPropertyChanged(nameof(StepProgressText));
        OnPropertyChanged(nameof(CanFinish));
        OnPropertyChanged(nameof(IsSingleStep));
        OnPropertyChanged(nameof(IsCurrentEmpty));
        OnPropertyChanged(nameof(NextStepVariant));
        OnPropertyChanged(nameof(NextStepText));
        OfferPhrasesForCurrentStep();
    }
}
