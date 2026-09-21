using System.Text;
using System.Text.Json;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Everything the companion needs to continue a chat exactly where it stopped. Persisted with the session.
/// Immutable: the engine returns an updated copy for every turn.
/// </summary>
public sealed record CompanionState
{
    private const int MaxRecentTexts = 3;
    private const int MaxAsked = 40;

    /// <summary>Number of free-text messages the person has sent.</summary>
    public int Turns { get; init; }

    /// <summary>A <c>CompanionEmotion</c> name; "Unknown" until something is recognised.</summary>
    public string Emotion { get; init; } = "Unknown";

    public string? Theme { get; init; }
    public int? FirstIntensity { get; init; }
    public int? LastIntensity { get; init; }

    /// <summary>User turns since a practice was last offered. Large at the start so the first offer is not blocked.</summary>
    public int TurnsSinceOffer { get; init; } = 99;

    public int UnknownStreak { get; init; }
    public bool ScaleAsked { get; init; }

    /// <summary>The next 0..10 answer is the tension after a practice, not before it.</summary>
    public bool AwaitingPostPracticeRating { get; init; }

    public IReadOnlyList<string> AskedQuestions { get; init; } = [];
    public IReadOnlyList<string> RecentTexts { get; init; } = [];

    /// <summary>Technique started from this chat whose result the companion still wants to ask about.</summary>
    public string? PendingPractice { get; init; }
    public DateTime? PendingPracticeStartedUtc { get; init; }

    public string? OfferedPrimary { get; init; }
    public string? OfferedAlternative { get; init; }

    /// <summary>Who the conversation is about (a <c>CompanionPerson</c> name), if it has come up.</summary>
    public string? Person { get; init; }

    /// <summary>The second state of a mixed feeling (a <c>CompanionEmotion</c> name), or Unknown.</summary>
    public string Secondary { get; init; } = "Unknown";

    /// <summary>The first thing the person said, quoted back later ("at the start you said...").</summary>
    public string? FirstQuote { get; init; }

    /// <summary>User turn at which the last summary was offered.</summary>
    public int LastRecapTurn { get; init; }

    public bool CallbackAsked { get; init; }

    /// <summary>The companion's last message asked something, so a bare "yes" / "no" is an answer to it.</summary>
    public bool QuestionPending { get; init; }

    /// <summary>How the person asked to be called, if they said so. Used sparingly, like a friend would.</summary>
    public string? UserName { get; init; }

    /// <summary>The companion has just asked for a name, so a short reply may be one.</summary>
    public bool NamePending { get; init; }

    /// <summary>The last question the companion asked, so "what?" / "repeat" can be answered.</summary>
    public string? LastQuestion { get; init; }

    /// <summary>Beginnings of the latest companion messages, used to avoid saying the same thing twice in a row.</summary>
    public IReadOnlyList<string> RecentReplies { get; init; } = [];

    /// <summary>The practice whose result the companion asked about last; if the tension dropped, it becomes a helped practice.</summary>
    public string? LastPractice { get; init; }

    /// <summary>Practices in this chat after which the tension dropped.</summary>
    public IReadOnlyList<string> HelpedPractices { get; init; } = [];

    /// <summary>The practice that has helped this person most across chats, loaded when the chat starts.</summary>
    public string? PreferredPractice { get; init; }

    /// <summary>The companion has already said "last time this helped you" in this chat.</summary>
    public bool PreferredMentioned { get; init; }

    public CompanionState WithText(string text) => this with
    {
        Turns = Turns + 1,
        RecentTexts = [.. RecentTexts.TakeLast(MaxRecentTexts - 1), text]
    };

    public CompanionState WithAsked(string questionId) => this with
    {
        AskedQuestions = [.. AskedQuestions.TakeLast(MaxAsked - 1), questionId]
    };

