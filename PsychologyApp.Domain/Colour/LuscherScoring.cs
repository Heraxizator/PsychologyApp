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

    public static int CalculateCoBetweenPasses(
        IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> firstPass,
        IReadOnlyList<(ColourValue Colour, ColourMeaning Meaning)> secondPass)
    {
        ArgumentNullException.ThrowIfNull(firstPass);
        ArgumentNullException.ThrowIfNull(secondPass);

        if (firstPass.Count < 8 || secondPass.Count < 8)
        {
            return 0;
        }

        List<ColourValue> first = firstPass.Select(item => item.Colour).ToList();
        List<ColourValue> second = secondPass.Select(item => item.Colour).ToList();
        int coValue = 0;

        foreach (ColourValue color in first)
        {
            int rank1 = first.FindIndex(item => item.Code == color.Code);
            int rank2 = second.FindIndex(item => item.Code == color.Code);
            coValue += Math.Abs(rank1 - rank2);
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
