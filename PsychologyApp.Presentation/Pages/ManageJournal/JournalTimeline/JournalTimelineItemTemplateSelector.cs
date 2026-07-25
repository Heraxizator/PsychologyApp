using PsychologyApp.Presentation.Entities.Journal;
using PsychologyApp.Presentation.Entities.Profile;

namespace PsychologyApp.Presentation.Pages.ManageJournal.JournalTimeline;

public sealed class JournalTimelineItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate? HeaderTemplate { get; set; }
    public DataTemplate? EntryTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) =>
        item switch
        {
            JournalTimelineHeaderItem when HeaderTemplate is not null => HeaderTemplate,
            MoodNoteItem when EntryTemplate is not null => EntryTemplate,
            _ => EntryTemplate ?? HeaderTemplate ?? new DataTemplate()
        };
}
