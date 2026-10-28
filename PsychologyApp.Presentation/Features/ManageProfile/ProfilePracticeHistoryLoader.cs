using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Profile;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfilePracticeHistoryLoader(
    IUserProgressService userProgressService,
    PracticeHistoryFormatter practiceHistoryFormatter)
{
    // Off the UI thread: building the snapshot after each query would otherwise run there.
    public Task<IReadOnlyList<PracticeHistoryItem>> LoadAsync(int count = 10, CancellationToken cancellationToken = default) =>
        Task.Run(() => LoadCoreAsync(count, cancellationToken), cancellationToken);

    /// <summary>
    /// The profile's recent practices: one row per practice, its latest run with "×4" when it was done more than once. Four rows of the
    /// same breathing exercise said less than one. The full history keeps every run.
    /// </summary>
    public Task<IReadOnlyList<PracticeHistoryItem>> LoadByPracticeAsync(int practices = 4, CancellationToken cancellationToken = default) =>
        Task.Run(async () =>
        {
            IReadOnlyList<SessionResultDTO> sessionResults =
                await userProgressService.GetRecentSessionResultsAsync(LookbackRuns, cancellationToken);
            List<PracticeHistoryItem> items = [];
            foreach ((SessionResultDTO latest, int runs) in GroupByPractice(sessionResults, practices))
            {
                items.Add(await ToItemAsync(latest, cancellationToken, runs));
            }

            return (IReadOnlyList<PracticeHistoryItem>)items;
        }, cancellationToken);

    private const int LookbackRuns = 50;

    /// <summary>The latest run of each practice, newest first, with how many runs it had among those given.</summary>
    internal static IReadOnlyList<(SessionResultDTO Latest, int Runs)> GroupByPractice(IReadOnlyList<SessionResultDTO> results, int practices) =>
        results
            .OrderByDescending(result => result.CompletedAt)
            .GroupBy(result => result.ItemKey)
            .Select(group => (group.First(), group.Count()))
            .Take(practices)
            .ToList();

    private async Task<IReadOnlyList<PracticeHistoryItem>> LoadCoreAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SessionResultDTO> sessionResults =
            await userProgressService.GetRecentSessionResultsAsync(count, cancellationToken);

        List<PracticeHistoryItem> items = [];
        foreach (SessionResultDTO result in sessionResults)
        {
            items.Add(await ToItemAsync(result, cancellationToken));
        }

        return items;
    }

    /// <param name="runs">How many runs this row stands for; above one it is said after the date.</param>
    private async Task<PracticeHistoryItem> ToItemAsync(SessionResultDTO result, CancellationToken cancellationToken, int runs = 1)
    {
        string date = result.CompletedAt.ToLocalTime().ToString("g");
        string name = await practiceHistoryFormatter.ResolveNameAsync(result.ItemKey, cancellationToken: cancellationToken);
        (string durationText, bool hasDuration) = practiceHistoryFormatter.ResolveDuration(result);
        (string sudsDeltaText, bool hasSudsDelta) = practiceHistoryFormatter.ResolveSudsDelta(result);
        return new PracticeHistoryItem
        {
            DateText = runs > 1 ? date + " · ×" + runs.ToString(System.Globalization.CultureInfo.InvariantCulture) : date,
            TechniqueName = name,
            IconName = await practiceHistoryFormatter.ResolveIconAsync(result.ItemKey, cancellationToken),
            DurationText = durationText,
            HasDuration = hasDuration,
            ItemKey = result.ItemKey,
            SudsDeltaText = sudsDeltaText,
            HasSudsDelta = hasSudsDelta,
            DisplayText = AppStrings.PracticeHistoryEntry(date, name)
        };
    }
}
