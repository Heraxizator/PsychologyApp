using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.Question;

public partial class QuestionViewModel
{
    private int? _estimatedMinutes;

    public string DurationText =>
        _estimatedMinutes is int minutes ? AppStrings.TestDuration(minutes) : string.Empty;

    public string QuestionCountText =>
        TotalCount > 0 ? AppStrings.TestQuestionCount(TotalCount) : string.Empty;

    public string RemainingDurationText
    {
        get
        {
            if (_estimatedMinutes is not int totalMinutes || totalMinutes <= 0)
            {
                return string.Empty;
            }

            int remaining = Math.Max(1, (int)Math.Ceiling(totalMinutes * (1.0 - Progress)));
            return AppStrings.TestsRemainingDuration(remaining);
        }
    }

    private void LoadMetaAsync() => LoadMetaCoreAsync().FireAndForget();

    private async Task LoadMetaCoreAsync()
    {
        if (string.IsNullOrWhiteSpace(_session?.TestId))
        {
            NotifyMeta();
            return;
        }

        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_session.TestId);
        if (definition is null)
        {
            NotifyMeta();
            return;
        }

        _estimatedMinutes = definition.EstimatedMinutes;
        NotifyMeta();
    }

    private void NotifyMeta() =>
        Notify(
            nameof(DurationText),
            nameof(QuestionCountText),
            nameof(RemainingDurationText));
}
