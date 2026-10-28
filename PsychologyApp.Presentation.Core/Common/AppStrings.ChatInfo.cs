namespace PsychologyApp.Presentation.Common;

/// <summary>The page about one chat: what it held and what can be done with it.</summary>
public static partial class AppStrings
{
    public static string ChatInfoTitle => R(nameof(ChatInfoTitle));
    public static string ChatInfoStarted(string started, string updated) => F(nameof(ChatInfoStarted), started, updated);
    public static string ChatInfoOpen => R(nameof(ChatInfoOpen));
    public static string ChatInfoRename => R(nameof(ChatInfoRename));
    public static string ChatInfoDelete => R(nameof(ChatInfoDelete));
    public static string ChatInfoSection => R(nameof(ChatInfoSection));
    public static string ChatInfoFeeling => R(nameof(ChatInfoFeeling));
    public static string ChatInfoTension => R(nameof(ChatInfoTension));
    public static string ChatInfoPractice => R(nameof(ChatInfoPractice));
    public static string ChatInfoStep => R(nameof(ChatInfoStep));
    public static string ChatInfoEmpty => R(nameof(ChatInfoEmpty));
    public static string ChatInfoOnPhone => R(nameof(ChatInfoOnPhone));
    public static string ChatInfoAboutCompanion => R(nameof(ChatInfoAboutCompanion));

    /// <summary>The name of a recognised state (a <c>CompanionEmotion</c> name) in the interface language; null for an unknown one.</summary>
    public static string? ChatEmotion(string? emotion) => emotion switch
    {
        "Panic" or "Anxiety" or "Overthinking" or "Anger" or "Resentment" or "Guilt" or "Sadness" or "Exhaustion" or "Loneliness" or "Procrastination"
            => R("ChatEmotion" + emotion),
        _ => null
    };
}
