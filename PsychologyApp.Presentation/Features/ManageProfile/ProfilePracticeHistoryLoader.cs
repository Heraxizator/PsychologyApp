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

        return completions
            .Select(completion =>
            {
                string date = completion.CompletedAt.ToLocalTime().ToString("g");
                string name = practiceHistoryFormatter.ResolveName(completion);
                return new PracticeHistoryItem
                {
                    DisplayText = AppStrings.PracticeHistoryEntry(date, name)
                };
            })
            .ToList();
    }
}
