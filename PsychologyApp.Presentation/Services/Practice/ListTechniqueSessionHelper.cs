using PsychologyApp.Application.Services.UserProgress;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Dialogs;

namespace PsychologyApp.Presentation.Services.Practice;

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
