using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.BodyCheck;

/// <summary>A chip of the Body page: a place, a sensation or a feeling.</summary>
public sealed class BodyChoiceItem : INotifyPropertyChanged
{
    private bool _isSelected;

    public required string Text { get; init; }

    /// <summary>The place, sensation or feeling the chip stands for.</summary>
    public required object Value { get; init; }

    public required ICommand TapCommand { get; init; }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                Changed();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void Changed([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed record BodyPatternItem(string Title, string CountText, string? FeelingText, string? TimeText)
{
    public bool HasFeeling => FeelingText is not null;
    public bool HasTime => TimeText is not null;
}

public sealed record BodyEntryItem(string Title, string DateText, string? FeelingsText, string? Note, ICommand DeleteCommand)
{
    public bool HasFeelings => FeelingsText is not null;
    public bool HasNote => Note is not null;
}

public sealed record BodyPracticeItem(string Title, ICommand OpenCommand);
