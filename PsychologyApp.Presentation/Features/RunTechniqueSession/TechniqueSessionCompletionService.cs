using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

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
