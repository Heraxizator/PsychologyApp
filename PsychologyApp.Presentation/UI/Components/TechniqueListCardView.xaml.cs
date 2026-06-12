using PsychologyApp.Presentation.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class TechniqueListCardView : ContentView
{
    public TechniqueListCardView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
    }

    public static readonly BindableProperty NumberProperty =
        BindableProperty.Create(nameof(Number), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string Number
    {
        get => (string)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public static readonly BindableProperty DateProperty =
        BindableProperty.Create(nameof(Date), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string Date
    {
        get => (string)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public static readonly BindableProperty ImageProperty =
        BindableProperty.Create(nameof(Image), typeof(string), typeof(TechniqueListCardView), string.Empty, propertyChanged: OnVisualChanged);

    public string Image
    {
        get => (string)GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public static readonly BindableProperty IconNameProperty =
        BindableProperty.Create(nameof(IconName), typeof(string), typeof(TechniqueListCardView), string.Empty, propertyChanged: OnVisualChanged);

    public string IconName
    {
        get => (string)GetValue(IconNameProperty);
        set => SetValue(IconNameProperty, value);
    }

    public static readonly BindableProperty HasIconProperty =
        BindableProperty.Create(nameof(HasIcon), typeof(bool), typeof(TechniqueListCardView), false);

    public bool HasIcon
    {
        get => (bool)GetValue(HasIconProperty);
        private set => SetValue(HasIconProperty, value);
    }

    public static readonly BindableProperty HasImageProperty =
        BindableProperty.Create(nameof(HasImage), typeof(bool), typeof(TechniqueListCardView), false);

    public bool HasImage
    {
        get => (bool)GetValue(HasImageProperty);
        private set => SetValue(HasImageProperty, value);
    }

    public static readonly BindableProperty MetaTextProperty =
        BindableProperty.Create(nameof(MetaText), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string MetaText
    {
        get => (string)GetValue(MetaTextProperty);
        set => SetValue(MetaTextProperty, value);
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty ThemeProperty =
        BindableProperty.Create(nameof(Theme), typeof(string), typeof(TechniqueListCardView), string.Empty);

    public string Theme
    {
        get => (string)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly BindableProperty AuthorProperty =
        BindableProperty.Create(nameof(Author), typeof(string), typeof(TechniqueListCardView), string.Empty, propertyChanged: OnAuthorChanged);

    public string Author
    {
        get => (string)GetValue(AuthorProperty);
        set => SetValue(AuthorProperty, value);
    }

    public static readonly BindableProperty HasAuthorProperty =
        BindableProperty.Create(nameof(HasAuthor), typeof(bool), typeof(TechniqueListCardView), false);

    public bool HasAuthor
    {
        get => (bool)GetValue(HasAuthorProperty);
        private set => SetValue(HasAuthorProperty, value);
    }

    public static readonly BindableProperty ActiveProperty =
        BindableProperty.Create(nameof(Active), typeof(bool), typeof(TechniqueListCardView), true);

    public bool Active
    {
        get => (bool)GetValue(ActiveProperty);
        set => SetValue(ActiveProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(TechniqueListCardView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TechniqueListCardView)bindable;
        bool hasIcon = !string.IsNullOrWhiteSpace(view.IconName);
        bool hasImage = !hasIcon && !string.IsNullOrWhiteSpace(view.Image);
        view.HasIcon = hasIcon;
        view.HasImage = hasImage;
    }

    private static void OnAuthorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (TechniqueListCardView)bindable;
        view.HasAuthor = !string.IsNullOrWhiteSpace((string)newValue);
    }
}
