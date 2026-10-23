using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Application.Practice;
using PsychologyApp.Domain.Practice;
using Xunit;

namespace PsychologyApp.Application.Tests.Practice;

public class TechniqueCatalogTests
{
    private static ITechniqueCatalogService CreateCatalog() =>
        new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => "ru"));

    [Theory]
    [InlineData(TechniqueId.Spin, TechniqueUiKind.Entry, "Крутилка")]
    [InlineData(TechniqueId.Polarity, TechniqueUiKind.Polarity, "Полярности")]
    [InlineData(TechniqueId.Paper, TechniqueUiKind.Paper, "Лист бумаги")]
    [InlineData(TechniqueId.Hack, TechniqueUiKind.Entry, "Белое пятно")]
    [InlineData(TechniqueId.Copied, TechniqueUiKind.Copied, "Повтори это")]
    [InlineData(TechniqueId.Observer, TechniqueUiKind.Entry, "Позиция наблюдателя")]
    [InlineData(TechniqueId.Anchor, TechniqueUiKind.Entry, "Якорь ресурса")]
    [InlineData(TechniqueId.Grounding, TechniqueUiKind.Entry, "Заземление 5-4-3-2-1")]
    [InlineData(TechniqueId.Breathing, TechniqueUiKind.Entry, "Квадратное дыхание")]
    [InlineData(TechniqueId.SmallStep, TechniqueUiKind.Entry, "Один маленький шаг")]
    [InlineData(TechniqueId.ThoughtRecord, TechniqueUiKind.Entry, "Запись мысли")]
    [InlineData(TechniqueId.SelfCompassion, TechniqueUiKind.Entry, "Добрые слова себе")]
    public async Task GetAsync_returns_expected_ui_kind_and_page_name(TechniqueId id, TechniqueUiKind kind, string pageName)
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(id);

        Assert.Equal(kind, definition.UiKind);
        Assert.Equal(pageName, definition.PageName);
        Assert.Equal("Практик", definition.ModuleName);
    }

    [Fact]
    public async Task GetAllAsync_contains_eighteen_builtin_techniques()
    {
        IReadOnlyList<BuiltInTechniqueDefinition> all = await CreateCatalog().GetAllAsync();
        Assert.Equal(21, all.Count);
    }

    [Fact]
    public async Task GetBuiltInListEntriesAsync_aligns_with_catalog()
    {
        ITechniqueCatalogService catalog = CreateCatalog();
        IReadOnlyList<BuiltInTechniqueDefinition> all = await catalog.GetAllAsync();
        IReadOnlyList<TechniqueListEntry> entries = await catalog.GetBuiltInListEntriesAsync();

        Assert.Equal(all.Count, entries.Count);

        foreach (TechniqueListEntry entry in entries)
        {
            BuiltInTechniqueDefinition definition = await catalog.GetAsync(entry.TechniqueId);
            Assert.Equal(definition.ListTitle, entry.Title);
            Assert.Equal(definition.ListSubtitle, entry.Subtitle);
            Assert.Equal(definition.ListNumber, entry.Number);
            Assert.False(string.IsNullOrWhiteSpace(definition.ListIcon));
            Assert.True(definition.ListDurationMinutes > 0);
            Assert.Equal(definition.ListIcon, entry.Icon);
            Assert.Equal(definition.ListDurationMinutes, entry.DurationMinutes);
        }
    }

    [Fact]
    public async Task All_builtin_techniques_have_theory_sections()
    {
        foreach (BuiltInTechniqueDefinition definition in await CreateCatalog().GetAllAsync())
        {
            Assert.NotNull(definition.TheorySections);
            Assert.Equal(4, definition.TheorySections!.Count);
        }
    }

    [Theory]
    [InlineData(TechniqueId.Spin, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Future, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Experience, EntryFieldKind.RatingNeg10To10)]
    [InlineData(TechniqueId.Observer, EntryFieldKind.Rating0To10)]
    [InlineData(TechniqueId.Anchor, EntryFieldKind.Rating0To10)]
    public async Task Entry_techniques_include_rating_fields(TechniqueId id, EntryFieldKind expectedKind)
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(id);
        Assert.Contains(definition.Entries!, entry => entry.Kind == expectedKind);
    }

    [Fact]
    public async Task Comparison_includes_reflection_field()
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(TechniqueId.Comparison);
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("изменилось", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ThoughtRecord_includes_cbt_fields()
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(TechniqueId.ThoughtRecord);

        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("Ситуация", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("Автоматическая мысль", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(definition.Entries!, entry => entry.Title.Contains("Альтернативная мысль", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Breathing_includes_before_after_ratings()
    {
        BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(TechniqueId.Breathing);

        Assert.Equal(2, definition.Entries!.Count(entry => entry.Kind == EntryFieldKind.Rating0To10));
    }

    [Theory]
    [InlineData("ru")]
    [InlineData("fr")]
    [InlineData("de")]
    [InlineData("es")]
    [InlineData("pt")]
    [InlineData("zh")]
    public async Task Every_language_offers_the_same_fields_and_help_as_english(string language)
    {
        ITechniqueCatalogService translated = new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => language));
        ITechniqueCatalogService english = new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => "en"));

        foreach (TechniqueId id in Enum.GetValues<TechniqueId>())
        {
            BuiltInTechniqueDefinition other = await translated.GetAsync(id);
            BuiltInTechniqueDefinition en = await english.GetAsync(id);
            IReadOnlyList<TechniqueEntrySeed> otherFields = other.Entries ?? [];
            IReadOnlyList<TechniqueEntrySeed> enFields = en.Entries ?? [];

            Assert.Equal(en.UiKind, other.UiKind);
            Assert.Equal(enFields.Count, otherFields.Count);
            for (int i = 0; i < enFields.Count; i++)
            {
                Assert.True(enFields[i].Role == otherFields[i].Role, $"{language} {id} field {i}: role differs");
                Assert.True(enFields[i].Kind == otherFields[i].Kind, $"{language} {id} field {i}: kind differs");
                Assert.True((enFields[i].Suggestions?.Count ?? 0) == (otherFields[i].Suggestions?.Count ?? 0), $"{language} {id} field {i}: suggestion count differs");
            }
        }
    }

    [Theory]
    [InlineData("fr", TechniqueId.Grounding, "Ancrage 5-4-3-2-1")]
    [InlineData("de", TechniqueId.Breathing, "Box-Atmung")]
    [InlineData("zh", TechniqueId.SmallStep, "一小步")]
    [InlineData("ja", TechniqueId.SmallStep, "One small step")]
    public async Task A_language_shows_its_own_practice_names_and_an_unknown_one_gets_english(string language, TechniqueId id, string pageName)
    {
        ITechniqueCatalogService catalog = new TechniqueCatalogService(new BuiltInTechniqueCatalogProvider(() => language));

        Assert.Equal(pageName, (await catalog.GetAsync(id)).PageName);
    }

    [Fact]
    public async Task Every_text_field_offers_some_help_besides_the_keyboard()
    {
        ITechniqueCatalogService catalog = CreateCatalog();
        string[] free = ["Негативный опыт"];

        foreach (BuiltInTechniqueDefinition definition in await catalog.GetAllAsync())
        {
            foreach (TechniqueEntrySeed entry in (definition.Entries ?? []).Where(e => e.Kind == EntryFieldKind.Text && !free.Contains(e.Title)))
            {
                bool helped = entry.Suggestions is { Count: > 0 } || entry.Role is not EntryFieldRole.None;
                Assert.True(helped, $"{definition.PageName}: \"{entry.Title}\" has neither chips nor a role");
            }
        }
    }

    [Fact]
    public async Task Problem_fields_come_first_so_stages_and_dialogues_can_find_the_problem()
    {
        foreach (TechniqueId id in new[] { TechniqueId.Resize, TechniqueId.Hack, TechniqueId.Check, TechniqueId.Future, TechniqueId.Spin, TechniqueId.Extend })
        {
            BuiltInTechniqueDefinition definition = await CreateCatalog().GetAsync(id);
            Assert.Equal(EntryFieldRole.Problem, definition.Entries![0].Role);
        }
    }
}
