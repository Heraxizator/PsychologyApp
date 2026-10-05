using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Onboarding;

public partial class OnboardingConcernOptionView : ContentView
{
    public OnboardingConcernOptionView()
    {
        InitializeComponent();
        TemplatePressFeedback.Attach(this, new PressFeedbackOptions { HapticOnRelease = true });
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(OnboardingConcernOptionView), string.Empty);

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(OnboardingConcernOptionView), string.Empty);

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(OnboardingConcernOptionView), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(OnboardingConcernOptionView), default);

    public static readonly BindableProperty IsSelectedProperty =
        BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(OnboardingConcernOptionView), false);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private readonly TapDebouncer _tapDebouncer = new();

    private void OnOptionTapped(object? sender, TappedEventArgs e) => SafeAsync.Run(OptionTappedAsync);

    private async Task OptionTappedAsync()
    {
        // A double tap would run the pulse and the command twice (and answer the same question twice).
        if (!_tapDebouncer.TryEnter())
        {
            return;
        }

        await UiAnimations.SafePulseAsync(this);

        if (TapCommand?.CanExecute(null) == true)
        {
            TapCommand.Execute(null);
        }
    }
}
