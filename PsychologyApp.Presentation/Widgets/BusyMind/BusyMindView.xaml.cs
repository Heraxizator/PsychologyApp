using PsychologyApp.Presentation.Core.Practice;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Widgets.BusyMind;

/// <summary>
/// "Keep the mind busy": one task at a time, the next one when the person says it is done, another in its place if this one does not
/// fit. A round has a fixed number of tasks (<see cref="BusyMindRound"/>) and then hands the person back to the rating.
/// </summary>
public partial class BusyMindView : ContentView
{
    private BusyMindRound? _round;

    public BusyMindView()
    {
        InitializeComponent();
        MainButton.TapCommand = new Command(OnMain);
        SkipButton.BodyText = AppStrings.BusyMindSkip;
        SkipButton.TapCommand = new Command(OnSkip);
        ShowIdle(AppStrings.BusyMindIntro, AppStrings.BusyMindStart);
    }

    private void OnMain()
    {
        if (_round is null || _round.IsOver)
        {
            _round = new BusyMindRound();
            MainButton.BodyText = AppStrings.BusyMindNext;
            StepLabel.IsVisible = true;
            SkipButton.IsVisible = true;
            ShowTask();
            return;
        }

        _round.Next();
        if (_round.IsOver)
        {
            ShowIdle(AppStrings.BusyMindDone, AppStrings.BusyMindAgain);
            SemanticScreenReader.Announce(AppStrings.BusyMindDone);
            UiHaptics.Tick();
            return;
        }

        ShowTask();
    }

    private void OnSkip()
    {
        if (_round?.Skip() == true)
        {
            ShowTask();
        }
    }

    private void ShowTask()
    {
        if (_round is null)
        {
            return;
        }

        StepLabel.Text = AppStrings.BusyMindStep(_round.Step, _round.Length);
        TaskLabel.Text = AppStrings.BusyMindTask(_round.Current);
        SemanticScreenReader.Announce(TaskLabel.Text);
        UiHaptics.Tick();
    }

    private void ShowIdle(string text, string button)
    {
        StepLabel.IsVisible = false;
        SkipButton.IsVisible = false;
        TaskLabel.Text = text;
        MainButton.BodyText = button;
    }
}
