namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// Free-form entry point of the app: the person describes what is going on, the companion reflects it back and offers a practice.
/// Routing, safety and the choice of practices are deterministic; the optional on-device language model only phrases the empathic reflection,
/// and every reply passes <see cref="CompanionReplyGuard"/> with a scripted fallback.
/// </summary>
public sealed class CompanionSession : IDialogueSession
{
    public const int MaxVentRounds = 3;

    private enum Stage
    {
        NotStarted,
        Describe,
        Clarify,
        Offer,
        Vent,
        Done
    }

    private readonly ISituationAnalyzer _analyzer;
    private readonly ICrisisDetector _crisisDetector;
    private readonly ILanguageModel _model;
    private readonly bool _english;
    private readonly Random _random;
    private readonly TimeSpan _modelTimeout;

    private readonly List<string> _userTexts = [];
    private readonly List<LlmMessage> _history = [];
    private Stage _stage = Stage.NotStarted;
    private SituationAnalysis _analysis = SituationAnalysis.Empty;
    private IReadOnlyList<TechniqueId> _offered = [];
    private int _ventRounds;

    public CompanionSession(
        ISituationAnalyzer analyzer,
        ICrisisDetector crisisDetector,
        ILanguageModel model,
        bool english,
        Random? random = null,
        TimeSpan? modelTimeout = null)
    {
        _analyzer = analyzer;
        _crisisDetector = crisisDetector;
        _model = model;
        _english = english;
        _random = random ?? Random.Shared;
        _modelTimeout = modelTimeout ?? TimeSpan.FromSeconds(25);
    }

    public ConversationStatus Status { get; private set; } = ConversationStatus.WaitingForInput;

    /// <summary>True when the last reflection was written by the language model rather than the scripted fallback.</summary>
    public bool LastReplyFromModel { get; private set; }

    public SituationAnalysis Analysis => _analysis;

    public Task<ConversationTurn> StartAsync(CancellationToken cancellationToken = default)
    {
        if (_stage != Stage.NotStarted)
        {
            throw new InvalidOperationException("Conversation already started.");
        }

        _stage = Stage.Describe;
        return Task.FromResult(new ConversationTurn(
            [CompanionContent.Greeting(_english, _random), CompanionContent.PrivacyNote(_english)],
            DescribePrompt(),
            ConversationStatus.WaitingForInput));
    }

    public async Task<ConversationTurn> SubmitTextAsync(string text, CancellationToken cancellationToken = default)
    {
        if (_stage is not (Stage.Describe or Stage.Vent))
        {
            throw new InvalidOperationException("Conversation is not waiting for text input.");
        }

        string value = text?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            return new ConversationTurn([], DescribePrompt(), ConversationStatus.WaitingForInput);
        }

        if (_crisisDetector.IsCrisis(value))
        {
            _stage = Stage.Done;
            Status = ConversationStatus.Interrupted;
            return new ConversationTurn(
                [CompanionContent.Crisis(_english)],
                null,
                ConversationStatus.Interrupted,
                new DialogueAction(DialogueActionKind.OpenCrisisHub));
        }

        bool wasVenting = _stage == Stage.Vent;
        _userTexts.Add(value);
        _history.Add(new LlmMessage(LlmRole.User, value));

        SituationAnalysis fresh = _analyzer.Analyze(string.Join(' ', _userTexts));
        _analysis = fresh.Emotion == CompanionEmotion.Unknown && _analysis.Emotion != CompanionEmotion.Unknown ? _analysis : fresh;

        string reflection = await ReflectAsync(cancellationToken);
        _history.Add(new LlmMessage(LlmRole.Assistant, reflection));

        if (_analysis.Emotion == CompanionEmotion.Unknown)
        {
            _stage = Stage.Clarify;
            return new ConversationTurn(
                [reflection, CompanionContent.ClarifyLead(_english)],
                new ConversationPrompt(
                    ConversationInputKind.Choice,
                    null,
                    ClarifyLabels()),
                ConversationStatus.WaitingForInput);
        }

        if (wasVenting)
        {
            _ventRounds++;
        }

