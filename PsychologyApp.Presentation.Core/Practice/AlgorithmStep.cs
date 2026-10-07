using System.Text.RegularExpressions;

namespace PsychologyApp.Presentation.Common;

/// <summary>A step of a practice as it is stored ("2. Inhale for 4 counts") split into its number and its words, so the number can be drawn as a badge.</summary>
public sealed partial record AlgorithmStep(int? Number, string Text)
{
    public static AlgorithmStep Parse(string? raw)
    {
        string text = raw?.Trim() ?? string.Empty;
        Match match = NumberedStep().Match(text);
        return match.Success && int.TryParse(match.Groups[1].Value, out int number)
            ? new AlgorithmStep(number, match.Groups[2].Value.Trim())
            : new AlgorithmStep(null, text);
    }

    [GeneratedRegex(@"^(\d{1,2})\s*[\.\)]\s+(.+)$", RegexOptions.Singleline)]
    private static partial Regex NumberedStep();
}
