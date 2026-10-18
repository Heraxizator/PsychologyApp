namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string ShellTabMusic => R(nameof(ShellTabMusic));
    public static string ShellTabPractice => R(nameof(ShellTabPractice));
    public static string ShellTabDetector => R(nameof(ShellTabDetector));
    public static string ShellTabArticles => R(nameof(ShellTabArticles));
    [Obsolete("Use ShellTabMusic")]
    public static string ShellTabCleaner => ShellTabMusic;
    public static string ShellTabMotivator => R(nameof(ShellTabMotivator));

    public static string ShellTabPracticeShort => R(nameof(ShellTabPracticeShort));
    public static string ShellTabDetectorShort => R(nameof(ShellTabDetectorShort));
    public static string ShellTabArticlesShort => R(nameof(ShellTabArticlesShort));
    public static string ShellTabMusicShort => R(nameof(ShellTabMusicShort));
    [Obsolete("Use ShellTabMusicShort")]
    public static string ShellTabCleanerShort => ShellTabMusicShort;
    public static string ShellTabMotivatorShort => R(nameof(ShellTabMotivatorShort));
    public static string ShellTabChatShort => R(nameof(ShellTabChatShort));
    public static string ShellTabLibraryShort => R(nameof(ShellTabLibraryShort));
    public static string ShellTabJournalShort => R(nameof(ShellTabJournalShort));
    public static string ForYouEmpty => R(nameof(ForYouEmpty));
    public static string ForYouTakeTest => R(nameof(ForYouTakeTest));
    public static string ForYouSavedTitle => R(nameof(ForYouSavedTitle));

}