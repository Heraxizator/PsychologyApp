using PsychologyApp.Application.Articles;
using PsychologyApp.Application.Conversation.Companion;

namespace PsychologyApp.Application.Chat;

/// <summary>
/// Wording of the guided exercises the companion runs inside the chat (checking a thought, solving a problem step by step), of the
/// follow-up on a step decided in an earlier chat, and of pointing to a questionnaire or an article. Psychologist review before release.
/// </summary>
public static class CompanionProtocolContent
{
    public const string Think = "think";
    public const string Solve = "solve";

    public const int ThinkBeliefBefore = 1;
    public const int ThinkBeliefAfter = 6;
    public const int ThinkSteps = 7;
    public const int SolveSteps = 4;

    /// <summary>The step decided on is kept short: it is quoted back in the next chat.</summary>
    public const int MaxPlanLength = 160;

    // ----- offers -----

    public static ChatQuickReply ThinkChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Check this thought together" : "Проверить эту мысль вместе", "protocol:" + Think);

    public static ChatQuickReply SolveChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Work out what to do, step by step" : "Разобрать по шагам, что делать", "protocol:" + Solve);

    public static ChatQuickReply StopChip(bool english) =>
        new(ChatQuickReplyKinds.Act, english ? "Stop here" : "Остановиться", "protocol:stop");

    public static string Stopped(bool english) => english
        ? "Okay, let's stop here. What you've already written is not lost: we can come back to it whenever you like."
        : "Хорошо, остановимся. То, что вы уже написали, не пропало: к этому можно вернуться в любой момент.";

    // ----- checking a thought (cognitive restructuring) -----

    public static string ThinkQuestion(int step, bool english) => step switch
    {
        0 => english
            ? "Let's look at the thought itself. Which thought is the heaviest right now? Write it as one sentence, the way it sounds in your head."
            : "Давайте посмотрим на саму мысль. Какая мысль сейчас самая тяжёлая? Запишите её одной фразой, так, как она звучит в голове.",
        1 => english ? "How much do you believe it right now, from 0 to 10?" : "Насколько вы в неё верите прямо сейчас, от 0 до 10?",
        2 => english
            ? "What supports this thought? Only facts, what someone watching from the side could see."
            : "Что говорит за эту мысль? Только факты, то, что увидел бы человек со стороны.",
        3 => english
            ? "And what goes against it? What doesn't fit this thought, even something small?"
            : "А что говорит против неё? Что в неё не вписывается, даже мелочь?",
        4 => english
            ? "If someone close to you had this thought, what would you tell them?"
            : "Если бы так думал близкий вам человек, что бы вы ему сказали?",
        5 => english
            ? "Taking all this into account, how could the same thing be said more accurately and fairly?"
            : "С учётом всего этого, как можно сказать то же самое точнее и честнее?",
        _ => english ? "How much do you believe the first thought now, from 0 to 10?" : "Насколько вы теперь верите первой мысли, от 0 до 10?"
    };

    public static string ThinkSummary(string thought, string balanced, int before, int after, bool english)
    {
        string change = english
            ? after < before
                ? "The thought has loosened its grip: that is exactly how work with thoughts goes."
                : "The thought is still holding on, and that's okay: sometimes it needs more facts. A thought record helps to go deeper."
            : after < before
                ? "Мысль ослабла: так и выглядит работа с мыслями."
                : "Мысль пока держится, и это нормально: иногда ей нужно больше фактов. Глубже поможет практика «Запись мысли».";
        return english
            ? $"Thought: “{thought}”. Belief was {before}, now {after}.\nMore accurate: “{balanced}”.\n{change} You can reread the new wording when the thought comes back."
            : $"Мысль: «{thought}». Верили на {before}, сейчас на {after}.\nТочнее: «{balanced}».\n{change} Новую формулировку можно перечитать, когда мысль вернётся.";
    }

    // ----- problem solving -----

    public static string SolveQuestion(int step, bool english) => step switch
    {
        0 => english
            ? "Let's take it apart. In one sentence: what exactly needs solving?"
            : "Давайте разберём. Одной фразой: что именно нужно решить?",
        1 => english
            ? "What options are there? Name two or three, even strange or imperfect ones."
            : "Какие есть варианты? Назовите два-три, даже странные или неидеальные.",
        2 => english ? "Which of them is the most doable right now?" : "Какой из них самый выполнимый прямо сейчас?",
        _ => english
            ? "What is the first small step, for 10–15 minutes? And when will you take it?"
            : "Какой первый маленький шаг, на 10–15 минут? И когда вы его сделаете?"
    };

