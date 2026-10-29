using System.Text.RegularExpressions;

namespace PsychologyApp.Presentation.Entities.Test;

/// <summary>
/// One step of a test's instruction. The texts number themselves ("1. Read…"); the number is taken out to stand in its own circle,
/// so the steps read as steps rather than a block of grey text.
/// </summary>
public partial class AlgorithmRow
{
    public string Text { get; set; } = default!;

    /// <summary>The step's number: the one the text starts with, or its place in the list when it has none.</summary>
    public string Number { get; init; } = string.Empty;

    /// <summary>The step without its leading number.</summary>
    public string Body { get; init; } = string.Empty;

    public static AlgorithmRow From(string text, int index)
    {
        Match numbered = LeadingNumber().Match(text);
        return numbered.Success
            ? new AlgorithmRow { Text = text, Number = numbered.Groups[1].Value, Body = numbered.Groups[2].Value }
            : new AlgorithmRow { Text = text, Number = (index + 1).ToString(System.Globalization.CultureInfo.InvariantCulture), Body = text.Trim() };
    }

    [GeneratedRegex(@"^\s*(\d+)[.)]\s*(.*)$", RegexOptions.Singleline)]
    private static partial Regex LeadingNumber();
}
