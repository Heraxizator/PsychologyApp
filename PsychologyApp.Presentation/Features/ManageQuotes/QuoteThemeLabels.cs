using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.ManageQuotes;

public static class QuoteThemeLabels
{
    public static IReadOnlyList<(string Key, string Title)> GetFilterThemes() =>
    [
        ("wisdom", GetLabel("wisdom")),
        ("motivation", GetLabel("motivation")),
        ("resilience", GetLabel("resilience")),
        ("calm", GetLabel("calm")),
        ("anxiety", GetLabel("anxiety")),
        ("mindfulness", GetLabel("mindfulness")),
        ("hope", GetLabel("hope")),
        ("self-awareness", GetLabel("self-awareness")),
        ("relationships", GetLabel("relationships")),
        ("habits", GetLabel("habits"))
    ];

    public static string GetLabel(string? themeSlug) =>
        themeSlug switch
        {
            "wisdom" => AppStrings.QuoteThemeWisdom,
            "motivation" => AppStrings.QuoteThemeMotivation,
            "resilience" => AppStrings.QuoteThemeResilience,
            "self-awareness" => AppStrings.QuoteThemeSelfAwareness,
            "mindfulness" => AppStrings.QuoteThemeMindfulness,
            "self-esteem" => AppStrings.QuoteThemeSelfEsteem,
            "hope" => AppStrings.QuoteThemeHope,
            "empathy" => AppStrings.QuoteThemeEmpathy,
            "happiness" => AppStrings.QuoteThemeHappiness,
            "habits" => AppStrings.QuoteThemeHabits,
            "love" => AppStrings.QuoteThemeLove,
            "relationships" => AppStrings.QuoteThemeRelationships,
            "responsibility" => AppStrings.QuoteThemeResponsibility,
            "purpose" => AppStrings.QuoteThemePurpose,
            "growth" => AppStrings.QuoteThemeGrowth,
            "healing" => AppStrings.QuoteThemeHealing,
            "self-love" => AppStrings.QuoteThemeSelfLove,
            "acceptance" => AppStrings.QuoteThemeAcceptance,
            "gratitude" => AppStrings.QuoteThemeGratitude,
            "calm" => AppStrings.QuoteThemeCalm,
            "anxiety" => AppStrings.QuoteThemeAnxiety,
            "general" => AppStrings.QuoteThemeGeneral,
            _ => AppStrings.QuoteThemeGeneral
        };
}
