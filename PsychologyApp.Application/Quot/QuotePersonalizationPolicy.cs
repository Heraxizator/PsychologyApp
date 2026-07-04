using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Quot;

public static class QuotePersonalizationPolicy
{
    public static IReadOnlyList<string> ResolveThemes(string? onboardingConcern) =>
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
