namespace PsychologyApp.Presentation.Common;

public static class InputFieldChrome
{
    public const string VariantDefault = "Default";
    public const string VariantPlain = "Plain";

    public static async Task ApplyFocusAsync(Border border, string variant = VariantDefault)
    {
        if (variant == VariantPlain)
        {
            return;
        }

        await UiAnimations.SafeInputFocusAsync(border, focused: true);
    }

    public static async Task ApplyBlurAsync(Border border, string variant = VariantDefault)
    {
        if (variant == VariantPlain)
        {
            return;
        }

        await UiAnimations.SafeInputFocusAsync(border, focused: false);
    }
}
