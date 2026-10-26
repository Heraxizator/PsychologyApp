using PsychologyApp.Presentation.Core.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.MuscleRelax;

/// <summary>
/// The guide for the short muscle relaxation: which group to tense, when to let go, and the seconds left. It starts only on request
/// and stops by itself after the last group (<see cref="MuscleRelaxation"/>); each change is felt as a light tick and read aloud,
/// so the person can follow it with closed eyes. Nothing moves, only the words and counts change.
/// </summary>
public partial class MuscleRelaxView : ContentView
{
    private IDispatcherTimer? _timer;
    private DateTime _startedAt;
    private (int Group, bool Tense)? _lastStep;

    public MuscleRelaxView()
    {
        InitializeComponent();
        ToggleButton.TapCommand = new Command(Toggle);
        Unloaded += (_, _) => StopTimer();
        ShowIdle(AppStrings.MuscleIntro, AppStrings.MuscleStart);
    }

    private bool IsRunning => _timer?.IsRunning == true;

    private void Toggle()
    {
        if (IsRunning)
        {
            StopTimer();
            ShowIdle(AppStrings.MuscleIntro, AppStrings.MuscleStart);
            return;
        }

        _startedAt = DateTime.UtcNow;
        _lastStep = null;
        PhaseLabel.IsVisible = true;
        CountLabel.IsVisible = true;
        ToggleButton.BodyText = AppStrings.MuscleStop;
        _timer ??= Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick -= OnTick;
        _timer.Tick += OnTick;
        _timer.Start();
        Render();
    }

    private void OnTick(object? sender, EventArgs e) => Render();

    private void Render()
    {
        MusclePosition position = MuscleRelaxation.At((DateTime.UtcNow - _startedAt).TotalSeconds);
        if (position.Finished)
        {
            StopTimer();
            ShowIdle(AppStrings.MuscleDone, AppStrings.MuscleAgain);
            SemanticScreenReader.Announce(AppStrings.MuscleDone);
            UiHaptics.Tick();
            return;
        }

        CountLabel.Text = position.SecondsLeft.ToString();
        if (_lastStep == (position.GroupNumber, position.Tense))
        {
            return;
        }

        _lastStep = (position.GroupNumber, position.Tense);
        PhaseLabel.Text = position.Tense ? AppStrings.MuscleTense : AppStrings.MuscleRelease;
        InstructionLabel.Text = position.Tense ? GroupText(position.Group) : AppStrings.MuscleReleaseHint;
        StepLabel.Text = AppStrings.MuscleStep(position.GroupNumber, MuscleRelaxation.Groups.Count);
        SemanticScreenReader.Announce(InstructionLabel.Text);
        UiHaptics.Tick();
    }

    private void ShowIdle(string text, string button)
    {
        PhaseLabel.IsVisible = false;
        CountLabel.IsVisible = false;
        InstructionLabel.Text = text;
        StepLabel.Text = string.Empty;
        ToggleButton.BodyText = button;
    }

    private void StopTimer()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
        }
    }

    private static string GroupText(MuscleGroup group) => group switch
    {
        MuscleGroup.Hands => AppStrings.MuscleGroupHands,
        MuscleGroup.Shoulders => AppStrings.MuscleGroupShoulders,
        MuscleGroup.Face => AppStrings.MuscleGroupFace,
        MuscleGroup.Belly => AppStrings.MuscleGroupBelly,
        _ => AppStrings.MuscleGroupLegs
    };
}
