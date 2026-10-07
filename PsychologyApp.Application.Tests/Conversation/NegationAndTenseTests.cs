using PsychologyApp.Application.Conversation.Companion;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>A feeling that is denied or over is not the feeling the person has now.</summary>
public class NegationAndTenseTests
{
    private static readonly LexiconSituationAnalyzer Analyzer = new();

    [Theory]
    [InlineData("мне не страшно")]
    [InlineData("я больше не злюсь на него")]
    [InlineData("раньше было тревожно, а сейчас спокойно")]
    [InlineData("мне совсем не грустно")]
    [InlineData("перестала переживать из-за этого")]
    [InlineData("i'm not scared anymore")]
    [InlineData("i don't feel sad")]
    [InlineData("it used to make me anxious but not now")]
    public void ADeniedOrFinishedFeelingIsNotRead(string text) => Assert.Equal(CompanionEmotion.Unknown, Analyzer.Analyze(text).Emotion);

    [Theory]
    [InlineData("я больше не могу, мне так тревожно", CompanionEmotion.Anxiety)]
    [InlineData("раньше было легче, а сейчас мне очень грустно", CompanionEmotion.Sadness)]
    [InlineData("i can't take it anymore, i'm so anxious", CompanionEmotion.Anxiety)]
    public void ThePresentFeelingStaysWhenAnOldOneIsMentioned(string text, CompanionEmotion expected) => Assert.Equal(expected, Analyzer.Analyze(text).Emotion);
}
