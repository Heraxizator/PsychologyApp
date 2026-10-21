using PsychologyApp.Presentation.Shared.Common;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

[Collection("Localization")]
public sealed class QuoteShareFormatterTests
{
    [Fact]
    public void Format_IncludesQuoteAuthorAndFooter()
    {
        string formatted = QuoteShareFormatter.Format("Calm mind", "Seneca");

        Assert.Contains("Calm mind", formatted);
        Assert.Contains("Seneca", formatted);
        Assert.Contains(AppStrings.QuoteShareFooter, formatted);
    }
}
