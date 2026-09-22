namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string ChatsTitle => T("Чаты", "Chats");
    public static string ChatNew => T("Новый чат", "New chat");
    public static string ChatInputPlaceholder => T("Напишите сообщение…", "Write a message…");
    public static string ChatEmptyTitle => T("Пока нет разговоров", "No conversations yet");

    public static string ChatEmptyBody => T(
        "Расскажите, что происходит. Я выслушаю, помогу разобраться и подскажу практику.",
        "Tell me what is going on. I will listen, help you sort it out and suggest a practice.");

    public static string ChatRename => T("Переименовать", "Rename");
    public static string ChatDelete => T("Удалить", "Delete");
    public static string ChatRenameTitle => T("Название чата", "Chat name");
    public static string ChatRenameMessage => T("Как назвать этот разговор?", "What should this conversation be called?");
    public static string ChatRenameAccept => T("Сохранить", "Save");
    public static string ChatCancel => T("Отмена", "Cancel");
    public static string ChatDeleteTitle => T("Удалить чат?", "Delete this chat?");

    public static string ChatDeleteBody => T(
        "Разговор и его история будут удалены с устройства без возможности восстановления.",
        "The conversation and its history will be deleted from this device and cannot be restored.");

    public static string ChatHeroTitle => T("Поговорить", "Talk");
    public static string ChatHeroFreshSubtitle => T("Расскажите, что происходит, и я помогу разобраться", "Tell me what is going on and I will help you sort it out");
    public static string ChatHeroContinue => T("Продолжить", "Continue");
    public static string ChatHeroStart => T("Начать разговор", "Start talking");
    public static string ChatHeroAllChats => T("Все чаты", "All chats");
    public static string ChatHeroNewChat => T("Новый чат", "New chat");
    public static string ChatStatusIdle => T("Собеседник · всё остаётся на вашем телефоне", "Companion · everything stays on your phone");
    public static string ChatError => T("Не удалось отправить сообщение. Попробуйте ещё раз.", "Could not send the message. Please try again.");
    public static string ChatLoadingText => T("Открываем чат…", "Opening the chat…");
    public static string ChatScrollToNewest => T("К последним сообщениям", "Jump to newest messages");
    public static string ChatScrollToOldest => T("К началу разговора", "Jump to the start of the conversation");

    // ----- companion profile -----
    public static string ChatProfileTitle => T("Собеседник", "Companion");
    public static string ChatProfileOpen => T("Открыть профиль собеседника", "Open the companion's profile");
    public static string ChatProfileOnline => T("На связи", "Online");
    public static string ChatProfileOffline => T("Работает офлайн", "Works offline");
    public static string ChatProfileStatChats => T("Разговоров", "Chats");
    public static string ChatProfileStatMessages => T("Ваших сообщений", "Your messages");
    public static string ChatProfileStatDays => T("Дней вместе", "Days together");
    public static string ChatProfileStatStreak => T("Дней подряд", "Day streak");
    public static string ChatProfileTrustTitle => T("Уровень доверия", "Level of trust");
    public static string ChatProfileNameTitle => T("Как к вам обращаться", "What to call you");
    public static string ChatProfileNameEmpty => T("Имя не указано", "No name yet");
    public static string ChatProfileNameHint => T("Собеседник изредка будет обращаться к вам по имени", "The companion will occasionally address you by name");
    public static string ChatProfileNameEdit => T("Изменить", "Change");
    public static string ChatProfileNamePromptTitle => T("Ваше имя", "Your name");
    public static string ChatProfileNamePromptMessage => T("Как к вам обращаться? Оставьте пустым, чтобы убрать имя.", "What should I call you? Leave empty to remove the name.");
    public static string ChatProfileInsightsTitle => T("Что видно из ваших разговоров", "What your conversations show");
    public static string ChatProfileTensionTitle => T("Динамика напряжения", "Tension over time");
    public static string ChatProfileTensionStart => T("В начале", "At the start");
    public static string ChatProfileTensionEnd => T("В конце", "At the end");
    public static string ChatProfilePracticesTitle => T("Что вам помогает", "What helps you");
    public static string ChatProfilePracticesEmpty => T("Пройдите практику из чата и оцените напряжение до и после, тогда здесь появится то, что вам помогает.", "Do a practice from a chat and rate the tension before and after, and what helps you will appear here.");
    public static string ChatProfilePracticeBest => T("Лучшее", "Best");
    public static string ChatProfilePracticeStart => T("Начать", "Start");
    public static string ChatProfilePracticeDetail(int helped, int tried) => T($"Помогла {helped} из {tried}", $"Helped {helped} of {tried}");
    public static string ChatProfileEmotionsTitle => T("О чём вы говорите чаще всего", "What you talk about most");
    public static string ChatProfileAboutTitle => T("О собеседнике", "About the companion");
    public static string ChatProfileActionsTitle => T("Управление", "Manage");
    public static string ChatProfileAllChats => T("Все чаты", "All chats");
    public static string ChatProfileForget => T("Забыть моё имя и предпочтения", "Forget my name and preferences");
    public static string ChatProfileForgetTitle => T("Забыть о вас?", "Forget about you?");
    public static string ChatProfileForgetBody => T("Собеседник забудет имя и то, какие практики вам помогали. Чаты останутся.", "The companion will forget your name and which practices helped. Your chats will stay.");
    public static string ChatProfileDeleteAll => T("Удалить все чаты", "Delete all chats");
    public static string ChatProfileDeleteAllTitle => T("Удалить все чаты?", "Delete all chats?");
    public static string ChatProfileDeleteAllBody => T("Все разговоры будут удалены с устройства без возможности восстановления. Имя и предпочтения сохранятся.", "All conversations will be deleted from this device and cannot be restored. Your name and preferences will stay.");
    public static string ChatProfileConfirm => T("Да", "Yes");
    public static string ChatProfileLoadingText => T("Считаем вашу статистику…", "Working out your statistics…");
}
