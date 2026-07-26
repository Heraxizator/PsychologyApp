namespace PsychologyApp.Domain.UserProgress;

/// <summary>
/// Local-time boundary between morning and evening journal check-in slots.
/// </summary>
public static class MoodCheckInSlotPolicy
{
    public const int MorningHourCutoff = 15;

    public static bool IsMorningLocalHour(int hour) => hour < MorningHourCutoff;

    public static bool IsEveningLocalHour(int hour) => hour >= MorningHourCutoff;
}
