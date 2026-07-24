using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Profile;

public partial class ProfileWeekStoryView : ContentView
{
    public ProfileWeekStoryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty WeekDaysProperty =
        BindableProperty.Create(
            nameof(WeekDays),
            typeof(IEnumerable),
            typeof(ProfileWeekStoryView),
            null);

    public IEnumerable? WeekDays
    {
        get => (IEnumerable?)GetValue(WeekDaysProperty);
        set => SetValue(WeekDaysProperty, value);
    }

    public static readonly BindableProperty SummaryTextProperty =
        BindableProperty.Create(
            nameof(SummaryText),
            typeof(string),
            typeof(ProfileWeekStoryView),
            string.Empty,
            propertyChanged: OnTextVisibilityChanged);

    public string SummaryText
    {
        get => (string)GetValue(SummaryTextProperty);
        set => SetValue(SummaryTextProperty, value);
    }

    public static readonly BindableProperty EmptyTextProperty =
        BindableProperty.Create(
            nameof(EmptyText),
            typeof(string),
            typeof(ProfileWeekStoryView),
            string.Empty,
            propertyChanged: OnTextVisibilityChanged);

    public string EmptyText
    {
        get => (string)GetValue(EmptyTextProperty);
        set => SetValue(EmptyTextProperty, value);
    }

    public static readonly BindableProperty ShowEmptyProperty =
        BindableProperty.Create(
            nameof(ShowEmpty),
            typeof(bool),
            typeof(ProfileWeekStoryView),
            false,
            propertyChanged: OnTextVisibilityChanged);

    public bool ShowEmpty
    {
        get => (bool)GetValue(ShowEmptyProperty);
        set => SetValue(ShowEmptyProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(ProfileWeekStoryView),
            null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public bool ShowEmptyText => ShowEmpty && !string.IsNullOrWhiteSpace(EmptyText);
    public bool ShowSummaryText => !ShowEmpty && !string.IsNullOrWhiteSpace(SummaryText);

    private static void OnTextVisibilityChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ProfileWeekStoryView view)
        {
            view.OnPropertyChanged(nameof(ShowEmptyText));
            view.OnPropertyChanged(nameof(ShowSummaryText));
        }
    }
}
