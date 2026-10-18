using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Pages.ClinicalCare.Exposure;

public abstract class ExposureItem : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string name = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    protected void Raise(params string[] names)
    {
        foreach (string name in names)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

/// <summary>A yes/no question of the safety screening; unanswered until one of the two is tapped.</summary>
public sealed class ExposureQuestionItem : ExposureItem
{
    private bool? _answer;

    public required string Text { get; init; }
    public required ICommand SelectCommand { get; init; }

    public bool? Answer
    {
        get => _answer;
        set
        {
            Set(ref _answer, value);
            Raise(nameof(IsYes), nameof(IsNo));
        }
    }

    public bool IsYes => _answer == true;
    public bool IsNo => _answer == false;
}

/// <summary>One of several options (kind of fear, outcome of a task), shown as a card with a check when chosen.</summary>
public sealed class ExposureChoiceItem : ExposureItem
{
    private bool _isSelected;

    public required string Title { get; init; }
    public string Subtitle { get; init; } = string.Empty;
    public required ICommand SelectCommand { get; init; }
    public bool HasSubtitle => Subtitle.Length > 0;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            Set(ref _isSelected, value);
            Raise(nameof(SelectionIcon));
        }
    }

    public string SelectionIcon => _isSelected ? "RadioButtonChecked" : "RadioButtonUnchecked";
}

/// <summary>A step of the ladder: in the builder it can be moved up or down the scale or removed; on the home it shows its state.</summary>
public sealed class ExposureStepRow : ExposureItem
{
    private int _distress;
    private bool _isChosen;

    public required string Id { get; init; }
    public required string Text { get; init; }
    public string StatusText { get; init; } = string.Empty;
    public string StatusIcon { get; init; } = string.Empty;
    public bool IsCurrent { get; init; }
    public bool IsMastered { get; init; }
    public bool HasStatus => StatusText.Length > 0;

    public ICommand? EasierCommand { get; init; }
    public ICommand? HarderCommand { get; init; }
    public ICommand? ToggleCommand { get; init; }

    public int Distress
    {
        get => _distress;
        set
        {
            Set(ref _distress, Math.Clamp(value, 0, 100));
            Raise(nameof(DistressText));
        }
    }

    public string DistressText => _distress.ToString(System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>For examples in the builder: on the ladder or not.</summary>
    public bool IsChosen
    {
        get => _isChosen;
        set
        {
            Set(ref _isChosen, value);
            Raise(nameof(ChosenIcon));
        }
    }

    public string ChosenIcon => _isChosen ? "CheckCircle" : "RadioButtonUnchecked";
}
