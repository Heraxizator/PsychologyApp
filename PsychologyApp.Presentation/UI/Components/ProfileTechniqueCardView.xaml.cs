using PsychologyApp.Presentation.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class ProfileTechniqueCardView : ContentView
{
    public ProfileTechniqueCardView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
    }

    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(nameof(Image), typeof(string), typeof(ProfileTechniqueCardView), string.Empty, propertyChanged: OnVisualChanged);

    public string Image
    {
        get => (string)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(ProfileTechniqueCardView), string.Empty, propertyChanged: OnVisualChanged);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public static readonly BindableProperty HasIconProperty =
        BindableProperty.Create(nameof(HasIcon), typeof(bool), typeof(ProfileTechniqueCardView), false);

    public bool HasIcon
    {
        get => (bool)GetValue(HasIconProperty);
        private set => SetValue(HasIconProperty, value);
    }

    public static readonly BindableProperty HasImageProperty =
        BindableProperty.Create(nameof(HasImage), typeof(bool), typeof(ProfileTechniqueCardView), false);

    public bool HasImage
    {
        get => (bool)GetValue(HasImageProperty);
        private set => SetValue(HasImageProperty, value);
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (ProfileTechniqueCardView)bindable;
        bool hasIcon = !string.IsNullOrWhiteSpace(view.IconName);
        bool hasImage = !hasIcon && !string.IsNullOrWhiteSpace(view.Image);
        view.HasIcon = hasIcon;
        view.HasImage = hasImage;
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(ProfileTechniqueCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(ProfileTechniqueCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty ThemeProperty =
        BindableProperty.Create(nameof(Theme), typeof(string), typeof(ProfileTechniqueCardView), string.Empty);

    public string Theme
    {
        get => (string)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(ProfileTechniqueCardView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}
