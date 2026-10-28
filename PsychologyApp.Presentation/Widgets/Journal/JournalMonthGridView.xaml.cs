using System.Collections;
using System.Globalization;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Journal;

public partial class JournalMonthGridView : ContentView
{
    public JournalMonthGridView()
    {
        InitializeComponent();
        Loaded += (_, _) => FillWeekdays();
    }

    /// <summary>In the app's language, read when the grid is shown, so a language switch is picked up the next time.</summary>
    private void FillWeekdays()
    {
        Label[] labels = [Weekday0, Weekday1, Weekday2, Weekday3, Weekday4, Weekday5, Weekday6];
        string[] names = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        for (int i = 0; i < labels.Length; i++)
        {
            // AbbreviatedDayNames starts on Sunday; the cells start on Monday.
            labels[i].Text = names[(i + 1) % 7];
        }
    }

    public static readonly BindableProperty CaptionTextProperty =
        BindableProperty.Create(
            nameof(CaptionText),
            typeof(string),
            typeof(JournalMonthGridView),
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
            typeof(JournalMonthGridView),
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
            typeof(JournalMonthGridView),
            null);

    public ICommand? SelectDayCommand
    {
        get => (ICommand?)GetValue(SelectDayCommandProperty);
        set => SetValue(SelectDayCommandProperty, value);
    }

    private static void OnCaptionChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalMonthGridView view)
        {
            view.OnPropertyChanged(nameof(HasCaptionText));
        }
    }
}
