namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>Scripted wording of the companion. Used on its own when no language model is installed and as a fallback when the model's reply is rejected.</summary>
public static class CompanionContent
{
    public static string Greeting(bool english, Random random) => Pick(random, english
        ? ["Hi. I'm here to listen. Tell me in your own words what is going on right now.", "Hello. What's on your mind at the moment? Tell me as it comes."]
        : ["Привет. Я здесь, чтобы выслушать. Расскажите своими словами, что сейчас происходит.", "Здравствуйте. Что у вас на душе прямо сейчас? Расскажите, как есть."]);

    public static string PrivacyNote(bool english) => english
        ? "Everything you write stays on your device."
        : "Всё, что вы пишете, остаётся только на вашем устройстве.";

    public static string InputHint(bool english) => english ? "For example: I can't stop worrying about tomorrow's meeting" : "Например: не могу перестать переживать из-за завтрашней встречи";

    public static string Reflection(SituationAnalysis analysis, bool english, Random random)
    {
        string line = Pick(random, ReflectionLines(analysis.Emotion, english));
        if (analysis.Themes.Count > 0)
        {
            line += english
                ? $" It seems to be connected with {ThemeWith(analysis.Themes[0], true)}."
                : $" Судя по всему, это связано с {ThemeWith(analysis.Themes[0], false)}.";
        }

        if (analysis.IsIntense)
        {
            line += english ? " That sounds really hard." : " Звучит действительно тяжело.";
        }

        return line;
    }

    public static string ClarifyLead(bool english) => english
        ? "Thank you for telling me. I want to understand you better: which of these is closest to what you feel?"
        : "Спасибо, что рассказали. Хочу понять вас точнее: что из этого ближе всего к тому, что вы чувствуете?";

    public static IReadOnlyList<(string Label, CompanionEmotion Emotion)> ClarifyChoices(bool english) => english
        ?
        [
            ("Anxiety or fear", CompanionEmotion.Anxiety), ("Panic", CompanionEmotion.Panic), ("Thoughts going in circles", CompanionEmotion.Overthinking),
            ("Anger", CompanionEmotion.Anger), ("Hurt", CompanionEmotion.Resentment), ("Guilt or shame", CompanionEmotion.Guilt),
            ("Sadness", CompanionEmotion.Sadness), ("Exhaustion", CompanionEmotion.Exhaustion)
        ]
        :
        [
            ("Тревога, страх", CompanionEmotion.Anxiety), ("Паника", CompanionEmotion.Panic), ("Мысли крутятся по кругу", CompanionEmotion.Overthinking),
            ("Злость", CompanionEmotion.Anger), ("Обида", CompanionEmotion.Resentment), ("Вина, стыд", CompanionEmotion.Guilt),
            ("Грусть", CompanionEmotion.Sadness), ("Усталость", CompanionEmotion.Exhaustion)
        ];

    public static string UnknownChoiceLabel(bool english) => english ? "Not sure yet" : "Пока не знаю";

    public static string Understood(CompanionEmotion emotion, bool english) => english
        ? $"I see: {EmotionName(emotion, true)}. Thank you."
        : $"Понимаю: {EmotionName(emotion, false)}. Спасибо, что поделились.";

    public static string OfferLine(TechniqueId id, bool english) => english
        ? $"I can guide you through the practice \"{TechniqueTitle(id, true)}\": {TechniqueWhy(id, true)}"
        : $"Могу провести с вами практику «{TechniqueTitle(id, false)}»: {TechniqueWhy(id, false)}";

    public static string StartLabel(TechniqueId id, bool english) => english ? $"Let's start: {TechniqueTitle(id, true)}" : $"Начнём: {TechniqueTitle(id, false)}";

    public static string AlternativeLabel(TechniqueId id, bool english) => english ? $"Another practice: {TechniqueTitle(id, true)}" : $"Другая практика: {TechniqueTitle(id, false)}";

    public static string MoreLabel(bool english) => english ? "I want to say more" : "Хочу ещё рассказать";

    public static string VentPrompt(bool english, Random random) => Pick(random, english
        ? ["I'm listening. What feels hardest about it?", "Go on, I'm here. What else is important?"]
        : ["Я слушаю. Что в этом сложнее всего?", "Продолжайте, я здесь. Что ещё важно?"]);

    public static string Starting(bool english) => english ? "Okay, let's begin." : "Хорошо, начинаем.";

