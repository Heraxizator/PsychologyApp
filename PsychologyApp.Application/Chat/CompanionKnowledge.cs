using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Short psychoeducation the companion gives when asked "what is…", "why do I…" or "is this normal?".
/// General, non-diagnostic wording; every text ends by returning to the person. Needs review by a psychologist before release.
/// </summary>
public static class CompanionKnowledge
{
    private sealed record Topic(string Id, string[] Keys, string Ru, string En, string LabelRu, string LabelEn);

    private static readonly Topic[] Topics =
    [
        new("panic", ["паническ", "паник", "panic"],
            "Паническая атака — внезапный всплеск тревоги: сердце колотится, не хватает воздуха, кружится голова. Это работа системы «бей или беги», которая включилась без реальной угрозы. Обычно она достигает пика за 5–10 минут и проходит сама, хотя ощущается очень страшно. Помогают медленный выдох и опора на ощущения тела.",
            "A panic attack is a sudden surge of anxiety: pounding heart, shortness of breath, dizziness. It is the fight-or-flight system switching on without a real threat. It usually peaks within 5–10 minutes and passes on its own, even though it feels frightening. A slow exhale and focusing on physical sensations help.",
            "Паническая атака", "Panic attack"),
        new("anxiety", ["тревог", "тревож", "anxiety", "anxious"],
            "Тревога — сигнал о возможной опасности, обращённый в будущее. В небольших дозах она полезна, но когда сигнал звучит постоянно, тело остаётся в напряжении, а мысли крутятся вокруг «а вдруг». Помогают замедление дыхания, возвращение к настоящему моменту и проверка тревожных мыслей на реалистичность.",
            "Anxiety is a signal of possible danger directed at the future. In small doses it is useful, but when it keeps sounding, the body stays tense and thoughts circle around “what if”. Slowing the breath, returning to the present moment and checking anxious thoughts for realism help.",
            "Тревога", "Anxiety"),
        new("cbt", ["кпт", "когнитивно", "cbt", "cognitive"],
            "КПТ — когнитивно-поведенческая терапия. Идея простая: чувства зависят не только от событий, но и от того, что мы о них думаем. В КПТ учатся замечать автоматические мысли, проверять их фактами и пробовать иные поступки. Это один из самых изученных подходов.",
            "CBT is cognitive behavioural therapy. The idea is simple: feelings depend not only on events but on what we think about them. In CBT you learn to notice automatic thoughts, test them against facts and try different actions. It is one of the most researched approaches.",
            "Что такое КПТ", "What is CBT"),
        new("mindfulness", ["осознанност", "майндфулнес", "mindful"],
            "Осознанность — умение замечать, что происходит сейчас (дыхание, ощущения, мысли), не оценивая и не споря с этим. Она не убирает неприятное, но помогает не уноситься за мыслями. Начать можно с минуты внимания к дыханию.",
            "Mindfulness is the skill of noticing what is happening now (breath, sensations, thoughts) without judging or arguing with it. It does not remove unpleasant things, but helps you not be swept away by thoughts. You can start with one minute of attention to the breath.",
            "Осознанность", "Mindfulness"),
        new("burnout", ["выгора", "burnout", "burn out", "burnt out"],
            "Выгорание — состояние истощения от длительного стресса, чаще рабочего: усталость, цинизм, ощущение, что ничего не получается. Это не слабость, а реакция на нагрузку без достаточного восстановления. Помогают границы, отдых, который действительно восстанавливает, и разговор о нагрузке.",
            "Burnout is exhaustion from prolonged stress, usually work-related: fatigue, cynicism, a sense that nothing works. It is not weakness but a reaction to load without enough recovery. Boundaries, rest that truly restores and talking about the workload help.",
            "Выгорание", "Burnout"),
        new("procrastination", ["прокрастинац", "procrastinat"],
            "Прокрастинация — откладывание дел, хотя мы понимаем, что будет хуже. Чаще это не лень, а попытка избежать неприятного чувства: страха ошибки, скуки, неопределённости. Помогает уменьшить первый шаг до пяти минут и не требовать от себя идеала.",
            "Procrastination is putting things off even though we know it will get worse. It is often not laziness but an attempt to avoid an unpleasant feeling: fear of failure, boredom, uncertainty. Shrinking the first step to five minutes and not demanding perfection helps.",
            "Прокрастинация", "Procrastination"),
        new("boundaries", ["границ", "boundar"],
            "Личные границы — это то, что для вас допустимо и недопустимо в общении и отношениях. Когда границы размыты, копится усталость и обида. Их можно обозначать спокойно и без объяснений: «Мне это не подходит».",
            "Personal boundaries are what is acceptable and unacceptable to you in contact and relationships. When boundaries are blurred, fatigue and resentment build up. You can state them calmly and without lengthy explanations: “That doesn't work for me.”",
            "Личные границы", "Boundaries"),
        new("selfcompassion", ["самосострадан", "self-compassion", "self compassion"],
            "Самосострадание — отношение к себе так, как вы отнеслись бы к близкому другу в трудный момент: без самобичевания, с признанием, что ошибаться и уставать свойственно всем. Исследования показывают, что оно помогает меняться лучше, чем строгая самокритика.",
            "Self-compassion means treating yourself the way you would treat a close friend in a hard moment: without self-punishment, admitting that everyone makes mistakes and gets tired. Research suggests it helps people change better than harsh self-criticism.",
            "Самосострадание", "Self-compassion"),
        new("stress", ["стресс", "stress"],
            "Стресс — реакция организма на нагрузку. Кратковременный помогает мобилизоваться, длительный истощает: страдают сон, настроение, внимание. Важно не только убирать причины, но и давать телу восстановиться: сон, движение, паузы.",
            "Stress is the body's reaction to load. Short-term it helps mobilise, long-term it exhausts: sleep, mood and attention suffer. It matters not only to remove causes but to let the body recover: sleep, movement, pauses.",
            "Стресс", "Stress")
    ];

