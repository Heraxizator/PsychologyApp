using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Entities.Technique;
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
        IReadOnlyList<CompletionDTO> completions =
            await userProgressService.GetRecentTechniqueCompletionsAsync(count, cancellationToken);

        List<PracticeHistoryItem> items = [];
        foreach (CompletionDTO completion in completions)
        {
            string date = completion.CompletedAt.ToLocalTime().ToString("g");
            string name = await practiceHistoryFormatter.ResolveNameAsync(completion, cancellationToken);
            (string durationText, bool hasDuration) = practiceHistoryFormatter.ResolveDuration(completion);
            items.Add(new PracticeHistoryItem
            {
                DateText = date,
                TechniqueName = name,
                IconName = await practiceHistoryFormatter.ResolveIconAsync(completion, cancellationToken),
                DurationText = durationText,
                HasDuration = hasDuration,
                ItemKey = completion.ItemKey,
                DisplayText = AppStrings.PracticeHistoryEntry(date, name)
            });
        }

        return items;
    }
}
