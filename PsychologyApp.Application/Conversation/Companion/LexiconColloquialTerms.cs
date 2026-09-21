namespace PsychologyApp.Application.Conversation.Companion;

/// <summary>
/// Colloquial and figurative phrasings ("как белка в колесе", "выжатый лимон") that the base lexicon does not cover.
/// Kept apart so the core word lists stay readable. Same term syntax as the base lexicon.
/// </summary>
public sealed partial class LexiconSituationAnalyzer
{
    private static readonly Dictionary<CompanionEmotion, Entry[]> Colloquial = new()
    {
        [CompanionEmotion.Panic] =
        [
            new(2, ["сдавило", "обморок", "потеряю контроль", "случится что-то ужасное", "сердечный приступ", "feels unreal", "pass out"]),
            new(1, ["дрожу", "воздуха не хватает"])
        ],
        [CompanionEmotion.Anxiety] =
        [
            new(2, ["жду беды", "ждешь беды", "неспокойн", "беспокойно", "не могу расслабиться", "всё кажется опасным", "все кажется опасным"]),
            new(1, ["напряжени", "вдруг что-то случится", "вдруг случится", "опасн", "места себе не нахожу", "на грани"])
        ],
        [CompanionEmotion.Overthinking] =
        [
            new(2, ["по кругу", "думаю, думаю", "думаю думаю", "белка в колесе", "выкинуть из головы", "выбросить из головы", "из головы не", "снова и снова", "вертится в голове", "крутится в голове",
                    "перебираю в уме", "проигрываю в голове", "анализирую", "go in circles", "switch them off", "cant switch off", "on repeat"])
        ],
        [CompanionEmotion.Anger] =
        [
            new(2, ["кипит", "в ярости", "выводит из себя", "выводят из себя", "drives me mad", "makes me mad", "fed up"]),
            new(1, ["раздражает", "перебивают", "придирк", "дурацк", "заставили"])
        ],
        [CompanionEmotion.Resentment] =
        [
            new(2, ["преданн", "предан ", "не поддержал", "не поддержала", "унизил", "обесценил", "hurt that", "it is unfair", "its unfair"]),
            new(1, ["мне больно", "не могу этого забыть", "не существует", "не вспомнил", "не вспомнили"])
        ],
        [CompanionEmotion.Guilt] =
        [
            new(2, ["простить себе", "из-за меня", "мучает совесть", "совесть", "плохая мать", "плохой отец", "плохой родитель", "сорвалась на", "сорвался на", "feel terrible for", "my fault", "let down"]),
            new(1, ["стыдно смотреть", "жалею, что", "подвел", "подвела"])
        ],
        [CompanionEmotion.Sadness] =
        [
            new(2, ["потеряла краски", "потерял краски", "жизнь потеряла", "серо и", "тоскую", "empty and sad", "nothing makes me happy", "cry every night"]),
            new(1, ["плачу", "хочется плакать", "любимые занятия"])
        ],
        [CompanionEmotion.Exhaustion] =
        [
            new(2, ["выжатый лимон", "выжат", "валится из рук", "без выходных", "вымотан", "не соображает", "haven't slept", "havent slept", "no energy", "drained"]),
            new(1, ["не высыпаюсь", "лежу и смотрю в потолок", "нет сил", "сил нет"])
        ],
        [CompanionEmotion.Loneliness] =
        [
            new(2, ["совсем одна", "совсем один", "одна в этом", "один в этом", "ни с кем не общаюсь", "чужой среди", "чужая среди", "ни души", "изолирован", "поделиться не с кем", "поговорить не с кем", "alone in this", "have no one"])
        ],
        [CompanionEmotion.Procrastination] =
        [
            new(2, ["ленюсь", "не приступлю", "вместо работы", "не могу начать", "putting off", "make myself start", "cant make myself"]),
            new(1, ["дела копятся", "отвлекает", "сижу в телефоне", "не получается начать"])
        ]
    };
}
