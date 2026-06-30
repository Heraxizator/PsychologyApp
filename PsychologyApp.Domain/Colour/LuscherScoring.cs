using PsychologyApp.Domain.Colour.ValueObjects;

namespace PsychologyApp.Domain.Colour;

public static class LuscherScoring
{
    private static readonly IReadOnlyDictionary<string, int> ColourWeights = new Dictionary<string, int>
    {
        [ColourValue.Red.Code] = 1,
        [ColourValue.Yellow.Code] = 2,
        [ColourValue.Green.Code] = 3,
        [ColourValue.Purple.Code] = 4,
        [ColourValue.Blue.Code] = 5,
        [ColourValue.Brown.Code] = 6,
        [ColourValue.Gray.Code] = 7,
        [ColourValue.Black.Code] = 8
    };

    public static int CalculateCo(IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> selections)
    {
        ArgumentNullException.ThrowIfNull(selections);
        if (selections.Count < 2)
        {
            return 0;
        }

        int coValue = 0;
        for (int index = 1; index < selections.Count; index++)
        {
            string code = selections[index - 1].Colour.Code;
            int answer = ColourWeights[code];
            int expected = ColourWeights.ElementAt(index - 1).Value;
            coValue += Math.Abs(answer - expected);
        }

        return coValue;
    }

    public static double CalculateBk(IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> selections)
    {
        ArgumentNullException.ThrowIfNull(selections);

        List<ColourValue> colourValues = selections.Select(x => x.Colour).ToList();

        double redValue = colourValues.FindIndex(x => x.Code == ColourValue.Red.Code);
        double yellowValue = colourValues.FindIndex(x => x.Code == ColourValue.Yellow.Code);
        double blueValue = colourValues.FindIndex(x => x.Code == ColourValue.Blue.Code);
        double greenValue = colourValues.FindIndex(x => x.Code == ColourValue.Green.Code);

        double denominator = 18 - blueValue - greenValue;
        if (denominator == 0)
        {
            return 0;
        }

        return (18 - redValue - yellowValue) / denominator;
    }
}
