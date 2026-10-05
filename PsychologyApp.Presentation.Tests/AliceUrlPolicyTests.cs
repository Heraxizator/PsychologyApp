using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public class AliceUrlPolicyTests
{
    [Theory]
    [InlineData("https://alice.yandex.ru/")]
    [InlineData("https://passport.yandex.ru/auth?retpath=x")]
    [InlineData("https://yandex.ru/")]
    [InlineData("https://id.yandex.com/")]
    [InlineData("https://ya.ru/")]
    public void The_yandex_family_over_https_is_allowed(string url) => Assert.True(AliceUrlPolicy.IsAllowed(url));

    [Theory]
    [InlineData("http://alice.yandex.ru/")]
    [InlineData("https://evil.com/")]
    [InlineData("https://yandex.ru.evil.com/")]
    [InlineData("https://notyandex.ru/")]
    [InlineData("https://evil.com/?u=https://alice.yandex.ru/")]
    [InlineData("javascript:alert(1)")]
    [InlineData("file:///sdcard/x.html")]
    [InlineData("")]
    [InlineData(null)]
    public void Everything_else_is_cancelled(string? url) => Assert.False(AliceUrlPolicy.IsAllowed(url));
}
