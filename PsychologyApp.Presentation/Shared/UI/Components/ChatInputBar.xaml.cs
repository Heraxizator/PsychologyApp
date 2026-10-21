using System.Windows.Input;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// The bar at the bottom of every chat: a rounded field with the send button inside. The main chat and the dialogue inside a practice both use it,
/// so they cannot drift apart in look.
/// </summary>
public partial class ChatInputBar : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(ChatInputBar), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(ChatInputBar), string.Empty);

    public static readonly BindableProperty SendCommandProperty = BindableProperty.Create(
        nameof(SendCommand), typeof(ICommand), typeof(ChatInputBar));

    /// <summary>There is something to send: the button is filled with the accent instead of a soft tint.</summary>
    public static readonly BindableProperty CanSendProperty = BindableProperty.Create(
        nameof(CanSend), typeof(bool), typeof(ChatInputBar), false);

    public static readonly BindableProperty SendDescriptionProperty = BindableProperty.Create(
        nameof(SendDescription), typeof(string), typeof(ChatInputBar), string.Empty);

    public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
        nameof(MaxLength), typeof(int), typeof(ChatInputBar), 1500);

    private readonly VoiceDictation _dictation;

    public ChatInputBar()
    {
        InitializeComponent();
        CenterInputText();
        _dictation = new VoiceDictation(() => Text ?? string.Empty, value => Text = value);
        _dictation.Changed += (_, _) => RefreshVoice();
        _dictation.Attach(MicButton, InputEditor.Unfocus);
        MicButton.IsVisible = VoiceDictation.IsAvailable;
        SemanticProperties.SetDescription(MicButton, AppStrings.PracticeVoiceStart);
        Unloaded += (_, _) => _dictation.StopAsync().FireAndForget();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public ICommand? SendCommand
    {
        get => (ICommand?)GetValue(SendCommandProperty);
        set => SetValue(SendCommandProperty, value);
    }

    public bool CanSend
    {
        get => (bool)GetValue(CanSendProperty);
        set => SetValue(CanSendProperty, value);
    }

    public string SendDescription
    {
        get => (string)GetValue(SendDescriptionProperty);
        set => SetValue(SendDescriptionProperty, value);
    }

    public int MaxLength
    {
        get => (int)GetValue(MaxLengthProperty);
        set => SetValue(MaxLengthProperty, value);
    }

    /// <summary>The text field, for the owner to set the keyboard on.</summary>
    public Editor Input => InputEditor;

    private void OnSendTapped(object? sender, TappedEventArgs e)
    {
        if (!CanSend)
        {
            return;
        }

        UiHaptics.Tick();
        if (sender is VisualElement button)
        {
            UiAnimations.SafePulseAsync(button).FireAndForget();
        }
    }

    private void RefreshVoice()
    {
        bool listening = _dictation.IsListening;
        VoiceStatusLabel.Text = _dictation.StatusText;
        VoiceStatusLabel.IsVisible = !string.IsNullOrWhiteSpace(_dictation.StatusText);
        MicButton.BackgroundColor = listening ? AppResources.Color("Danger", Colors.Gray) : Colors.Transparent;
        if (listening)
        {
            MicIcon.TextColor = Colors.White;
        }
        else
        {
            MicIcon.SetAppThemeColor(Label.TextColorProperty, AppResources.Color("PrimaryTextLight", Colors.Gray), AppResources.Color("PrimaryTextDark", Colors.Gray));
        }

        SemanticProperties.SetDescription(MicButton, listening ? AppStrings.PracticeVoiceStop : AppStrings.PracticeVoiceStart);
    }

    /// <summary>
    /// Android puts the text of a multi-line editor at its top edge, which left the words above the middle of the bar and the send button.
    /// Centred vertically, with the editor's own padding removed, a single line sits on the same line as the button and a long message still grows the bar.
    /// </summary>
    private void CenterInputText()
    {
#if ANDROID
        InputEditor.HandlerChanged += (_, _) =>
        {
            if (InputEditor.Handler?.PlatformView is Android.Widget.EditText editText)
            {
                editText.Gravity = Android.Views.GravityFlags.CenterVertical | Android.Views.GravityFlags.Start;
                editText.SetPadding(0, 0, 0, 0);
            }
        };
#endif
    }
}
