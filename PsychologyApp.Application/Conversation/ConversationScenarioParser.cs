using System.Text.Json;
using System.Text.Json.Serialization;
using PsychologyApp.Application.Models.Tests;

namespace PsychologyApp.Application.Conversation;

public sealed class JsonConversationScenario
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Start { get; set; }
    public string? Crisis { get; set; }
    public List<JsonConversationNode>? Nodes { get; set; }
}

public sealed class JsonConversationNode
{
    public string? Id { get; set; }
    public string? Kind { get; set; }
    public List<string>? Text { get; set; }
    public string? Next { get; set; }
    public string? Capture { get; set; }
    public string? Hint { get; set; }
    public bool Pause { get; set; }
    public List<JsonConversationChoice>? Choices { get; set; }
    public List<JsonConversationRoute>? Routes { get; set; }
}

public sealed class JsonConversationChoice
{
    public string? Label { get; set; }
    public string? Next { get; set; }
    public string? Value { get; set; }
}

public sealed class JsonConversationRoute
{
    public int? Max { get; set; }
    public string? Below { get; set; }
    public string? Same { get; set; }
    public string? Next { get; set; }
}

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, ReadCommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true)]
[JsonSerializable(typeof(JsonConversationScenario))]
public partial class ConversationJsonSerializerContext : JsonSerializerContext;

/// <summary>Parses and validates scenario JSON so that <see cref="ConversationSession"/> never meets a dangling reference at runtime.</summary>
public static class ConversationScenarioParser
{
    public static ParseResult<ConversationScenario> Parse(string json)
    {
        JsonConversationScenario? dto;
        try
        {
            dto = JsonSerializer.Deserialize(json, ConversationJsonSerializerContext.Default.JsonConversationScenario);
        }
        catch (JsonException ex)
        {
            return ParseResult<ConversationScenario>.Failure($"Invalid JSON: {ex.Message}");
        }

        if (dto is null)
        {
            return ParseResult<ConversationScenario>.Failure("Scenario is empty.");
        }

        return Build(dto);
    }

