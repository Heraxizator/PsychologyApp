using PsychologyApp.Application.Chat;
using PsychologyApp.Application.Conversation;
using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Chat;

public class CompanionKnowledgeTests
{
    private static CompanionDialogue Create(bool english = false) =>
        new(new LexiconSituationAnalyzer(), new KeywordCrisisDetector(), english, new Random(3));

    private static CompanionReply Say(CompanionDialogue d, CompanionState state, string text) =>
        d.Respond(state, new CompanionInput.FreeText(text));

    [Theory]
    [InlineData("Что такое паническая атака?", Utterance.AsksToExplain)]
    [InlineData("объясни, что такое КПТ", Utterance.AsksToExplain)]
    [InlineData("what is burnout?", Utterance.AsksToExplain)]
    [InlineData("Почему я так реагирую?", Utterance.AsksWhy)]
    [InlineData("why do I feel like this", Utterance.AsksWhy)]
    [InlineData("Это нормально?", Utterance.AsksIfNormal)]
    [InlineData("со мной что-то не так?", Utterance.AsksIfNormal)]
    [InlineData("is this normal?", Utterance.AsksIfNormal)]
    public void Questions_about_psychology_are_recognised(string text, Utterance expected) =>
        Assert.Equal(expected, UtteranceClassifier.Classify(text, hasFeeling: false));

    [Fact]
    public void A_known_topic_is_explained_and_tied_back_to_the_person()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Что такое паническая атака?");

        Assert.Contains("«бей или беги»", reply.Messages[0]);
        Assert.EndsWith("?", reply.Messages[0]);
        Assert.True(reply.State.QuestionPending);
    }

    [Fact]
    public void An_unknown_topic_offers_a_list_to_choose_from()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Объясни, что такое квантовая запутанность");

        Assert.NotEmpty(reply.QuickReplies);
        Assert.All(reply.QuickReplies, chip => Assert.StartsWith("topic:", chip.Payload));
    }

    [Fact]
    public void A_frightened_question_with_no_known_topic_is_listened_to_not_answered_with_a_topic_list()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Что это такое со мной, мне так страшно и одиноко");

        Assert.DoesNotContain(reply.QuickReplies, chip => chip.Payload?.StartsWith("topic:", StringComparison.Ordinal) == true);
        Assert.NotEmpty(reply.State.RecentTexts);
    }

    [Fact]
    public void Choosing_a_topic_chip_explains_it()
    {
        CompanionDialogue d = Create();
        ChatQuickReply chip = CompanionKnowledge.TopicChips(false).First(c => c.Payload == "topic:cbt");

        CompanionReply reply = d.Respond(new CompanionState(), new CompanionInput.QuickReply(chip));

        Assert.Contains("КПТ", reply.Messages[0]);
    }

    [Fact]
    public void Why_is_answered_for_the_current_feeling_without_a_diagnosis()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 2, Emotion = "Guilt" }, "Почему я так себя чувствую?");

        Assert.Contains("Вина", reply.Messages[0]);
        Assert.DoesNotContain("расстройств", reply.Messages[0]);
    }

    [Fact]
    public void Is_this_normal_reassures_and_points_to_a_specialist_for_lasting_problems()
    {
        CompanionReply reply = Say(Create(), new CompanionState { Turns = 2, Emotion = "Anxiety" }, "Это нормально?");

        Assert.Contains("обычная человеческая реакция", reply.Messages[0]);
        Assert.Contains("специалист", reply.Messages[0]);
        Assert.Contains("Диагнозов я не ставлю", reply.Messages[0]);
    }

    [Fact]
    public void Advice_for_a_known_feeling_says_what_often_helps_and_offers_a_practice()
    {
        CompanionDialogue d = Create();
        CompanionReply advice = Say(d, new CompanionState { Turns = 2, Emotion = "Anxiety" }, "Что мне делать?");

        Assert.Equal(2, advice.Messages.Count);
        Assert.Contains("выдох", advice.Messages[0]);
        Assert.Contains(advice.QuickReplies, c => c.Payload == "practice");

        CompanionReply offer = d.Respond(advice.State, new CompanionInput.QuickReply(advice.QuickReplies.First(c => c.Payload == "practice")));

        Assert.Contains(offer.QuickReplies, c => c.Kind == ChatQuickReplyKinds.Practice);
    }

    [Fact]
    public void English_answers_exist_for_every_emotion()
    {
        foreach (CompanionEmotion emotion in Enum.GetValues<CompanionEmotion>())
        {
            Assert.False(string.IsNullOrWhiteSpace(CompanionKnowledge.Why(emotion, true)));
            Assert.False(string.IsNullOrWhiteSpace(CompanionKnowledge.WhatHelps(emotion, true)));
            Assert.False(string.IsNullOrWhiteSpace(CompanionKnowledge.Normal(emotion, true)));
            Assert.False(string.IsNullOrWhiteSpace(CompanionKnowledge.Why(emotion, false)));
        }
    }

    [Fact]
    public void English_explanation_works()
    {
        CompanionReply reply = Say(Create(english: true), new CompanionState(), "What is burnout?");

        Assert.Contains("Burnout", reply.Messages[0]);
    }

    [Fact]
    public void Crisis_wording_is_still_caught_first()
    {
        CompanionReply reply = Say(Create(), new CompanionState(), "Почему мне хочется умереть?");

        Assert.Equal(DialogueActionKind.OpenCrisisHub, reply.Action?.Kind);
    }
}
