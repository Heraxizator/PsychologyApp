using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Models.Tests;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

public class ConversationSessionTests
{
    private static ConversationSession Start(string language, out ConversationTurn first, int seed = 1)
    {
        ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", language == "ru" ? "Observer.json" : $"Observer.{language}.json")));
        Assert.True(parsed.IsSuccess, parsed.Error);

        ConversationSession session = new(parsed.Value!, new KeywordCrisisDetector(), new Random(seed));
        first = session.Start();
        return session;
    }

    private static ConversationSession RunToRatingBefore(out ConversationTurn turn, int rating = 7)
    {
        ConversationSession session = Start("ru", out _);
        session.SubmitText("Начальник раскритиковал мой отчёт при всех");
        session.SubmitChoice(1); // Обида
        session.SubmitText("Сжатие в груди");
        turn = session.SubmitRating(rating);
        return session;
    }

    [Fact]
    public void Start_says_greeting_then_asks_for_situation()
    {
        Start("ru", out ConversationTurn first);

        Assert.Equal(ConversationStatus.WaitingForInput, first.Status);
        Assert.Equal(3, first.BotMessages.Count);
        Assert.Equal(ConversationInputKind.Text, first.Prompt!.Kind);
        Assert.False(string.IsNullOrWhiteSpace(first.Prompt.Hint));
    }

    [Fact]
    public void Full_run_echoes_user_words_and_reports_improvement()
    {
        ConversationSession session = RunToRatingBefore(out ConversationTurn turn);

        Assert.Equal("обида", session.Captured["feeling"]);
        Assert.Equal(ConversationInputKind.Text, turn.Prompt!.Kind); // observer description

        turn = session.SubmitText("Двое разговаривают на повышенных тонах");
        Assert.Contains(turn.BotMessages, m => m.Contains("Начальник раскритиковал мой отчёт при всех") && m.Contains("повышенных тонах"));

        session.SubmitText("Ты справляешься лучше, чем думаешь");
        turn = session.SubmitRating(4);

        Assert.Equal(ConversationStatus.Completed, turn.Status);
        Assert.Null(turn.Prompt);
        Assert.Contains("Было 7, стало 4", turn.BotMessages[^1]);
        Assert.Equal(7, session.GetRating("rating_before"));
        Assert.Equal(4, session.GetRating("rating_after"));
    }

    [Theory]
    [InlineData(7, "Напряжение осталось прежним")]
    [InlineData(9, "Стало сильнее")]
    public void Final_message_depends_on_rating_change(int after, string expectedFragment)
    {
        ConversationSession session = RunToRatingBefore(out _);
        session.SubmitText("Со стороны это разговор");
        session.SubmitText("Всё пройдёт");

        ConversationTurn turn = session.SubmitRating(after);

        Assert.Equal(ConversationStatus.Completed, turn.Status);
        Assert.Contains(expectedFragment, turn.BotMessages[^1]);
    }

    [Fact]
    public void Low_rating_skips_breathing_pause_and_high_rating_adds_it()
    {
        ConversationSession low = RunToRatingBefore(out ConversationTurn lowTurn, rating: 1);
        Assert.Contains(lowTurn.BotMessages, m => m.Contains("небольшое"));
        Assert.Equal(ConversationInputKind.Text, lowTurn.Prompt!.Kind);

        ConversationSession high = RunToRatingBefore(out ConversationTurn highTurn, rating: 10);
        Assert.Contains(highTurn.BotMessages, m => m.Contains("выдох"));
        Assert.Equal(ConversationInputKind.Choice, highTurn.Prompt!.Kind);

        ConversationTurn paused = high.SubmitChoice(1);
        Assert.Equal(ConversationStatus.Paused, paused.Status);
        Assert.Null(low.GetRating("rating_after"));
    }

    [Theory]
    [InlineData("Иногда я думаю, что не хочу жить")]
    [InlineData("хочу СУИЦИД")]
    [InlineData("I want to die")]
    public void Crisis_text_interrupts_scenario_and_shows_help(string text)
    {
        ConversationSession session = Start("ru", out _);

        ConversationTurn turn = session.SubmitText(text);

        Assert.Equal(ConversationStatus.Interrupted, turn.Status);
        Assert.Null(turn.Prompt);
        Assert.Contains(turn.BotMessages, m => m.Contains("112"));
        Assert.DoesNotContain("situation", session.Captured.Keys);
    }