    private static ParseResult<ConversationScenario> Build(JsonConversationScenario dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            return Fail("Scenario id is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Start) || string.IsNullOrWhiteSpace(dto.Crisis))
        {
            return Fail("Scenario 'start' and 'crisis' node ids are required.");
        }

        Dictionary<string, ConversationNode> nodes = new(StringComparer.Ordinal);
        foreach (JsonConversationNode raw in dto.Nodes ?? [])
        {
            ParseResult<ConversationNode> node = BuildNode(raw);
            if (!node.IsSuccess)
            {
                return Fail(node.Error!);
            }

            if (!nodes.TryAdd(node.Value!.Id, node.Value))
            {
                return Fail($"Duplicate node id '{node.Value.Id}'.");
            }
        }

        string? error = Validate(dto.Start, dto.Crisis, nodes);
        if (error is not null)
        {
            return Fail(error);
        }

        return ParseResult<ConversationScenario>.Success(
            new ConversationScenario(dto.Id, dto.Title ?? dto.Id, dto.Start, dto.Crisis, nodes));

        ParseResult<ConversationScenario> Fail(string message) =>
            ParseResult<ConversationScenario>.Failure($"Scenario '{dto.Id ?? "?"}': {message}");
    }

    private static ParseResult<ConversationNode> BuildNode(JsonConversationNode raw)
    {
        if (string.IsNullOrWhiteSpace(raw.Id))
        {
            return ParseResult<ConversationNode>.Failure("Node without id.");
        }

        string id = raw.Id;
        ConversationNodeKind? kind = raw.Kind?.ToLowerInvariant() switch
        {
            "say" => ConversationNodeKind.Say,
            "asktext" => ConversationNodeKind.AskText,
            "askchoice" => ConversationNodeKind.AskChoice,
            "askrating" => ConversationNodeKind.AskRating,
            "end" => ConversationNodeKind.End,
            _ => null
        };

        if (kind is null)
        {
            return ParseResult<ConversationNode>.Failure($"Node '{id}' has unknown kind '{raw.Kind}'.");
        }

        List<string> text = (raw.Text ?? []).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        if (kind != ConversationNodeKind.End && text.Count == 0)
        {
            return ParseResult<ConversationNode>.Failure($"Node '{id}' needs at least one text variant.");
        }

        List<ConversationChoice>? choices = null;
        if (kind == ConversationNodeKind.AskChoice)
        {
            if (raw.Choices is not { Count: > 0 })
            {
                return ParseResult<ConversationNode>.Failure($"Node '{id}' needs choices.");
            }

            choices = [];
            foreach (JsonConversationChoice choice in raw.Choices)
            {
                if (string.IsNullOrWhiteSpace(choice.Label) || string.IsNullOrWhiteSpace(choice.Next))
                {
                    return ParseResult<ConversationNode>.Failure($"Node '{id}' has a choice without label or next.");
                }

                choices.Add(new ConversationChoice(choice.Label, choice.Next, choice.Value));
            }
        }

        List<ConversationRoute>? routes = raw.Routes?
            .Where(r => !string.IsNullOrWhiteSpace(r.Next))
            .Select(r => new ConversationRoute(r.Max, r.Next!, r.Below, r.Same))
            .ToList();

        bool needsNext = kind is ConversationNodeKind.Say or ConversationNodeKind.AskText
            || (kind == ConversationNodeKind.AskRating && routes is not { Count: > 0 });
        if (needsNext && string.IsNullOrWhiteSpace(raw.Next))
        {
            return ParseResult<ConversationNode>.Failure($"Node '{id}' needs 'next'.");
        }

        if (kind == ConversationNodeKind.AskRating && routes is { Count: > 0 } && string.IsNullOrWhiteSpace(raw.Next)
            && routes.All(r => r.MaxRating is not null || r.BelowKey is not null || r.SameKey is not null))
        {
            return ParseResult<ConversationNode>.Failure($"Node '{id}' needs a fallback route without 'max' or a 'next'.");
        }

        return ParseResult<ConversationNode>.Success(new ConversationNode(
            id, kind.Value, text, raw.Next, raw.Capture, raw.Hint, choices, routes, raw.Pause));
    }

    private static string? Validate(string start, string crisis, Dictionary<string, ConversationNode> nodes)
    {
        foreach (string required in new[] { start, crisis })
        {
            if (!nodes.ContainsKey(required))
            {
                return $"Node '{required}' does not exist.";
            }
        }

        HashSet<string> captureKeys = nodes.Values
            .Where(n => n.Capture is not null)
            .Select(n => n.Capture!)
            .ToHashSet(StringComparer.Ordinal);

        foreach (ConversationNode node in nodes.Values)
        {
            foreach (string target in Targets(node))
            {
                if (!nodes.ContainsKey(target))
                {
                    return $"Node '{node.Id}' points to missing node '{target}'.";
                }
            }

            foreach (string placeholder in node.Text.SelectMany(ConversationTemplate.Placeholders))
            {
                if (!captureKeys.Contains(placeholder))
                {
                    return $"Node '{node.Id}' uses placeholder '{{{placeholder}}}' that no node captures.";
                }
            }
        }

        string? loop = FindSayLoop(nodes);
        if (loop is not null)
        {
            return $"Say nodes loop forever without asking for input (through '{loop}').";
        }

        if (!CanReachEnd(start, nodes) || !CanReachEnd(crisis, nodes))
        {
            return "Start and crisis nodes must both be able to reach an 'end' node.";
        }

        return null;
    }

    private static IEnumerable<string> Targets(ConversationNode node)
    {
        if (node.Next is not null)
        {
            yield return node.Next;
        }

        foreach (ConversationChoice choice in node.Choices ?? [])
        {
            yield return choice.Next;
        }

        foreach (ConversationRoute route in node.Routes ?? [])
        {
            yield return route.Next;
        }
    }

    private static string? FindSayLoop(Dictionary<string, ConversationNode> nodes)
    {
        foreach (ConversationNode origin in nodes.Values.Where(n => n.Kind == ConversationNodeKind.Say))
        {
            HashSet<string> seen = new(StringComparer.Ordinal);
            ConversationNode current = origin;
            while (current.Kind == ConversationNodeKind.Say)
            {
                if (!seen.Add(current.Id))
                {
                    return current.Id;
                }

                current = nodes[current.Next!];
            }
        }

        return null;
    }

    private static bool CanReachEnd(string from, Dictionary<string, ConversationNode> nodes)
    {
        HashSet<string> seen = new(StringComparer.Ordinal);
        Stack<string> stack = new([from]);
        while (stack.Count > 0)
        {
            string id = stack.Pop();
            if (!seen.Add(id))
            {
                continue;
            }

            ConversationNode node = nodes[id];
            if (node.Kind == ConversationNodeKind.End)
            {
                return true;
            }

            foreach (string target in Targets(node))
            {
                stack.Push(target);
            }
        }

        return false;
    }
}
