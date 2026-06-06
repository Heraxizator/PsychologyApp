using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.Modules.Practice.Techniques;
using PsychologyApp.Presentation.Services;
using MvvmHelpers;
using PsychologyApp.Presentation.UI.Components;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels;

public class BaseViewModel : INotifyPropertyChanged
{
    protected INavigation? Navigation { get; private set; }
    protected INavigationService? NavigationService { get; private set; }
    protected string? TheoryInfo { get; set; }

    public ICommand Finish { get; protected set; } = new Command(() => { });
    public ICommand Theory { get; protected set; } = new Command(() => { });
    public ICommand? Cancel { get; protected set; }
    public ICommand? Reload { get; protected set; }
    public ICommand? Start { get; protected set; }

    private string _module_name = string.Empty;
    public string ModuleName
    {
        get => _module_name;
        set => SetProperty(ref _module_name, value);
    }

    private string _page_name = string.Empty;
    public string PageName
    {
        get => _page_name;
        set => SetProperty(ref _page_name, value);
    }

    public ObservableRangeCollection<string> Algorithm { get; protected set; } = [];

    public List<EntryItem> Entries { get; protected set; } = [];

    protected void BindNavigation(INavigation navigation, INavigationService? navigationService = null)
    {
        Navigation = navigation;
        NavigationService = navigationService;
        Finish = new AsyncCommand(GoBackAsync);
        Theory = new Command(() =>
        {
            if (navigationService is null)
            {
                return;
            }

            string content = TheoryInfo ?? string.Empty;
            navigationService.GoToTheoryAsync(content).FireAndForget();
        });
    }

    protected Task GoBackAsync() =>
        NavigationService?.GoBackAsync() ?? Navigation!.PopAsync(false);

    protected Task GoToRootAsync() =>
        NavigationService?.GoToRootAsync() ?? Navigation!.PopToRootAsync(false);

    protected void ApplyTechnique(TechniqueId id)
    {
        TechniqueDefinition def = TechniqueCatalog.Get(id);
        ModuleName = def.ModuleName;
        PageName = def.PageName;
        Algorithm.Clear();
        Algorithm.AddRange(def.Algorithm);
        TheoryInfo = def.TheoryInfo;
        if (def.Entries is not null)
        {
            Entries.Clear();
            Entries.AddRange(def.Entries);
        }
    }

    protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action? onChanged = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        OnPropertyChanged(propertyName);
        return true;
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Public Properties

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
            }
        }
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

    #endregion

    protected void SetDefault()
    {
        IsInit = false;
        IsFail = false;
        IsDone = false;
        IsCreated = false;
    }

    public void SetDone()
    {
        SetDefault();
        IsDone = true;
    }

    public void SetInit()
    {
        SetDefault();
        IsInit = true;
    }

    public void SetFail()
    {
        SetDefault();
        IsFail = true;
    }

    public void SetCreated()
    {
        SetDefault();
        IsCreated = true;
    }
}
