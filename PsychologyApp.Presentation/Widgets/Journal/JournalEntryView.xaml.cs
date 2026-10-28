using System.Windows.Input;
using PsychologyApp.Presentation.Core.Charts;
using PsychologyApp.Presentation.Shared.UI.Components;

namespace PsychologyApp.Presentation.Widgets.Journal;

public partial class JournalEntryView : ContentView
{
    public JournalEntryView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty DateTextProperty =
        BindableProperty.Create(nameof(DateText), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty NoteTextProperty =
        BindableProperty.Create(nameof(NoteText), typeof(string), typeof(JournalEntryView), string.Empty);

    public static readonly BindableProperty MoodLevelProperty =
        BindableProperty.Create(nameof(MoodLevel), typeof(int), typeof(JournalEntryView), 0, propertyChanged: OnMoodLevelChanged);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(JournalEntryView));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(JournalEntryView));

    public static readonly BindableProperty ShareCommandProperty =
        BindableProperty.Create(
            nameof(ShareCommand),
            typeof(ICommand),
            typeof(JournalEntryView),
            propertyChanged: OnShareChanged);

    public static readonly BindableProperty ShareCommandParameterProperty =
        BindableProperty.Create(nameof(ShareCommandParameter), typeof(object), typeof(JournalEntryView));

    public static readonly BindableProperty ShareDescriptionProperty =
        BindableProperty.Create(nameof(ShareDescription), typeof(string), typeof(JournalEntryView), string.Empty);

    public string DateText
    {
        get => (string)GetValue(DateTextProperty);
        set => SetValue(DateTextProperty, value);
    }

    public string NoteText
    {
        get => (string)GetValue(NoteTextProperty);
        set => SetValue(NoteTextProperty, value);
    }

    /// <summary>The mood drawn as its face on its colour, as on the check-in card.</summary>
    public int MoodLevel
    {
        get => (int)GetValue(MoodLevelProperty);
        set => SetValue(MoodLevelProperty, value);
    }

    private static void OnMoodLevelChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalEntryView view && newValue is int level and >= 1)
        {
            MoodTint.SetLevel(view.MoodTile, level);
            view.MoodIcon.IconName = MoodPalette.IconName(level);
            MoodTint.SetIconLevel(view.MoodIcon, level);
            SemanticProperties.SetDescription(view.MoodTile, AppStrings.MoodLevelName(level));
        }
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

    public ICommand? ShareCommand
    {
        get => (ICommand?)GetValue(ShareCommandProperty);
        set => SetValue(ShareCommandProperty, value);
    }

    public object? ShareCommandParameter
    {
        get => GetValue(ShareCommandParameterProperty);
        set => SetValue(ShareCommandParameterProperty, value);
    }

    public string ShareDescription
    {
        get => (string)GetValue(ShareDescriptionProperty);
        set => SetValue(ShareDescriptionProperty, value);
    }

    public bool HasShareCommand => ShareCommand is not null;

    public bool ShowChevron => !HasShareCommand;

    private static void OnShareChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is JournalEntryView view)
        {
            view.OnPropertyChanged(nameof(HasShareCommand));
            view.OnPropertyChanged(nameof(ShowChevron));
        }
    }
}
