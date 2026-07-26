using PsychologyApp.Application.Models;
using PsychologyApp.Domain.UserProgress;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public static class JournalEditorSlotResolver
{
    public static JournalCheckInSlot Resolve(
        JournalCheckInSlot requested,
        MoodEntryDTO? morning,
        MoodEntryDTO? evening,
        int localHourNow)
    {
        if (requested == JournalCheckInSlot.Morning && morning is not null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (requested == JournalCheckInSlot.Evening && evening is not null)
        {
            return JournalCheckInSlot.Evening;
        }

        if (requested == JournalCheckInSlot.Morning && morning is null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (requested == JournalCheckInSlot.Evening && evening is null)
        {
            return JournalCheckInSlot.Evening;
        }

        if (morning is not null)
        {
            return JournalCheckInSlot.Morning;
        }

        if (evening is not null)
        {
            return JournalCheckInSlot.Evening;
        }

        return MoodCheckInSlotPolicy.IsMorningLocalHour(localHourNow)
            ? JournalCheckInSlot.Morning
            : JournalCheckInSlot.Evening;
    }
}
