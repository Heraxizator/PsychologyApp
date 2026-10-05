namespace PsychologyApp.Application.Common;

/// <summary>
/// Forgiving text search for a Russian-language catalogue: case-insensitive, "ё" is "е", and a word matches its other forms
/// ("тревога" finds "тревоги", "тревогу", "тревожный" does not, but "тревож" does). Every word of the query must be found.
/// Not a morphology engine: each query word is cut by its likely ending, so short words are matched exactly as typed.
/// </summary>
public static class SearchText
{
    public static string Normalize(string? text) =>
        string.IsNullOrEmpty(text) ? string.Empty : text.ToLowerInvariant().Replace('ё', 'е');

    /// <summary>The parts of the query that must all occur in the text.</summary>
    public static IReadOnlyList<string> Stems(string query) =>
        Normalize(query)
            .Split([' ', ',', '.', ';', ':', '!', '?', '-', '—', '"', '«', '»', '(', ')'], StringSplitOptions.RemoveEmptyEntries)
            .Select(Stem)
            .ToList();

    public static bool Matches(string? text, IReadOnlyList<string> stems)
    {
        if (stems.Count == 0 || string.IsNullOrEmpty(text))
        {
            return false;
        }

        string normalized = Normalize(text);
        return stems.All(stem => normalized.Contains(stem, StringComparison.Ordinal));
    }

    private static string Stem(string word) => word.Length switch
    {
        >= 7 => word[..^2],
        >= 5 => word[..^1],
        _ => word
    };
}
