namespace PsychologyApp.Presentation.Shared.ViewModels;

public partial class BaseViewModel
{
    protected bool _fail_visibility;
    public bool IsFail
    {
        get => _fail_visibility;
        set
        {
            if (_fail_visibility != value)
            {
                _fail_visibility = value;
                OnPropertyChanged(nameof(IsFail));
            }
        }
    }

    private bool _init_visibility;
    public bool IsInit
    {
        get => _init_visibility;
        set
        {
            if (_init_visibility != value)
            {
                _init_visibility = value;
                OnPropertyChanged(nameof(IsInit));
                OnPropertyChanged(nameof(IsLoadingOverlayVisible));
            }
        }
    }

    protected bool _done_visibility;
    public bool IsDone
    {
        get => _done_visibility;
        set
        {
            if (_done_visibility != value)
            {
                _done_visibility = value;
                OnPropertyChanged(nameof(IsDone));
                OnPropertyChanged(nameof(IsLoadingOverlayVisible));
            }
        }
    }

    public bool IsLoadingOverlayVisible => IsInit && !IsDone;

    protected string _progress_text = string.Empty;
    public string ProgressText
    {
        get => _progress_text;
        set
        {
            if (_progress_text != value)
            {
                _progress_text = value;
                OnPropertyChanged(nameof(ProgressText));
            }
        }
    }

    protected bool _created_visibility;
    public bool IsCreated
    {
        get => _created_visibility;
        set
        {
            if (_created_visibility != value)
            {
                _created_visibility = value;
                OnPropertyChanged(nameof(IsCreated));
            }
        }
    }
}
