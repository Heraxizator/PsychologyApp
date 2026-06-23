namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class AlgorithmBoxView : ContentView
{
    public AlgorithmBoxView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(AlgorithmBoxView), string.Empty, BindingMode.TwoWay);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BodySourceProperty =
        BindableProperty.Create(
            nameof(BodySource),
            typeof(IEnumerable<string>),
            typeof(AlgorithmBoxView),
            default,
            BindingMode.TwoWay,
            propertyChanged: OnDisplayModeChanged);

    public IEnumerable<string> BodySource
    {
        get => (IEnumerable<string>)GetValue(BodySourceProperty);
        set => SetValue(BodySourceProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable<object>),
            typeof(AlgorithmBoxView),
            default,
            BindingMode.TwoWay,
            propertyChanged: OnDisplayModeChanged);

    public IEnumerable<object>? ItemsSource
    {
        get => (IEnumerable<object>?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(AlgorithmBoxView),
            default,
            BindingMode.TwoWay,
            propertyChanged: OnDisplayModeChanged);

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly BindableProperty UseItemTemplateProperty =
        BindableProperty.Create(nameof(UseItemTemplate), typeof(bool), typeof(AlgorithmBoxView), false);

    public bool UseItemTemplate
    {
        get => (bool)GetValue(UseItemTemplateProperty);
        private set => SetValue(UseItemTemplateProperty, value);
    }

    public static readonly BindableProperty UseStringItemsProperty =
        BindableProperty.Create(nameof(UseStringItems), typeof(bool), typeof(AlgorithmBoxView), true);

    public bool UseStringItems
    {
        get => (bool)GetValue(UseStringItemsProperty);
        private set => SetValue(UseStringItemsProperty, value);
    }

    private static void OnDisplayModeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not AlgorithmBoxView view)
        {
            return;
        }

        bool useTemplate = view.ItemTemplate is not null && view.ItemsSource is not null;
        view.UseItemTemplate = useTemplate;
        view.UseStringItems = !useTemplate;
    }
}
