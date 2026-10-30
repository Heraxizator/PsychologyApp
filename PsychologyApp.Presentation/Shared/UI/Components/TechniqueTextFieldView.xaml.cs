using PsychologyApp.Domain.Practice;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Services.Preferences;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// The text answer of a practice step. Typing is the last resort: typical answers are chips (one tap adds, another takes out),
/// the problem from the last practice comes back with one tap, feelings are picked from a palette, kind or balanced phrases are offered
/// ready-made, and the microphone dictates on the device.
/// </summary>
public partial class TechniqueTextFieldView : ContentView
{
    public static readonly BindableProperty TitleTextProperty = BindableProperty.Create(
        nameof(TitleText), typeof(string), typeof(TechniqueTextFieldView), string.Empty,
        propertyChanged: (b, _, n) =>
        {
            var field = (TechniqueTextFieldView)b;
            field.TitleLabel.IsVisible = !string.IsNullOrWhiteSpace(n as string);
            field.BuildChips();
        });

    public static readonly BindableProperty PlaceholderTextProperty = BindableProperty.Create(
        nameof(PlaceholderText), typeof(string), typeof(TechniqueTextFieldView), string.Empty);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(TechniqueTextFieldView), string.Empty, BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((TechniqueTextFieldView)b).OnTextChanged());

    public static readonly BindableProperty SuggestionsProperty = BindableProperty.Create(
        nameof(Suggestions), typeof(IReadOnlyList<string>), typeof(TechniqueTextFieldView), null,
        propertyChanged: (b, _, _) => ((TechniqueTextFieldView)b).BuildChips());

    public static readonly BindableProperty AssistSuggestionsProperty = BindableProperty.Create(
        nameof(AssistSuggestions), typeof(IReadOnlyList<string>), typeof(TechniqueTextFieldView), null,
        propertyChanged: (b, _, _) => ((TechniqueTextFieldView)b).BuildAssist());

    public static readonly BindableProperty RecentTextProperty = BindableProperty.Create(
        nameof(RecentText), typeof(string), typeof(TechniqueTextFieldView), string.Empty,
        propertyChanged: (b, _, _) => ((TechniqueTextFieldView)b).RefreshRecent());

    public static readonly BindableProperty RoleProperty = BindableProperty.Create(
        nameof(Role), typeof(EntryFieldRole), typeof(TechniqueTextFieldView), EntryFieldRole.None,
        propertyChanged: (b, _, _) => ((TechniqueTextFieldView)b).RefreshRole());

    private readonly VoiceDictation _dictation;
    private readonly List<(string Chip, Border View, Label Label)> _chips = [];
    private readonly List<(string Phrase, Border View)> _phrases = [];

    public TechniqueTextFieldView()
    {
        InitializeComponent();
        _dictation = new VoiceDictation(() => Text ?? string.Empty, value => Text = value);
        _dictation.Changed += (_, _) => RefreshVoice();
        _dictation.Attach(MicButton, InputEditor.Unfocus);
        MicButton.IsVisible = VoiceDictation.IsAvailable;
        SemanticProperties.SetDescription(MicButton, AppStrings.PracticeVoiceStart);
        ChipsHost.SizeChanged += (_, _) => UpdateChipWidths();
        AssistTitle.Text = AppStrings.PracticeAssistTitle;
        TitleLabel.IsVisible = false;
        Unloaded += (_, _) => _dictation.StopAsync().FireAndForget();
        Loaded += (_, _) => RankWhenLoadedAsync().FireAndForget();
    }

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string PlaceholderText
    {
        get => (string)GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Typical short answers shown as chips.</summary>
    public IReadOnlyList<string>? Suggestions
    {
        get => (IReadOnlyList<string>?)GetValue(SuggestionsProperty);
        set => SetValue(SuggestionsProperty, value);
    }

    /// <summary>Longer ready phrases (balanced thoughts, kind words) shown as cards.</summary>
    public IReadOnlyList<string>? AssistSuggestions
    {
        get => (IReadOnlyList<string>?)GetValue(AssistSuggestionsProperty);
        set => SetValue(AssistSuggestionsProperty, value);
    }

    /// <summary>The last problem worked on; offered while the field is empty.</summary>
    public string RecentText
    {
        get => (string)GetValue(RecentTextProperty);
        set => SetValue(RecentTextProperty, value);
    }

    public EntryFieldRole Role
    {
        get => (EntryFieldRole)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    private void OnTextChanged()
    {
        if (EmotionPicker.Text != Text)
        {
            EmotionPicker.Text = Text;
        }

        RefreshChipSelection();
        RefreshRecent();
    }

    private void RefreshRole()
    {
        bool emotion = Role == EntryFieldRole.Emotion;
        EmotionPicker.IsVisible = emotion;
        if (emotion)
        {
            EmotionPicker.Text = Text;
            EmotionPicker.PropertyChanged -= OnEmotionPickerChanged;
            EmotionPicker.PropertyChanged += OnEmotionPickerChanged;
        }
    }

    private void OnEmotionPickerChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EmotionPickerView.Text) && EmotionPicker.Text != Text)
        {
            Text = EmotionPicker.Text;
        }
    }

    private void BuildChips()
    {
        ChipsHost.Children.Clear();
        _chips.Clear();
        _chipsExpanded = false;
        ChipUsageStore? usage = ChipUsage;
        IReadOnlyList<string> offered = Suggestions ?? [];
        IReadOnlyList<string> ordered = usage is { IsLoaded: true } ? PracticeAnswers.Rank(offered, usage.Counts) : offered;

        // What the person keeps typing here ("Cat" among the things they see) comes first, as a chip of their own.
        IReadOnlyList<string> own = usage?.OwnChips(TitleText, offered) ?? [];
        foreach (string chip in own.Concat(ordered))
        {
            Label label = new() { Text = chip, Style = AppResources.Find<Style>("AnswerChipLabelStyle") };
            Border view = new() { Style = AppResources.Find<Style>("AnswerChipStyle"), Content = label };
            FlexLayout.SetShrink(view, 0);
            label.LineBreakMode = LineBreakMode.WordWrap;
            SemanticProperties.SetDescription(view, chip);
            view.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnChipTapped(chip, view)) });
            ChipsHost.Children.Add(view);
            _chips.Add((chip, view, label));
        }

        _moreLabel = new Label { Style = AppResources.Find<Style>("AnswerChipLabelStyle"), TextColor = AppResources.ThemedColor("PrimaryTextLight", "PrimaryTextDark", Colors.Gray) };
        _moreChip = new Border { Style = AppResources.Find<Style>("AnswerChipStyle"), Content = _moreLabel, BackgroundColor = Colors.Transparent, StrokeThickness = 1, Stroke = new SolidColorBrush(AppResources.ThemedColor("Primary", "PrimaryTextDark", Colors.Gray)) };
        FlexLayout.SetShrink(_moreChip, 0);
        _moreChip.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(ExpandChips) });
        ChipsHost.Children.Add(_moreChip);

        ChipsHost.IsVisible = _chips.Count > 0;
        UpdateChipWidths();
        RefreshChipSelection();
    }

    private void UpdateChipWidths()
    {
        if (ChipsHost.Width <= 0)
            return;

        // Leave room for each chip's trailing FlexLayout margin and wrap its text to the actual viewport.
        double maximumWidth = Math.Min(320, Math.Max(0, ChipsHost.Width - 6));
        foreach (var chip in _chips)
        {
            chip.View.MaximumWidthRequest = maximumWidth;
            chip.Label.MaximumWidthRequest = maximumWidth;
        }
    }

    /// <summary>
    /// How many typical answers show at first; the rest wait behind a "More +N" chip. Eight filled a screen of chips under one
    /// question, louder than the question itself.
    /// </summary>
    private const int ChipsShownFirst = 4;

    private bool _chipsExpanded;
    private Border? _moreChip;
    private Label? _moreLabel;

    /// <summary>The first few chips, every chosen one, and all of them once "More" was tapped.</summary>
    private void ApplyChipVisibility()
    {
        int hidden = 0;
        for (int i = 0; i < _chips.Count; i++)
        {
            bool show = _chipsExpanded || i < ChipsShownFirst || AnswerText.Contains(Text, _chips[i].Chip);
            _chips[i].View.IsVisible = show;
            hidden += show ? 0 : 1;
        }

        if (_moreChip is not null && _moreLabel is not null)
        {
            _moreChip.IsVisible = hidden > 0;
            _moreLabel.Text = AppStrings.PracticeMoreChips(hidden);
            SemanticProperties.SetDescription(_moreChip, _moreLabel.Text);
        }
    }

    private void ExpandChips()
    {
        _chipsExpanded = true;
        UiHaptics.Tick();
        ApplyChipVisibility();
    }

    private void OnChipTapped(string chip, Border view)
    {
        Text = AnswerText.Toggle(Text, chip);
        if (AnswerText.Contains(Text, chip))
        {
            ChipUsage?.Used(chip);
        }

        UiHaptics.Tick();
        if (UiAnimations.ShouldAnimate(view))
        {
            view.Scale = 0.92;
            view.ScaleToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing).FireAndForget();
        }
    }

    private void RefreshChipSelection()
    {
        foreach ((string chip, Border view, Label label) in _chips)
        {
            bool selected = AnswerText.Contains(Text, chip);
            view.Style = AppResources.Find<Style>(selected ? "AnswerChipSelectedStyle" : "AnswerChipStyle");
            label.Style = AppResources.Find<Style>(selected ? "AnswerChipSelectedLabelStyle" : "AnswerChipLabelStyle");
            label.Text = selected ? $"✓  {chip}" : chip;
        }

        ApplyChipVisibility();

        foreach ((string phrase, Border view) in _phrases)
        {
            bool used = (Text ?? string.Empty).Contains(phrase, StringComparison.OrdinalIgnoreCase);
            view.StrokeThickness = used ? 1.8 : 1;
            view.Stroke = used ? new SolidColorBrush(AppResources.ThemedColor("Primary", "Primary", Colors.Gray)) : new SolidColorBrush(AppResources.ThemedColor("NeutralBorder", "InputBorderDark", Colors.Gray));
        }
    }

    private void BuildAssist()
    {
        AssistHost.Children.Clear();
        _phrases.Clear();
        foreach (string phrase in AssistSuggestions ?? [])
        {
            Label label = new()
            {
                Text = phrase,
                Style = AppResources.Find<Style>("BodyStyle"),
                LineBreakMode = LineBreakMode.WordWrap
            };
            Border view = new()
            {
                Padding = new Thickness(14, 10),
                StrokeThickness = 1,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                BackgroundColor = AppResources.ThemedColor("SurfaceLight", "SurfaceDark", Colors.Gray),
                Content = label
            };
            SemanticProperties.SetDescription(view, phrase);
            view.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => OnPhraseTapped(phrase, view)) });
            AssistHost.Children.Add(view);
            _phrases.Add((phrase, view));
            if (UiAnimations.ShouldAnimate(AssistHost))
            {
                view.Opacity = 0;
                view.FadeToAsync(1, UiAnimations.MediumDuration, Easing.CubicOut).FireAndForget();
            }
        }

        AssistSection.IsVisible = _phrases.Count > 0;
        RefreshChipSelection();
    }

    private void OnPhraseTapped(string phrase, Border view)
    {
        string current = Text ?? string.Empty;
        if (current.Contains(phrase, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        // A ready phrase is a start to adjust, so it goes into the field rather than replacing what was written.
        Text = string.IsNullOrWhiteSpace(current) ? phrase : $"{current.TrimEnd()} {phrase}";
        UiHaptics.Tick();
        if (UiAnimations.ShouldAnimate(view))
        {
            view.Scale = 0.97;
            view.ScaleToAsync(1, UiAnimations.ReleaseDuration, UiAnimations.ReleaseEasing).FireAndForget();
        }
    }

    private void RefreshRecent()
    {
        bool show = !string.IsNullOrWhiteSpace(RecentText) && string.IsNullOrWhiteSpace(Text);
        RecentLabel.Text = show ? AppStrings.PracticeRecentProblem(AnswerText.Shorten(RecentText, 36)) : string.Empty;
        RecentChip.IsVisible = show;
        if (show)
        {
            SemanticProperties.SetDescription(RecentChip, AppStrings.PracticeRecentProblem(RecentText));
        }
    }

    private void OnRecentTapped(object? sender, TappedEventArgs e)
    {
        Text = RecentText;
        UiHaptics.Tick();
    }

    private void RefreshVoice()
    {
        bool listening = _dictation.IsListening;
        VoiceStatusLabel.Text = _dictation.StatusText;
        VoiceStatusLabel.IsVisible = !string.IsNullOrWhiteSpace(_dictation.StatusText);
        MicButton.BackgroundColor = listening ? AppResources.ThemedColor("Danger", "Danger", Colors.Gray) : AppResources.ThemedColor("PrimaryTint", "PrimaryTintDark", Colors.Gray);
        MicIcon.TextColor = listening ? Colors.White : AppResources.ThemedColor("PrimaryTextLight", "PrimaryTextDark", Colors.Gray);
        SemanticProperties.SetDescription(MicButton, listening ? AppStrings.PracticeVoiceStop : AppStrings.PracticeVoiceStart);

        if (listening && UiAnimations.ShouldAnimate(MicButton))
        {
            PulseAsync().FireAndForget();
        }
    }

    /// <summary>The microphone breathes while it listens, so it is clear that the phone is waiting for words.</summary>
    private async Task PulseAsync()
    {
        while (_dictation.IsListening && UiAnimations.CanAnimate(MicButton))
        {
            await MicButton.ScaleToAsync(1.12, 420, Easing.SinInOut);
            await MicButton.ScaleToAsync(1, 420, Easing.SinInOut);
        }

        MicButton.Scale = 1;
    }

    private void OnInputFocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyFocusAsync(InputBorder).FireAndForget();

    private void OnInputUnfocused(object? sender, FocusEventArgs e) =>
        InputFieldChrome.ApplyBlurAsync(InputBorder).FireAndForget();

    private static ChipUsageStore? ChipUsage =>
        Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services.GetService<ChipUsageStore>();

    /// <summary>The usage counts come from the database once; until then the chips keep their written order, then they are re-laid.</summary>
    private async Task RankWhenLoadedAsync()
    {
        if (ChipUsage is not { } usage || usage.IsLoaded)
        {
            return;
        }

        await usage.EnsureLoadedAsync();
        BuildChips();
    }
}
