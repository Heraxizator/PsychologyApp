using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Shared.Common;

public static class PracticeCompletionNavigator
{
    public static async Task NavigateAfterCompletionAsync(
        INavigationService navigation,
        IDialogService dialog,
        IUserProgressService progress)
    {
        int streak = await progress.GetStreakDaysAsync();
        bool goHome = await dialog.AskAsync(
            AppStrings.PracticeCompletedTitle,
            AppStrings.PracticeCompletedBody(streak),
            AppStrings.PracticeGoHomeButton,
            AppStrings.PracticeMoreButton);

        if (goHome)
        {
            await navigation.GoToRootAsync();
        }
        else
        {
            await navigation.GoBackAsync();
        }
    }
}
