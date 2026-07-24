using System.Collections;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Journal;

public partial class JournalWeekStripView : ContentView
{
    public JournalWeekStripView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(
            nameof(CaptionText),
            typeof(string),
            typeof(JournalWeekStripView),
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
            typeof(JournalWeekStripView),
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
            typeof(JournalWeekStripView),
            null);

    public ICommand? SelectDayCommand
    {
        get => (ICommand?)GetValue(SelectDayCommandProperty);
        set => SetValue(SelectDayCommandProperty, value);
    }

    public static readonly BindableProperty IsInteractiveProperty =
        BindableProperty.Create(
            nameof(IsInteractive),
            typeof(bool),
            typeof(JournalWeekStripView),
            true,
            propertyChanged: OnInteractiveChanged);

    public bool IsInteractive
    {
        get => (bool)GetValue(IsInteractiveProperty);
        set => SetValue(IsInteractiveProperty, value);
    }

    public bool IsInteractionBlocked => !IsInteractive;

    private static void OnCaptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalWeekStripView view)
        {
            view.OnPropertyChanged(nameof(HasCaptionText));
        }
    }

    private static void OnInteractiveChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalWeekStripView view)
        {
            view.OnPropertyChanged(nameof(IsInteractionBlocked));
        }
    }
}
