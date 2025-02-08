using System.Windows.Input;

namespace PsychologyApp.Presentation.Templates;

public partial class NavigationBarExtendedView : ContentView
{
	public NavigationBarExtendedView()
	{
		InitializeComponent();
	}

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(NavigationBarExtendedView), string.Empty);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public static readonly BindableProperty BackTextProperty =
        BindableProperty.Create(nameof(BackText), typeof(string), typeof(NavigationBarExtendedView), string.Empty);

    public string BackText
    {
        get => (string)GetValue(BackTextProperty);
        set => SetValue(BackTextProperty, value);
    }

    public static readonly BindableProperty BackCommandProperty =
        BindableProperty.Create(nameof(BackCommand), typeof(ICommand), typeof(NavigationBarExtendedView), default, BindingMode.TwoWay);

    public ICommand BackCommand
    {
        get => (ICommand)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public static readonly BindableProperty ExtensionTextProperty =
        BindableProperty.Create(nameof(ExtensionText), typeof(string), typeof(NavigationBarExtendedView), string.Empty);

    public string ExtensionText
    {
        get => (string)GetValue(ExtensionTextProperty);
        set => SetValue(ExtensionTextProperty, value);
    }

    public static readonly BindableProperty ExtensionCommandProperty =
        BindableProperty.Create(nameof(ExtensionCommand), typeof(ICommand), typeof(NavigationBarExtendedView), default, BindingMode.TwoWay);

    public ICommand ExtensionCommand
    {
        get => (ICommand)GetValue(ExtensionCommandProperty);
        set => SetValue(ExtensionCommandProperty, value);
    }

    public static readonly BindableProperty ExtensionEnabledProperty =
        BindableProperty.Create(nameof(ExtensionText), typeof(bool), typeof(NavigationBarExtendedView), true);

    public bool ExtensionEnabled
    {
        get => (bool)GetValue(ExtensionEnabledProperty);
        set => SetValue(ExtensionEnabledProperty, value);
    }
}