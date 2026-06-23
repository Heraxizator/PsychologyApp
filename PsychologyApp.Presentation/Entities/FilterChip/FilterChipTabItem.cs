using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Entities.FilterChip;

public sealed class FilterChipTabItem : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private bool _isSelected;

    public string Key { get; init; } = string.Empty;

    public string Title
    {
        get => _title;
        set
        {
            if (_title == value)
            {
                return;
            }

            _title = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
