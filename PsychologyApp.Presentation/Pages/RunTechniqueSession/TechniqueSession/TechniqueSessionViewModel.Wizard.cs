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

    public string PreviousStepText => AppStrings.TechniqueStepBack;
    public string NextStepText => AppStrings.TechniqueStepNext;

    public ICommand PreviousStepCommand { get; private set; } = default!;
    public ICommand NextStepCommand { get; private set; } = default!;

    private void InitializeWizardCommands()
    {
        PreviousStepCommand = new Command(() =>
        {
            if (CanGoToPreviousStep)
            {
                CurrentStepIndex--;
            }
        });
        NextStepCommand = new Command(() =>
        {
            if (!CanGoToNextStep)
            {
                return;
            }

            CurrentStepIndex++;
            TechniqueEntryFeedback.PlayAddFeedback();
        });
    }

    private void ResetWizardStep() => CurrentStepIndex = 0;

    private void NotifyStepChanged()
    {
        OnPropertyChanged(nameof(CurrentEntry));
        OnPropertyChanged(nameof(IsWizardMode));
        OnPropertyChanged(nameof(CanGoToPreviousStep));
        OnPropertyChanged(nameof(CanGoToNextStep));
        OnPropertyChanged(nameof(StepProgressText));
    }
}
