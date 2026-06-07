namespace PsychologyApp.Presentation.Infrastructure;

public static class LocalizedContentView
{
    public static void SubscribePreferences(BindableObject view, Action refresh) =>
        UserPreferences.Changed += refresh;
}
