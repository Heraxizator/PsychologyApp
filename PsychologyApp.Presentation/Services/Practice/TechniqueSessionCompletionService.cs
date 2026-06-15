using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;

namespace PsychologyApp.Presentation.Services.Practice;

public sealed class TechniqueSessionCompletionService
{
    public async Task CompleteStandardSessionAsync(
        IUserProgressService progress,
        INavigationService navigation,
        IDialogService dialog,
        string itemKey,
        string moduleName,
        string pageName,
        DateTime sessionStartedAt,
        bool deleteDraft = true,
        CancellationToken cancellationToken = default)
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - sessionStartedAt).TotalSeconds);
        await progress.RecordTechniqueCompletionAsync(
            itemKey,
            moduleName,
            pageName,
            durationSeconds,
            cancellationToken);

        if (deleteDraft)
        {
            await progress.DeleteSessionDraftAsync(itemKey, cancellationToken);
        }

        await PracticeCompletionNavigator.NavigateAfterCompletionAsync(
            navigation,
            dialog,
            progress);
    }
}
