using PsychologyApp.Application.Practice;

namespace PsychologyApp.Presentation.Shared.UI.Components;

/// <summary>
/// A small emotion wheel laid flat: six coloured families; tapping one adds it to the answer and opens its finer shades below,
/// so "fear" can become "anxiety" or "panic" with a second tap. Several feelings may be picked; a second tap takes one out.
/// </summary>
public sealed class EmotionPickerView : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(EmotionPickerView), string.Empty, BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((EmotionPickerView)b).RefreshSelection());

    private readonly FlexLayout _families = new() { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap };
    private readonly FlexLayout _shades = new() { Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap, IsVisible = false };
    private readonly Label _hint = new() { Style = AppResources.Find<Style>("CaptionStyle") };
    private readonly List<(string Word, Border Chip, Label Label, EmotionFamily Family)> _chips = [];
    private EmotionFamily? _openFamily;

    public EmotionPickerView()
    {
        Content = new VerticalStackLayout { Spacing = 8, Children = { _hint, _families, _shades } };
        Loaded += (_, _) => Build();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static bool IsDark => Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;

    private void Build()
    {
        _hint.Text = AppStrings.PracticeEmotionHint;
        _families.Children.Clear();
        _chips.Clear();
        foreach (EmotionGroup group in EmotionPalette.For(AppStrings.Language))
        {
            _families.Children.Add(CreateChip(group.Name, group.Family, isFamily: true, () => OnFamilyTapped(group)));
        }

        if (_openFamily is { } open)
        {
            ShowShades(open);
        }

        RefreshSelection();
    }

    private void OnFamilyTapped(EmotionGroup group)
    {
        Text = AnswerText.Toggle(Text, group.Name);
        ShowShades(group.Family);
        UiHaptics.Tick();
    }

    private void ShowShades(EmotionFamily family)
    {
        _openFamily = family;
        EmotionGroup? group = EmotionPalette.For(AppStrings.Language).FirstOrDefault(g => g.Family == family);
        _shades.Children.Clear();
        _chips.RemoveAll(c => !_families.Children.Contains(c.Chip));
        if (group is null)
        {
            _shades.IsVisible = false;
            return;
        }

        foreach (string shade in group.Shades)
        {
            _shades.Children.Add(CreateChip(shade, family, isFamily: false, () =>
            {
                Text = AnswerText.Toggle(Text, shade);
                UiHaptics.Tick();
            }));
        }

        _shades.IsVisible = true;
        if (UiAnimations.ShouldAnimate(_shades))
        {
            _shades.Opacity = 0;
            _shades.TranslationY = -6;
            _shades.FadeToAsync(1, UiAnimations.MicroDuration, Easing.CubicOut).FireAndForget();
            _shades.TranslateToAsync(0, 0, UiAnimations.MicroDuration, Easing.CubicOut).FireAndForget();
        }

        RefreshSelection();
    }

    private Border CreateChip(string word, EmotionFamily family, bool isFamily, Action onTap)
    {
        (string fill, string text) = EmotionColors.For(family, IsDark);
        Label label = new()
        {
            Text = word,
            Style = AppResources.Find<Style>(isFamily ? "BodyStyle" : "CaptionStyle"),
            TextColor = Color.FromArgb(text),
            VerticalOptions = LayoutOptions.Center
        };
        ApplyFont(label, isFamily ? "SemiBoldFontFamily" : "MediumFontFamily");

        Border chip = new()
        {
            Margin = new Thickness(0, 0, 6, 6),
            Padding = isFamily ? new Thickness(14, 7) : new Thickness(11, 5),
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 18 },
            BackgroundColor = isFamily ? Color.FromArgb(fill) : Colors.Transparent,
            Stroke = new SolidColorBrush(Color.FromArgb(isFamily ? fill : text)),
            StrokeThickness = 1.2,
            Content = label
        };
        FlexLayout.SetShrink(chip, 0);
        SemanticProperties.SetDescription(chip, word);
        chip.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(onTap) });
        _chips.Add((word, chip, label, family));
        return chip;
    }

    private void RefreshSelection()
    {
        foreach ((string word, Border chip, Label label, EmotionFamily family) in _chips)
        {
            bool selected = AnswerText.Contains(Text, word);
            (string fill, string text) = EmotionColors.For(family, IsDark);
            bool isFamily = _families.Children.Contains(chip);
            chip.StrokeThickness = selected ? 2.4 : 1.2;
            chip.Stroke = new SolidColorBrush(Color.FromArgb(selected ? text : isFamily ? fill : text));
            if (!isFamily)
            {
                chip.BackgroundColor = selected ? Color.FromArgb(fill) : Colors.Transparent;
            }

            ApplyFont(label, selected || isFamily ? "SemiBoldFontFamily" : "MediumFontFamily");
        }
    }

    private static void ApplyFont(Label label, string key) =>
        label.FontFamily = AppResources.Find<string>(key) ?? label.FontFamily;
}
