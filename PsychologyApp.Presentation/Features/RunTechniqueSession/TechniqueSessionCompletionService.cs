using PsychologyApp.Application.ClinicalCare;
using PsychologyApp.Application.Models;
using PsychologyApp.Application.UserProgress;
using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Shared.Services.Notifications;

namespace PsychologyApp.Presentation.Features.RunTechniqueSession;

public sealed class TechniqueSessionCompletionService(
    IPracticeReminderCoordinator practiceReminderCoordinator,
    IClinicalCareService clinicalCareService)
{
    public async Task CompleteStandardSessionAsync(
        IUserProgressService progress,
        INavigationService navigation,
        string itemKey,
        string moduleName,
        string pageName,
        DateTime sessionStartedAt,
        int? preIntensity = null,
        bool deleteDraft = true,
        CancellationToken cancellationToken = default)
    {
        int durationSeconds = Math.Max(0, (int)(DateTime.UtcNow - sessionStartedAt).TotalSeconds);
        string? draftJson = await progress.GetSessionDraftAsync(itemKey, cancellationToken);

        string? programType = null;
        int? programWeek = null;
        try
        {
            TherapyProgramStateDTO? program = await clinicalCareService.GetActiveProgramAsync(cancellationToken);
            if (program is { IsActive: true })
            {
                programType = program.ProgramType.ToString();
                programWeek = program.CurrentWeek;
            }
        }
        catch
        {
            // Program context is optional for session results.
        }

        long sessionResultId = await progress.RecordSessionOutcomeAsync(
            new SessionOutcomeRequest
            {
                ItemKey = itemKey,
                ModuleName = moduleName,
                PageName = pageName,
                DurationSeconds = durationSeconds,
                PayloadJson = draftJson,
                PreIntensity = preIntensity,
                ProgramType = programType,
                ProgramWeek = programWeek,
                DeleteDraft = deleteDraft
            },
            cancellationToken);

        await PracticeCompletionNavigator.NavigateAfterCompletionAsync(
            navigation,
            progress,
            itemKey,
            sessionResultId);

        await practiceReminderCoordinator.SyncAsync(cancellationToken);
    }
}
