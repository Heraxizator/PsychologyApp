namespace PsychologyApp.Presentation.Shared.Common;

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

    /// <summary>
    /// Focus/blur animations assign local Stroke/BackgroundColor that block AppThemeBinding.
    /// Clear those locals and re-apply chrome colors for the current theme.
    /// </summary>
    public static Task RefreshForThemeAsync(Border border, string variant, bool isFocused)
    {
        border.ClearValue(Border.StrokeProperty);
        border.ClearValue(Border.BackgroundColorProperty);
        border.ClearValue(Border.StrokeThicknessProperty);

        if (variant == VariantPlain)
        {
            return Task.CompletedTask;
        }

        return isFocused
            ? ApplyFocusAsync(border, variant)
            : ApplyBlurAsync(border, variant);
    }
}
