namespace PsychologyApp.Presentation.Features.ManageJournal;

public sealed class JournalEditorContext
{
    public DateOnly? PendingEditorDay { get; set; }

    public DateOnly? ConsumePendingEditorDay()
    {
        DateOnly? day = PendingEditorDay;
        PendingEditorDay = null;
        return day;
    }
}
