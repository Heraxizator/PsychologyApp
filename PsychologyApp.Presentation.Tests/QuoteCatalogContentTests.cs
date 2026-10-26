using System.Text.Json;
using PsychologyApp.Application.Models.Quot;
using PsychologyApp.Application.Quot;
using PsychologyApp.Presentation.Serialization;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

public sealed class QuoteCatalogContentTests
{
    // A floor, not the exact size: the catalogue is curated, and quotes that are unsafe, fabricated or repeated are taken out.
    private const int MinimumCount = 500;

    [Theory]
    [InlineData("quotes/quotes.ru.json")]
    [InlineData("quotes/quotes.en.json")]
    public async Task Catalog_HasMinimumCount(string relativePath)
    {
        List<QuoteJsonEntry> entries = await LoadAsync(relativePath);
        Assert.True(entries.Count >= MinimumCount, $"{relativePath} has {entries.Count} entries; expected >= {MinimumCount}.");
    }

    [Fact]
    public async Task Catalog_RuAndEnHaveSameCount()
    {
        List<QuoteJsonEntry> ru = await LoadAsync("quotes/quotes.ru.json");
        List<QuoteJsonEntry> en = await LoadAsync("quotes/quotes.en.json");
        Assert.Equal(ru.Count, en.Count);
    }

    [Theory]
    [InlineData("quotes/quotes.ru.json")]
    [InlineData("quotes/quotes.en.json")]
    public async Task Catalog_HasNoDuplicateText(string relativePath)
    {
        List<QuoteJsonEntry> entries = await LoadAsync(relativePath);
        HashSet<string> texts = new(StringComparer.Ordinal);
        foreach (QuoteJsonEntry entry in entries)
        {
            Assert.False(string.IsNullOrWhiteSpace(entry.Text), $"{relativePath} contains an empty quote text.");
            Assert.True(texts.Add(entry.Text.Trim()), $"{relativePath} contains duplicate text: {entry.Text}");
        }
    }

    [Fact]
    public void QuoteCatalogPolicy_CurrentVersionIsAtLeastTwo()
    {
        Assert.True(QuoteCatalogPolicy.CurrentVersion >= 7);
    }

    [Fact]
    public async Task Catalog_ContainsByzantineAndRussianAuthors()
    {
        List<QuoteJsonEntry> en = await LoadAsync("quotes/quotes.en.json");
        List<QuoteJsonEntry> ru = await LoadAsync("quotes/quotes.ru.json");

        Assert.Contains(en, e => e.Author == "John Chrysostom");
        Assert.Contains(en, e => e.Author == "Alexander Pushkin");
        Assert.Contains(ru, e => e.Author == "Иоанн Золотоуст");
        Assert.Contains(ru, e => e.Author == "Александр Пушкин");
    }

    [Fact]
    public async Task Catalog_ContainsSovietAndGermanAuthors()
    {
        List<QuoteJsonEntry> en = await LoadAsync("quotes/quotes.en.json");
        List<QuoteJsonEntry> ru = await LoadAsync("quotes/quotes.ru.json");

        Assert.Contains(en, e => e.Author == "Vladimir Mayakovsky");
        Assert.Contains(en, e => e.Author == "Johann Wolfgang von Goethe");
        Assert.Contains(ru, e => e.Author == "Владимир Маяковский");
        Assert.Contains(ru, e => e.Author == "Иоганн Вольфганг фон Гёте");
    }

    [Fact]
    public async Task Catalog_ContainsWorldAuthors()
    {
        List<QuoteJsonEntry> en = await LoadAsync("quotes/quotes.en.json");
        List<QuoteJsonEntry> ru = await LoadAsync("quotes/quotes.ru.json");

        Assert.Contains(en, e => e.Author == "Albert Camus");
        Assert.Contains(en, e => e.Author == "Sun Tzu");
        Assert.Contains(ru, e => e.Author == "Альбер Камю");
        Assert.Contains(ru, e => e.Author == "Сунь-цзы");
    }

    // The quote filters offer these themes: one missing from a translation would open an empty feed in that language.
    private static readonly string[] FilterThemes =
        ["wisdom", "motivation", "resilience", "calm", "anxiety", "mindfulness", "hope", "self-awareness", "relationships", "habits"];

    [Theory]
    [InlineData("fr")]
    [InlineData("de")]
    [InlineData("es")]
    [InlineData("pt")]
    [InlineData("zh")]
    public async Task A_translated_catalog_fills_every_filter_without_repeats(string language)
    {
        List<QuoteJsonEntry> entries = await LoadAsync($"quotes/quotes.{language}.json");

        Assert.True(entries.Count >= 100, $"{language} has only {entries.Count} quotes.");
        Assert.Equal(entries.Count, entries.Select(entry => entry.Text.Trim()).Distinct(StringComparer.Ordinal).Count());
        Assert.All(entries, entry => Assert.False(string.IsNullOrWhiteSpace(entry.Author)));
        Assert.All(FilterThemes, theme => Assert.True(entries.Count(entry => entry.Theme == theme) >= 5, $"{language}: too few quotes on {theme}."));
    }

    // People in crisis read these: a quote about dying, killing or welcoming death can push in the wrong direction, so none is allowed.
    [Theory]
    [InlineData("ru", @"\b(смерт\w*|умер(еть|ла|ли|ший)?|умира\w*|мёртв\w*|мертв\w*|гроб\w*|убива\w*|убить|убил\w*|самоуби\w*)\b")]
    [InlineData("en", @"\b(death|die|dies|dying|dead|deadly|grave|suicide|kill|kills|killed|corpse)\b")]
    [InlineData("fr", @"\b(mort|morte|morts|mourir|meurt|suicide|tuer)\b")]
    [InlineData("de", @"\b(Tod|Todes|sterben|stirbt|tot|töten|Selbstmord|Suizid)\b")]
    [InlineData("es", @"\b(muerte|morir|muerto|muerta|suicidio|matar)\b")]
    [InlineData("pt", @"\b(morte|morrer|morto|morta|suicídio|matar)\b")]
    [InlineData("zh", "死|自杀|杀")]
    public async Task No_quote_speaks_of_death_or_killing(string language, string pattern)
    {
        List<QuoteJsonEntry> entries = await LoadAsync($"quotes/quotes.{language}.json");
        System.Text.RegularExpressions.Regex words = new(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        Assert.All(entries, entry => Assert.False(words.IsMatch(entry.Text), $"{language}: {entry.Text}"));
    }

    private static async Task<List<QuoteJsonEntry>> LoadAsync(string relativePath)
    {
        string assetPath = Path.Combine(GetRawAssetsRoot(), relativePath.Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(assetPath), $"Missing test asset: {assetPath}");

        await using FileStream stream = File.OpenRead(assetPath);
        List<QuoteJsonEntry>? entries = await JsonSerializer.DeserializeAsync(
            stream,
            AppJsonSerializerContext.Default.ListQuoteJsonEntry);

        return entries ?? throw new InvalidOperationException($"Could not deserialize {relativePath}.");
    }

    private static string GetRawAssetsRoot()
    {
        string? current = AppContext.BaseDirectory;
        while (current is not null)
        {
            string candidate = Path.Combine(current, "PsychologyApp.Presentation", "Resources", "Raw");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new InvalidOperationException("Could not locate PsychologyApp.Presentation/Resources/Raw.");
    }
}
