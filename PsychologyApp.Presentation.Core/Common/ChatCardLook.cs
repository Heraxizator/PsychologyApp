using PsychologyApp.Presentation.Features.RunTechniqueSession;

namespace PsychologyApp.Presentation.Shared.Common;

/// <summary>
/// The icon and tile colours of a chat in the list, by the state the chat recognised, in the same icons and colour groups as the
/// tests (<see cref="TestCardLook"/>): anxiety looks the same in a chat and in the anxiety tests. A chat whose state is not known
/// yet has no icon and keeps the first letter of its title.
/// </summary>
public static class ChatCardLook
{
    /// <param name="emotion">The name of the recognised state (a <c>CompanionEmotion</c>), or null.</param>
    public static (string? Icon, TechniqueFlavor Flavor) For(string? emotion) => emotion switch
    {
        "Panic" or "Anxiety" => ("Air", TechniqueFlavor.Mind),
        "Overthinking" => ("Loop", TechniqueFlavor.Mind),
        "Sadness" or "Guilt" => ("Cloud", TechniqueFlavor.Heart),
        "Loneliness" => ("FavoriteBorder", TechniqueFlavor.Heart),
        "Exhaustion" => ("Snooze", TechniqueFlavor.Body),
        "Anger" or "Resentment" => ("Whatshot", TechniqueFlavor.Action),
        "Procrastination" => ("TaskAlt", TechniqueFlavor.Action),
        _ => (null, TechniqueFlavor.None)
    };
}
