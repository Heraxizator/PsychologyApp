using System.Windows.Input;

namespace PsychologyApp.Presentation.Widgets.Audio;

public partial class MusicTransportControlsView : ContentView
{
    public MusicTransportControlsView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty PlayPreviousCommandProperty =
        BindableProperty.Create(nameof(PlayPreviousCommand), typeof(ICommand), typeof(MusicTransportControlsView));

    public static readonly BindableProperty TogglePlayPauseCommandProperty =
        BindableProperty.Create(nameof(TogglePlayPauseCommand), typeof(ICommand), typeof(MusicTransportControlsView));

    public static readonly BindableProperty PlayNextCommandProperty =
        BindableProperty.Create(nameof(PlayNextCommand), typeof(ICommand), typeof(MusicTransportControlsView));

    public static readonly BindableProperty IsPlayingProperty =
        BindableProperty.Create(nameof(IsPlaying), typeof(bool), typeof(MusicTransportControlsView), false);

    public ICommand? PlayPreviousCommand
    {
        get => (ICommand?)GetValue(PlayPreviousCommandProperty);
        set => SetValue(PlayPreviousCommandProperty, value);
    }

    public ICommand? TogglePlayPauseCommand
    {
        get => (ICommand?)GetValue(TogglePlayPauseCommandProperty);
        set => SetValue(TogglePlayPauseCommandProperty, value);
    }

    public ICommand? PlayNextCommand
    {
        get => (ICommand?)GetValue(PlayNextCommandProperty);
        set => SetValue(PlayNextCommandProperty, value);
    }

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }
}
