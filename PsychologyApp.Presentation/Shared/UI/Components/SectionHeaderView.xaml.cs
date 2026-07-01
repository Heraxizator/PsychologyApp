using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class SectionHeaderView : ContentView
{
    public SectionHeaderView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty TitleTextProperty =
        BindableProperty.Create(nameof(TitleText), typeof(string), typeof(SectionHeaderView), string.Empty);

    public static readonly BindableProperty SubtitleTextProperty =
        BindableProperty.Create(
            nameof(SubtitleText),
            typeof(string),
            typeof(SectionHeaderView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty ActionTextProperty =
        BindableProperty.Create(
            nameof(ActionText),
            typeof(string),
            typeof(SectionHeaderView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty ActionCommandProperty =
        BindableProperty.Create(
            nameof(ActionCommand),
            typeof(ICommand),
            typeof(SectionHeaderView),
            null,
            propertyChanged: OnPresentationChanged);

    public static readonly BindableProperty HasSubtitleProperty =
        BindableProperty.Create(nameof(HasSubtitle), typeof(bool), typeof(SectionHeaderView), false);

    public static readonly BindableProperty HasActionProperty =
        BindableProperty.Create(nameof(HasAction), typeof(bool), typeof(SectionHeaderView), false);

    public string TitleText
    {
        get => (string)GetValue(TitleTextProperty);
        set => SetValue(TitleTextProperty, value);
    }

    public string SubtitleText
    {
        get => (string)GetValue(SubtitleTextProperty);
        set => SetValue(SubtitleTextProperty, value);
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

    public bool HasSubtitle
    {
        get => (bool)GetValue(HasSubtitleProperty);
        private set => SetValue(HasSubtitleProperty, value);
    }

    public bool HasAction
    {
        get => (bool)GetValue(HasActionProperty);
        private set => SetValue(HasActionProperty, value);
    }

    private static void OnPresentationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SectionHeaderView view)
        {
            view.RefreshPresentationFlags();
        }
    }

    private void RefreshPresentationFlags()
    {
        HasSubtitle = !string.IsNullOrWhiteSpace(SubtitleText);
        HasAction = !string.IsNullOrWhiteSpace(ActionText) && ActionCommand is not null;
    }
}
