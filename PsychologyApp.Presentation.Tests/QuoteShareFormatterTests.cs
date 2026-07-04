using PsychologyApp.Application.Models;
using PsychologyApp.Presentation.Shared.Common.Formatting;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

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
