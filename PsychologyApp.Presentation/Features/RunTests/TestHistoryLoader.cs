using PsychologyApp.Domain.Colour.Enums;
﻿using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Entities.Test;
using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public sealed record TestHistoryLoadResult(
    IReadOnlyList<TestHistoryEntryItem> Entries,
    IReadOnlyList<TestScoreChartPoint> ChartPoints,
    int ChartDomainMin,
    int ChartDomainMax,
    string ChartSubtitle,
    string Title);

public sealed class TestHistoryLoader(
    TestTrendResolver trendResolver,
    IQuestionnaireResultDetailService detailService,
    LuscherDetailReader luscherDetailReader)
{
    // Off the UI thread: building the snapshot after each query would otherwise run there.
    public Task<TestHistoryLoadResult> LoadEntriesAsync(
        string testId,
        string fallbackTitle,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService,
        CancellationToken cancellationToken = default) =>
        Task.Run(() => LoadEntriesCoreAsync(testId, fallbackTitle, userProgressService, testCatalogService, cancellationToken), cancellationToken);

    private async Task<TestHistoryLoadResult> LoadEntriesCoreAsync(
        string testId,
        string fallbackTitle,
        IUserProgressService userProgressService,
        ITestCatalogService testCatalogService,
        CancellationToken cancellationToken = default)
    {
        TestDefinition? definition = await testCatalogService.GetByIdAsync(testId, cancellationToken);
        string title = definition?.Title ?? fallbackTitle;
        ScoreDirection direction = definition?.ScoreDirection ?? ScoreDirection.LowerIsBetter;
        TestKind kind = definition?.Kind ?? TestKind.Questionnaire;

        IReadOnlyList<TestResultDTO> history =
            await userProgressService.GetTestResultHistoryAsync(testId, 50, cancellationToken);

        List<TestHistoryEntryItem> entries = [];
        for (int i = 0; i < history.Count; i++)
        {
            TestResultDTO item = history[i];
            TestResultDTO? older = i + 1 < history.Count ? history[i + 1] : null;
            TestTrendKind trend = older is null
                ? TestTrendKind.None
                : trendResolver.Compare(item.Score, older.Score, direction);

            QuestionnaireResultDetail? detail = kind == TestKind.Questionnaire
                ? detailService.TryParse(item.DetailJson)
                : null;
            LuscherStandardResultDetail? standardDetail = kind == TestKind.LuscherStandard
                ? luscherDetailReader.TryParseStandard(item.DetailJson)
                : null;
            LuscherBriefResultDetail? briefDetail = kind == TestKind.LuscherBrief
                ? luscherDetailReader.TryParseBrief(item.DetailJson)
                : null;

            entries.Add(new TestHistoryEntryItem
            {
                DateText = item.CompletedAt.ToLocalTime().ToString("g"),
                SummaryText = ColourStrings.BriefSummary(briefDetail?.First?.Code, briefDetail?.Second?.Code) ?? ResolveHistorySummary(definition?.AnalyzerId, item),
                ScoreText = item.Score is int score ? AppStrings.TestHistoryScore(score) : string.Empty,
                TrendText = TestTrendComparer.ToLabel(trend),
                TrendKind = trend,
                Detail = detail,
                LuscherStandardDetail = standardDetail,
                LuscherBriefDetail = briefDetail,
                LuscherFirstPassTitle = AppStrings.TestsLuscherHistoryFirstPass,
                LuscherFirstPassText = FormatColorPass(standardDetail?.FirstPassColors),
                LuscherSecondPassTitle = AppStrings.TestsLuscherHistorySecondPass,
                LuscherSecondPassText = FormatColorPass(standardDetail?.Colors),
                LuscherBkText = standardDetail is null ? string.Empty : AppStrings.TestsLuscherHistoryBk(standardDetail.Bk),
                LuscherBriefFirstTitle = AppStrings.TestsFirstColor,
                LuscherBriefFirstText = FormatBriefColor(briefDetail?.First, ColourMeaningType.Wanted),
                LuscherBriefSecondTitle = AppStrings.TestsSecondColor,
                LuscherBriefSecondText = FormatBriefColor(briefDetail?.Second, ColourMeaningType.Unwanted),
                DurationText = detail is null ? string.Empty : AppStrings.TestResultDuration(detail.DurationSeconds)
            });
        }

        IReadOnlyList<TestScoreChartPoint> chartPoints = trendResolver.BuildChartPoints(history);
        IReadOnlyList<int> chartValues = chartPoints.Select(point => point.Score).ToList();
        (int chartDomainMin, int chartDomainMax) =
            TestScoreChartRangeResolver.ResolveDomain(definition?.AnalyzerId ?? testId, chartValues);
        string chartSubtitle = AppStrings.ResolveChartSubtitle(chartPoints.Count);
        return new TestHistoryLoadResult(entries, chartPoints, chartDomainMin, chartDomainMax, chartSubtitle, title);
    }

    private static string ResolveHistorySummary(string? analyzerId, TestResultDTO item)
    {
        if (item.Score is int score)
        {
            string? mapped = TestScoreLabelMapper.GetSummary(analyzerId, score);
            if (!string.IsNullOrWhiteSpace(mapped))
            {
                return mapped;
            }
        }

        return item.Summary;
    }

    private static string FormatColorPass(IReadOnlyList<LuscherStandardColorDetail>? colors) =>
        colors is null || colors.Count == 0
            ? string.Empty
            : string.Join(" → ", colors.Select(color => ColourStrings.NameOfCode(color.Code) ?? color.Name ?? color.Code));

    // Names and meanings are taken from the stored colour code in today's language; the saved text was in the language of that day.
    private static string FormatBriefColor(LuscherBriefColorDetail? color, ColourMeaningType meaningType)
    {
        if (color is null)
        {
            return string.Empty;
        }

        string name = ColourStrings.NameOfCode(color.Code) ?? color.Name ?? string.Empty;
        string? text = ColourStrings.ExplanationOfCode(color.Code, meaningType) ?? color.Text;
        return string.IsNullOrWhiteSpace(text) ? name : $"{name}: {text}";
    }
}
