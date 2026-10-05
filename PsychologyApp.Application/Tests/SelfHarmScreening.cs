using PsychologyApp.Application.Models.Tests;

namespace PsychologyApp.Application.Tests;

/// <summary>
/// The questionnaires with a question about thoughts of suicide or self-harm. An answer other than "never" must not just add to a
/// total: the person is shown where to get help, whatever the total says. Matched by the position of the question in the
/// published catalogue (and its length, so a changed catalogue is not silently misread).
/// </summary>
public static class SelfHarmScreening
{
    // analyzer id -> (index of the question, number of questions in the questionnaire)
    private static readonly Dictionary<string, (int Index, int Count)> Items = new(StringComparer.Ordinal)
    {
        ["beck"] = (8, 21),   // BDI item 9: suicidal thoughts or wishes
        ["phq9"] = (8, 9)     // PHQ-9 item 9: thoughts that one would be better off dead, or of hurting oneself
    };

    public static bool IsEndorsed(string? analyzerId, IEnumerable<Question> questions)
    {
        if (analyzerId is null || !Items.TryGetValue(analyzerId, out (int Index, int Count) item))
        {
            return false;
        }

        List<Question> list = questions.ToList();
        return list.Count == item.Count
            && list[item.Index].Answers.Any(answer => answer.Selected && answer.Ball >= 1);
    }
}
