using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class MusicAudioCacheTests
{
    [Fact]
    public void SharedHttpClient_HasConfiguredTimeout()
    {
        Assert.Equal(TimeSpan.FromSeconds(30), MusicAudioCache.SharedHttpClient.Timeout);
    }
}
