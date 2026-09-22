using PsychologyApp.Application.Abstractions.Integration;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Wording for pointing the person at something else in the app besides a technique: a quote shown right in the chat,
/// or a chip that opens tests, the body explorer or calming audio. Kept light — these are offers, not homework, and
/// each is made once per chat so the conversation does not turn into a menu.
/// </summary>
public static class CompanionResourceContent
{
    public static string QuoteLead(bool english, Random random) => Pick(random, english
        ? ["This one might land:", "Something that might resonate:", "Here's something worth sitting with:"]
        : ["Вот что может откликнуться:", "Возможно, это созвучно тому, что вы чувствуете:", "Есть слова, которые иногда помогают:"]);

    public static string QuoteLine(QuotSeed quote, bool english, Random random)
    {
        string lead = QuoteLead(english, random);
        string attribution = string.IsNullOrWhiteSpace(quote.Author)
            ? string.Empty
            : english ? $" — {quote.Author}" : $" — {quote.Author}";
        return $"{lead}\n«{quote.Text}»{attribution}";
    }

    public static ChatQuickReply MoreQuotesChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "More quotes like this" : "Ещё похожие цитаты", "resource:quotes");

    public static string SomaticOffer(bool english) => english
        ? "Feelings like this often show up in the body too. There's a section that helps notice where — want a look?"
        : "Такие чувства часто отзываются и в теле. Есть раздел, который помогает заметить, где именно — заглянуть?";

    public static ChatQuickReply SomaticChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Check what the body feels" : "Заметить, что чувствует тело", "resource:somatic");

    public static string SomaticTransition(bool english) => english
        ? "Opening it — take your time there."
        : "Открываю. Не торопитесь там.";

    public static string TestOffer(bool english) => english
        ? "If you'd like a clearer picture, there's a short, optional self-assessment for this in the Tests section."
        : "Если хочется увидеть картину яснее, в разделе тестов есть короткий необязательный опросник как раз про это.";

    public static ChatQuickReply TestChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Take a short test" : "Пройти короткий тест", "resource:test");

    public static string TestTransition(bool english) => english
        ? "Opening the tests — pick whichever one fits."
        : "Открываю тесты — выберите тот, что подходит.";

    public static string PrayerOffer(bool english) => english
        ? "There's also calming audio in the app, if a voice to breathe along with would help."
        : "В приложении есть ещё успокаивающие аудио — если хочется, чтобы кто-то вёл дыхание голосом.";

    public static ChatQuickReply PrayerChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Open calming audio" : "Открыть успокаивающее аудио", "resource:prayer");

    public static string PrayerTransition(bool english) => english
        ? "Opening it now."
        : "Открываю.";

    public static string QuotesTransition(bool english) => english
        ? "Opening more quotes."
        : "Открываю ещё цитаты.";

    private static string Pick(Random random, string[] variants) => variants[random.Next(variants.Length)];
}
