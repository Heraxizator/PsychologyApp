namespace PsychologyApp.Application.Conversation.Companion;

public sealed partial class LexiconSituationAnalyzer
{
    private static readonly Dictionary<CompanionPerson, string[]> PersonTerms = new()
    {
        [CompanionPerson.Boss] = ["начальник", "начальниц", "шеф", "руководител", "директор", "=босс", "=boss", "manager", "supervisor"],
        [CompanionPerson.Colleague] = ["коллег", "сотрудник", "colleague", "coworker"],
        [CompanionPerson.Partner] = ["=муж", "мужа", "мужу", "мужем", "=жена", "жену", "женой", "парень", "парня", "парнем", "девушк", "партнер", "любимый", "любимая", "husband", "=wife", "boyfriend", "girlfriend", "partner"],
        [CompanionPerson.Parent] = ["=мама", "мамы", "маму", "мамой", "маме", "=папа", "папы", "папу", "отец", "отца", "отцу", "=мать", "матери", "родител", "=mother", "=father", "parents", "=mom", "=dad"],
        [CompanionPerson.Child] = ["ребен", "ребён", "=сын", "сына", "сыну", "дочь", "дочк", "=дети", "детей", "=kid", "=kids", "=child", "=son", "daughter"],
        [CompanionPerson.Friend] = ["подруг", "=друг", "друга", "другу", "друзь", "=friend", "friends"],
        [CompanionPerson.Relative] = ["сестр", "=брат", "брата", "бабушк", "дедушк", "тетя", "тётя", "дядя", "=sister", "=brother", "grandmother", "grandfather"]
    };
}
