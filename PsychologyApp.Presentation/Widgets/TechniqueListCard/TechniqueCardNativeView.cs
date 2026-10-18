namespace PsychologyApp.Presentation.Widgets.TechniqueListCard;

/// <summary>
/// The practice card of the catalogue, drawn on Android by one plain native view (Platforms/Android/TechniqueCardHandler). The XAML card
/// (<see cref="TechniqueListCardView"/>) is about 17 native views with a MAUI handler each; building them while a list scrolls was what
/// made it stutter. This view carries the data and the tap command; the native view plays the press feedback and the reveal animation stays on this view.
/// Other platforms keep the XAML card.
/// </summary>
public sealed class TechniqueCardNativeView : View
{
    public static readonly BindableProperty TitleProperty = Text(nameof(Title));
    public static readonly BindableProperty SubtitleProperty = Text(nameof(Subtitle));
    public static readonly BindableProperty MetaTextProperty = Text(nameof(MetaText));
    public static readonly BindableProperty DateProperty = Text(nameof(Date));
    public static readonly BindableProperty AuthorProperty = Text(nameof(Author));
    public static readonly BindableProperty IconNameProperty = Text(nameof(IconName));
    public static readonly BindableProperty ImageProperty = Text(nameof(Image));

    /// <summary>A <c>TechniqueFlavor</c> name; colours the icon tile.</summary>
    public static readonly BindableProperty FlavorProperty = Text(nameof(Flavor));

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public string MetaText { get => (string)GetValue(MetaTextProperty); set => SetValue(MetaTextProperty, value); }
    public string Date { get => (string)GetValue(DateProperty); set => SetValue(DateProperty, value); }
    public string Author { get => (string)GetValue(AuthorProperty); set => SetValue(AuthorProperty, value); }
    public string IconName { get => (string)GetValue(IconNameProperty); set => SetValue(IconNameProperty, value); }
    public string Image { get => (string)GetValue(ImageProperty); set => SetValue(ImageProperty, value); }
    public string Flavor { get => (string)GetValue(FlavorProperty); set => SetValue(FlavorProperty, value); }

    /// <summary>
    /// Runs on a tap. The native view takes the click itself: cheaper than a MAUI tap and pointer recognizer on every card, and Android
    /// clears the pressed state when the finger starts scrolling.
    /// </summary>
    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(System.Windows.Input.ICommand), typeof(TechniqueCardNativeView));

    public System.Windows.Input.ICommand? TapCommand
    {
        get => (System.Windows.Input.ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private static BindableProperty Text(string name) =>
        BindableProperty.Create(name, typeof(string), typeof(TechniqueCardNativeView), string.Empty);
}