    [Fact]
    public void Blank_text_reprompts_without_advancing()
    {
        ConversationSession session = Start("ru", out _);

        ConversationTurn turn = session.SubmitText("   ");

        Assert.Empty(turn.BotMessages);
        Assert.Equal(ConversationInputKind.Text, turn.Prompt!.Kind);
        Assert.Empty(session.Captured);
    }

    [Fact]
    public void Invalid_choice_and_rating_reprompt()
    {
        ConversationSession session = Start("ru", out _);
        session.SubmitText("Ситуация");

        Assert.Empty(session.SubmitChoice(99).BotMessages);
        Assert.Equal(ConversationInputKind.Choice, session.SubmitChoice(-1).Prompt!.Kind);

        session.SubmitChoice(0);
        session.SubmitText("В груди");
        ConversationTurn turn = session.SubmitRating(11);
        Assert.Empty(turn.BotMessages);
        Assert.Equal(ConversationInputKind.Rating, turn.Prompt!.Kind);
    }

    [Fact]
    public void Wrong_input_kind_throws()
    {
        ConversationSession session = Start("ru", out _);

        Assert.Throws<InvalidOperationException>(() => session.SubmitRating(5));
    }

    [Fact]
    public void Other_feeling_asks_free_text_and_uses_it_in_template()
    {
        ConversationSession session = Start("ru", out _);
        session.SubmitText("Ситуация");

        ConversationTurn turn = session.SubmitChoice(5); // Другое
        Assert.Equal(ConversationInputKind.Text, turn.Prompt!.Kind);

        turn = session.SubmitText("растерянность");
        Assert.Contains(turn.BotMessages, m => m.Contains("«растерянность»"));
    }

    [Fact]
    public void Same_seed_gives_same_phrasing_and_variants_differ_across_seeds()
    {
        HashSet<string> greetings = [];
        for (int seed = 0; seed < 20; seed++)
        {
            Start("ru", out ConversationTurn a, seed);
            Start("ru", out ConversationTurn b, seed);
            Assert.Equal(a.BotMessages, b.BotMessages);
            greetings.Add(a.BotMessages[0]);
        }

        Assert.True(greetings.Count > 1);
    }

    [Theory]
    [InlineData("Grounding.json", "Было 8, стало 3")]
    [InlineData("Grounding.en.json", "It was 8, now 3")]
    public void Grounding_walks_all_five_senses_and_compares_ratings(string fileName, string expectedFinal)
    {
        ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", fileName)));
        Assert.True(parsed.IsSuccess, parsed.Error);
        ConversationSession session = new(parsed.Value!, new KeywordCrisisDetector(), new Random(1));
        session.Start();

        session.SubmitRating(8);
        ConversationTurn turn = session.SubmitText("окно, чашка, книга, лампа, рука");
        Assert.Contains(turn.BotMessages, m => m.Contains("окно, чашка"));
        session.SubmitText("стул");
        session.SubmitText("часы");
        session.SubmitText("кофе");
        turn = session.SubmitText("вода");
        Assert.Equal(ConversationInputKind.Rating, turn.Prompt!.Kind);

        turn = session.SubmitRating(3);

        Assert.Equal(ConversationStatus.Completed, turn.Status);
        Assert.Contains(expectedFinal, turn.BotMessages[^1]);
    }

    [Fact]
    public void Grounding_high_tension_does_not_stop_the_exercise()
    {
        ParseResult<ConversationScenario> parsed = ConversationScenarioParser.Parse(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "conversations", "Grounding.json")));
        ConversationSession session = new(parsed.Value!, new KeywordCrisisDetector(), new Random(1));
        session.Start();

        ConversationTurn turn = session.SubmitRating(10);

        Assert.Equal(ConversationInputKind.Text, turn.Prompt!.Kind);
    }

    [Fact]
    public void English_scenario_runs_end_to_end()
    {
        ConversationSession session = Start("en", out _);
        session.SubmitText("Tough call with my manager");
        session.SubmitChoice(0);
        session.SubmitText("Tight chest");
        session.SubmitRating(6);
        session.SubmitText("Two people talking");
        session.SubmitText("You are doing fine");

        ConversationTurn turn = session.SubmitRating(3);

        Assert.Equal(ConversationStatus.Completed, turn.Status);
        Assert.Contains("It was 6, now 3", turn.BotMessages[^1]);
    }
}
