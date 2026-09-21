namespace PsychologyApp.Application.Conversation.Companion;

public static class CompanionPromptBuilder
{
    private const int MaxHistoryMessages = 6;

    private const string SystemRu =
        "Ты тёплый и спокойный помощник по эмоциональной поддержке в приложении для самопомощи. Ты не психотерапевт и не врач. " +
        "Правила: отвечай по-русски, обращайся к человеку на «вы», 2–3 коротких предложения обычными словами, без списков и форматирования. " +
        "Отрази чувства и ситуацию человека своими словами, чтобы он почувствовал, что его услышали. " +
        "Не задавай вопросов. Не давай советов и не предлагай упражнений: приложение сделает это само. " +
        "Не ставь диагнозов, не упоминай лекарства и лечение. Не обесценивай («всё будет хорошо», «не переживай»). " +
        "Не выдумывай фактов, которых человек не называл. Не говори, что ты ИИ или модель.";

    private const string SystemEn =
        "You are a warm, calm emotional-support helper inside a self-help app. You are not a therapist or a doctor. " +
        "Rules: reply in English, 2-3 short sentences in plain words, no lists or formatting. " +
        "Reflect the person's feelings and situation in your own words so they feel heard. " +
        "Do not ask questions. Do not give advice or suggest exercises: the app will do that itself. " +
        "Do not diagnose and do not mention medication or treatment. Do not dismiss feelings (\"it will be fine\", \"don't worry\"). " +
        "Do not invent facts the person did not state. Never say you are an AI or a model.";

    /// <param name="turns">Alternating user / companion messages so far, oldest first; the last one is the user's newest message.</param>
    public static LlmRequest Build(bool english, IReadOnlyList<LlmMessage> turns, SituationAnalysis analysis)
    {
        string system = english ? SystemEn : SystemRu;
        string hint = Hint(english, analysis);
        if (hint.Length > 0)
        {
            system += " " + hint;
        }

        // Always copy: callers keep appending to their own history list after the request has been built.
        IReadOnlyList<LlmMessage> recent = turns.Skip(Math.Max(0, turns.Count - MaxHistoryMessages)).ToArray();

        return new LlmRequest(system, [.. Examples(english), .. recent], Temperature: 0.3f);
    }

    /// <summary>Small models follow examples far better than rules: two model answers set the tone, length and register.</summary>
    private static IReadOnlyList<LlmMessage> Examples(bool english) => english
        ?
        [
            new(LlmRole.User, "I have an exam tomorrow and I'm afraid I'll forget everything."),
            new(LlmRole.Assistant, "It sounds like you are really anxious before the exam. When so much feels at stake, the fear of forgetting everything is very understandable."),
            new(LlmRole.User, "I argued with my mom and I'm still angry."),
            new(LlmRole.Assistant, "It seems the argument with your mom hurt you deeply, and the anger will not let go. That is exhausting.")
        ]
        :
        [
            new(LlmRole.User, "Завтра экзамен, боюсь, что всё забуду."),
            new(LlmRole.Assistant, "Слышу, как вам тревожно перед экзаменом. Когда поставлено так много, страх всё забыть очень понятен."),
            new(LlmRole.User, "Поссорилась с мамой, до сих пор злюсь."),
            new(LlmRole.Assistant, "Похоже, ссора с мамой сильно вас задела, и злость никак не отпускает. Это очень выматывает.")
        ];

    private static string Hint(bool english, SituationAnalysis analysis)
    {
        if (analysis.Emotion == CompanionEmotion.Unknown)
        {
            return string.Empty;
        }

        string emotion = CompanionContent.EmotionName(analysis.Emotion, english);
        string themes = analysis.Themes.Count == 0
            ? string.Empty
            : (english ? " Topic: " : " Тема: ") + string.Join(", ", analysis.Themes.Select(t => CompanionContent.ThemeName(t, english))) + ".";

        return english
            ? $"The app guesses the person's main state is: {emotion}.{themes} Treat this only as a hint and rely on their own words."
            : $"Приложение предполагает, что главное состояние человека: {emotion}.{themes} Считай это лишь подсказкой и опирайся на слова самого человека.";
    }
}
