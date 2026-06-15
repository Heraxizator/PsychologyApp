using PsychologyApp.Application.Models;
using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Models.Practice;
using PsychologyApp.Presentation.Models.Profile;

namespace PsychologyApp.Presentation.Services.Profile;

public sealed class ProfilePracticeHistoryLoader(IUserProgressService userProgressService)
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
                string name = PracticeHistoryFormatter.ResolveName(completion);
                return new PracticeHistoryItem
                {
                    DisplayText = AppStrings.PracticeHistoryEntry(date, name)
                };
            })
            .ToList();
    }
}
