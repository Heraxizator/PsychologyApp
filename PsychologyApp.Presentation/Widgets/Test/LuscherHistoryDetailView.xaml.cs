namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherHistoryDetailView : ContentView
{
    public LuscherHistoryDetailView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty HasStandardDetailProperty =
        BindableProperty.Create(nameof(HasStandardDetail), typeof(bool), typeof(LuscherHistoryDetailView), false);

    public static readonly BindableProperty HasBriefDetailProperty =
        BindableProperty.Create(nameof(HasBriefDetail), typeof(bool), typeof(LuscherHistoryDetailView), false);

    public static readonly BindableProperty FirstPassTitleProperty =
        BindableProperty.Create(nameof(FirstPassTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty FirstPassTextProperty =
        BindableProperty.Create(nameof(FirstPassText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty SecondPassTitleProperty =
        BindableProperty.Create(nameof(SecondPassTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty SecondPassTextProperty =
        BindableProperty.Create(nameof(SecondPassText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BkTextProperty =
        BindableProperty.Create(nameof(BkText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefFirstTitleProperty =
        BindableProperty.Create(nameof(BriefFirstTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefFirstTextProperty =
        BindableProperty.Create(nameof(BriefFirstText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefSecondTitleProperty =
        BindableProperty.Create(nameof(BriefSecondTitle), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public static readonly BindableProperty BriefSecondTextProperty =
        BindableProperty.Create(nameof(BriefSecondText), typeof(string), typeof(LuscherHistoryDetailView), string.Empty);

    public bool HasStandardDetail
    {
        get => (bool)GetValue(HasStandardDetailProperty);
        set => SetValue(HasStandardDetailProperty, value);
    }

    public bool HasBriefDetail
    {
        get => (bool)GetValue(HasBriefDetailProperty);
        set => SetValue(HasBriefDetailProperty, value);
    }

    public string FirstPassTitle
    {
        get => (string)GetValue(FirstPassTitleProperty);
        set => SetValue(FirstPassTitleProperty, value);
    }

    public string FirstPassText
    {
        get => (string)GetValue(FirstPassTextProperty);
        set => SetValue(FirstPassTextProperty, value);
    }

    public string SecondPassTitle
    {
        get => (string)GetValue(SecondPassTitleProperty);
        set => SetValue(SecondPassTitleProperty, value);
    }

    public string SecondPassText
    {
        get => (string)GetValue(SecondPassTextProperty);
        set => SetValue(SecondPassTextProperty, value);
    }

    public string BkText
    {
        get => (string)GetValue(BkTextProperty);
        set => SetValue(BkTextProperty, value);
    }

    public string BriefFirstTitle
    {
        get => (string)GetValue(BriefFirstTitleProperty);
        set => SetValue(BriefFirstTitleProperty, value);
    }

    public string BriefFirstText
    {
        get => (string)GetValue(BriefFirstTextProperty);
        set => SetValue(BriefFirstTextProperty, value);
    }

    public string BriefSecondTitle
    {
        get => (string)GetValue(BriefSecondTitleProperty);
        set => SetValue(BriefSecondTitleProperty, value);
    }

    public string BriefSecondText
    {
        get => (string)GetValue(BriefSecondTextProperty);
        set => SetValue(BriefSecondTextProperty, value);
    }
}
