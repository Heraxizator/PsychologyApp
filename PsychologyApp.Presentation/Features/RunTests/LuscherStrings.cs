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

        if (coValue < 17)
        {
            return "Noticeable tension and emotional strain. Efficiency may drop; recovery and rest are recommended.";
        }

        if (coValue < 23)
        {
            return "Elevated tension and reduced emotional stability. Sustained effort may feel forced; rest and support are recommended.";
        }

        return "High tension and emotional instability. Significant strain; professional support may be helpful.";
    }

    private static string InterpretBkEn(double bkValue)
    {
        if (bkValue <= 0.4)
        {
            return "Low vegetative coefficient: exhaustion, passivity, and need for substantial recovery.";
        }

        if (bkValue <= 0.8)
        {
            return "Balanced vegetative coefficient: moderate need for rest with enough energy for familiar routines.";
        }

        if (bkValue <= 1.9)
        {
            return "Optimal mobilization: physical and mental resources are well aligned for action.";
        }

        return "High vegetative coefficient: strong emotional arousal and autonomic reactivity.";
    }
}
