namespace PsychologyApp.Application.Conversation;

public interface ICrisisDetector
{
    bool IsCrisis(string? text);
}

/// <summary>
/// Conservative phrase matcher that runs locally before any scenario logic sees free text.
/// It favours false positives: a wrongly shown helpline card costs little, a missed one costs a lot.
/// </summary>
public sealed class KeywordCrisisDetector : ICrisisDetector
{
    private static readonly string[] Markers =
    [
        "суицид", "самоубий", "покончить с собой", "покончу с собой", "убить себя", "убью себя",
        "не хочу жить", "не хочется жить", "не хочу больше жить", "хочу умереть", "хочу сдохнуть",
        "лучше бы я умер", "лучше бы меня не было", "жить не стоит", "смысла жить нет", "нет смысла жить",
        "свести счеты", "наложить на себя руки", "вскрыть вены", "повеситься", "спрыгнуть с крыши",
        "причинить себе вред", "навредить себе", "режу себя", "резать себя", "самоповрежд",
        "kill myself", "suicide", "suicidal", "want to die", "end my life", "end it all",
        "hurt myself", "self-harm", "self harm"
    ];

    public bool IsCrisis(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string normalized = text.Trim().ToLowerInvariant().Replace('ё', 'е');
        foreach (string marker in Markers)
        {
            if (normalized.Contains(marker, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
