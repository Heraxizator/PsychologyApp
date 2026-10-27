using PsychologyApp.Application.Conversation.Companion;
using PsychologyApp.Presentation.Features.RunTechniqueSession;
using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class ChatCardLookTests
{
    // A state the chat can recognise but the list does not know would fall back to a bare letter.
    [Fact]
    public void Every_recognised_state_has_an_icon_and_a_colour()
    {
        CompanionEmotion[] missing = [.. Enum.GetValues<CompanionEmotion>()
            .Where(emotion => emotion != CompanionEmotion.Unknown)
            .Where(emotion => ChatCardLook.For(emotion.ToString()) is var look && (look.Icon is null || look.Flavor == TechniqueFlavor.None))];

        Assert.Empty(missing);
    }

    // The same state reads the same across the app: anxiety in a chat and in the anxiety tests, low mood in a chat and in Beck.
    [Theory]
    [InlineData("Anxiety", "gad7")]
    [InlineData("Sadness", "beck")]
    public void A_chat_looks_like_the_tests_of_its_state(string emotion, string testId)
    {
        (string testIcon, TechniqueFlavor testFlavor) = TestCardLook.For(testId);
        (string? chatIcon, TechniqueFlavor chatFlavor) = ChatCardLook.For(emotion);

        Assert.Equal(testIcon, chatIcon);
        Assert.Equal(testFlavor, chatFlavor);
    }

    [Fact]
    public void A_chat_with_no_state_yet_keeps_its_letter() =>
        Assert.Null(ChatCardLook.For(null).Icon);
}
