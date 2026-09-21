using System.Globalization;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Wording for the companion's profile: trust levels, insights drawn from the person's own statistics and the honest description
/// of what the companion is. Insights only state what the numbers show and never diagnose. Needs review by a psychologist before release.
/// </summary>
public static class ChatProfileContent
{
    public const int TrustLevels = 4;

    public static string TrustName(int level, bool english) => level switch
    {
        0 => english ? "Just met" : "Только знакомимся",
        1 => english ? "Getting to know each other" : "Знакомство",
        2 => english ? "Trust" : "Доверие",
        _ => english ? "Close companion" : "Близкий собеседник"
    };

    public static string TrustHint(TrustLevel trust, bool english) => trust.MessagesToNext == 0
        ? english ? "The highest level. Thank you for your trust." : "Высший уровень. Спасибо за доверие."
        : english
            ? $"{trust.MessagesToNext} more {(trust.MessagesToNext == 1 ? "message" : "messages")} to “{TrustName(trust.Index + 1, true)}”"
            : $"Ещё {trust.MessagesToNext} {Plural(trust.MessagesToNext, "сообщение", "сообщения", "сообщений")} до уровня «{TrustName(trust.Index + 1, false)}»";

    public static string Tagline(bool english) => english
        ? "Listens, helps you sort out feelings and suggests short practices"
        : "Слушает, помогает разобраться в чувствах и предлагает короткие практики";

    public static string PrivacyLine(bool english) => english
        ? "Works offline. Everything stays on this phone."
        : "Работает без интернета. Всё остаётся на этом телефоне.";

    public static string Disclaimer(bool english) => english
        ? "I'm an app, not a person and not a doctor. I don't diagnose and I don't replace a specialist. If things are very hard, the crisis section is one tap away."
        : "Я приложение, не человек и не врач. Я не ставлю диагнозов и не заменяю специалиста. Если очень тяжело, раздел экстренной помощи всегда под рукой.";

    public static IReadOnlyList<string> Abilities(bool english) => english
        ?
        [
            "Listen without judging",
            "Name what you feel",
            "Measure tension from 0 to 10",
            "Suggest a short practice",
            "Explain things like panic or CBT"
        ]
        :
        [
            "Выслушать без оценок",
            "Назвать то, что вы чувствуете",
            "Измерить напряжение от 0 до 10",
            "Предложить короткую практику",
            "Объяснить, что такое паника или КПТ"
        ];

    public static string Since(DateTime? sinceUtc, bool english)
    {
        if (sinceUtc is not { } at)
        {
            return english ? "We haven't talked yet" : "Мы ещё не разговаривали";
        }

        string date = english
            ? at.ToString("d MMMM yyyy", CultureInfo.GetCultureInfo("en-US"))
            : at.ToString("d MMMM yyyy", CultureInfo.GetCultureInfo("ru-RU"));
        return english ? $"Together since {date}" : $"Вместе с {date}";
    }

    public static string TensionCaption(CompanionProfile profile, bool english)
    {
        if (profile.TensionMeasured == 0)
        {
            return english
                ? "Rate your tension from 0 to 10 in a chat and the changes will appear here."
                : "Оцените напряжение от 0 до 10 в чате, и здесь появится динамика.";
        }

        if (profile.AverageDrop is not { } drop)
        {
            return string.Empty;
        }

        string value = Math.Abs(drop).ToString("0.#", CultureInfo.InvariantCulture);
        if (drop > 0.05)
        {
            return english
                ? $"On average the tension dropped by {value} points during a chat."
                : $"В среднем за разговор напряжение снижалось на {value} {PluralPoints(Math.Abs(drop))}.";
        }

        return drop < -0.05
            ? english
                ? $"On average the tension rose by {value} points. Hard topics do that, and talking about them still counts."
                : $"В среднем напряжение росло на {value} {PluralPoints(Math.Abs(drop))}. Так бывает с трудными темами, и говорить о них всё равно важно."
            : english ? "On average the tension stayed at the same level." : "В среднем напряжение оставалось на одном уровне.";
    }

    /// <summary>Short sentences that turn the numbers into something the person can use. Only what is true for them; empty history gives an invitation.</summary>
    public static IReadOnlyList<string> Insights(CompanionProfile profile, bool english)
    {
        if (!profile.HasHistory)
        {
            return
            [
                english
                    ? "Write about what is on your mind. After the first conversations this page will show what helps you."
                    : "Напишите, что у вас на душе. После первых разговоров здесь появится то, что вам помогает."
            ];
        }

        List<string> lines = [];

        if (profile.TensionMeasured >= 2 && profile.TensionImproved > 0)
        {
            lines.Add(english
                ? $"In {profile.TensionImproved} of {profile.TensionMeasured} chats the tension dropped."
                : $"В {profile.TensionImproved} из {profile.TensionMeasured} разговоров напряжение снизилось.");
        }

        if (profile.Practices.FirstOrDefault(p => p.Helped > 0) is { } best)
        {
            string title = CompanionContent.TechniqueTitle(best.Technique, english);
            lines.Add(english
                ? $"“{title}” helps you most: it worked {best.Helped} {(best.Helped == 1 ? "time" : "times")}."
                : $"Лучше всего вам помогает «{title}»: она сработала {best.Helped} {Plural(best.Helped, "раз", "раза", "раз")}.");
        }

        if (profile.Emotions.FirstOrDefault() is { } top && profile.Emotions.Sum(e => e.Chats) >= 3)
        {
            string name = CompanionContent.EmotionName(top.Emotion, english);
            int percent = (int)Math.Round(top.Share * 100);
            lines.Add(english
                ? $"Most often you talk about {name} ({percent}% of chats)."
                : $"Чаще всего вы говорите о теме «{name}» ({percent}% разговоров).");
        }

        if (profile.StreakDays >= 2)
        {
            lines.Add(english
                ? $"You have been coming back for {profile.StreakDays} days in a row. Regularity helps."
                : $"Вы заходите {profile.StreakDays} {Plural(profile.StreakDays, "день", "дня", "дней")} подряд. Регулярность помогает.");
        }

        if (lines.Count == 0)
        {
            lines.Add(english
                ? "The more we talk, the more precisely I can see what helps you."
                : "Чем больше мы разговариваем, тем точнее видно, что вам помогает.");
        }

        return lines;
    }

    public static string Plural(int n, string one, string few, string many)
    {
        int tens = n % 100;
        int units = n % 10;
        if (tens is >= 11 and <= 14)
        {
            return many;
        }

        return units switch
        {
            1 => one,
            >= 2 and <= 4 => few,
            _ => many
        };
    }

    private static string PluralPoints(double value) =>
        Math.Abs(value - Math.Round(value)) > 0.05 ? "балла" : Plural((int)Math.Round(value), "балл", "балла", "баллов");
}
