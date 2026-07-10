using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class CompletionCelebrationView : ContentView
{
    private VisualElement? _iconHalo;
    private bool _milestonePulseStarted;

    public CompletionCelebrationView()
    {
        InitializeComponent();
        IconName = MaterialIconNames.CheckCircle;
        Loaded += OnLoaded;
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(CompletionCelebrationView), MaterialIconNames.CheckCircle);

    public static readonly BindableProperty IsMilestoneProperty =
        BindableProperty.Create(
            nameof(IsMilestone),
            typeof(bool),
            typeof(CompletionCelebrationView),
            false,
            propertyChanged: OnIsMilestoneChanged);

    public static readonly BindableProperty StreakValueTextProperty =
        BindableProperty.Create(nameof(StreakValueText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty StreakLabelTextProperty =
        BindableProperty.Create(nameof(StreakLabelText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty HasStreakProperty =
        BindableProperty.Create(nameof(HasStreak), typeof(bool), typeof(CompletionCelebrationView), false);

    public static readonly BindableProperty PrimaryActionTextProperty =
        BindableProperty.Create(nameof(PrimaryActionText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty SecondaryActionTextProperty =
        BindableProperty.Create(nameof(SecondaryActionText), typeof(string), typeof(CompletionCelebrationView), string.Empty);

    public static readonly BindableProperty PrimaryCommandProperty =
        BindableProperty.Create(nameof(PrimaryCommand), typeof(ICommand), typeof(CompletionCelebrationView), null);

    public static readonly BindableProperty SecondaryCommandProperty =
        BindableProperty.Create(nameof(SecondaryCommand), typeof(ICommand), typeof(CompletionCelebrationView), null);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public bool IsMilestone
    {
        get => (bool)GetValue(IsMilestoneProperty);
        set => SetValue(IsMilestoneProperty, value);
    }

    public string StreakValueText
    {
        get => (string)GetValue(StreakValueTextProperty);
        set => SetValue(StreakValueTextProperty, value);
    }

    public string StreakLabelText
    {
        get => (string)GetValue(StreakLabelTextProperty);
        set => SetValue(StreakLabelTextProperty, value);
    }

    public bool HasStreak
    {
        get => (bool)GetValue(HasStreakProperty);
        set => SetValue(HasStreakProperty, value);
    }

    public string PrimaryActionText
    {
        get => (string)GetValue(PrimaryActionTextProperty);
        set => SetValue(PrimaryActionTextProperty, value);
    }

    public string SecondaryActionText
    {
        get => (string)GetValue(SecondaryActionTextProperty);
        set => SetValue(SecondaryActionTextProperty, value);
    }

    public ICommand? PrimaryCommand
    {
        get => (ICommand?)GetValue(PrimaryCommandProperty);
        set => SetValue(PrimaryCommandProperty, value);
    }

    public ICommand? SecondaryCommand
    {
        get => (ICommand?)GetValue(SecondaryCommandProperty);
        set => SetValue(SecondaryCommandProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _iconHalo = GetTemplateChild("IconHalo") as VisualElement;
    }

    private static void OnIsMilestoneChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CompletionCelebrationView view && newValue is true)
        {
            view.TryPulseMilestoneAsync().FireAndForget();
        }
    }

    private void OnLoaded(object? sender, EventArgs e) =>
        TryPulseMilestoneAsync().FireAndForget();

    private async Task TryPulseMilestoneAsync()
    {
        if (!IsMilestone || _milestonePulseStarted || !IsLoaded)
        {
            return;
        }

        _milestonePulseStarted = true;
        await Task.Delay((int)UiAnimations.MicroDuration);
        VisualElement? target = _iconHalo
            ?? GetTemplateChild("IconHalo") as VisualElement
            ?? this;
        await UiAnimations.SafePulseAsync(target);
    }
}
