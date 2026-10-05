namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string ShellTabMusic => R(nameof(ShellTabMusic));
    public static string ShellTabPractice => R(nameof(ShellTabPractice));
    public static string ShellTabDetector => R(nameof(ShellTabDetector));
    public static string ShellTabSomatic => R(nameof(ShellTabSomatic));
    [Obsolete("Use ShellTabMusic")]
    public static string ShellTabCleaner => ShellTabMusic;
    public static string ShellTabMotivator => R(nameof(ShellTabMotivator));

    public static string ShellTabPracticeShort => R(nameof(ShellTabPracticeShort));
    public static string ShellTabDetectorShort => R(nameof(ShellTabDetectorShort));
    public static string ShellTabSomaticShort => R(nameof(ShellTabSomaticShort));
    public static string ShellTabMusicShort => R(nameof(ShellTabMusicShort));
    [Obsolete("Use ShellTabMusicShort")]
    public static string ShellTabCleanerShort => ShellTabMusicShort;
    public static string ShellTabMotivatorShort => R(nameof(ShellTabMotivatorShort));

}