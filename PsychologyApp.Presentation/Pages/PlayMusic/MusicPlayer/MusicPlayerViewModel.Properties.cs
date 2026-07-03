using PsychologyApp.Presentation.Entities.Audio;

namespace PsychologyApp.Presentation.Pages.PlayMusic.MusicPlayer;

public partial class MusicPlayerViewModel
{
    public Audio? ActiveTrack
    {
        get => _activeTrack;
        private set
        {
            if (_activeTrack == value)
            {
                return;
            }

            _activeTrack = value;
            OnPropertyChanged(nameof(ActiveTrack));
            OnPropertyChanged(nameof(HasActiveTrack));
            OnPropertyChanged(nameof(ActiveTrackTitle));
            OnPropertyChanged(nameof(ActiveTrackCategory));
            SyncActiveTrackFlags();
        }
    }

    public bool HasActiveTrack => ActiveTrack is not null;
    public string ActiveTrackTitle => ActiveTrack?.Name ?? string.Empty;
    public string ActiveTrackCategory => ActiveTrack?.Category ?? string.Empty;
    public bool ShowBufferingOverlay => IsBuffering && HasActiveTrack;

    public bool IsPlaying
    {
        get => _isPlaying;
        private set
        {
            if (_isPlaying == value)
            {
                return;
            }

            _isPlaying = value;
            OnPropertyChanged(nameof(IsPlaying));
            SyncActiveTrackFlags();
        }
    }

    public bool IsBuffering
    {
        get => _isBuffering;
        set
        {
            if (SetProperty(ref _isBuffering, value))
            {
                OnPropertyChanged(nameof(ShowBufferingOverlay));
            }
        }
    }

    public string PositionDisplay
    {
        get => _positionDisplay;
        set => SetProperty(ref _positionDisplay, value);
    }

    public string DurationDisplay
    {
        get => _durationDisplay;
        set => SetProperty(ref _durationDisplay, value);
    }

    public string ProgressDisplay => $"{PositionDisplay} / {DurationDisplay}";

    public double ProgressFraction
    {
        get => _progressFraction;
        set => SetProperty(ref _progressFraction, value);
    }

    public bool HasFilteredItems => FilteredItems.Count > 0;
    public bool IsSearchEmptyVisible => IsDone && !HasFilteredItems && AllItems.Count > 0;

    public string SearchText
    {
        get => search_text;
        set
        {
            if (SetProperty(ref search_text, value))
            {
                ApplyFilter();
            }
        }
    }
}
