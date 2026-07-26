namespace PsychologyApp.Domain.Colour;

public enum LuscherCoBand
{
    Stable,
    MildTension,
    ModerateTension,
    ElevatedTension,
    HighTension
}

public enum LuscherBkBand
{
    Exhausted,
    Conserving,
    Optimal,
    Overaroused
}

public static class LuscherInterpretationBands
{
    public static LuscherCoBand ResolveCo(int coValue)
    {
        if (coValue < 6)
        {
            return LuscherCoBand.Stable;
        }

        if (coValue < 12)
        {
            return LuscherCoBand.MildTension;
        }

        if (coValue < 17)
        {
            return LuscherCoBand.ModerateTension;
        }

        if (coValue < 23)
        {
            return LuscherCoBand.ElevatedTension;
        }

        return LuscherCoBand.HighTension;
    }

    public static LuscherBkBand ResolveBk(double bkValue)
    {
        if (bkValue <= 0.4)
        {
            return LuscherBkBand.Exhausted;
        }

        if (bkValue <= 0.8)
        {
            return LuscherBkBand.Conserving;
        }

        if (bkValue <= 1.9)
        {
            return LuscherBkBand.Optimal;
        }

        return LuscherBkBand.Overaroused;
    }
}
