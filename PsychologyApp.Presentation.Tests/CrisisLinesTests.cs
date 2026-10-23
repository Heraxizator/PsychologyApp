using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>A person in danger dials what the crisis screen offers, so a wrong or missing number there is a safety defect.</summary>
[Collection("Localization")]
public sealed class CrisisLinesTests
{
    [Theory]
    [InlineData("FR", "112", "3114")]
    [InlineData("ES", "112", "024")]
    [InlineData("PT", "112", "808242424")]
    [InlineData("BR", "192", "188")]
    [InlineData("CN", "120", "12356")]
    [InlineData("US", "911", "988")]
    [InlineData("de", "112", "08001110111")]
    public void A_phone_in_a_country_with_a_listed_line_gets_its_emergency_number_and_line(string country, string emergency, string line)
    {
        CrisisLines.Lines lines = CrisisLines.For(country);

        Assert.Equal(emergency, lines.Emergency);
        Assert.Equal(line, lines.LineNumber);
        Assert.False(string.IsNullOrWhiteSpace(lines.LineName));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("JP")]
    public void Without_a_known_country_the_emergency_number_is_still_there(string? country)
    {
        CrisisLines.Lines lines = CrisisLines.For(country);

        Assert.Equal("112", lines.Emergency);
        Assert.False(lines.HasLine);
    }

    [Fact]
    public void The_Russian_line_is_named_in_the_interface_language()
    {
        string? previous = AppStrings.LanguageOverride;
        try
        {
            AppStrings.LanguageOverride = AppLanguages.Russian;
            Assert.Equal("Детский телефон доверия", CrisisLines.For("RU").LineName);
            AppStrings.LanguageOverride = AppLanguages.English;
            Assert.Equal("Children's helpline", CrisisLines.For("RU").LineName);
        }
        finally
        {
            AppStrings.LanguageOverride = previous;
        }
    }
}
