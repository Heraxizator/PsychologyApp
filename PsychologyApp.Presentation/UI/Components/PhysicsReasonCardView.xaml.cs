using PsychologyApp.Presentation.Common;
using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class PhysicsReasonCardView : ContentView
{
    private StackLayout? _expandedSection;
    private bool _isAnimatingExpand;

    public PhysicsReasonCardView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        VisualElementPressFeedback.AttachToTemplateRoot(this);
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _expandedSection = this.GetVisualTreeDescendants()
            .OfType<StackLayout>()
            .FirstOrDefault(layout => layout.StyleId == "ExpandedSection");

        if (_expandedSection is not null)
        {
            _expandedSection.IsVisible = IsExpanded;
            _expandedSection.Opacity = IsExpanded ? 1 : 0;
        }
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName != nameof(IsExpanded) || _expandedSection is null || _isAnimatingExpand)
        {
            return;
        }

        HandleExpandedChangedAsync().FireAndForget();
    }

    private async Task HandleExpandedChangedAsync()
    {
        if (_expandedSection is null)
        {
            return;
        }

        if (!UiAnimations.ShouldAnimate(this))
        {
            _expandedSection.IsVisible = IsExpanded;
            _expandedSection.Opacity = IsExpanded ? 1 : 0;
            _expandedSection.TranslationY = 0;
            return;
        }

        _isAnimatingExpand = true;
        try
        {
            if (IsExpanded)
            {
                _expandedSection.IsVisible = true;
                _expandedSection.Opacity = 0;
                _expandedSection.TranslationY = -UiAnimations.SlideOffset;
                await Task.WhenAll(
                    _expandedSection.FadeToAsync(1, UiAnimations.MicroDuration, UiAnimations.StandardEasing),
                    _expandedSection.TranslateToAsync(0, 0, UiAnimations.MicroDuration, UiAnimations.StandardEasing));
            }
            else
            {
                await Task.WhenAll(
                    _expandedSection.FadeToAsync(0, UiAnimations.MicroDuration, UiAnimations.StandardEasing),
                    _expandedSection.TranslateToAsync(0, -UiAnimations.SlideOffset, UiAnimations.MicroDuration, UiAnimations.StandardEasing));
                _expandedSection.IsVisible = false;
                _expandedSection.Opacity = 1;
                _expandedSection.TranslationY = 0;
            }
        }
        finally
        {
            _isAnimatingExpand = false;
        }
    }

    public static readonly BindableProperty TitleFormattedProperty =
        BindableProperty.Create(nameof(TitleFormatted), typeof(FormattedString), typeof(PhysicsReasonCardView), null);

    public FormattedString? TitleFormatted
    {
        get => (FormattedString?)GetValue(TitleFormattedProperty);
        set => SetValue(TitleFormattedProperty, value);
    }

    public static readonly BindableProperty SubtitleFormattedProperty =
        BindableProperty.Create(nameof(SubtitleFormatted), typeof(FormattedString), typeof(PhysicsReasonCardView), null);

    public FormattedString? SubtitleFormatted
    {
        get => (FormattedString?)GetValue(SubtitleFormattedProperty);
        set => SetValue(SubtitleFormattedProperty, value);
    }

    public static readonly BindableProperty SolutionProperty =
        BindableProperty.Create(nameof(Solution), typeof(string), typeof(PhysicsReasonCardView), string.Empty);

    public string Solution
    {
        get => (string)GetValue(SolutionProperty);
        set => SetValue(SolutionProperty, value);
    }

    public static readonly BindableProperty IsExpandedProperty =
        BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(PhysicsReasonCardView), false);

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly BindableProperty ToggleExpandCommandProperty =
        BindableProperty.Create(nameof(ToggleExpandCommand), typeof(ICommand), typeof(PhysicsReasonCardView), null);

    public ICommand? ToggleExpandCommand
    {
        get => (ICommand?)GetValue(ToggleExpandCommandProperty);
        set => SetValue(ToggleExpandCommandProperty, value);
    }

    public static readonly BindableProperty SolutionHeaderProperty =
        BindableProperty.Create(nameof(SolutionHeader), typeof(string), typeof(PhysicsReasonCardView), string.Empty);

    public string SolutionHeader
    {
        get => (string)GetValue(SolutionHeaderProperty);
        set => SetValue(SolutionHeaderProperty, value);
    }

    public static readonly BindableProperty RecommendedPracticesLabelProperty =
        BindableProperty.Create(nameof(RecommendedPracticesLabel), typeof(string), typeof(PhysicsReasonCardView), string.Empty);

    public string RecommendedPracticesLabel
    {
        get => (string)GetValue(RecommendedPracticesLabelProperty);
        set => SetValue(RecommendedPracticesLabelProperty, value);
    }

    public static readonly BindableProperty SuggestedTechniquesProperty =
        BindableProperty.Create(nameof(SuggestedTechniques), typeof(IEnumerable), typeof(PhysicsReasonCardView), null);

    public IEnumerable? SuggestedTechniques
    {
        get => (IEnumerable?)GetValue(SuggestedTechniquesProperty);
        set => SetValue(SuggestedTechniquesProperty, value);
    }
}
