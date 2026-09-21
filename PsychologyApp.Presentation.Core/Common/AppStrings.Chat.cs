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
    public static string ChatError => T("Не удалось отправить сообщение. Попробуйте ещё раз.", "Could not send the message. Please try again.");
}
