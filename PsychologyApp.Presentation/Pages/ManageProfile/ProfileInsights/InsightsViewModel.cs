using System.Globalization;
using System.Windows.Input;
using PsychologyApp.Application.Body;
using PsychologyApp.Application.Insights;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.Models.Tests;
using PsychologyApp.Application.Tests;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Domain.Tests;
using PsychologyApp.Presentation.Body;
using PsychologyApp.Presentation.Features.ManageJournal;
using PsychologyApp.Presentation.Features.ManageProfile;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.ViewModels;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileInsights;

/// <param name="Command">Opens the practice or the test; null when there is nothing to open.</param>
public sealed record InsightItem(string Title, string Detail, string? Note, ICommand? Command, string? ActionText)
{
    public bool HasDetail => Detail.Length > 0;
    public bool HasNote => Note is not null;
    public bool HasAction => Command is not null && ActionText is not null;
}

/// <summary>"What helps me": the rules are in <see cref="HelpInsights"/>; this page loads the data and words the results.</summary>
public sealed class InsightsViewModel : BaseViewModel
{
    /// <summary>The journal entries the mood factors are read from: about three months.</summary>
    private static readonly TimeSpan MoodWindow = TimeSpan.FromDays(90);

    private readonly INavigationService _navigation;
    private readonly IUserProgressService _progress;
    private readonly ITestCatalogService _tests;
    private readonly IBodyService _body;
    private readonly PracticeHistoryFormatter _names;
    private readonly IDatabaseReadySignal _databaseReady;

    private IReadOnlyList<InsightItem> _practices = [];
    private IReadOnlyList<InsightItem> _testItems = [];
    private IReadOnlyList<InsightItem> _moodItems = [];
    private IReadOnlyList<InsightItem> _bodyItems = [];
    private bool _isLoaded;

    public InsightsViewModel(
        INavigationService navigation,
        IUserProgressService progress,
        ITestCatalogService tests,
        IBodyService body,
        PracticeHistoryFormatter names,
        IDatabaseReadySignal databaseReady)
    {
        BindNavigation(navigation);
        _navigation = navigation;
        _progress = progress;
        _tests = tests;
        _body = body;
        _names = names;
        _databaseReady = databaseReady;
    }

    public string PageTitle => AppStrings.InsightsTitle;
    public string LeadText => AppStrings.InsightsLead;
    public string PracticesTitle => AppStrings.InsightsPracticesTitle;
    public string PracticesEmptyText => AppStrings.InsightsPracticesEmpty;
    public string TestsTitle => AppStrings.InsightsTestsTitle;
    public string TestsEmptyText => AppStrings.InsightsTestsEmpty;
    public string MoodTitle => AppStrings.InsightsMoodTitle;
    public string MoodNote => AppStrings.InsightsMoodNote;
    public string MoodEmptyText => AppStrings.InsightsMoodEmpty;
    public string BodyTitle => AppStrings.InsightsBodyTitle;

    public IReadOnlyList<InsightItem> Practices => _practices;
    public IReadOnlyList<InsightItem> Tests => _testItems;
    public IReadOnlyList<InsightItem> Moods => _moodItems;
    public IReadOnlyList<InsightItem> BodyPatterns => _bodyItems;

    public bool HasPractices => _practices.Count > 0;
    public bool HasTests => _testItems.Count > 0;
    public bool HasMoods => _moodItems.Count > 0;
    public bool HasBody => _bodyItems.Count > 0;

    // Empty hints only after loading, so they do not flash before the data arrives.
    public bool ShowPracticesEmpty => _isLoaded && !HasPractices;
    public bool ShowTestsEmpty => _isLoaded && !HasTests;
    public bool ShowMoodsEmpty => _isLoaded && !HasMoods;

