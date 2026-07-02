using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class LuscherStrings
{
    public static string InterpretCo(int coValue)
    {
        if (UserPreferences.IsEnglish(AppStrings.Language))
        {
            return InterpretCoEn(coValue);
        }

        return LuscherInterpretationsRu.InterpretCo(coValue);
    }

    public static string InterpretBk(double bkValue)
    {
        if (UserPreferences.IsEnglish(AppStrings.Language))
        {
            return InterpretBkEn(bkValue);
        }

        return LuscherInterpretationsRu.InterpretBk(bkValue);
    }

    private static string InterpretCoEn(int coValue)
    {
        if (coValue < 6)
        {
            return "No unproductive tension; high emotional stability. Actions are purposeful and efficient. Overall mood is optimistic.";
        }

        if (coValue < 12)
        {
            return "Moderate tension that may reduce efficiency. Emotional stability is generally adequate with occasional strain.";
        }

        if (coValue < 18)
        {
            return "Noticeable tension and emotional strain. Efficiency may drop; recovery and rest are recommended.";
        }

        return "High tension and emotional instability. Significant strain; professional support may be helpful.";
    }

    private static string InterpretBkEn(double bkValue)
    {
        if (bkValue < 0.8)
        {
            return "Low vegetative coefficient: restrained emotional expression, possible internal tension.";
        }

        if (bkValue < 1.2)
        {
            return "Balanced vegetative coefficient: emotions and self-regulation are relatively stable.";
        }

        if (bkValue < 1.6)
        {
            return "Elevated vegetative coefficient: increased emotional reactivity and stress sensitivity.";
        }

        return "High vegetative coefficient: strong emotional arousal and autonomic reactivity.";
    }
}
