using PsychologyApp.Presentation.Shared.Common;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Test;

public class TestItem : INotifyPropertyChanged
{
    public string TestId { get; set; } = string.Empty;
    public string? AnalyzerId { get; set; }
    public string Title { get; set; } = default!;
    public string Subtitle { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string Comment { get; set; } = default!;
    public List<string> Algorithm { get; set; } = default!;
    public Func<Task> StartAsync { get; set; } = () => Task.CompletedTask;
    public ICommand? TapCommand { get; set; }
    public string? MetaText { get; set; }
    public bool HasMetaText => !string.IsNullOrWhiteSpace(MetaText);

    private string? _lastResultSummary;
    public string? LastResultSummary
    {
        get => _lastResultSummary;
        set
        {
            if (_lastResultSummary == value)
            {
                return;
            }

            _lastResultSummary = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasLastResult));
            OnPropertyChanged(nameof(ShowNeverTaken));
        }
    }

    public bool HasLastResult => !string.IsNullOrWhiteSpace(LastResultSummary);
    public bool ShowNeverTaken => !HasLastResult;

    private bool _hasMultipleResults;
    public bool HasMultipleResults
    {
        get => _hasMultipleResults;
        set
        {
            if (_hasMultipleResults == value)
            {
                return;
            }

            _hasMultipleResults = value;
            OnPropertyChanged();
        }
    }

    public string HistoryLabel => AppStrings.TestOpenHistory;
    public string RetakeLabel => AppStrings.TestRetakeButton;
    public string NeverTakenLabel => AppStrings.TestNeverTakenYet;
    public ICommand? OpenHistoryCommand { get; set; }
    public ICommand? RetakeCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
