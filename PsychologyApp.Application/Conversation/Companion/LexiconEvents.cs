namespace PsychologyApp.Application.Conversation.Companion;

public sealed partial class LexiconSituationAnalyzer
{
    /// <summary>
    /// Words for what happened. They do not name a feeling (that is the emotion lexicon's job); they let the companion answer the situation
    /// itself ("a quarrel hurts most from those close to us") and ask the question that fits it. Same term syntax as the emotion lexicon.
    /// </summary>
    private static readonly Dictionary<CompanionEvent, string[]> Events = new()
    {
        [CompanionEvent.Conflict] = ["поссор", "поруга", "ругал", "накричал", "наорал", "обозвал", "оскорбил", "скандал", "конфликт", "истеричк", "эгоистк", "поспорил", "argued", "argument", "fight with", "fought with", "yelled at", "shouted at", "called me names"],
        [CompanionEvent.Humiliation] = ["при всех", "перед всеми", "унизил", "высмеял", "посмеялись", "in front of everyone", "in front of the whole", "humiliated", "laughed at me"],
        [CompanionEvent.Breakup] = ["бросил меня", "бросила меня", "меня бросил", "меня бросила", "расстал", "разошл", "развод", "разводим", "ушел от меня", "ушла от меня", "разлюбил", "broke up", "left me", "divorce", "separated"],
        [CompanionEvent.JobLoss] = ["сократили", "уволили", "потеряла работу", "потерял работу", "лишилась работы", "лишился работы", "lost my job", "laid off", "got fired", "layoff", "made redundant"],
        [CompanionEvent.HealthWorry] = ["диагноз", "анализ", "обследовани", "онколог", "операци", "биопси", "результаты", "жду врача", "test results", "diagnosis", "biopsy", "surgery", "scan results", "waiting for the doctor"],
        [CompanionEvent.CaringForIll] = ["болеет", "ухаживаю", "тяжело болен", "лежачий", "уход за", "caring for my", "my mom is sick", "my mother is sick", "my dad is sick", "my father is sick", "sick parent"],
        [CompanionEvent.Failure] = ["провалил", "завалил", "не сдал", "не прошел", "не получилось", "неудачниц", "неудачник", "failed", "i failed", "didnt pass", "bombed", "messed up"],
        [CompanionEvent.Betrayal] = ["изменил мне", "изменила мне", "мне изменил", "мне изменила", "предал", "обманул", "обманула", "cheated on me", "betrayed", "lied to me"],
        [CompanionEvent.Overload] = ["не успеваю", "слишком много", "завал", "горят сроки", "дедлайн", "overwhelmed", "too much to do", "deadline", "cant keep up"],
        [CompanionEvent.Sleeplessness] = ["не могу уснуть", "не могу заснуть", "не сплю", "бессонниц", "не спится", "cant sleep", "insomnia", "cannot sleep", "awake all night"],
        [CompanionEvent.Performance] = ["выступлени", "экзамен", "собеседовани", "презентаци", "защита диплома", "защиту диплома", "interview", "exam", "presentation", "public speaking", "my speech"]
    };

    private static readonly string[] EndedMarkers = ["раньше", "прежде ", "давно не", "перестал", "больше не", "used to", "no longer", "not anymore", "stopped ", "anymore"];
    private static readonly string[] ClauseBreaks = [",", ";", ".", "!", "?", " а ", " но ", " but ", " and now ", " now "];

    /// <summary>
    /// "I used to be anxious, but now I am calm" is not anxiety: a clause about a feeling that is over is dropped, if a clause about the present remains
    /// (or nothing is left, as in "I stopped worrying"). "Больше не могу" is the opposite of over and stays.
    /// </summary>
    private static string DropEndedClauses(string text)
    {
        string lower = text.ToLowerInvariant();
        if (!EndedMarkers.Any(lower.Contains))
        {
            return text;
        }

        List<string> kept = [];
        string rest = text;
        foreach (string clause in SplitClauses(rest))
        {
            string c = clause.ToLowerInvariant();
            bool ended = EndedMarkers.Any(c.Contains) && !c.Contains("не могу", StringComparison.Ordinal) && !c.Contains("cant", StringComparison.Ordinal) && !c.Contains("can't", StringComparison.Ordinal);
            if (!ended)
            {
                kept.Add(clause);
            }
        }

        return string.Join(", ", kept);
    }

    private static IEnumerable<string> SplitClauses(string text)
    {
        string[] parts = [text];
        foreach (string brk in ClauseBreaks)
        {
            parts = [.. parts.SelectMany(p => p.Split(brk, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))];
        }

        return parts;
    }

    private CompanionEvent DetectEvent(string normalized, string[] tokens, int[] offsets, bool hasLoss)
    {
        if (hasLoss)
        {
            return CompanionEvent.Loss;
        }

        // Most hits first; the event mentioned first wins a tie ("my husband shouted at me in front of everyone" is a quarrel before it is a humiliation).
        (CompanionEvent Event, int Hits, int First) best = (CompanionEvent.None, 0, int.MaxValue);
        foreach ((CompanionEvent candidate, string[] terms) in Events)
        {
            int hits = 0;
            int first = int.MaxValue;
            foreach (string term in terms)
            {
                int at = IndexOf(term, normalized, tokens, offsets, respectNegation: false);
                if (at >= 0)
                {
                    hits++;
                    first = Math.Min(first, at);
                }
            }

            if (hits > best.Hits || (hits == best.Hits && hits > 0 && first < best.First))
            {
                best = (candidate, hits, first);
            }
        }

        return best.Event;
    }
}
