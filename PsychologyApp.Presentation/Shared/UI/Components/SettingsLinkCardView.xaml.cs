using PsychologyApp.Presentation.Shared.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Shared.UI.Components;

public partial class SettingsLinkCardView : ContentView
{
    public SettingsLinkCardView()
    {
        InitializeComponent();
        TemplatePressFeedback.Attach(this, new PressFeedbackOptions { HapticOnRelease = true });
        Loaded += (_, _) => UpdateSemantics();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
            nameof(Title),
            typeof(string),
            typeof(SettingsLinkCardView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(
            nameof(Subtitle),
            typeof(string),
            typeof(SettingsLinkCardView),
            string.Empty,
            propertyChanged: OnPresentationChanged);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(SettingsLinkCardView), null);

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private static void OnPresentationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SettingsLinkCardView view)
        {
            view.UpdateSemantics();
        }
    }

    private void UpdateSemantics()
    {
        string description = string.IsNullOrWhiteSpace(Subtitle)
            ? Title
            : $"{Title}. {Subtitle}";
        SemanticProperties.SetDescription(this, description);
        SemanticProperties.SetHint(this, Title);
    }
}