        return OfferTurn([reflection]);
    }

    public Task<ConversationTurn> SubmitChoiceAsync(int index, CancellationToken cancellationToken = default)
    {
        switch (_stage)
        {
            case Stage.Clarify:
                return Task.FromResult(ClarifyChoice(index));
            case Stage.Offer:
                return Task.FromResult(OfferChoice(index));
            default:
                throw new InvalidOperationException("Conversation is not waiting for a choice.");
        }
    }

    public Task<ConversationTurn> SubmitRatingAsync(int rating, CancellationToken cancellationToken = default) =>
        throw new InvalidOperationException("The companion never asks for a rating.");

    private ConversationTurn ClarifyChoice(int index)
    {
        IReadOnlyList<(string Label, CompanionEmotion Emotion)> options = CompanionContent.ClarifyChoices(_english);
        if (index < 0 || index > options.Count)
        {
            return new ConversationTurn([], new ConversationPrompt(ConversationInputKind.Choice, null, ClarifyLabels()), ConversationStatus.WaitingForInput);
        }

        // The extra last option is "not sure yet": keep the emotion unknown and offer the calming default.
        if (index < options.Count)
        {
            _analysis = _analysis with { Emotion = options[index].Emotion, Confidence = 1 };
            return OfferTurn([CompanionContent.Understood(options[index].Emotion, _english)]);
        }

        return OfferTurn([]);
    }

    private ConversationTurn OfferChoice(int index)
    {
        List<(string Label, Func<ConversationTurn> Run)> options = OfferOptions();
        if (index < 0 || index >= options.Count)
        {
            return OfferPrompt([]);
        }

        return options[index].Run();
    }

    private ConversationTurn OfferTurn(IReadOnlyList<string> leading)
    {
        _offered = TechniqueSuggester.Suggest(_analysis);
        _stage = Stage.Offer;
        return OfferPrompt([.. leading, CompanionContent.OfferLine(_offered[0], _english)]);
    }

    private ConversationTurn OfferPrompt(IReadOnlyList<string> messages) =>
        new(messages,
            new ConversationPrompt(ConversationInputKind.Choice, null, OfferOptions().Select(o => o.Label).ToArray()),
            ConversationStatus.WaitingForInput);

    private List<(string Label, Func<ConversationTurn> Run)> OfferOptions()
    {
        List<(string, Func<ConversationTurn>)> options =
        [
            (CompanionContent.StartLabel(_offered[0], _english), () => Launch(_offered[0]))
        ];

        if (_offered.Count > 1)
        {
            options.Add((CompanionContent.AlternativeLabel(_offered[1], _english), () => Launch(_offered[1])));
        }

        if (_ventRounds < MaxVentRounds)
        {
            options.Add((CompanionContent.MoreLabel(_english), StartVent));
        }

        return options;
    }

    private ConversationTurn Launch(TechniqueId id)
    {
        _stage = Stage.Done;
        Status = ConversationStatus.Completed;
        return new ConversationTurn(
            [CompanionContent.Starting(_english)],
            null,
            ConversationStatus.Completed,
            new DialogueAction(DialogueActionKind.StartTechnique, id));
    }

    private ConversationTurn StartVent()
    {
        _stage = Stage.Vent;
        return new ConversationTurn([CompanionContent.VentPrompt(_english, _random)], DescribePrompt(), ConversationStatus.WaitingForInput);
    }

    private ConversationPrompt DescribePrompt() =>
        new(ConversationInputKind.Text, CompanionContent.InputHint(_english), []);

    private string[] ClarifyLabels() =>
        [.. CompanionContent.ClarifyChoices(_english).Select(c => c.Label), CompanionContent.UnknownChoiceLabel(_english)];

    private async Task<string> ReflectAsync(CancellationToken cancellationToken)
    {
        LastReplyFromModel = false;

        if (_model.IsAvailable)
        {
            using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(_modelTimeout);
            try
            {
                string? raw = await _model.GenerateAsync(CompanionPromptBuilder.Build(_english, _history, _analysis), timeout.Token);
                string? safe = CompanionReplyGuard.Sanitize(raw, _english, string.Join(' ', _userTexts));
                if (safe is not null)
                {
                    LastReplyFromModel = true;
                    return safe;
                }
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                // Model too slow: fall through to the scripted reflection.
            }
            catch (Exception) when (!cancellationToken.IsCancellationRequested)
            {
                // A broken model must never break the conversation.
            }
        }

        return CompanionContent.Reflection(_analysis, _english, _random);
    }
}
