using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Widgets.PracticeStages;

/// <summary>
/// "Make it small": the problem stands as a card in the foreground of an open landscape. Pinching it (or the button) sends it back
/// toward the horizon: smaller, paler, further, until it is a small mark where the sky meets the land. Size and distance change how much
/// an image pulls at the nerves.
/// </summary>
public sealed class ShrinkStageView : PracticeStageView
{
    private const double MinScale = 0.16;
    private const double DoneScale = 0.4;

    // The card starts in the foreground, a little below the middle, and ends on the horizon line.
    private static readonly double ForegroundY = CanvasHeight * 0.14;
    private static readonly double HorizonY = CanvasHeight * StageScenery.HorizonShare - CanvasHeight / 2;

    private readonly Label _words = CreateWordsLabel();
    private readonly Border _card;
    private double _scale = 1;
    private bool _done;

    public ShrinkStageView(TechniqueFlavor flavor)
        : base(flavor, AppStrings.StageResizeTitle, AppStrings.StageResizeHint)
    {
        SetScene(StageScenery.LandscapeBase(IsDark), darkScene: IsDark, StageScenery.Landscape(CanvasHeight, IsDark));
        _card = CreateWordsCard(_words);

        PinchGestureRecognizer pinch = new();
        pinch.PinchUpdated += OnPinch;
        _card.GestureRecognizers.Add(pinch);

        Canvas.Children.Add(_card);
        Canvas.Children.Add(EmptyLabel);
        SetActions(CreateButton(AppStrings.StageResizeAction, () => PushAwayAsync().FireAndForget()));
        Apply(1, tick: false);
        OnSourceTextChanged();
    }

    protected override void OnSourceTextChanged()
    {
        _words.Text = SourceText;
        _card.IsVisible = HasSource;
        EmptyLabel.IsVisible = !HasSource;
        Actions.IsEnabled = HasSource;
    }

    private void OnPinch(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (!HasSource)
        {
            return;
        }

        if (e.Status == GestureStatus.Running)
        {
            Apply(Math.Clamp(_scale * e.Scale, MinScale, 1.12));
        }
        else if (e.Status == GestureStatus.Completed && _scale <= DoneScale)
        {
            PushAwayAsync().FireAndForget();
        }
    }

    /// <summary>Scale, distance and colour move together: the smaller the card, the nearer the horizon and the paler it is.</summary>
    private void Apply(double scale, bool tick = true)
    {
        int previousStep = (int)(_scale * 10);
        _scale = scale;
        double away = (1 - Math.Clamp(scale, MinScale, 1)) / (1 - MinScale);
        _card.Scale = scale;
        _card.TranslationY = ForegroundY + (HorizonY - ForegroundY) * away;
        _card.Opacity = 1 - 0.45 * away;
        if (tick && (int)(scale * 10) != previousStep)
        {
            UiHaptics.Tick();
        }
    }

    private async Task PushAwayAsync()
    {
        if (!HasSource)
        {
            return;
        }

        if (_done)
        {
            await ResetAsync();
            return;
        }

        _done = true;
        await AnimateToAsync(MinScale, 2800, Easing.CubicInOut);
        UiHaptics.Tick();
        ShowStatus(AppStrings.StageResizeDone);
        SetActions(CreateButton(AppStrings.StageAgain, () => ResetAsync().FireAndForget()));
    }

    private async Task ResetAsync()
    {
        _done = false;
        ShowStatus(string.Empty);
        await AnimateToAsync(1, 500, Easing.CubicOut);
        SetActions(CreateButton(AppStrings.StageResizeAction, () => PushAwayAsync().FireAndForget()));
    }

    private Task AnimateToAsync(double target, uint length, Easing easing)
    {
        if (!UiAnimations.ShouldAnimate(_card))
        {
            Apply(target, tick: false);
            return Task.CompletedTask;
        }

        TaskCompletionSource finished = new();
        _card.Animate("shrink", v => Apply(v), _scale, target, length: length, easing: easing, finished: (_, _) => finished.TrySetResult());
        return finished.Task;
    }
}
