using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Profile;

namespace PsychologyApp.Presentation.Features.ManageProfile;

public sealed class ProfilePracticeHistoryLoader(
    IUserProgressService userProgressService,
    PracticeHistoryFormatter practiceHistoryFormatter)
{
    public async Task<IReadOnlyList<PracticeHistoryItem>> LoadAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SessionResultDTO> sessionResults =
            await userProgressService.GetRecentSessionResultsAsync(count, cancellationToken);

        List<PracticeHistoryItem> items = [];
        foreach (SessionResultDTO result in sessionResults)
        {
            string date = result.CompletedAt.ToLocalTime().ToString("g");
            string name = await practiceHistoryFormatter.ResolveNameAsync(result.ItemKey, cancellationToken: cancellationToken);
            (string durationText, bool hasDuration) = practiceHistoryFormatter.ResolveDuration(result);
            (string sudsDeltaText, bool hasSudsDelta) = practiceHistoryFormatter.ResolveSudsDelta(result);
            items.Add(new PracticeHistoryItem
            {
                DateText = date,
                TechniqueName = name,
                IconName = await practiceHistoryFormatter.ResolveIconAsync(result.ItemKey, cancellationToken),
                DurationText = durationText,
                HasDuration = hasDuration,
                ItemKey = result.ItemKey,
                SudsDeltaText = sudsDeltaText,
                HasSudsDelta = hasSudsDelta,
                DisplayText = AppStrings.PracticeHistoryEntry(date, name)
            });
        }

        return items;
    }
}
