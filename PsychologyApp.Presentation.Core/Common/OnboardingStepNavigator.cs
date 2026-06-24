namespace PsychologyApp.Presentation.Core.Common;

public static class OnboardingStepNavigator
{
    public const int FinishStep = 3;
    public const int MaxNextStep = 2;

    public static int GoNext(int step) => step < MaxNextStep ? step + 1 : step;

    public static int GoBack(int step) => step > 0 ? step - 1 : step;
}
