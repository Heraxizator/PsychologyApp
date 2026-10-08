using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>The main chat and the dialogue inside a practice must look like one chat: they share the bottom bar, the typing dots and the backdrop.</summary>
public class ChatConsistencyTests
{
    private static string Read(params string[] path) => File.ReadAllText(Path.GetFullPath(Path.Combine(
        [AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation", .. path])));

    [Theory]
    [InlineData("Pages", "Chat", "Conversation", "ChatPage.xaml")]
    [InlineData("Widgets", "DialogueChat", "DialogueChatView.xaml")]
    public void BothChatsUseTheSharedInputBarAndNoEditorOfTheirOwn(params string[] path)
    {
        string xaml = Read(path);

        Assert.Contains("<ui:ChatInputBar", xaml);
        Assert.DoesNotContain("<Editor", xaml);
    }

    [Theory]
    [InlineData("Pages", "Chat", "Conversation", "ChatPage.xaml")]
    [InlineData("Widgets", "DialogueChat", "DialogueChatView.xaml")]
    public void BothChatsShowTheSameTypingDots(params string[] path) => Assert.Contains("<ui:TypingBubbleView", Read(path));

    [Theory]
    [InlineData("Pages", "Chat", "Conversation", "ChatPage.xaml")]
    [InlineData("Pages", "RunTechniqueSession", "TechniqueDialogue", "TechniqueDialoguePage.xaml")]
    public void BothChatPagesHaveTheSameBackdropAndResizeForTheKeyboard(params string[] path)
    {
        Assert.Contains("ChatBackdropLight", Read(path));
        Assert.Contains("ChatBackdropDark", Read(path));
        Assert.Contains("KeyboardResize", Read([.. path[..^1], path[^1] + ".cs"]));
    }

    [Fact]
    public void TheirBubblesHaveTheSameShapeAndColours()
    {
        string main = Read("Pages", "Chat", "Conversation", "ChatPage.xaml");
        string dialogue = Read("Widgets", "DialogueChat", "DialogueChatView.xaml");

        foreach (string shared in new[] { "RoundRectangle 18,18,4,18", "RoundRectangle 18,18,18,4", "PrimaryFill", "OnPrimary" })
        {
            Assert.Contains(shared, main);
            Assert.Contains(shared, dialogue);
        }
    }

    [Fact]
    public void TheDialogueAsksForTensionWithTheSameSliderAsTheMainChat()
    {
        Assert.Contains("TensionSliderView", Read("Pages", "Chat", "Conversation", "ChatPage.xaml"));
        Assert.Contains("TensionSliderView", Read("Widgets", "DialogueChat", "DialogueChatView.xaml"));
    }
}
