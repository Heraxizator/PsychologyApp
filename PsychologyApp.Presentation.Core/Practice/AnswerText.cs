namespace PsychologyApp.Presentation.Common;

/// <summary>
/// How a practice answer is put together from taps and voice instead of typing: chips toggle in and out as comma-separated parts,
/// dictated words are added after what is already written, and long text is shortened for a one-line chip.
/// </summary>
public static class AnswerText
{
    public const string Separator = ", ";

    /// <summary>True when the chip is already one of the comma-separated parts of the answer (a chip may itself contain a comma).</summary>
    public static bool Contains(string? answer, string chip) => Find(answer, chip) is not null;

    /// <summary>Adds the chip as a new part, or takes it out again when it is already there.</summary>
    public static string Toggle(string? answer, string chip)
    {
        string trimmed = chip?.Trim() ?? string.Empty;
        string current = (answer ?? string.Empty).Trim().TrimEnd(',').TrimEnd();
        if (trimmed.Length == 0)
        {
            return answer ?? string.Empty;
        }

        if (Find(current, trimmed) is not { } match)
        {
            return current.Length == 0 ? trimmed : current + Separator + trimmed;
        }

        string without = current.Remove(match.Index, match.Length);
        return string.Join(Separator, without
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(part => part.Length > 0));
    }

    /// <summary>Dictated words go after what is already written, separated by a space; the first letter of a fresh answer is capitalised.</summary>
    public static string AppendSpoken(string? answer, string? spoken)
    {
        string addition = spoken?.Trim() ?? string.Empty;
        string current = answer?.TrimEnd() ?? string.Empty;
        if (addition.Length == 0)
        {
            return current;
        }

        if (current.Length == 0)
        {
            return char.ToUpper(addition[0], System.Globalization.CultureInfo.CurrentCulture) + addition[1..];
        }

        return $"{current} {addition}";
    }

    /// <summary>A single line for a chip: the start of the text with an ellipsis when it is longer than <paramref name="maxLength"/>.</summary>
    public static string Shorten(string? text, int maxLength = 40)
    {
        string line = string.Join(' ', (text ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        if (line.Length <= maxLength || maxLength < 2)
        {
            return line;
        }

        string cut = line[..(maxLength - 1)].TrimEnd(' ', ',', '.', ';', ':');
        return cut + "…";
    }

    /// <summary>The chip as a whole part of the answer: at the start or after a comma, and followed by a comma or the end.</summary>
    private static System.Text.RegularExpressions.Match? Find(string? answer, string chip)
    {
        string trimmed = chip?.Trim() ?? string.Empty;
        if (trimmed.Length == 0 || string.IsNullOrWhiteSpace(answer))
        {
            return null;
        }

        System.Text.RegularExpressions.Match match = System.Text.RegularExpressions.Regex.Match(
            answer,
            $@"(?<=^|,)\s*{System.Text.RegularExpressions.Regex.Escape(trimmed)}\s*(?=,|$)",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        return match.Success ? match : null;
    }
}
