namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class AlgorithmBoxView : ContentView
{
    public AlgorithmBoxView()
    {
        InitializeComponent();
        ApplyBadgeColours();
        Loaded += (_, _) =>
        {
            ApplyBadgeColours();
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged += OnThemeChanged;
            }
        };
        Unloaded += (_, _) =>
        {
            if (Microsoft.Maui.Controls.Application.Current is { } app)
            {
                app.RequestedThemeChanged -= OnThemeChanged;
            }
        };
    }

    /// <summary>Colours of the step badges chosen by whoever owns the page (a practice group); unset, the badges use the accent.</summary>
    public static readonly BindableProperty BadgeTintProperty =
        BindableProperty.Create(nameof(BadgeTint), typeof(Color), typeof(AlgorithmBoxView), null, propertyChanged: (b, _, _) => ((AlgorithmBoxView)b).ApplyBadgeColours());

    public static readonly BindableProperty BadgeInkProperty =
        BindableProperty.Create(nameof(BadgeInk), typeof(Color), typeof(AlgorithmBoxView), null, propertyChanged: (b, _, _) => ((AlgorithmBoxView)b).ApplyBadgeColours());

    public Color? BadgeTint
    {
        get => (Color?)GetValue(BadgeTintProperty);
        set => SetValue(BadgeTintProperty, value);
    }

    public Color? BadgeInk
    {
        get => (Color?)GetValue(BadgeInkProperty);
        set => SetValue(BadgeInkProperty, value);
    }

    public static readonly BindableProperty ShownBadgeTintProperty =
        BindableProperty.Create(nameof(ShownBadgeTint), typeof(Color), typeof(AlgorithmBoxView), Colors.Transparent);

    public static readonly BindableProperty ShownBadgeInkProperty =
        BindableProperty.Create(nameof(ShownBadgeInk), typeof(Color), typeof(AlgorithmBoxView), Colors.Black);

    public Color ShownBadgeTint
    {
        get => (Color)GetValue(ShownBadgeTintProperty);
        private set => SetValue(ShownBadgeTintProperty, value);
    }

    public Color ShownBadgeInk
    {
        get => (Color)GetValue(ShownBadgeInkProperty);
        private set => SetValue(ShownBadgeInkProperty, value);
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => Dispatcher.Dispatch(ApplyBadgeColours);

    private void ApplyBadgeColours()
    {
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        ShownBadgeTint = BadgeTint ?? Resource(dark ? "PrimaryTintDark" : "PrimaryTint", Colors.LightSkyBlue);
        ShownBadgeInk = BadgeInk ?? Resource(dark ? "PrimaryTextDark" : "PrimaryTextLight", Colors.DodgerBlue);
    }

    private static Color Resource(string key, Color fallback) =>
        Microsoft.Maui.Controls.Application.Current?.Resources.TryGetValue(key, out object? value) == true && value is Color c ? c : fallback;

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
