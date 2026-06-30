using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Test;
using System.Windows.Input;

namespace PsychologyApp.Presentation.Entities.Test;

public class TestItem
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
    public string? LastResultSummary { get; set; }
    public bool HasLastResult => !string.IsNullOrWhiteSpace(LastResultSummary);
    public bool HasMultipleResults { get; set; }
    public string HistoryLabel => AppStrings.TestOpenHistory;
    public ICommand? OpenHistoryCommand { get; set; }
}
