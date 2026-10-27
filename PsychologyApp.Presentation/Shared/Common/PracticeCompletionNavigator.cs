using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Shared.Common;

public static class PracticeCompletionNavigator
{
    public static async Task NavigateAfterCompletionAsync(
        INavigationService navigation,
        IUserProgressService progress,
        string? completedItemKey = null,
        long? sessionResultId = null)
    {
        int activeDays = await progress.GetActiveDaysThisWeekAsync();
        await navigation.GoToPracticeCompletionAsync(activeDays, completedItemKey, sessionResultId);
    }
}
