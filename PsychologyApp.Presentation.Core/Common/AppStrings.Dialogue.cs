namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string DialogueInputPlaceholder => T("Ваш ответ…", "Your answer…");
    public static string DialogueTyping => T("печатает…", "typing…");
    public static string DialogueFinish => T("Завершить", "Finish");
    public static string DialogueClose => T("Закрыть", "Close");
    public static string DialogueRatingScale => T("0 — совсем нет, 10 — очень сильно", "0 — not at all, 10 — very strong");
    public static string DialogueLoadFailed => T(
        "Не удалось открыть диалог. Попробуйте ещё раз позже.",
        "Could not open the dialogue. Please try again later.");
}
