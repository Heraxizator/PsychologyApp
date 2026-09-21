using System.Text;
using System.Text.Json;

namespace PsychologyApp.Application.Chat;

/// <summary>Hand-written (de)serialization: reflection-based JSON is not trim-safe in release builds.</summary>
public static class ChatQuickReplyJson
{
    public static string? Serialize(IReadOnlyList<ChatQuickReply> replies)
    {
        if (replies.Count == 0)
        {
            return null;
        }

        using MemoryStream buffer = new();
        using (Utf8JsonWriter writer = new(buffer))
        {
            writer.WriteStartArray();
            foreach (ChatQuickReply reply in replies)
            {
                writer.WriteStartObject();
                writer.WriteString("k", reply.Kind);
                writer.WriteString("l", reply.Label);
                if (reply.Payload is not null)
                {
                    writer.WriteString("p", reply.Payload);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    public static IReadOnlyList<ChatQuickReply> Deserialize(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            List<ChatQuickReply> replies = [];
            foreach (JsonElement item in document.RootElement.EnumerateArray())
            {
                string? kind = item.GetProperty("k").GetString();
                string? label = item.GetProperty("l").GetString();
                if (kind is null || label is null)
                {
                    continue;
                }

                replies.Add(new ChatQuickReply(kind, label, item.TryGetProperty("p", out JsonElement payload) ? payload.GetString() : null));
            }

            return replies;
        }
        catch (Exception ex) when (ex is JsonException or InvalidOperationException or KeyNotFoundException)
        {
            return [];
        }
    }
}
