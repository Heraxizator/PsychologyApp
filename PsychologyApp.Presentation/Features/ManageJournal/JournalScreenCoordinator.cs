using PsychologyApp.Presentation.Shared.Navigation;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public interface IJournalHubPage;

public sealed class JournalScreenCoordinator(JournalEditorContext editorContext)
{
    public Task OpenEditorDayAsync(DateOnly day, INavigationService navigation)
    {
        editorContext.PendingEditorDay = day;

        IReadOnlyList<Page> stack = navigation.Navigation.NavigationStack;
        bool hubBelowCurrent = stack.Count >= 2
            && stack.Take(stack.Count - 1).Any(static page => page is IJournalHubPage);

        if (hubBelowCurrent)
        {
            return navigation.GoBackAsync();
        }

        return navigation.GoToJournalAsync();
    }
}
