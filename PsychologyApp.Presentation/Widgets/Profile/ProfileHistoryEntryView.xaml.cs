namespace PsychologyApp.Presentation.Widgets.Profile;

using System.Windows.Input;

public partial class ProfileHistoryEntryView : ContentView
{
    public ProfileHistoryEntryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(ProfileHistoryEntryView), string.Empty);

    public static readonly BindableProperty TechniqueNameProperty =
        BindableProperty.Create(nameof(TechniqueName), typeof(string), typeof(ProfileHistoryEntryView), string.Empty);

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(ProfileHistoryEntryView), string.Empty);

    public static readonly BindableProperty DurationTextProperty =
        BindableProperty.Create(nameof(DurationText), typeof(string), typeof(ProfileHistoryEntryView), string.Empty);

    public static readonly BindableProperty HasDurationProperty =
        BindableProperty.Create(nameof(HasDuration), typeof(bool), typeof(ProfileHistoryEntryView), false);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ProfileHistoryEntryView), null);

    public static readonly BindableProperty SudsDeltaTextProperty =
        BindableProperty.Create(nameof(SudsDeltaText), typeof(string), typeof(ProfileHistoryEntryView), string.Empty);

    public static readonly BindableProperty HasSudsDeltaProperty =
        BindableProperty.Create(nameof(HasSudsDelta), typeof(bool), typeof(ProfileHistoryEntryView), false);

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
    }

    public string TechniqueName
    {
        get => (string)GetValue(TechniqueNameProperty);
        set => SetValue(TechniqueNameProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public string DurationText
    {
        get => (string)GetValue(DurationTextProperty);
        set => SetValue(DurationTextProperty, value);
    }

    public bool HasDuration
    {
        get => (bool)GetValue(HasDurationProperty);
        set => SetValue(HasDurationProperty, value);
    }

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public string SudsDeltaText
    {
        get => (string)GetValue(SudsDeltaTextProperty);
        set => SetValue(SudsDeltaTextProperty, value);
    }

    public bool HasSudsDelta
    {
        get => (bool)GetValue(HasSudsDeltaProperty);
        set => SetValue(HasSudsDeltaProperty, value);
    }
}
