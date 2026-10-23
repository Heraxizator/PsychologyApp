namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// Which packaged asset holds the content for the interface language. Russian is the original (<c>beck.json</c>), every
/// other language has a suffix (<c>beck.en.json</c>, <c>beck.fr.json</c>). A language whose file is not written yet reads
/// the English one, so a missing translation never leaves a screen empty.
/// </summary>
public static class ContentAssets
{
    /// <summary>The files to try in order: the language's own, then English, then the Russian original.</summary>
    public static IReadOnlyList<string> Candidates(string assetPath)
    {
        string language = AppLanguages.Normalize(AppStrings.Language);
        if (language == AppLanguages.Russian)
        {
            return [assetPath];
        }

        return language == AppLanguages.English
            ? [WithSuffix(assetPath, AppLanguages.English), assetPath]
            : [WithSuffix(assetPath, language), WithSuffix(assetPath, AppLanguages.English), assetPath];
    }

    /// <summary>The quote files to try in order; the Russian one has its own suffix, so English is the last resort here.</summary>
    public static IReadOnlyList<string> QuotesFiles
    {
        get
        {
            string language = AppLanguages.Normalize(AppStrings.Language);
            return language switch
            {
                AppLanguages.Russian => ["quotes/quotes.ru.json"],
                AppLanguages.English => ["quotes/quotes.en.json"],
                _ => [$"quotes/quotes.{language}.json", "quotes/quotes.en.json"]
            };
        }
    }

    private static string WithSuffix(string assetPath, string language)
    {
        int dotIndex = assetPath.LastIndexOf('.');
        return dotIndex < 0
            ? $"{assetPath}.{language}"
            : $"{assetPath[..dotIndex]}.{language}{assetPath[dotIndex..]}";
    }
}
