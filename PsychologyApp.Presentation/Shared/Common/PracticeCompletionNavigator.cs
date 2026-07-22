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
        int streak = await progress.GetStreakDaysAsync();
        await navigation.GoToPracticeCompletionAsync(streak, completedItemKey, sessionResultId);
    }
}
