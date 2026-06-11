using PsychologyApp.Presentation.Common;
using System.Windows.Input;

namespace PsychologyApp.Presentation.UI.Components;

public partial class MusicListCardView : ContentView
{
    public MusicListCardView()
    {
        InitializeComponent();
        VisualElementPressFeedback.AttachToTemplateRoot(this, new PressFeedbackOptions { HapticOnRelease = true });
        UpdatePlayIcon();
    }

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(MusicListCardView), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly BindableProperty SubtitleProperty =
        BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(MusicListCardView), string.Empty);

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(MusicListCardView), null);

    public ICommand? TapCommand
    {
        get => (ICommand?)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }

    public static readonly BindableProperty IsActiveProperty =
        BindableProperty.Create(nameof(IsActive), typeof(bool), typeof(MusicListCardView), false, propertyChanged: OnPlaybackStateChanged);

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public static readonly BindableProperty IsPlayingThisProperty =
        BindableProperty.Create(nameof(IsPlayingThis), typeof(bool), typeof(MusicListCardView), false, propertyChanged: OnPlaybackStateChanged);

    public bool IsPlayingThis
    {
        get => (bool)GetValue(IsPlayingThisProperty);
        set => SetValue(IsPlayingThisProperty, value);
    }

    public static readonly BindableProperty ShowOfflineBadgeProperty =
        BindableProperty.Create(nameof(ShowOfflineBadge), typeof(bool), typeof(MusicListCardView), false);

    public bool ShowOfflineBadge
    {
        get => (bool)GetValue(ShowOfflineBadgeProperty);
        set => SetValue(ShowOfflineBadgeProperty, value);
    }

    public static readonly BindableProperty OfflineBadgeTextProperty =
        BindableProperty.Create(nameof(OfflineBadgeText), typeof(string), typeof(MusicListCardView), string.Empty);

    public string OfflineBadgeText
    {
        get => (string)GetValue(OfflineBadgeTextProperty);
        set => SetValue(OfflineBadgeTextProperty, value);
    }

    private static void OnPlaybackStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MusicListCardView card)
        {
            card.UpdatePlayIcon();
        }
    }

    private void UpdatePlayIcon()
    {
        if (PlayIcon is null)
        {
            return;
        }

        PlayIcon.Text = IsActive && IsPlayingThis ? "⏸" : "▶";
    }
}
