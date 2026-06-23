namespace PsychologyApp.Presentation.Shared.Common;

public static class ContentAssets
{
    public static string Localized(string assetPath)
    {
        if (!UserPreferences.IsEnglish(AppStrings.Language))
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
        UserPreferences.IsEnglish(AppStrings.Language)
            ? "Psyhosomatic.en.txt"
            : "Psyhosomatic.txt";

    public static string QuotesFile =>
        UserPreferences.IsEnglish(AppStrings.Language)
            ? "quotes/quotes.en.json"
            : "quotes/quotes.ru.json";
}
