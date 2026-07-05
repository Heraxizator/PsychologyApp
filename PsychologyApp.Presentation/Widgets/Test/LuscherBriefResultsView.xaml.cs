namespace PsychologyApp.Presentation.Widgets.Test;

public partial class LuscherBriefResultsView : ContentView
{
    public LuscherBriefResultsView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty FirstColorProperty =
        BindableProperty.Create(nameof(FirstColor), typeof(Color), typeof(LuscherBriefResultsView), Colors.Transparent);

    public static readonly BindableProperty SecondColorProperty =
        BindableProperty.Create(nameof(SecondColor), typeof(Color), typeof(LuscherBriefResultsView), Colors.Transparent);

    public static readonly BindableProperty FirstNameProperty =
        BindableProperty.Create(nameof(FirstName), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty SecondNameProperty =
        BindableProperty.Create(nameof(SecondName), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty FirstResultProperty =
        BindableProperty.Create(nameof(FirstResult), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty SecondResultProperty =
        BindableProperty.Create(nameof(SecondResult), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty FirstColorRoleLabelProperty =
        BindableProperty.Create(nameof(FirstColorRoleLabel), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public static readonly BindableProperty SecondColorRoleLabelProperty =
        BindableProperty.Create(nameof(SecondColorRoleLabel), typeof(string), typeof(LuscherBriefResultsView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Color FirstColor
    {
        get => (Color)GetValue(FirstColorProperty);
        set => SetValue(FirstColorProperty, value);
    }

    public Color SecondColor
    {
        get => (Color)GetValue(SecondColorProperty);
        set => SetValue(SecondColorProperty, value);
    }

    public string FirstName
    {
        get => (string)GetValue(FirstNameProperty);
        set => SetValue(FirstNameProperty, value);
    }

    public string SecondName
    {
        get => (string)GetValue(SecondNameProperty);
        set => SetValue(SecondNameProperty, value);
    }

    public string FirstResult
    {
        get => (string)GetValue(FirstResultProperty);
        set => SetValue(FirstResultProperty, value);
    }

    public string SecondResult
    {
        get => (string)GetValue(SecondResultProperty);
        set => SetValue(SecondResultProperty, value);
    }

    public string FirstColorRoleLabel
    {
        get => (string)GetValue(FirstColorRoleLabelProperty);
        set => SetValue(FirstColorRoleLabelProperty, value);
    }

    public string SecondColorRoleLabel
    {
        get => (string)GetValue(SecondColorRoleLabelProperty);
        set => SetValue(SecondColorRoleLabelProperty, value);
    }
}
