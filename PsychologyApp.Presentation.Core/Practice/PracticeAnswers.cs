using System.Text.Json;
using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Presentation.Common;

/// <param name="Title">The question, e.g. "Automatic thought".</param>
/// <param name="Answer">What the person answered, e.g. "They'll judge me", or "7 / 10" for a scale.</param>
public sealed record PracticeAnswer(string Title, string Answer);

/// <summary>Small pure rules behind the practice screens: the order of chips, the answers shown after a practice, the step to resume at.</summary>
public static class PracticeAnswers
{
    /// <summary>
    /// Chips the person picks often come first; the rest keep their written order. Usage is counted on the device only,
    /// so someone who sits at a desk sees "Computer" before "Sky".
    /// </summary>
    public static IReadOnlyList<string> Rank(IReadOnlyList<string> chips, IReadOnlyDictionary<string, int> usage) =>
        chips
            .Select((chip, index) => (chip, index, uses: usage.TryGetValue(chip, out int count) ? count : 0))
            .OrderByDescending(item => item.uses)
            .ThenBy(item => item.index)
            .Select(item => item.chip)
            .ToList();

    /// <summary>How many times a typed answer has to recur before it becomes a chip of its own.</summary>
    public const int OwnChipMinUses = 2;

    /// <summary>At most this many of the person's own chips per question, so they add to the list instead of taking it over.</summary>
    public const int OwnChipMax = 4;

    private const int OwnChipMaxLength = 32;
    private const int OwnChipMaxWords = 4;
    private const char OwnKeySeparator = '\u001F';

    /// <summary>
    /// The short pieces of a typed answer that could become chips: "Кот, чашка чая" gives "Кот" and "Чашка чая". Pieces already offered
    /// as chips, long sentences and stray letters are left out; the first letter is raised, as on the ready chips.
    /// </summary>
    public static IReadOnlyList<string> TypedParts(string? answer, IReadOnlyList<string> offered)
    {
        if (string.IsNullOrWhiteSpace(answer))
        {
            return [];
        }

        return answer
            .Split([',', ';', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part => part.Trim(' ', '.', '!', '?', '…', '-', '—', '"', '«', '»'))
            .Where(part => part.Length >= 2
                && part.Length <= OwnChipMaxLength
                && part.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length <= OwnChipMaxWords
                && part.Any(char.IsLetter)
                && !offered.Contains(part, StringComparer.OrdinalIgnoreCase))
            .Select(part => char.ToUpperInvariant(part[0]) + part[1..])
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>The key under which a typed piece is counted: per question, so "Cat" typed for "5 things I see" stays there.</summary>
    public static string OwnChipKey(string question, string part) => $"typed:{question}{OwnKeySeparator}{part}";

    /// <summary>
    /// The person's own chips for a question: pieces typed at least <see cref="OwnChipMinUses"/> times in finished practices,
    /// most frequent first, none that is already offered.
    /// </summary>
    public static IReadOnlyList<string> OwnChips(string question, IReadOnlyDictionary<string, int> counts, IReadOnlyList<string> offered)
    {
        string prefix = OwnChipKey(question, string.Empty);
        return counts
            .Where(pair => pair.Value >= OwnChipMinUses && pair.Key.StartsWith(prefix, StringComparison.Ordinal))
            .Select(pair => (part: pair.Key[prefix.Length..], uses: pair.Value))
            .Where(item => !offered.Contains(item.part, StringComparer.OrdinalIgnoreCase))
            .OrderByDescending(item => item.uses)
            .ThenBy(item => item.part, StringComparer.CurrentCulture)
            .Select(item => item.part)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(OwnChipMax)
            .ToList();
    }

    /// <summary>
    /// Where to reopen a step-by-step practice: the first step still without an answer, after at least one answered step;
    /// the last step when all are answered; the first when nothing is.
    /// </summary>
    public static int ResumeStep(IReadOnlyList<string?> answers)
    {
        if (answers.Count == 0 || answers.All(string.IsNullOrWhiteSpace))
        {
            return 0;
        }

        for (int i = 0; i < answers.Count; i++)
        {
            if (string.IsNullOrWhiteSpace(answers[i]))
            {
                return i;
            }
        }

        return answers.Count - 1;
    }

    /// <summary>
    /// The answers saved with a practice, paired with their questions, for the card after the practice. Tension ratings are left
    /// out (the result page shows them as before and after); empty answers too. Understands the form payload
    /// (<c>{"Fields":{"0":"..."}}</c>) and the list payload of the paper practices (<c>{"Items":[{"Text":"..."}]}</c>).
    /// </summary>
    public static IReadOnlyList<PracticeAnswer> FromPayload(string? payloadJson, IReadOnlyList<TechniqueEntrySeed> entries)
    {
        if (string.IsNullOrWhiteSpace(payloadJson))
        {
            return [];
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(payloadJson);
            JsonElement root = document.RootElement;
            if (TryGet(root, "Fields", out JsonElement fields) && fields.ValueKind == JsonValueKind.Object)
            {
                return FromFields(fields, entries);
            }

            if (TryGet(root, "Items", out JsonElement items) && items.ValueKind == JsonValueKind.Array)
            {
                string title = entries.Count > 0 ? entries[0].Title : string.Empty;
                return items.EnumerateArray()
                    .Select(item => TryGet(item, "Text", out JsonElement text) && text.ValueKind == JsonValueKind.String ? text.GetString() : null)
                    .Where(text => !string.IsNullOrWhiteSpace(text))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(text => new PracticeAnswer(title, text!.Trim()))
                    .ToList();
            }
        }
        catch (JsonException)
        {
            // An old or foreign payload: the card is simply not shown.
        }

        return [];
    }

    private static List<PracticeAnswer> FromFields(JsonElement fields, IReadOnlyList<TechniqueEntrySeed> entries)
    {
        List<PracticeAnswer> answers = [];
        for (int i = 0; i < entries.Count; i++)
        {
            TechniqueEntrySeed entry = entries[i];
            if (entry.Role is EntryFieldRole.IntensityBefore or EntryFieldRole.IntensityAfter
                || !fields.TryGetProperty(i.ToString(System.Globalization.CultureInfo.InvariantCulture), out JsonElement value)
                || value.ValueKind != JsonValueKind.String
                || string.IsNullOrWhiteSpace(value.GetString()))
            {
                continue;
            }

            string answer = value.GetString()!.Trim();
            if (entry.Kind == EntryFieldKind.Rating0To10)
            {
                answer = $"{answer} / 10";
            }

            answers.Add(new PracticeAnswer(entry.Title, answer));
        }

        return answers;
    }

    private static bool TryGet(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }
}
