using PsychologyApp.Presentation.Features.ManageJournal;

namespace PsychologyApp.Presentation.Features.ManageJournal.Index;

/// <summary>
/// Public entry point for the ManageJournal slice.
/// Cross-slice consumers open Journal via <c>GoToJournalAsync</c> / <c>GoToJournalDayAsync</c>.
/// Day handoff uses <see cref="JournalEditorContext"/> through <see cref="JournalScreenCoordinator"/>.
/// </summary>
public static class ManageJournalPublicApi;
