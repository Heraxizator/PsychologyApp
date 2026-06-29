using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Widgets.Test;

public partial class TestProgressHeaderView : ContentView
{
    private const int BarProgressThreshold = 7;

    public TestProgressHeaderView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        if (ProgressBarView is not null)
        {
            ProgressBarView.Progress = Progress;
        }
    }

    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(nameof(Step), typeof(int), typeof(TestProgressHeaderView), 0, propertyChanged: OnProgressModeChanged);

    public static readonly BindableProperty StepCountProperty =
        BindableProperty.Create(nameof(StepCount), typeof(int), typeof(TestProgressHeaderView), 1, propertyChanged: OnProgressModeChanged);

    public static readonly BindableProperty StepLabelProperty =
        BindableProperty.Create(nameof(StepLabel), typeof(string), typeof(TestProgressHeaderView), string.Empty);

    public static readonly BindableProperty ShowStepLabelProperty =
        BindableProperty.Create(nameof(ShowStepLabel), typeof(bool), typeof(TestProgressHeaderView), true);

    public static readonly BindableProperty ProgressProperty =
        BindableProperty.Create(nameof(Progress), typeof(double), typeof(TestProgressHeaderView), 0d, propertyChanged: OnProgressChanged);

    public static readonly BindableProperty UseBarProgressProperty =
        BindableProperty.Create(nameof(UseBarProgress), typeof(bool), typeof(TestProgressHeaderView), false, propertyChanged: OnProgressModeChanged);

    public static readonly BindableProperty RemainingDurationTextProperty =
        BindableProperty.Create(nameof(RemainingDurationText), typeof(string), typeof(TestProgressHeaderView), string.Empty, propertyChanged: OnRemainingDurationChanged);

    public static readonly BindableProperty ShowBarProgressProperty =
        BindableProperty.Create(nameof(ShowBarProgress), typeof(bool), typeof(TestProgressHeaderView), false);

    public static readonly BindableProperty ShowDotProgressProperty =
        BindableProperty.Create(nameof(ShowDotProgress), typeof(bool), typeof(TestProgressHeaderView), true);

    public static readonly BindableProperty HasRemainingDurationProperty =
        BindableProperty.Create(nameof(HasRemainingDuration), typeof(bool), typeof(TestProgressHeaderView), false);

    public int Step
    {
        get => (int)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public int StepCount
    {
        get => (int)GetValue(StepCountProperty);
        set => SetValue(StepCountProperty, value);
    }

    public string StepLabel
    {
        get => (string)GetValue(StepLabelProperty);
        set => SetValue(StepLabelProperty, value);
    }

    public bool ShowStepLabel
    {
        get => (bool)GetValue(ShowStepLabelProperty);
        set => SetValue(ShowStepLabelProperty, value);
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public bool UseBarProgress
    {
        get => (bool)GetValue(UseBarProgressProperty);
        set => SetValue(UseBarProgressProperty, value);
    }

    public string RemainingDurationText
    {
        get => (string)GetValue(RemainingDurationTextProperty);
        set => SetValue(RemainingDurationTextProperty, value);
    }

    public bool ShowBarProgress
    {
        get => (bool)GetValue(ShowBarProgressProperty);
        private set => SetValue(ShowBarProgressProperty, value);
    }

    public bool ShowDotProgress
    {
        get => (bool)GetValue(ShowDotProgressProperty);
        private set => SetValue(ShowDotProgressProperty, value);
    }

    public bool HasRemainingDuration
    {
        get => (bool)GetValue(HasRemainingDurationProperty);
        private set => SetValue(HasRemainingDurationProperty, value);
    }

    private static void OnProgressModeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestProgressHeaderView view)
        {
            view.UpdateProgressMode();
        }
    }

    private static void OnProgressChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestProgressHeaderView view)
        {
            view.AnimateProgressAsync((double)newValue).FireAndForget();
        }
    }

    private static void OnRemainingDurationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TestProgressHeaderView view)
        {
            view.HasRemainingDuration = !string.IsNullOrWhiteSpace(view.RemainingDurationText);
        }
    }

    private void UpdateProgressMode()
    {
        bool useBar = UseBarProgress || StepCount > BarProgressThreshold;
        SetValue(ShowBarProgressProperty, useBar);
        SetValue(ShowDotProgressProperty, !useBar);
    }

    private async Task AnimateProgressAsync(double target)
    {
        if (ProgressBarView is null)
        {
            return;
        }

        if (!UiAnimations.ShouldAnimate(this))
        {
            ProgressBarView.Progress = target;
            return;
        }

        double start = ProgressBarView.Progress;
        if (Math.Abs(start - target) < 0.001)
        {
            return;
        }

        const uint duration = 200;
        uint elapsed = 0;
        const uint stepMs = 16;

        while (elapsed < duration)
        {
            elapsed += stepMs;
            double t = Math.Min(1.0, elapsed / (double)duration);
            ProgressBarView.Progress = start + ((target - start) * t);
            await Task.Delay((int)stepMs);
        }

        ProgressBarView.Progress = target;
    }
}