    public string Serialize()
    {
        using MemoryStream buffer = new();
        using (Utf8JsonWriter w = new(buffer))
        {
            w.WriteStartObject();
            w.WriteNumber("turns", Turns);
            w.WriteString("emotion", Emotion);
            WriteOptional(w, "theme", Theme);
            WriteOptional(w, "first", FirstIntensity);
            WriteOptional(w, "last", LastIntensity);
            w.WriteNumber("sinceOffer", TurnsSinceOffer);
            w.WriteNumber("unknownStreak", UnknownStreak);
            w.WriteBoolean("scaleAsked", ScaleAsked);
            w.WriteBoolean("awaitingPost", AwaitingPostPracticeRating);
            WriteArray(w, "asked", AskedQuestions);
            WriteArray(w, "recent", RecentTexts);
            WriteOptional(w, "pending", PendingPractice);
            WriteOptional(w, "pendingAt", PendingPracticeStartedUtc?.ToUniversalTime().ToString("O"));
            WriteOptional(w, "primary", OfferedPrimary);
            WriteOptional(w, "alternative", OfferedAlternative);
            WriteOptional(w, "person", Person);
            w.WriteString("secondary", Secondary);
            WriteOptional(w, "firstQuote", FirstQuote);
            w.WriteNumber("lastRecap", LastRecapTurn);
            w.WriteBoolean("callbackAsked", CallbackAsked);
            w.WriteBoolean("questionPending", QuestionPending);
            WriteOptional(w, "userName", UserName);
            w.WriteBoolean("namePending", NamePending);
            WriteOptional(w, "lastQuestion", LastQuestion);
            WriteArray(w, "recentReplies", RecentReplies);
            WriteOptional(w, "lastPractice", LastPractice);
            WriteArray(w, "helped", HelpedPractices);
            WriteOptional(w, "preferred", PreferredPractice);
            w.WriteBoolean("preferredMentioned", PreferredMentioned);
            w.WriteEndObject();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    /// <summary>Restores a state; a missing or damaged blob yields a fresh state instead of an error.</summary>
    public static CompanionState Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new CompanionState();
        }

        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement r = doc.RootElement;
            return new CompanionState
            {
                Turns = Int(r, "turns") ?? 0,
                Emotion = Str(r, "emotion") ?? "Unknown",
                Theme = Str(r, "theme"),
                FirstIntensity = Int(r, "first"),
                LastIntensity = Int(r, "last"),
                TurnsSinceOffer = Int(r, "sinceOffer") ?? 99,
                UnknownStreak = Int(r, "unknownStreak") ?? 0,
                ScaleAsked = Bool(r, "scaleAsked"),
                AwaitingPostPracticeRating = Bool(r, "awaitingPost"),
                AskedQuestions = Array(r, "asked"),
                RecentTexts = Array(r, "recent"),
                PendingPractice = Str(r, "pending"),
                PendingPracticeStartedUtc = Str(r, "pendingAt") is { } at
                    && DateTime.TryParse(at, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsed)
                        ? parsed.ToUniversalTime()
                        : null,
                OfferedPrimary = Str(r, "primary"),
                OfferedAlternative = Str(r, "alternative"),
                Person = Str(r, "person"),
                Secondary = Str(r, "secondary") ?? "Unknown",
                FirstQuote = Str(r, "firstQuote"),
                LastRecapTurn = Int(r, "lastRecap") ?? 0,
                CallbackAsked = Bool(r, "callbackAsked"),
                QuestionPending = Bool(r, "questionPending"),
                UserName = Str(r, "userName"),
                NamePending = Bool(r, "namePending"),
                LastQuestion = Str(r, "lastQuestion"),
                RecentReplies = Array(r, "recentReplies"),
                LastPractice = Str(r, "lastPractice"),
                HelpedPractices = Array(r, "helped"),
                PreferredPractice = Str(r, "preferred"),
                PreferredMentioned = Bool(r, "preferredMentioned")
            };
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException)
        {
            return new CompanionState();
        }
    }

    private static void WriteOptional(Utf8JsonWriter w, string name, string? value)
    {
        if (value is not null)
        {
            w.WriteString(name, value);
        }
    }

    private static void WriteOptional(Utf8JsonWriter w, string name, int? value)
    {
        if (value is { } v)
        {
            w.WriteNumber(name, v);
        }
    }

    private static void WriteArray(Utf8JsonWriter w, string name, IReadOnlyList<string> values)
    {
        w.WriteStartArray(name);
        foreach (string value in values)
        {
            w.WriteStringValue(value);
        }

        w.WriteEndArray();
    }

    private static string? Str(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.String ? p.GetString() : null;

    private static int? Int(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out int v) ? v : null;

    private static bool Bool(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.True;

    private static string[] Array(JsonElement e, string name) =>
        e.TryGetProperty(name, out JsonElement p) && p.ValueKind == JsonValueKind.Array
            ? p.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()!).ToArray()
            : [];
}
