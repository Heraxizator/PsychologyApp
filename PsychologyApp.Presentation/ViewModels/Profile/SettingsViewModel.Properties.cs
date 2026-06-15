using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Preferences;

namespace PsychologyApp.Presentation.ViewModels.Profile;

public partial class SettingsViewModel
{
    public string language { get; private set; } = string.Empty;
    public string Language
    {
        get => language;
        set
        {
            if (language != value)
            {
                language = value;
                RefreshLocalizedCollections();
                OnPropertyChanged(nameof(Language));
                ApplyLivePreview();
            }
        }
    }

    public string theme { get; private set; } = string.Empty;
    public string Theme
    {
        get => theme;
        set
        {
            if (theme != value)
            {
                theme = value;
                OnPropertyChanged(nameof(Theme));
                ApplyLivePreview();
            }
        }
    }

    public string color { get; private set; } = string.Empty;
    public string Color
    {
        get => color;
        set
        {
            if (color != value)
            {
                color = value;
                OnPropertyChanged(nameof(Color));
                ApplyLivePreview();
            }
        }
    }

    public string form { get; private set; } = string.Empty;
    public string Form
    {
        get => form;
        set
        {
            if (form != value)
            {
                form = value;
                OnPropertyChanged(nameof(Form));
                ApplyLivePreview();
            }
        }
    }

    public string size { get; private set; } = string.Empty;
    public string Size
    {
        get => size;
        set
        {
            if (size != value)
            {
                size = value;
                OnPropertyChanged(nameof(Size));
                ApplyLivePreview();
            }
        }
    }

    public bool isThick;
    public bool IsThick
    {
        get => isThick;
        set
        {
            if (isThick != value)
            {
                isThick = value;
                OnPropertyChanged(nameof(IsThick));
                ApplyLivePreview();
            }
        }
    }

    private UserPreferencesState BuildCurrentState() =>
        _presenter.BuildState(Language, Theme, Color, Form, Size, IsThick, _savedState);

    private void ApplyLivePreview() => _userPreferencesStore.ApplyPreview(BuildCurrentState());
}
