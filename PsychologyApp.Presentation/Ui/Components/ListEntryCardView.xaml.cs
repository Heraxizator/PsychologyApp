using PsychologyApp.Presentation.Infrastructure;

namespace PsychologyApp.Presentation.UI.Components;

public partial class ListEntryCardView : ContentView
{
    public ListEntryCardView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(ListEntryCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty BodyProperty =
        BindableProperty.Create(nameof(Body), typeof(string), typeof(ListEntryCardView), string.Empty);

    public string Body
    {
        get => (string)GetValue(BodyProperty);
        set => SetValue(BodyProperty, value);
    }
}
