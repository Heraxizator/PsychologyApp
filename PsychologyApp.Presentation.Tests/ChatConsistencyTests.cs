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

/// <summary>Mistakes that only show when a screen is first drawn on the phone, caught here instead.</summary>
public class XamlRuntimeSafetyTests
{
    /// <summary>
    /// The icon markup (<c>{mi:Material Icon=...}</c>) throws when it is the value of a Setter ("does not support Style Setter in conjunction with Xaml Extension"),
    /// which crashed the practice page the first time a list row was drawn. Use two labels and switch their visibility instead.
    /// </summary>
    [Fact]
    public void NoSetterUsesTheIconMarkup()
    {
        string root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PsychologyApp.Presentation"));
        string[] offenders =
        [
            .. Directory.EnumerateFiles(root, "*.xaml", SearchOption.AllDirectories)
                .Where(path => !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}") && !path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                .Where(path => System.Text.RegularExpressions.Regex.IsMatch(File.ReadAllText(path), @"<Setter\b[^>]*\{mi:"))
                .Select(path => Path.GetRelativePath(root, path))
        ];

        Assert.Empty(offenders);
    }
}
