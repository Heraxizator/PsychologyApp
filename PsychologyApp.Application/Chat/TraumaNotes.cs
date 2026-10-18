using System.Text;
using System.Text.Json;

namespace PsychologyApp.Application.Chat;

/// <summary>A belief that got stuck after a hard event, checked in the chat (cognitive processing therapy calls it a stuck point).</summary>
/// <param name="Theme">One of <see cref="CompanionTraumaContent.Themes"/>.</param>
public sealed record StuckPointNote(string Theme, string Phrase, string Balanced, int Before, int After, DateTime DateUtc);

/// <summary>When it got harder, what the body and mind did, and what helped: a line of the person's trigger map.</summary>
public sealed record TriggerNote(string Situation, string Reaction, string Helped, DateTime DateUtc);

/// <summary>
/// What the trauma conversations found, kept between chats so it can go into the summary for a specialist. The person's own words: kept in
/// the chat memory of the encrypted database, never sent anywhere. Only the latest entries are kept.
/// </summary>
public sealed record TraumaNotes(IReadOnlyList<StuckPointNote> StuckPoints, IReadOnlyList<TriggerNote> Triggers)
{
    public const int MaxEntries = 10;

    public static TraumaNotes Empty { get; } = new([], []);

    public bool IsEmpty => StuckPoints.Count == 0 && Triggers.Count == 0;

    public TraumaNotes With(StuckPointNote note) => this with { StuckPoints = [.. StuckPoints.TakeLast(MaxEntries - 1), note] };

    public TraumaNotes With(TriggerNote note) => this with { Triggers = [.. Triggers.TakeLast(MaxEntries - 1), note] };

    public string Serialize()
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter w = new(buffer))
        {
            w.WriteStartObject();
            w.WriteStartArray("stuck");
            foreach (StuckPointNote note in StuckPoints)
            {
                w.WriteStartObject();
                w.WriteString("theme", note.Theme);
                w.WriteString("phrase", note.Phrase);
                w.WriteString("balanced", note.Balanced);
                w.WriteNumber("before", note.Before);
                w.WriteNumber("after", note.After);
                w.WriteString("at", note.DateUtc.ToUniversalTime().ToString("O"));
                w.WriteEndObject();
            }

            w.WriteEndArray();
            w.WriteStartArray("triggers");
            foreach (TriggerNote note in Triggers)
            {
                w.WriteStartObject();
                w.WriteString("situation", note.Situation);
                w.WriteString("reaction", note.Reaction);
                w.WriteString("helped", note.Helped);
                w.WriteString("at", note.DateUtc.ToUniversalTime().ToString("O"));
                w.WriteEndObject();
            }

            w.WriteEndArray();
            w.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>A missing or damaged value reads as no notes rather than an error.</summary>
    public static TraumaNotes Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Empty;
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;
            List<StuckPointNote> stuck = [.. Items(root, "stuck").Select(e => new StuckPointNote(
                Str(e, "theme"), Str(e, "phrase"), Str(e, "balanced"), Int(e, "before"), Int(e, "after"), Date(e)))];
            List<TriggerNote> triggers = [.. Items(root, "triggers").Select(e => new TriggerNote(
                Str(e, "situation"), Str(e, "reaction"), Str(e, "helped"), Date(e)))];
            return new TraumaNotes(stuck, triggers);
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return Empty;
        }
    }

    private static IEnumerable<JsonElement> Items(JsonElement root, string name) =>
        root.TryGetProperty(name, out JsonElement array) && array.ValueKind == JsonValueKind.Array
            ? array.EnumerateArray().Where(e => e.ValueKind == JsonValueKind.Object).ToList()
            : [];

    private static string Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.String ? p.GetString() ?? string.Empty : string.Empty;

    private static int Int(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out int v) ? v : 0;

    private static DateTime Date(JsonElement e) =>
        DateTime.TryParse(Str(e, "at"), null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime at) ? at.ToUniversalTime() : default;
}
