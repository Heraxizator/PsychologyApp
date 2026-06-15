using PsychologyApp.Presentation.Services.Preferences;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels;

public partial class BaseViewModel : INotifyPropertyChanged
{
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

    protected BaseViewModel()
    {
        UserPreferencesStore.Changed += HandlePreferencesChanged;
    }

    protected void BindPreferences(IUserPreferencesStore store)
    {
        UserPreferencesStore.Changed -= HandlePreferencesChanged;
        UserPreferencesStore = store;
        UserPreferencesStore.Changed += HandlePreferencesChanged;
    }

    protected virtual void RefreshLocalizedProperties()
    {
    }

    protected void Notify(params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            OnPropertyChanged(propertyName);
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

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