    /// <summary>Explanation of the topic named in <paramref name="text"/>, or null when none is recognised.</summary>
    public static string? Explain(string text, bool english)
    {
        string lower = text.ToLowerInvariant();
        Topic? topic = Topics.FirstOrDefault(t => t.Keys.Any(lower.Contains));
        return topic is null ? null : (english ? topic.En : topic.Ru) + (english ? " Would you like to relate it to your situation?" : " Хотите соотнести это со своей ситуацией?");
    }

    /// <summary>Explanation for a chip payload produced by <see cref="TopicChips"/>.</summary>
    public static string? ExplainById(string? id, bool english)
    {
        Topic? topic = Topics.FirstOrDefault(t => t.Id == id);
        return topic is null ? null : english ? topic.En : topic.Ru;
    }

    /// <summary>Topics offered when the person asks to explain something we do not recognise.</summary>
    public static IReadOnlyList<ChatQuickReply> TopicChips(bool english) =>
        Topics.Take(6).Select(t => new ChatQuickReply(ChatQuickReplyKinds.Act, english ? t.LabelEn : t.LabelRu, "topic:" + t.Id)).ToArray();

    public static string ExplainFallback(bool english) => english
        ? "I can briefly explain some things: panic, anxiety, CBT, mindfulness, burnout and more. Which one interests you?"
        : "Я могу коротко объяснить некоторые вещи: панику, тревогу, КПТ, осознанность, выгорание и другое. Что вас интересует?";

    public static string Why(CompanionEmotion emotion, bool english) => emotion switch
    {
        CompanionEmotion.Panic or CompanionEmotion.Anxiety => english
            ? "Anxiety usually comes when the mind sees a possible threat and the body prepares to defend itself: heart rate goes up, muscles tense. Often it is fed by tiredness, lack of sleep, uncertainty and long stress. It does not mean something is wrong with you."
            : "Тревога обычно появляется, когда мозг видит возможную угрозу, а тело готовится защищаться: учащается пульс, напрягаются мышцы. Часто её подпитывают усталость, недосып, неопределённость и долгий стресс. Это не значит, что с вами что-то не так.",
        CompanionEmotion.Overthinking => english
            ? "The mind circles around a problem because it is trying to solve it or to be ready for everything. But rumination rarely leads to a solution; it only adds tension. Writing thoughts down or naming a next step helps to break the circle."
            : "Ум ходит по кругу, потому что пытается решить проблему или подготовиться ко всему. Но «пережёвывание» редко приводит к решению, зато добавляет напряжения. Помогает записать мысли или назвать один следующий шаг.",
        CompanionEmotion.Anger or CompanionEmotion.Resentment => english
            ? "Anger and resentment usually point to something important that was crossed: a boundary, a need, fairness. It is a signal, not a flaw. It becomes heavy when it has no outlet, so it helps to name what exactly mattered."
            : "Злость и обида обычно указывают, что задето что-то важное: граница, потребность, справедливость. Это сигнал, а не изъян. Тяжело становится, когда ему нет выхода, поэтому помогает назвать, что именно для вас важно.",
        CompanionEmotion.Guilt => english
            ? "Guilt appears when our actions clash with our values. In moderation it helps to make amends, but it often grows out of proportion and turns into self-blame. It is worth separating what was really in your control from what was not."
            : "Вина возникает, когда поступок расходится с нашими ценностями. В меру она помогает исправить ситуацию, но часто разрастается и превращается в самобичевание. Стоит отделить то, что действительно было в вашей власти, от того, что нет.",
        CompanionEmotion.Sadness or CompanionEmotion.Loneliness => english
            ? "Sadness and loneliness often follow losses, changes or a lack of close contact. They tell us that something valuable is missing. Feelings like this need time and warmth rather than being pushed away."
            : "Грусть и одиночество часто приходят после потерь, перемен или при нехватке близкого контакта. Они говорят, что не хватает чего-то ценного. Таким чувствам нужны время и тепло, а не подавление.",
        CompanionEmotion.Exhaustion => english
            ? "Exhaustion builds up when demands exceed resources for too long: little sleep, constant tension, no time to recover. The body simply asks for a pause. That is not laziness."
            : "Истощение накапливается, когда требования долго превышают ресурсы: мало сна, постоянное напряжение, нет времени восстановиться. Тело просто просит паузы. Это не лень.",
        CompanionEmotion.Procrastination => english
            ? "Putting things off is usually about avoiding an unpleasant feeling — fear of failing, of criticism or of a huge task — not about laziness. When the first step shrinks, the resistance often shrinks with it."
            : "Откладывание чаще всего связано с желанием избежать неприятного чувства — страха не справиться, критики или огромной задачи, — а не с ленью. Когда первый шаг становится совсем маленьким, сопротивление тоже уменьшается.",
        _ => english
            ? "There is rarely a single reason: feelings come from a mix of events, thoughts, tiredness and the body's state. If you tell me what has been happening lately, we can look for what may lie behind it."
            : "Единственной причины обычно нет: чувства складываются из событий, мыслей, усталости и состояния тела. Если расскажете, что происходило в последнее время, вместе поищем, что за этим стоит."
    };

