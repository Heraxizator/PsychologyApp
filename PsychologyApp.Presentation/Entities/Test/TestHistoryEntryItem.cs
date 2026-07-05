using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Entities.Test;

public sealed class TestHistoryEntryItem
{
    public string DateText { get; init; } = string.Empty;
    public string SummaryText { get; init; } = string.Empty;
    public string ScoreText { get; init; } = string.Empty;
    public string TrendText { get; init; } = string.Empty;
    public string DurationText { get; init; } = string.Empty;
    public TestTrendKind TrendKind { get; init; } = TestTrendKind.None;
    public QuestionnaireResultDetail? Detail { get; init; }
    public LuscherStandardResultDetail? LuscherStandardDetail { get; init; }
    public LuscherBriefResultDetail? LuscherBriefDetail { get; init; }
    public string LuscherFirstPassTitle { get; init; } = string.Empty;
    public string LuscherFirstPassText { get; init; } = string.Empty;
    public string LuscherSecondPassTitle { get; init; } = string.Empty;
    public string LuscherSecondPassText { get; init; } = string.Empty;
    public string LuscherBkText { get; init; } = string.Empty;
    public string LuscherBriefFirstTitle { get; init; } = string.Empty;
    public string LuscherBriefFirstText { get; init; } = string.Empty;
    public string LuscherBriefSecondTitle { get; init; } = string.Empty;
    public string LuscherBriefSecondText { get; init; } = string.Empty;
    public string LuscherBriefFirstRoleLabel => AppStrings.TestsLuscherWantedRole;
    public string LuscherBriefSecondRoleLabel => AppStrings.TestsLuscherUnwantedRole;
    public IReadOnlyList<QuestionnaireResultQuestion> DetailQuestions =>
        Detail?.Questions ?? Array.Empty<QuestionnaireResultQuestion>();
    public bool HasScore => !string.IsNullOrWhiteSpace(ScoreText);
    public bool HasTrend => TrendKind is not TestTrendKind.None;
    public bool HasDetail => Detail is not null;
    public bool HasLuscherDetail => LuscherStandardDetail is not null || LuscherBriefDetail is not null;
    public bool HasStandardLuscherDetail => LuscherStandardDetail is not null;
    public bool HasBriefLuscherDetail => LuscherBriefDetail is not null;
    public bool IsImproved => TrendKind is TestTrendKind.Improved;
    public bool IsWorse => TrendKind is TestTrendKind.Worse;
    public bool IsSame => TrendKind is TestTrendKind.Same;
}
