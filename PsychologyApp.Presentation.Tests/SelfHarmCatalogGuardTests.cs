using System.Text.Json;
using Xunit;

namespace PsychologyApp.Presentation.Tests;

/// <summary>
/// The self-harm screening finds the suicide question by its position in the published questionnaires. If the catalogue is ever
/// reordered or extended, this test fails instead of the safety check silently watching the wrong question.
/// </summary>
public sealed class SelfHarmCatalogGuardTests
{
    private static string RawRoot()
    {
        string? directory = AppContext.BaseDirectory;
        while (directory is not null)
        {
            string candidate = Path.Combine(directory, "PsychologyApp.Presentation", "Resources", "Raw", "tests");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Path.GetDirectoryName(directory);
        }

        throw new DirectoryNotFoundException("The questionnaire assets were not found above " + AppContext.BaseDirectory);
    }

    private static JsonElement Load(string file)
    {
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(RawRoot(), file)));
        return doc.RootElement.Clone();
    }

    private static string AllText(JsonElement element) => element.GetRawText();

    [Theory]
    [InlineData("beck.json", "покончить")]
    [InlineData("beck.en.json", "kill myself")]
    [InlineData("beck.fr.json", "me tuer")]
    [InlineData("beck.de.json", "das leben zu nehmen")]
    [InlineData("beck.es.json", "quitarme la vida")]
    [InlineData("beck.pt.json", "me matar")]
    [InlineData("beck.zh.json", "自杀")]
    public void Beck_item_9_is_the_suicide_question(string file, string marker)
    {
        JsonElement test = Load(file);
        JsonElement questions = test.GetProperty("questions");

        Assert.Equal(21, questions.GetArrayLength());
        string item9 = AllText(questions[8]).ToLowerInvariant();
        Assert.True(item9.Contains(marker), "Item 9 of the Beck inventory is no longer the question about suicide.");
    }

    [Theory]
    [InlineData("questionnaires.json", "умереть")]
    [InlineData("questionnaires.en.json", "dead")]
    [InlineData("questionnaires.fr.json", "mourir")]
    [InlineData("questionnaires.de.json", "tot")]
    [InlineData("questionnaires.es.json", "muerto")]
    [InlineData("questionnaires.pt.json", "morto")]
    [InlineData("questionnaires.zh.json", "死")]
    public void Phq9_item_9_is_the_thoughts_of_death_or_self_harm_question(string file, string marker)
    {
        JsonElement catalogue = Load(file);
        JsonElement phq9 = catalogue.EnumerateArray().First(t => t.GetProperty("analyzerId").GetString() == "phq9");
        JsonElement questions = phq9.GetProperty("questions");

        Assert.Equal(9, questions.GetArrayLength());
        string item9 = AllText(questions[8]).ToLowerInvariant();
        Assert.True(item9.Contains(marker), "Item 9 of the PHQ-9 is no longer the question about thoughts of death or self-harm.");
    }
}
