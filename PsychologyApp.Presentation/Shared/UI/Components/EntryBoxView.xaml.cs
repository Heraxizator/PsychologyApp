namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class EntryBoxView : ContentView
{
    public EntryBoxView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(EntryBoxView), string.Empty, BindingMode.TwoWay);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BodySourceProperty =
        BindableProperty.Create(nameof(BodySource), typeof(IEnumerable<EntryItem>), typeof(EntryBoxView), default, BindingMode.TwoWay);

    public IEnumerable<EntryItem> BodySource
    {
        get => (IEnumerable<EntryItem>)GetValue(BodySourceProperty);
        set => SetValue(BodySourceProperty, value);
    }
}
