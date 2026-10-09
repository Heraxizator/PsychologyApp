using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.CompanionAvatar;

/// <summary>
/// The header of every chat: the companion's avatar, a title and a status line, with the avatar and text opening the companion's profile. The main chat
/// and the dialogue inside a practice both use it, so the same companion is recognisable in both.
/// </summary>
public partial class CompanionHeaderView : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(CompanionHeaderView), string.Empty);

    public static readonly BindableProperty StatusProperty = BindableProperty.Create(
        nameof(Status), typeof(string), typeof(CompanionHeaderView), string.Empty);

    public static readonly BindableProperty ProfileTextProperty = BindableProperty.Create(
        nameof(ProfileText), typeof(string), typeof(CompanionHeaderView), string.Empty);

    public static readonly BindableProperty BackCommandProperty = BindableProperty.Create(
        nameof(BackCommand), typeof(ICommand), typeof(CompanionHeaderView));

    public static readonly BindableProperty ProfileCommandProperty = BindableProperty.Create(
        nameof(ProfileCommand), typeof(ICommand), typeof(CompanionHeaderView));

    public CompanionHeaderView() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string ProfileText
    {
        get => (string)GetValue(ProfileTextProperty);
        set => SetValue(ProfileTextProperty, value);
    }

    public ICommand? BackCommand
    {
        get => (ICommand?)GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    public ICommand? ProfileCommand
    {
        get => (ICommand?)GetValue(ProfileCommandProperty);
        set => SetValue(ProfileCommandProperty, value);
    }
}
