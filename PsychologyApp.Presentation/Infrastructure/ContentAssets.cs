namespace PsychologyApp.Presentation.Infrastructure;

public static class ContentAssets
{
    public static string Localized(string assetPath)
    {
        if (!UserPreferences.IsEnglish(UserPreferences.Load().Language))
        {
            return assetPath;
        }

        int dotIndex = assetPath.LastIndexOf('.');
        if (dotIndex < 0)
        {
            return $"{assetPath}.en";
        }

        return $"{assetPath[..dotIndex]}.en{assetPath[dotIndex..]}";
    }

    public static string PsychosomaticFile =>
        UserPreferences.IsEnglish(UserPreferences.Load().Language)
            ? "Psyhosomatic.en.txt"
            : "Psyhosomatic.txt";

    public static string QuotesFile =>
        UserPreferences.IsEnglish(UserPreferences.Load().Language)
            ? "quotes/quotes.en.json"
            : "quotes/quotes.ru.json";
}