    public async Task LoadAsync()
    {
        try
        {
            await _databaseReady.WaitAsync();
            DateTime now = DateTime.UtcNow;

            IReadOnlyList<SessionResultDTO> sessions = await _progress.GetRecentSessionResultsAsync(500);
            List<InsightItem> practices = [];
            foreach (PracticeEffect effect in HelpInsights.PracticeEffects(sessions))
            {
                string name = await _names.ResolveNameAsync(effect.ItemKey);
                practices.Add(new InsightItem(
                    name,
                    AppStrings.InsightsPracticeLine(Number(effect.AverageDrop), effect.Sessions),
                    null,
                    ProfilePracticeHistoryTapFactory.OpenCommand(effect.ItemKey, _navigation),
                    null));
            }

            IReadOnlyList<TestDefinition> catalog = await _tests.GetCatalogAsync();
            Dictionary<string, TestDefinition> definitions = catalog.ToDictionary(test => test.TestId, StringComparer.Ordinal);
            IReadOnlyList<TestResultDTO> results = await _progress.GetAllTestResultsAsync(1000);
            List<InsightItem> tests = [.. HelpInsights
                .TestProgress(
                    results.Where(result => definitions.ContainsKey(result.TestId)),
                    id => definitions[id].ScoreDirection,
                    now)
                .Select(test => TestItem(test, definitions[test.TestId]))];

            IReadOnlyList<MoodEntryDTO> moods = await _progress.GetMoodsAsync(now - MoodWindow, now, 1000);
            List<MoodWithFactors> entries = [.. moods.Select(mood => new MoodWithFactors(
                mood.MoodLevel,
                JournalNoteFactors.AllKeys.Where(key => JournalNoteFactors.HasFactor(mood.Note, key)).ToList()))];
            List<InsightItem> moodItems = [.. HelpInsights.FactorEffects(entries, JournalNoteFactors.AllKeys).Select(effect => new InsightItem(
                effect.Difference > 0
                    ? AppStrings.InsightsMoodUp(JournalNoteFactors.GetLabel(effect.Factor), Number(effect.Difference))
                    : AppStrings.InsightsMoodDown(JournalNoteFactors.GetLabel(effect.Factor), Number(-effect.Difference)),
                string.Empty,
                null,
                null,
                null))];

            BodyLog body = await _body.GetLogAsync();
            List<InsightItem> bodyItems = [.. Application.Body.BodyPatterns.Find(body.Entries, TimeZoneInfo.Local).Select(pattern => new InsightItem(
                BodyLabels.Title(pattern.Area, pattern.Sensation),
                AppStrings.BodyPatternCount(pattern.Count),
                pattern.Feeling is { } feeling ? AppStrings.BodyPatternFeeling(BodyLabels.Feeling(feeling)) : null,
                null,
                null))];

            await UiThread.RunAsync(() =>
            {
                _isLoaded = true;
                _practices = practices;
                _testItems = tests;
                _moodItems = moodItems;
                _bodyItems = bodyItems;
                Notify(
                    nameof(Practices), nameof(Tests), nameof(Moods), nameof(BodyPatterns),
                    nameof(HasPractices), nameof(HasTests), nameof(HasMoods), nameof(HasBody),
                    nameof(ShowPracticesEmpty), nameof(ShowTestsEmpty), nameof(ShowMoodsEmpty));
            });
        }
        catch (Exception loadError)
        {
            // Each section has its own empty hint; a failed load leaves them, never a broken page.
            BestEffort.Report(loadError);
        }
    }

    private InsightItem TestItem(TestProgress test, TestDefinition definition)
    {
        string detail = test.Results > 1
            ? $"{AppStrings.InsightsTestChange(test.First, test.Latest)} · {Trend(test.Trend)}"
            : AppStrings.InsightsTestSingle(test.Latest);
        return new InsightItem(
            definition.Title,
            detail,
            test.RetestDue ? AppStrings.InsightsRetestHint : null,
            test.RetestDue ? new AsyncCommand(() => _navigation.GoToTestAsync(test.TestId)) : null,
            test.RetestDue ? AppStrings.InsightsRetest : null);
    }

    private static string Trend(TestTrendKind trend) => trend switch
    {
        TestTrendKind.Improved => AppStrings.InsightsTrendBetter,
        TestTrendKind.Worse => AppStrings.InsightsTrendWorse,
        _ => AppStrings.InsightsTrendSame
    };

    private static string Number(double value) => value.ToString("0.#", CultureInfo.CurrentCulture);
}
