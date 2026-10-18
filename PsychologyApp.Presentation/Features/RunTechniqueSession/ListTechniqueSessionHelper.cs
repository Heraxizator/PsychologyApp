using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class ListTechniqueSessionHelper(
    TechniqueSessionCompletionService sessionCompletionService,
    IUserProgressService progress,
    INavigationService navigation)
{
    public Task CompleteAsync(
        string techniqueKey,
        string moduleName,
        string pageName,
        DateTime sessionStartedAt,
        int? preIntensity = null,
        int? postIntensity = null) =>
        sessionCompletionService.CompleteStandardSessionAsync(
            progress,
            navigation,
            techniqueKey,
            moduleName,
            pageName,
            sessionStartedAt,
            preIntensity,
            postIntensity: postIntensity);
}
