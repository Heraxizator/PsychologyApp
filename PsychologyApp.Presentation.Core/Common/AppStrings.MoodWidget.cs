namespace PsychologyApp.Presentation.Common;

/// <summary>Copy of the home-screen mood widget.</summary>
public static partial class AppStrings
{
    public static string MoodWidgetTitle => R(nameof(MoodWidgetTitle));
    public static string MoodWidgetLevel(object arg0) => F(nameof(MoodWidgetLevel), arg0);
    public static string MoodWidgetPractice => R(nameof(MoodWidgetPractice));
}
