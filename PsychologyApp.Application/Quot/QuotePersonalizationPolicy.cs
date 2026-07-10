using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Quot;

public static class QuotePersonalizationPolicy
{
    private static readonly string[] LowMoodThemes = ["calm", "acceptance", "hope", "healing"];
    private static readonly string[] HighMoodThemes = ["motivation", "happiness", "growth", "gratitude"];

    public static IReadOnlyList<string> ResolveThemes(string? onboardingConcern) =>
        ResolveThemes(onboardingConcern, todayMoodLevel: null);

    public static IReadOnlyList<string> ResolveThemes(string? onboardingConcern, int? todayMoodLevel)
    {
        if (todayMoodLevel is int mood)
        {
            if (mood <= 2)
            {
                return LowMoodThemes;
            }

            if (mood >= 4)
            {
                return HighMoodThemes;
            }
        }

        return ResolveConcernThemes(onboardingConcern);
    }

    private static IReadOnlyList<string> ResolveConcernThemes(string? onboardingConcern) =>
        onboardingConcern switch
        {
            OnboardingConcernKeys.Anxiety => ["anxiety", "calm", "acceptance", "mindfulness"],
            OnboardingConcernKeys.Mood => ["happiness", "hope", "motivation", "self-love"],
            OnboardingConcernKeys.Body => ["mindfulness", "calm", "healing", "habits"],
            _ => ["wisdom", "general", "growth", "self-awareness"]
        };

    public static int ResolveDailyQuoteIndex(DateOnly date, int catalogCount)
    {
        if (catalogCount <= 0)
        {
            return 0;
        }

        int hash = HashCode.Combine(date.Year, date.Month, date.Day);
        return Math.Abs(hash) % catalogCount;
    }
}
