using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class EmptyStateView : ContentView
{
    public EmptyStateView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(
            nameof(TitleText),
            typeof(string),
            typeof(EmptyStateView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty HasTitleProperty =
        BindableProperty.Create(nameof(HasTitle), typeof(bool), typeof(EmptyStateView), false);

    public static readonly BindableProperty BodyTextProperty =
        BindableProperty.Create(nameof(BodyText), typeof(string), typeof(EmptyStateView), string.Empty);

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(
            nameof(IconName),
            typeof(string),
            typeof(EmptyStateView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(
            nameof(ActionText),
            typeof(string),
            typeof(EmptyStateView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(
            nameof(ActionCommand),
            typeof(ICommand),
            typeof(EmptyStateView),
            null,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty HasIconProperty =
        BindableProperty.Create(nameof(HasIcon), typeof(bool), typeof(EmptyStateView), false);

    public static readonly BindableProperty HasActionProperty =
        BindableProperty.Create(nameof(HasAction), typeof(bool), typeof(EmptyStateView), false);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string BodyText
    {
        get => (string)GetValue(BodyTextProperty);
        set => SetValue(BodyTextProperty, value);
    }

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public string ActionText
    {
        get => (string)GetValue(ActionTextProperty);
        set => SetValue(ActionTextProperty, value);
    }

    public ICommand? ActionCommand
    {
        get => (ICommand?)GetValue(ActionCommandProperty);
        set => SetValue(ActionCommandProperty, value);
    }

    public bool HasTitle
    {
        get => (bool)GetValue(HasTitleProperty);
        private set => SetValue(HasTitleProperty, value);
    }

    public bool HasIcon
    {
        get => (bool)GetValue(HasIconProperty);
        private set => SetValue(HasIconProperty, value);
    }

    public bool HasAction
    {
        get => (bool)GetValue(HasActionProperty);
        private set => SetValue(HasActionProperty, value);
    }

    private static void OnPresentationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is EmptyStateView view)
        {
            view.RefreshPresentationFlags();
        }
    }

    private void RefreshPresentationFlags()
    {
        HasTitle = !string.IsNullOrWhiteSpace(TitleText);
        HasIcon = !string.IsNullOrWhiteSpace(IconName);
        HasAction = !string.IsNullOrWhiteSpace(ActionText) && ActionCommand is not null;
    }
}
