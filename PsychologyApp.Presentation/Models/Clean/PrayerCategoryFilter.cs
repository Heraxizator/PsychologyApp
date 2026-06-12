using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsychologyApp.Presentation.Models.Clean;

public sealed class PrayerCategoryFilter : INotifyPropertyChanged
{
    private bool _isSelected;

    public string Key { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;

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
