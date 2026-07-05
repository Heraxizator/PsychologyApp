using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
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
        }
    }

    public bool HasLastResult => !string.IsNullOrWhiteSpace(LastResultSummary);

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
    public ICommand? OpenHistoryCommand { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