    public static string Crisis(bool english) => english
        ? "What you wrote is very important, and I'm worried about you. Right now it's best to talk to a real person. I'll open the urgent help section with phone numbers you can call."
        : "То, что вы написали, очень важно, и я переживаю за вас. Сейчас лучше всего поговорить с живым человеком. Я открою раздел срочной помощи с номерами, по которым можно позвонить.";

    public static string EmotionName(CompanionEmotion emotion, bool english) => emotion switch
    {
        CompanionEmotion.Panic => english ? "panic" : "паника",
        CompanionEmotion.Anxiety => english ? "anxiety" : "тревога",
        CompanionEmotion.Overthinking => english ? "racing thoughts" : "навязчивые мысли",
        CompanionEmotion.Anger => english ? "anger" : "злость",
        CompanionEmotion.Resentment => english ? "hurt feelings" : "обида",
        CompanionEmotion.Guilt => english ? "guilt" : "вина",
        CompanionEmotion.Sadness => english ? "sadness" : "грусть",
        CompanionEmotion.Exhaustion => english ? "exhaustion" : "усталость",
        CompanionEmotion.Loneliness => english ? "loneliness" : "одиночество",
        CompanionEmotion.Procrastination => english ? "difficulty getting started" : "трудности с началом дел",
        _ => english ? "unclear" : "неясно"
    };

    public static string ThemeName(CompanionTheme theme, bool english) => theme switch
    {
        CompanionTheme.Work => english ? "work" : "работа",
        CompanionTheme.Relationships => english ? "relationships" : "отношения",
        CompanionTheme.Family => english ? "family" : "семья",
        CompanionTheme.Health => english ? "health" : "здоровье",
        CompanionTheme.Money => english ? "money" : "деньги",
        _ => english ? "study" : "учёба"
    };

    public static string TechniqueTitle(TechniqueId id, bool english) => id switch
    {
        TechniqueId.Grounding => english ? "5-4-3-2-1 grounding" : "Заземление 5-4-3-2-1",
        TechniqueId.Breathing => english ? "Box breathing" : "Квадратное дыхание",
        TechniqueId.Observer => english ? "Observer position" : "Позиция наблюдателя",
        TechniqueId.ThoughtRecord => english ? "Thought record" : "Запись мысли",
        TechniqueId.Spin => english ? "Spin" : "Крутилка",
        TechniqueId.SelfCompassion => english ? "Kind words to yourself" : "Добрые слова себе",
        TechniqueId.SmallStep => english ? "One small step" : "Один маленький шаг",
        TechniqueId.Anchor => english ? "Resource anchor" : "Якорь ресурса",
        _ => id.ToString()
    };

    public static string TechniqueWhy(TechniqueId id, bool english) => id switch
    {
        TechniqueId.Grounding => english ? "it brings attention back to the present through your senses, about 3 minutes." : "она возвращает внимание в настоящий момент через органы чувств, около 3 минут.",
        TechniqueId.Breathing => english ? "it slows the body down when tension is high." : "она помогает телу замедлиться, когда напряжение высокое.",
        TechniqueId.Observer => english ? "it lets you step back from the situation and see it from the side, about 5 minutes." : "она помогает отойти от ситуации и увидеть её со стороны, около 5 минут.",
        TechniqueId.ThoughtRecord => english ? "it helps you check a painful thought against the facts." : "она помогает проверить тяжёлую мысль на прочность.",
        TechniqueId.Spin => english ? "it lowers the emotional charge of a painful memory." : "она снижает эмоциональный заряд болезненного воспоминания.",
        TechniqueId.SelfCompassion => english ? "it helps you treat yourself the way you would treat a friend." : "она помогает поддержать себя так, как вы поддержали бы друга.",
        TechniqueId.SmallStep => english ? "it turns a heavy task into one doable step." : "она превращает тяжёлое дело в один посильный шаг.",
        TechniqueId.Anchor => english ? "it brings back a feeling of support you already know." : "она возвращает знакомое ощущение опоры.",
        _ => string.Empty
    };

    private static string ThemeWith(CompanionTheme theme, bool english) => theme switch
    {
        CompanionTheme.Work => english ? "work" : "работой",
        CompanionTheme.Relationships => english ? "relationships" : "отношениями",
        CompanionTheme.Family => english ? "family" : "семьёй",
        CompanionTheme.Health => english ? "health" : "здоровьем",
        CompanionTheme.Money => english ? "money" : "деньгами",
        _ => english ? "study" : "учёбой"
    };

