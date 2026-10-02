using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

public class LocalModelCatalogTests
{
    [Fact]
    public void EveryFile_IsVerifiedByHash()
    {
        foreach (LocalModelFile file in LocalModelCatalog.Default.Files)
        {
            Assert.False(string.IsNullOrWhiteSpace(file.Sha256), $"{file.Path} has no SHA-256, so it would be installed unverified.");
            Assert.Equal(64, file.Sha256!.Length);
        }
    }

    [Fact]
    public void EveryUrl_IsHttpsAndPinnedToACommit()
    {
        foreach (LocalModelFile file in LocalModelCatalog.Default.Files)
        {
            Assert.StartsWith("https://", file.Url);
            Assert.DoesNotContain("/resolve/main/", file.Url);
        }
    }
}
