using PsychologyApp.Presentation.Features.RunTechniqueSession;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.TestListCard;

public partial class TestListCardView : ContentView
{
    public TestListCardView()
    {
        InitializeComponent();
        ApplyLook();
        Loaded += (_, _) =>
        {
            ApplyLook();
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

    /// <summary>Which test the card shows; picks its icon and tile colours (<see cref="TestCardLook"/>).</summary>
    public static readonly BindableProperty TestIdProperty =
        BindableProperty.Create(nameof(TestId), typeof(string), typeof(TestListCardView), string.Empty, propertyChanged: (b, _, _) => ((TestListCardView)b).ApplyLook());

    public string TestId
    {
        get => (string)GetValue(TestIdProperty);
        set => SetValue(TestIdProperty, value);
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(TestListCardView), TestCardLook.DefaultIcon);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        private set => SetValue(IconNameProperty, value);
    }

    public static readonly BindableProperty TileColorProperty =
        BindableProperty.Create(nameof(TileColor), typeof(Color), typeof(TestListCardView), Colors.Transparent);

    public Color TileColor
    {
        get => (Color)GetValue(TileColorProperty);
        private set => SetValue(TileColorProperty, value);
    }

    public static readonly BindableProperty IconTintProperty =
        BindableProperty.Create(nameof(IconTint), typeof(Color), typeof(TestListCardView), Colors.Transparent);

    public Color IconTint
    {
        get => (Color)GetValue(IconTintProperty);
        private set => SetValue(IconTintProperty, value);
    }

    private void OnThemeChanged(object? sender, AppThemeChangedEventArgs e) => Dispatcher.Dispatch(ApplyLook);

    /// <summary>The colours for the current theme; a test with no group keeps the accent tile it always had.</summary>
    private void ApplyLook()
    {
        (string icon, TechniqueFlavor flavor) = TestCardLook.For(TestId);
        IconName = icon;
        bool dark = Microsoft.Maui.Controls.Application.Current?.RequestedTheme == AppTheme.Dark;
        if (TechniqueFlavors.Colors(flavor, dark) is { } colours)
        {
            TileColor = Color.FromArgb(colours.Tile);
            IconTint = Color.FromArgb(colours.Icon);
            return;
        }

        TileColor = AppResources.ThemedColor("PrimaryTint", "PrimaryTintDark", Colors.Transparent);
        IconTint = AppResources.Color("Primary", Colors.DodgerBlue);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TestListCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TestListCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty MetaTextProperty =
        BindableProperty.Create(nameof(MetaText), typeof(string), typeof(TestListCardView), string.Empty, propertyChanged: OnMetaTextChanged);

    public string MetaText
    {
        get => (string)GetValue(MetaTextProperty);
        set => SetValue(MetaTextProperty, value);
    }

    public static readonly BindableProperty HasMetaTextProperty =
        BindableProperty.Create(nameof(HasMetaText), typeof(bool), typeof(TestListCardView), false);

    public bool HasMetaText
    {
        get => (bool)GetValue(HasMetaTextProperty);
        private set => SetValue(HasMetaTextProperty, value);
    }

    private static void OnMetaTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TestListCardView)bindable;
        view.HasMetaText = !string.IsNullOrWhiteSpace((string)newValue);
    }

    public static readonly BindableProperty LastResultTextProperty =
        BindableProperty.Create(nameof(LastResultText), typeof(string), typeof(TestListCardView), string.Empty);

    public string LastResultText
    {
        get => (string)GetValue(LastResultTextProperty);
        set => SetValue(LastResultTextProperty, value);
    }

    public static readonly BindableProperty HasLastResultProperty =
        BindableProperty.Create(nameof(HasLastResult), typeof(bool), typeof(TestListCardView), false);

    public bool HasLastResult
    {
        get => (bool)GetValue(HasLastResultProperty);
        set => SetValue(HasLastResultProperty, value);
    }

    public static readonly BindableProperty ShowNeverTakenProperty =
        BindableProperty.Create(nameof(ShowNeverTaken), typeof(bool), typeof(TestListCardView), false);

    public bool ShowNeverTaken
    {
        get => (bool)GetValue(ShowNeverTakenProperty);
        set => SetValue(ShowNeverTakenProperty, value);
    }

    public static readonly BindableProperty NeverTakenLabelProperty =
        BindableProperty.Create(nameof(NeverTakenLabel), typeof(string), typeof(TestListCardView), string.Empty);

    public string NeverTakenLabel
    {
        get => (string)GetValue(NeverTakenLabelProperty);
        set => SetValue(NeverTakenLabelProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TestListCardView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public static readonly BindableProperty OpenHistoryCommandProperty =
        BindableProperty.Create(nameof(OpenHistoryCommand), typeof(ICommand), typeof(TestListCardView), null);

    public ICommand? OpenHistoryCommand
    {
        get => (ICommand?)GetValue(OpenHistoryCommandProperty);
        set => SetValue(OpenHistoryCommandProperty, value);
    }

    public static readonly BindableProperty RetakeCommandProperty =
        BindableProperty.Create(nameof(RetakeCommand), typeof(ICommand), typeof(TestListCardView), null);

    public ICommand? RetakeCommand
    {
        get => (ICommand?)GetValue(RetakeCommandProperty);
        set => SetValue(RetakeCommandProperty, value);
    }

    public static readonly BindableProperty HasMultipleResultsProperty =
        BindableProperty.Create(nameof(HasMultipleResults), typeof(bool), typeof(TestListCardView), false);

    public bool HasMultipleResults
    {
        get => (bool)GetValue(HasMultipleResultsProperty);
        set => SetValue(HasMultipleResultsProperty, value);
    }

    public static readonly BindableProperty HistoryLabelProperty =
        BindableProperty.Create(nameof(HistoryLabel), typeof(string), typeof(TestListCardView), string.Empty);

    public string HistoryLabel
    {
        get => (string)GetValue(HistoryLabelProperty);
        set => SetValue(HistoryLabelProperty, value);
    }

    public static readonly BindableProperty RetakeLabelProperty =
        BindableProperty.Create(nameof(RetakeLabel), typeof(string), typeof(TestListCardView), string.Empty);

    public string RetakeLabel
    {
        get => (string)GetValue(RetakeLabelProperty);
        set => SetValue(RetakeLabelProperty, value);
    }
}
