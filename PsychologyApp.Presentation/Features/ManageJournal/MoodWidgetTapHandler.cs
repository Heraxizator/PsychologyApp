using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Common.Infrastructure;
using PsychologyApp.Presentation.Shared.Lib.Navigation;

namespace PsychologyApp.Presentation.Features.ManageJournal;

public static class MoodWidgetConstants
{
    public const string ActionMood = "com.subconscious.psychologyapp.WIDGET_MOOD";
    public const string ActionPractice = "com.subconscious.psychologyapp.WIDGET_PRACTICE";
    public const string ExtraMoodLevel = "mood_level";
    public const int MinMoodLevel = 1;
    public const int MaxMoodLevel = 5;
}

/// <summary>
/// A face tapped on the home-screen widget is a check-in: it is saved at once and the journal opens on it, where a note or
/// factors can be added. It goes through the journal's own saving, so a second tap replaces the current slot's check-in instead of adding one.
/// </summary>
public static class MoodWidgetTapHandler
{
    private static IShellTabNavigator? _navigator;
    private static JournalMoodLoader? _journal;
    private static IDatabaseReadySignal? _databaseReady;

    public static void Configure(IShellTabNavigator navigator, JournalMoodLoader journal, IDatabaseReadySignal databaseReady)
    {
        _navigator = navigator;
        _journal = journal;
        _databaseReady = databaseReady;
    }

    public static void HandleMood(int moodLevel)
    {
        if (moodLevel is < MoodWidgetConstants.MinMoodLevel or > MoodWidgetConstants.MaxMoodLevel)
        {
            return;
        }

        RecordAndOpenJournalAsync(moodLevel).FireAndForget();
    }

    public static void HandlePractice() => _navigator?.OpenPracticeTab();

    private static async Task RecordAndOpenJournalAsync(int moodLevel)
    {
        if (_journal is null || _databaseReady is null)
        {
            return;
        }

        await _databaseReady.WaitAsync();
        await _journal.SaveNowAsync(moodLevel);
        _navigator?.OpenJournal();
    }
}
