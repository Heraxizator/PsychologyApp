using PsychologyApp.Presentation.Models.Tests;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;

namespace PsychologyApp.Presentation.Pages.FindProblem;

public partial class FindProblemViewModel
{
    private int? _estimatedMinutes;
    private int? _questionCount;

    public string DurationText =>
        _estimatedMinutes is int minutes ? AppStrings.TestDuration(minutes) : string.Empty;

    public string QuestionCountText =>
        _questionCount is int count ? AppStrings.TestQuestionCount(count) : string.Empty;

    public string IntroLeadText => AppStrings.TestsIntroLead;

    partial void OnConstructed() => LoadMetaAsync().FireAndForget();

    private async Task LoadMetaAsync()
    {
        if (string.IsNullOrWhiteSpace(_testId))
        {
            return;
        }

        TestDefinition? definition = await _testCatalogService.GetByIdAsync(_testId);
        if (definition is null)
        {
            return;
        }

        _estimatedMinutes = definition.EstimatedMinutes;
        _questionCount = definition.QuestionCount ?? definition.Questions?.Count;

        Notify(nameof(DurationText), nameof(QuestionCountText));
    }
}
