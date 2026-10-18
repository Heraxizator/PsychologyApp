using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice;

public interface ISupportPhraseSuggester
{
    /// <summary>
    /// A few ready phrases for a "balanced thought" or "kind words" field, picked to fit what the person wrote before it.
    /// Empty for other fields, and empty when the context reads as a crisis: then a ready phrase would only sound dismissive.
    /// </summary>
    /// <param name="context">What the person wrote in the earlier fields of the same practice (the automatic thought, the self-criticism).</param>
    Task<IReadOnlyList<string>> SuggestAsync(EntryFieldRole role, string? context, bool english, CancellationToken cancellationToken = default);
}

/// <summary>
/// Reviewed phrases first; when an on-device model is installed it may add its own variants, each passed through
/// <see cref="CompanionReplyGuard"/> so that nothing unchecked reaches the person. The words never leave the phone.
/// </summary>
public sealed class SupportPhraseSuggester(ILanguageModel languageModel, ICrisisDetector crisisDetector) : ISupportPhraseSuggester
{
    public const int MaxSuggestions = 6;
    private const int ModelVariants = 2;

    public async Task<IReadOnlyList<string>> SuggestAsync(EntryFieldRole role, string? context, bool english, CancellationToken cancellationToken = default)
    {
        if (role is not (EntryFieldRole.Reframe or EntryFieldRole.Kindness or EntryFieldRole.SelfKindness)
            || crisisDetector.IsCrisis(context))
        {
            return [];
        }

        List<string> scripted = SupportPhraseContent.Pick(role, context, english);
        List<string> fromModel = role == EntryFieldRole.Kindness
            ? []
            : await AskModelAsync(role, context, english, cancellationToken).ConfigureAwait(false);

        return fromModel
            .Concat(scripted)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(MaxSuggestions)
            .ToList();
    }

    private async Task<List<string>> AskModelAsync(EntryFieldRole role, string? context, bool english, CancellationToken cancellationToken)
    {
        if (!languageModel.IsAvailable || string.IsNullOrWhiteSpace(context))
        {
            return [];
        }

        try
        {
            string? raw = await languageModel.GenerateAsync(
                new LlmRequest(SupportPhraseContent.SystemPrompt(role, english), [new LlmMessage(LlmRole.User, context.Trim())], MaxNewTokens: 90),
                cancellationToken).ConfigureAwait(false);

            return (raw ?? string.Empty)
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line => CompanionReplyGuard.Sanitize(line.TrimStart('-', '•', '*', ' ').Trim('"', '«', '»'), english))
                .Where(line => line is { Length: <= SupportPhraseContent.MaxPhraseLength })
                .Select(line => line!)
                .Take(ModelVariants)
                .ToList();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            // The model is a bonus: the reviewed phrases are always there.
            return [];
        }
    }
}
