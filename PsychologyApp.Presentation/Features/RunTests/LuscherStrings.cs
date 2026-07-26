using PsychologyApp.Domain.Colour;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.Features.RunTests;

public static class LuscherStrings
{
    public static string InterpretCo(int coValue) =>
        LuscherInterpretationBands.ResolveCo(coValue) switch
        {
            LuscherCoBand.Stable => AppStrings.LuscherCoStable,
            LuscherCoBand.MildTension => AppStrings.LuscherCoMildTension,
            LuscherCoBand.ModerateTension => AppStrings.LuscherCoModerateTension,
            LuscherCoBand.ElevatedTension => AppStrings.LuscherCoElevatedTension,
            _ => AppStrings.LuscherCoHighTension
        };

    public static string InterpretBk(double bkValue) =>
        LuscherInterpretationBands.ResolveBk(bkValue) switch
        {
            LuscherBkBand.Exhausted => AppStrings.LuscherBkExhausted,
            LuscherBkBand.Conserving => AppStrings.LuscherBkConserving,
            LuscherBkBand.Optimal => AppStrings.LuscherBkOptimal,
            _ => AppStrings.LuscherBkOveraroused
        };
}
