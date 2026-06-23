using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Audio;

public class Audio : INotifyPropertyChanged
{
    private bool _isActive;
    private bool _isPlayingThis;
    private bool _isCached;

    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? URL { get; set; }
    public ICommand? ClickCommand { get; set; }

    public bool IsActive
    {
        get => _isActive;
        set => SetField(ref _isActive, value);
    }

    public bool IsPlayingThis
    {
        get => _isPlayingThis;
        set => SetField(ref _isPlayingThis, value);
    }

    public bool IsCached
    {
        get => _isCached;
        set => SetField(ref _isCached, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
