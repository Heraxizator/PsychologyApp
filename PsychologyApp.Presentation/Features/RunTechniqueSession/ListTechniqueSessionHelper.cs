using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Dialogs;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class ListTechniqueSessionHelper(
    TechniqueSessionCompletionService sessionCompletionService,
    IUserProgressService progress,
    INavigationService navigation,
    IDialogService dialog)
{
    public Task CompleteAsync(
        string techniqueKey,
        string moduleName,
        string pageName,
        DateTime sessionStartedAt) =>
        sessionCompletionService.CompleteStandardSessionAsync(
            progress,
            navigation,
            dialog,
            techniqueKey,
            moduleName,
            pageName,
            sessionStartedAt);
}
