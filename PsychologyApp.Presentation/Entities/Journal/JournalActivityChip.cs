using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Entities.Journal;

public sealed class JournalActivityChip : INotifyPropertyChanged
{
    private bool _isActive;
    private string _label = string.Empty;

    public string Key { get; init; } = string.Empty;

    public string Label
    {
        get => _label;
        set
        {
            if (_label == value)
            {
                return;
            }

            _label = value;
            OnPropertyChanged();
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
