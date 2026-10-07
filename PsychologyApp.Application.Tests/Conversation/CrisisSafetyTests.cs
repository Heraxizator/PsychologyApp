using PsychologyApp.Application.Conversation;
using Xunit;

namespace PsychologyApp.Application.Tests.Conversation;

/// <summary>
/// The crisis detector against a labelled set of how people actually say it: direct, indirect, slang, typos, English. Zero misses is the bar:
/// a missed crisis costs far more than a helpline card shown too early.
/// </summary>
public class CrisisSafetyTests
{
    private static readonly KeywordCrisisDetector Detector = new();

    public static TheoryData<string> Crisis =>
    [
        "я хочу покончить с собой", "хочу умереть", "не хочу больше жить", "лучше бы меня не было", "всем будет лучше без меня",
        "думаю о самоубийстве", "я думаю о суициде", "хочу исчезнуть навсегда", "не вижу смысла жить дальше", "устала от всего, хочу чтобы всё закончилось навсегда",
        "я больше так не могу, хочу уснуть и не проснуться", "хочу сдохнуть", "иногда думаю, как бы всё прекратить", "у меня есть план как уйти из жизни",
        "я порезал себя вчера", "режу руки когда плохо", "проглотила все таблетки", "стою на мосту и смотрю вниз", "собираюсь прыгнуть с крыши",
        "жить не хочется совсем", "незачем жить", "без меня всем было бы легче", "я лишняя в этом мире, лучше бы не было", "мне кажется, пора со всем покончить",
        "хочу уйти навсегда", "я себя убью", "убью себя", "не хочу быть здесь больше", "хочу чтобы меня не стало", "накончу с собой", "суецид",
        "i want to kill myself", "i want to die", "i dont want to live anymore", "i'm thinking about suicide", "everyone would be better off without me",
        "i wish i could just disappear forever", "i have a plan to end my life", "i cut myself again", "i can't go on", "i want to end it all",
        "thinking of killing myself", "i took a bunch of pills", "no reason to keep living", "i dont want to be alive", "i want to unalive myself"
    ];

    public static TheoryData<string> NotCrisis =>
    [
        "я не хочу умирать, мне страшно", "боюсь смерти", "у меня умерла бабушка", "мне грустно и тяжело", "я так устала от работы", "хочу отдохнуть и выспаться",
        "i'm afraid of dying", "my grandmother died last week", "i am so tired of work", "i want to sleep", "i don't want to go to work",
        "это убивает меня на работе, столько дел", "смерть от смеха, ну и шутка", "хочу провалиться со стыда"
    ];

    [Theory]
    [MemberData(nameof(Crisis))]
    public void ACrisisIsNeverMissed(string text) => Assert.True(Detector.IsCrisis(text), text);

    [Theory]
    [MemberData(nameof(NotCrisis))]
    public void OrdinarySadnessAndFearDoNotTriggerTheHelplineCard(string text) => Assert.False(Detector.IsCrisis(text), text);
}
