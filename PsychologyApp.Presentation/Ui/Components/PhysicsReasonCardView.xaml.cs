using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class PhysicsReasonCardView : ContentView
{
    public PhysicsReasonCardView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(PhysicsReasonCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(PhysicsReasonCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
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
