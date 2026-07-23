using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Journal;

public partial class JournalEntryView : ContentView
{
    public JournalEntryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty TimeTextProperty =
        BindableProperty.Create(nameof(TimeText), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty NoteTextProperty =
        BindableProperty.Create(nameof(NoteText), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty HasNoteProperty =
        BindableProperty.Create(nameof(HasNote), typeof(bool), typeof(JournalEntryView), true);

    public static readonly BindableProperty MoodDisplayProperty =
        BindableProperty.Create(nameof(MoodDisplay), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(JournalEntryView));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(JournalEntryView));

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
    }

    public string TimeText
    {
        get => (string)GetValue(TimeTextProperty);
        set => SetValue(TimeTextProperty, value);
    }

    public string NoteText
    {
        get => (string)GetValue(NoteTextProperty);
        set => SetValue(NoteTextProperty, value);
    }

    public bool HasNote
    {
        get => (bool)GetValue(HasNoteProperty);
        set => SetValue(HasNoteProperty, value);
    }

    public string MoodDisplay
    {
        get => (string)GetValue(MoodDisplayProperty);
        set => SetValue(MoodDisplayProperty, value);
    }

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }
}
