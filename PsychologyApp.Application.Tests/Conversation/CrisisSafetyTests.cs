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
        "thinking of killing myself", "i took a bunch of pills", "no reason to keep living", "i dont want to be alive", "i want to unalive myself",
        "думаю застрелиться", "иногда хочется утопиться", "хочу отравиться таблетками", "сброшусь с крыши", "выпрыгну из окна, если так дальше",
        "лягу на рельсы и всё", "i keep thinking i should drown myself", "maybe i will poison myself", "i want to jump in front of a train",
        // French
        "je veux mourir", "j'ai envie d'en finir", "je pense au suicide", "je n'ai plus envie de vivre", "tout le monde serait mieux sans moi",
        "je me suis scarifiée hier", "je vais sauter du pont", "j'ai avalé toute la boîte de somnifères", "Je veux me tuer",
        // German
        "ich will sterben", "ich will nicht mehr leben", "ich denke an Selbstmord", "alle wären besser ohne mich", "ich ritze mich wieder",
        "ich springe von der Brücke", "ich nehme alle Tabletten", "ich bin lebensmüde", "ich will mir etwas antun",
        // Spanish
        "quiero morirme", "ya no quiero vivir", "pienso en suicidarme", "todos estarían mejor sin mí", "me corto las muñecas",
        "me voy a tirar del puente", "voy a tomarme todas las pastillas", "quiero acabar con mi vida", "quiero quitarme la vida",
        // Portuguese
        "quero morrer", "não quero mais viver", "penso em me matar", "todo mundo ficaria melhor sem mim", "eu me corto quando estou mal",
        "vou pular da ponte", "tomei todos os remédios", "não aguento mais viver", "quero sumir pra sempre",
        // Chinese
        "我不想活了", "我想自杀", "活着没意思", "我想结束自己的生命", "没有我大家会更好", "我又割腕了", "我站在楼顶上", "我想死", "我把药全吃了",
        "我想死你了，但是我真的想死"
    ];

    public static TheoryData<string> NotCrisis =>
    [
        "я не хочу умирать, мне страшно", "боюсь смерти", "у меня умерла бабушка", "мне грустно и тяжело", "я так устала от работы", "хочу отдохнуть и выспаться",
        "i'm afraid of dying", "my grandmother died last week", "i am so tired of work", "i want to sleep", "i don't want to go to work",
        "это убивает меня на работе, столько дел", "смерть от смеха, ну и шутка", "хочу провалиться со стыда",
        "давайте сброситься деньгами на подарок", "я сброшусь с Машей на подарок", "выпрыгну с парашютом летом", "я отравилась в кафе, весь день плохо",
        "j'ai peur de mourir", "je suis mort de rire", "ma grand-mère est morte", "je suis épuisée par le travail", "je veux dormir",
        "ich habe Angst vor dem Tod", "meine Oma ist gestorben", "ich bin total müde von der Arbeit", "ich lache mich tot",
        "tengo miedo a la muerte", "me muero de risa", "mi abuela murió", "estoy cansado del trabajo", "mañana me corto el pelo",
        "tenho medo de morrer", "morri de rir", "minha avó morreu", "estou cansada do trabalho",
        "我想死你了", "笑死我了", "累死了", "我奶奶去世了", "跳楼价大甩卖", "我怕死", "你可想死我了"
    ];

    [Theory]
    [MemberData(nameof(Crisis))]
    public void ACrisisIsNeverMissed(string text) => Assert.True(Detector.IsCrisis(text), text);

    [Theory]
    [MemberData(nameof(NotCrisis))]
    public void OrdinarySadnessAndFearDoNotTriggerTheHelplineCard(string text) => Assert.False(Detector.IsCrisis(text), text);
}