    public static string Normal(CompanionEmotion emotion, bool english)
    {
        string feeling = emotion == CompanionEmotion.Unknown
            ? (english ? "What you describe" : "То, что вы описываете")
            : Capitalize(CompanionContent.EmotionName(emotion, english));
        return english
            ? $"{feeling} is a common human reaction, and many people go through it. Feeling this does not mean you are “broken”. If it lasts for weeks, gets stronger or gets in the way of daily life, it is worth talking to a specialist. I can't diagnose, but I can be with you meanwhile."
            : $"{feeling} — обычная человеческая реакция, через неё проходят многие. Это не значит, что вы «сломаны». Если это длится неделями, усиливается или мешает жить, стоит обсудить это со специалистом. Диагнозов я не ставлю, но могу быть рядом в это время.";
    }

    /// <summary>What usually helps with the feeling. Used when the person asks for advice.</summary>
    public static string WhatHelps(CompanionEmotion emotion, bool english) => emotion switch
    {
        CompanionEmotion.Panic or CompanionEmotion.Anxiety => english
            ? "With anxiety it often helps to slow the exhale, name five things you see around you and let yourself do one small next step instead of solving everything at once."
            : "При тревоге часто помогает замедлить выдох, назвать пять вещей, которые вы видите вокруг, и сделать один маленький следующий шаг вместо того, чтобы решать всё сразу.",
        CompanionEmotion.Overthinking => english
            ? "With rumination it often helps to write the thought down, ask “what is the evidence?” and choose one action for today."
            : "Когда мысли ходят по кругу, часто помогает записать мысль, спросить себя «какие есть факты?» и выбрать одно действие на сегодня.",
        CompanionEmotion.Anger or CompanionEmotion.Resentment => english
            ? "With anger it often helps to give the body a release first (a walk, a long exhale), and only then to name what mattered and decide whether to say it."
            : "При злости часто помогает сначала дать телу разрядку (прогулка, долгий выдох), а потом назвать, что было важно, и решить, стоит ли это сказать.",
        CompanionEmotion.Guilt => english
            ? "With guilt it helps to separate what was in your control, decide whether anything can be repaired and speak to yourself as you would to a friend."
            : "При вине помогает отделить то, что было в вашей власти, решить, можно ли что-то исправить, и говорить с собой так, как вы говорили бы с другом.",
        CompanionEmotion.Sadness or CompanionEmotion.Loneliness => english
            ? "With sadness and loneliness small doses of warmth help: a short message to someone close, a walk, something pleasant and undemanding. Feelings don't have to be rushed."
            : "При грусти и одиночестве помогают маленькие дозы тепла: короткое сообщение близкому, прогулка, что-то приятное и необременительное. Чувства не нужно торопить.",
        CompanionEmotion.Exhaustion => english
            ? "With exhaustion sleep, short real pauses and dropping something non-essential from the list usually help more than pushing harder."
            : "При истощении обычно помогают сон, короткие настоящие паузы и отказ от чего-то несущественного в списке дел, а не давление на себя.",
        CompanionEmotion.Procrastination => english
            ? "With procrastination it helps to shrink the first step to five minutes and start with the easiest part."
            : "При откладывании помогает уменьшить первый шаг до пяти минут и начать с самой лёгкой части.",
        _ => english
            ? "It often helps to slow down, name what you feel and pick one small step."
            : "Часто помогает притормозить, назвать, что вы чувствуете, и выбрать один маленький шаг."
    };

    public static string AdviceOffer(bool english) => english
        ? "Shall I suggest a short practice for it?"
        : "Предложить короткую практику для этого?";

    public static ChatQuickReply PracticeChip(bool english) => new(ChatQuickReplyKinds.Act, english ? "Yes, suggest one" : "Да, предложи", "practice");

    private static string Capitalize(string value) => value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];
}
