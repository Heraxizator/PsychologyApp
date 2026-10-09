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
                UpdateSlowLoading();
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
                UpdateSlowLoading();
            }
        }
    }

    public bool IsLoadingOverlayVisible => IsInit && !IsDone;

    /// <summary>How long a load may take before the spinner with its cancel link appears; faster loads show the grey placeholders only.</summary>
    protected const int SlowLoadingDelayMilliseconds = 2500;

    private int _slowLoadingToken;
    private bool _slowLoadingVisible;

    /// <summary>True when a load has gone on for a while. The grey placeholders are enough for a quick load; the spinner and the cancel link come only for a slow one, so the two never sit on top of each other for the usual case.</summary>
    public bool IsSlowLoadingVisible
    {
        get => _slowLoadingVisible;
        private set
        {
            if (_slowLoadingVisible != value)
            {
                _slowLoadingVisible = value;
                OnPropertyChanged(nameof(IsSlowLoadingVisible));
            }
        }
    }

    private void UpdateSlowLoading()
    {
        int token = ++_slowLoadingToken;
        if (!IsLoadingOverlayVisible)
        {
            IsSlowLoadingVisible = false;
            return;
        }

        _ = Task.Delay(SlowLoadingDelayMilliseconds).ContinueWith(_ =>
        {
            if (token == _slowLoadingToken && IsLoadingOverlayVisible)
            {
                IsSlowLoadingVisible = true;
            }
        }, TaskScheduler.Default);
    }

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