    private static string[] ReflectionLines(CompanionEmotion emotion, bool english) => emotion switch
    {
        CompanionEmotion.Panic => english
            ? ["It sounds like panic is washing over you right now. It is a very hard state, but it passes, and you are not alone in it.", "It sounds like everything inside has tightened up with panic. Let's help your body settle first."]
            : ["Похоже, вас сейчас накрывает паника. Это очень тяжёлое состояние, но оно проходит, и вы не одни в этом.", "Звучит так, будто внутри всё сжалось от паники. Давайте сначала поможем телу успокоиться."],
        CompanionEmotion.Anxiety => english
            ? ["It sounds like this situation worries you a lot. Anxiety takes a lot of energy.", "I hear that you feel uneasy and on edge. That is an understandable reaction."]
            : ["Похоже, вас сильно тревожит эта ситуация. Тревога забирает много сил.", "Слышу, что вам тревожно и неспокойно. Это понятная реакция на такую ситуацию."],
        CompanionEmotion.Overthinking => english
            ? ["It sounds like your thoughts keep going in circles and give you no rest. That is exhausting.", "I hear that the same thought keeps coming back again and again."]
            : ["Похоже, мысли крутятся по кругу и не дают передышки. Это очень выматывает.", "Слышу, что одна и та же мысль возвращается снова и снова."],
        CompanionEmotion.Anger => english
            ? ["I hear that you're angry, and that makes sense. Anger often shows that something important was touched.", "It sounds like this really got to you and made you angry."]
            : ["Слышу, что вы злитесь, и это можно понять. Злость часто показывает, что задели что-то важное.", "Похоже, эта ситуация сильно вас задела и разозлила."],
        CompanionEmotion.Resentment => english
            ? ["It sounds like you feel really hurt. When someone touches you like that, it truly hurts.", "I hear hurt in your words. That feeling has a right to be here."]
            : ["Похоже, вам очень обидно. Когда задевают, это правда больно.", "Слышу обиду в ваших словах. Это чувство имеет право быть."],
        CompanionEmotion.Guilt => english
            ? ["It sounds like guilt is weighing on you. That is a heavy load.", "I hear that you are being very hard on yourself right now."]
            : ["Похоже, вас гложет чувство вины. Это тяжёлый груз.", "Слышу, что вы сейчас очень строги к себе."],
        CompanionEmotion.Sadness => english
            ? ["I'm sorry you feel so sad right now. It is important not to push this feeling away.", "I hear heaviness and sorrow in your words."]
            : ["Мне жаль, что вам сейчас так грустно. Это состояние важно не отталкивать.", "Слышу тоску и тяжесть в ваших словах."],
        CompanionEmotion.Exhaustion => english
            ? ["It sounds like you are very tired and have almost no energy left. That is a serious signal to take care of yourself.", "I hear that you are worn out. Exhaustion deserves to be taken seriously too."]
            : ["Похоже, вы очень устали, и сил почти не осталось. Это серьёзный сигнал беречь себя.", "Слышу, что вы измотаны. Усталость тоже нужно принимать всерьёз."],
        CompanionEmotion.Loneliness => english
            ? ["I'm sorry you feel lonely. That is a very hard feeling.", "I hear loneliness in your words. It matters that you told me."]
            : ["Мне жаль, что вы чувствуете себя одиноко. Это очень тяжёлое чувство.", "Слышу одиночество в ваших словах. Хорошо, что вы об этом рассказали."],
        CompanionEmotion.Procrastination => english
            ? ["It sounds like it is hard to get started, and that weighs on you. It happens when a task feels too big.", "I hear that things are stuck and you are blaming yourself for it. That is very common."]
            : ["Похоже, вам трудно начать, и это давит. Так бывает, когда задача кажется слишком большой.", "Слышу, что дело стоит, а вы себя за это ругаете. Это очень частая ситуация."],
        _ => english
            ? ["Thank you for telling me. It sounds like this matters to you."]
            : ["Спасибо, что рассказали. Похоже, это для вас важно."]
    };

    private static string Pick(Random random, string[] variants) =>
        variants.Length == 1 ? variants[0] : variants[random.Next(variants.Length)];
}