    public static string SolveSummary(string problem, string choice, string step, bool english) => english
        ? $"Task: “{problem}”. The way you chose: “{choice}”. First step: “{step}”.\nNext time we talk, I'll ask how it went."
        : $"Задача: «{problem}». Выбранный путь: «{choice}». Первый шаг: «{step}».\nВ следующий раз я спрошу, как получилось.";

    // ----- the step decided in an earlier chat -----

    public static string PlanQuestion(string plan, bool english) => english
        ? $"Last time you decided: “{plan}”. How did it go?"
        : $"В прошлый раз вы решили: «{plan}». Как получилось?";

    public static IReadOnlyList<ChatQuickReply> PlanChips(bool english) =>
    [
        new(ChatQuickReplyKinds.Act, english ? "I did it" : "Получилось", "plan:done"),
        new(ChatQuickReplyKinds.Act, english ? "Partly" : "Частично", "plan:partly"),
        new(ChatQuickReplyKinds.Act, english ? "It didn't work out" : "Не вышло", "plan:no")
    ];

    public static string PlanAnswer(string result, bool english) => result switch
    {
        "done" => english
            ? "That's a real step, and you took it. What helped you do it? Worth remembering for next time."
            : "Это настоящий шаг, и вы его сделали. Что помогло? Это стоит запомнить на следующий раз.",
        "partly" => english
            ? "Partly is already movement. What got in the way of the rest?"
            : "Частично — это уже движение. Что помешало сделать остальное?",
        _ => english
            ? "That happens: the step turned out bigger than it seemed, and it says nothing bad about you. What could be done in just five minutes instead?"
            : "Так бывает: шаг оказался больше, чем казалось, и это ничего плохого о вас не говорит. Что можно сделать хотя бы за пять минут?"
    };

    // ----- a questionnaire when heavy days keep coming back -----

    public static string TestNudge(string testName, bool english) => english
        ? $"This isn't the first hard conversation in the last two weeks. The short {testName} questionnaire shows how strong it is; its result is useful to a specialist too."
        : $"За последние две недели это уже не первый тяжёлый разговор. Короткий опросник {testName} покажет, насколько это сильно; его результат пригодится и специалисту.";

    public static ChatQuickReply TestNudgeChip(string testId, string testName, bool english) =>
        new(ChatQuickReplyKinds.Act, english ? $"Take {testName}" : $"Пройти {testName}", "test:" + testId);

    public static string TestNudgeTransition(bool english) => english ? "Opening it." : "Открываю.";

    public static string TestName(string testId) => testId switch
    {
        "phq9" => "PHQ-9",
        "gad7" => "GAD-7",
        "pcl5" => "PCL-5",
        _ => testId
    };

    /// <summary>Which questionnaire measures a feeling that keeps coming back; null when none fits.</summary>
    public static string? TestFor(CompanionEmotion emotion, bool flashback) => flashback
        ? "pcl5"
        : emotion switch
        {
            CompanionEmotion.Sadness or CompanionEmotion.Exhaustion or CompanionEmotion.Loneliness => "phq9",
            CompanionEmotion.Anxiety or CompanionEmotion.Panic or CompanionEmotion.Overthinking => "gad7",
            _ => null
        };

    // ----- an article for what was talked about -----

    /// <summary>The article that explains the recognised state; a closed table, like the practices.</summary>
    public static string? ArticleFor(CompanionEmotion emotion, bool flashback) => flashback
        ? ArticleIds.TraumaFlashbacks
        : emotion switch
        {
            CompanionEmotion.Panic => ArticleIds.Panic,
            CompanionEmotion.Anxiety => ArticleIds.Anxiety,
            CompanionEmotion.Overthinking => ArticleIds.Rumination,
            CompanionEmotion.Anger => ArticleIds.Anger,
            CompanionEmotion.Resentment => ArticleIds.Conflicts,
            CompanionEmotion.Guilt => ArticleIds.ShameAndGuilt,
            CompanionEmotion.Sadness => ArticleIds.LowMood,
            CompanionEmotion.Exhaustion => ArticleIds.Burnout,
            CompanionEmotion.Loneliness => ArticleIds.Loneliness,
            CompanionEmotion.Procrastination => ArticleIds.Procrastination,
            _ => null
        };

    public static ChatQuickReply ArticleChip(Article article, bool english) =>
        new(ChatQuickReplyKinds.Act, english ? $"Read: {article.Title}" : $"Почитать: {article.Title}", "article:" + article.Id);

    public static string ArticleTransition(bool english) => english ? "Opening the article." : "Открываю статью.";
}
