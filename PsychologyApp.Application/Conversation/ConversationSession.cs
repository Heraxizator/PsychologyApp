using System.Globalization;

namespace PsychologyApp.Application.Conversation;

public enum ConversationStatus
{
    WaitingForInput,
    /// <summary>Reached a normal ending: counts as a finished practice.</summary>
    Completed,
    /// <summary>The user chose to stop early; the practice must not be recorded as done.</summary>
    Paused,
    /// <summary>Crisis phrase detected; the scenario was cut short in favour of help information.</summary>
    Interrupted
}

public enum ConversationInputKind
{
    Text,
    Choice,
    Rating
}

public sealed record ConversationPrompt(
    ConversationInputKind Kind,
    string? Hint,
    IReadOnlyList<string> Choices);

public sealed record ConversationTurn(
    IReadOnlyList<string> BotMessages,
    ConversationPrompt? Prompt,
    ConversationStatus Status);

/// <summary>
/// One run of a scenario. Pure state machine: no I/O, no UI, deterministic given the same <see cref="Random"/>.
/// </summary>
public sealed class ConversationSession
{
    private const int MaxAutoSteps = 200;

    private readonly ConversationScenario _scenario;
    private readonly ICrisisDetector _crisisDetector;
    private readonly Random _random;
    private readonly Dictionary<string, string> _captured = new(StringComparer.Ordinal);
    private ConversationNode? _current;

    public ConversationSession(ConversationScenario scenario, ICrisisDetector crisisDetector, Random? random = null)
    {
        _scenario = scenario;
        _crisisDetector = crisisDetector;
        _random = random ?? Random.Shared;
    }

    public ConversationStatus Status { get; private set; } = ConversationStatus.WaitingForInput;

    public IReadOnlyDictionary<string, string> Captured => _captured;

    public string ScenarioId => _scenario.Id;

    public int? GetRating(string key) =>
        _captured.TryGetValue(key, out string? raw) && int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
            ? value
            : null;

    public ConversationTurn Start()
    {
        if (_current is not null)
        {
            throw new InvalidOperationException("Conversation already started.");
        }

        return Run(_scenario.Start);
    }

    public ConversationTurn SubmitText(string text)
    {
        ConversationNode node = RequireInputNode(ConversationNodeKind.AskText);
        string value = text?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            return Reprompt(node);
        }

        if (_crisisDetector.IsCrisis(value))
        {
            Status = ConversationStatus.Interrupted;
            return Run(_scenario.CrisisNode);
        }

        Capture(node, value);
        return Run(node.Next!);
    }

    public ConversationTurn SubmitChoice(int index)
    {
        ConversationNode node = RequireInputNode(ConversationNodeKind.AskChoice);
        IReadOnlyList<ConversationChoice> choices = node.Choices!;
        if (index < 0 || index >= choices.Count)
        {
            return Reprompt(node);
        }

        ConversationChoice choice = choices[index];
        Capture(node, choice.Value ?? choice.Label);
        return Run(choice.Next);
    }

    public ConversationTurn SubmitRating(int rating)
    {
        ConversationNode node = RequireInputNode(ConversationNodeKind.AskRating);
        if (rating is < 0 or > 10)
        {
            return Reprompt(node);
        }

        Capture(node, rating.ToString(CultureInfo.InvariantCulture));

        string next = node.Routes?.FirstOrDefault(r => RouteMatches(r, rating))?.Next ?? node.Next!;
        return Run(next);
    }

    private bool RouteMatches(ConversationRoute route, int rating)
    {
        if (route.BelowKey is not null)
        {
            return GetRating(route.BelowKey) is int reference && rating < reference;
        }

        if (route.SameKey is not null)
        {
            return GetRating(route.SameKey) is int reference && rating == reference;
        }

        return route.MaxRating is null || rating <= route.MaxRating;
    }

    private ConversationTurn Run(string nodeId)
    {
        List<string> messages = [];
        string id = nodeId;

        for (int step = 0; step < MaxAutoSteps; step++)
        {
            ConversationNode node = _scenario.Nodes[id];
            _current = node;

            switch (node.Kind)
            {
                case ConversationNodeKind.Say:
                    messages.Add(Render(node));
                    id = node.Next!;
                    break;

                case ConversationNodeKind.End:
                    if (node.Text.Count > 0)
                    {
                        messages.Add(Render(node));
                    }

                    if (Status != ConversationStatus.Interrupted)
                    {
                        Status = node.IsPause ? ConversationStatus.Paused : ConversationStatus.Completed;
                    }

                    return new ConversationTurn(messages, null, Status);

                default:
                    messages.Add(Render(node));
                    return new ConversationTurn(messages, BuildPrompt(node), ConversationStatus.WaitingForInput);
            }
        }

        throw new InvalidOperationException($"Scenario '{_scenario.Id}' looped without asking for input.");
    }

    private static ConversationTurn Reprompt(ConversationNode node) =>
        new([], BuildPrompt(node), ConversationStatus.WaitingForInput);

    private static ConversationPrompt BuildPrompt(ConversationNode node) => node.Kind switch
    {
        ConversationNodeKind.AskText => new(ConversationInputKind.Text, node.Hint, []),
        ConversationNodeKind.AskChoice => new(ConversationInputKind.Choice, node.Hint, node.Choices!.Select(c => c.Label).ToArray()),
        _ => new(ConversationInputKind.Rating, node.Hint, [])
    };

    private ConversationNode RequireInputNode(ConversationNodeKind expected)
    {
        if (_current is null)
        {
            throw new InvalidOperationException("Conversation not started.");
        }

        if (Status != ConversationStatus.WaitingForInput || _current.Kind != expected)
        {
            throw new InvalidOperationException($"Conversation is not waiting for {expected} input.");
        }

        return _current;
    }

    private void Capture(ConversationNode node, string value)
    {
        if (node.Capture is not null)
        {
            _captured[node.Capture] = value;
        }
    }

    private string Render(ConversationNode node)
    {
        string variant = node.Text.Count == 1 ? node.Text[0] : node.Text[_random.Next(node.Text.Count)];
        return ConversationTemplate.Render(variant, _captured);
    }
}
