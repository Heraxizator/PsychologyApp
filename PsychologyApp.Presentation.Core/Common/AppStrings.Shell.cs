namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string ShellTabMusic => T("Музыка", "Music");
    public static string ShellTabPractice => T("Практик", "Practice");
    public static string ShellTabDetector => T("Детектор", "Detector");
    public static string ShellTabSomatic => T("Соматик", "Somatic");
    [Obsolete("Use ShellTabMusic")]
    public static string ShellTabCleaner => ShellTabMusic;
    public static string ShellTabMotivator => T("Мотиватор", "Motivator");

    public static string ShellTabPracticeShort => T("Практик", "Practice");
    public static string ShellTabDetectorShort => T("Тесты", "Tests");
    public static string ShellTabSomaticShort => T("Тело", "Body");
    public static string ShellTabMusicShort => T("Молитвы", "Prayers");
    [Obsolete("Use ShellTabMusicShort")]
    public static string ShellTabCleanerShort => ShellTabMusicShort;
    public static string ShellTabMotivatorShort => T("Цитаты", "Quotes");

}