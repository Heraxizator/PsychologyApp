using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Journal;

public partial class JournalYearHeatmapView : ContentView
{
    public JournalYearHeatmapView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(
            nameof(CaptionText),
            typeof(string),
            typeof(JournalYearHeatmapView),
            string.Empty,
            propertyChanged: OnCaptionChanged);

    public string CaptionText
    {
        get => (string)GetValue(CaptionTextProperty);
        set => SetValue(CaptionTextProperty, value);
    }

    public bool HasCaptionText => !string.IsNullOrWhiteSpace(CaptionText);

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(JournalYearHeatmapView),
            null);

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty SelectDayCommandProperty =
        BindableProperty.Create(
            nameof(SelectDayCommand),
            typeof(ICommand),
            typeof(JournalYearHeatmapView),
            null);

    public ICommand? SelectDayCommand
    {
        get => (ICommand?)GetValue(SelectDayCommandProperty);
        set => SetValue(SelectDayCommandProperty, value);
    }

    private static void OnCaptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalYearHeatmapView view)
        {
            view.OnPropertyChanged(nameof(HasCaptionText));
        }
    }
}
