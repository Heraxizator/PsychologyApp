namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string ChatsTitle => R(nameof(ChatsTitle));
    public static string ChatNew => R(nameof(ChatNew));
    public static string ChatInputPlaceholder => R(nameof(ChatInputPlaceholder));
    public static string ChatEmptyTitle => R(nameof(ChatEmptyTitle));

    public static string ChatEmptyBody => R(nameof(ChatEmptyBody));
    public static string ChatEmptyPrivacy => R(nameof(ChatEmptyPrivacy));
    public static string ChatOpenersTitle => R(nameof(ChatOpenersTitle));
    public static string ChatOpenerAnxious => R(nameof(ChatOpenerAnxious));
    public static string ChatOpenerSleep => R(nameof(ChatOpenerSleep));
    public static string ChatOpenerSad => R(nameof(ChatOpenerSad));
    public static string ChatOpenerLonely => R(nameof(ChatOpenerLonely));
    public static string ChatUnavailableTitle => R(nameof(ChatUnavailableTitle));
    public static string ChatUnavailableBody => R(nameof(ChatUnavailableBody));

    public static string ChatRename => R(nameof(ChatRename));
    public static string ChatDelete => R(nameof(ChatDelete));
    public static string ChatRenameTitle => R(nameof(ChatRenameTitle));
    public static string ChatRenameMessage => R(nameof(ChatRenameMessage));
    public static string ChatRenameAccept => R(nameof(ChatRenameAccept));
    public static string ChatCancel => R(nameof(ChatCancel));
    public static string ChatDeleteTitle => R(nameof(ChatDeleteTitle));

    public static string ChatDeleteBody => R(nameof(ChatDeleteBody));

    public static string ChatHeroTitle => R(nameof(ChatHeroTitle));
    public static string ChatHeroFreshSubtitle => R(nameof(ChatHeroFreshSubtitle));
    public static string ChatHeroContinue => R(nameof(ChatHeroContinue));
    public static string ChatHeroStart => R(nameof(ChatHeroStart));
    public static string ChatHeroAllChats => R(nameof(ChatHeroAllChats));
    public static string ChatHeroNewChat => R(nameof(ChatHeroNewChat));
    public static string ChatStatusIdle => R(nameof(ChatStatusIdle));
    public static string ChatStatusPractice => R(nameof(ChatStatusPractice));
    public static string ChatError => R(nameof(ChatError));
    public static string ChatLoadingText => R(nameof(ChatLoadingText));
    public static string ChatScrollToNewest => R(nameof(ChatScrollToNewest));
    public static string ChatScrollToOldest => R(nameof(ChatScrollToOldest));
    public static string ChatStressTestTitle => R(nameof(ChatStressTestTitle));

    public static string ChatReminderTitle => R(nameof(ChatReminderTitle));

    public static string ChatReminderBody => R(nameof(ChatReminderBody));

    // ----- companion profile -----
    public static string ChatProfileTitle => R(nameof(ChatProfileTitle));
    public static string ChatProfileOpen => R(nameof(ChatProfileOpen));
    public static string ChatProfileNameTitle => R(nameof(ChatProfileNameTitle));
    public static string ChatProfileNameEmpty => R(nameof(ChatProfileNameEmpty));
    public static string ChatProfileNameEdit => R(nameof(ChatProfileNameEdit));
    public static string ChatProfileNamePromptTitle => R(nameof(ChatProfileNamePromptTitle));
    public static string ChatProfileNamePromptMessage => R(nameof(ChatProfileNamePromptMessage));
    public static string ChatProfileTensionTitle => R(nameof(ChatProfileTensionTitle));
    public static string ChatProfileTensionStart => R(nameof(ChatProfileTensionStart));
    public static string ChatProfileTensionEnd => R(nameof(ChatProfileTensionEnd));
    public static string ChatProfilePracticesTitle => R(nameof(ChatProfilePracticesTitle));
    public static string ChatProfilePracticeBest => R(nameof(ChatProfilePracticeBest));
    public static string ChatProfilePracticeStart => R(nameof(ChatProfilePracticeStart));
    public static string ChatProfilePracticeDetail(int helped, int tried) => F(nameof(ChatProfilePracticeDetail), helped, tried);
    public static string ChatProfileAboutTitle => R(nameof(ChatProfileAboutTitle));
    public static string ChatProfileForget => R(nameof(ChatProfileForget));
    public static string ChatProfileForgetTitle => R(nameof(ChatProfileForgetTitle));
    public static string ChatProfileForgetBody => R(nameof(ChatProfileForgetBody));
    public static string ChatProfileDeleteAll => R(nameof(ChatProfileDeleteAll));
    public static string ChatProfileDeleteAllTitle => R(nameof(ChatProfileDeleteAllTitle));
    public static string ChatProfileDeleteAllBody => R(nameof(ChatProfileDeleteAllBody));
    public static string ChatProfileConfirm => R(nameof(ChatProfileConfirm));
    public static string ChatProfileLoadingText => R(nameof(ChatProfileLoadingText));
}
