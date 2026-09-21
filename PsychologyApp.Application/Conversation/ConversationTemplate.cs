using System.Text.RegularExpressions;

namespace PsychologyApp.Application.Conversation;

internal static partial class ConversationTemplate
{
    [GeneratedRegex(@"\{([a-z0-9_]+)\}", RegexOptions.CultureInvariant)]
    private static partial Regex PlaceholderRegex();

    internal static IEnumerable<string> Placeholders(string text) =>
        PlaceholderRegex().Matches(text).Select(m => m.Groups[1].Value);

    internal static string Render(string text, IReadOnlyDictionary<string, string> captured) =>
        PlaceholderRegex().Replace(text, m => captured.TryGetValue(m.Groups[1].Value, out string? value) ? value : string.Empty);
}
