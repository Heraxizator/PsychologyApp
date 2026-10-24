using PsychologyApp.Domain.Colour.Enums;
using PsychologyApp.Domain.Colour.ValueObjects;

namespace PsychologyApp.Presentation.Shared.Common;

public static class ColourStrings
{
    /// <summary>The order of the name keys and explanation arrays below.</summary>
    private static readonly ColourValue[] Order =
    [
        ColourValue.Black, ColourValue.Red, ColourValue.Blue, ColourValue.Purple,
        ColourValue.Yellow, ColourValue.Brown, ColourValue.Green, ColourValue.Gray
    ];

    private static readonly string[] NameKeys = ["Black", "Red", "Blue", "Purple", "Yellow", "Brown", "Green", "Gray"];

    private static readonly ColourMeaning[] WantedRu =
    [
        ColourMeaning.BlackVoted, ColourMeaning.RedVoted, ColourMeaning.BlueVoted, ColourMeaning.PurpleVoted,
        ColourMeaning.YellowVoted, ColourMeaning.BrownVoted, ColourMeaning.GreenVoted, ColourMeaning.GrayVoted
    ];

    private static readonly ColourMeaning[] UnwantedRu =
    [
        ColourMeaning.BlackUnvoted, ColourMeaning.RedUnvoted, ColourMeaning.BlueUnvoted, ColourMeaning.PurpleUnvoted,
        ColourMeaning.YellowUnvoted, ColourMeaning.BrownUnvoted, ColourMeaning.GreenUnvoted, ColourMeaning.GrayUnvoted
    ];

    // ColourValue has no == of its own: == compared references and never matched, so every colour got grey's meaning
    // and its hex code for a name. Equals compares the colour codes.
    private static int IndexOf(ColourValue colour) => Array.FindIndex(Order, value => value.Equals(colour));

    /// <summary>The name of a stored colour code ("#0000FF") in the interface language; null for an unknown code.</summary>
    public static string? NameOfCode(string? code)
    {
        int index = IndexOfCode(code);
        return index < 0 ? null : AppStrings.In(AppStrings.Language, $"LuscherColour{NameKeys[index]}");
    }

    public static string GetColorName(ColourValue colour)
    {
        int index = IndexOf(colour);
        return index < 0 ? colour.ToString() : AppStrings.In(AppStrings.Language, $"LuscherColour{NameKeys[index]}");
    }

    public static string GetExplanation(ColourValue colour, ColourMeaningType meaningType) =>
        ExplanationAt(Math.Max(0, IndexOf(colour)), meaningType);

    /// <summary>The meaning of a stored colour code in the interface language; null for an unknown code.</summary>
    public static string? ExplanationOfCode(string? code, ColourMeaningType meaningType)
    {
        int index = IndexOfCode(code);
        return index < 0 ? null : ExplanationAt(index, meaningType);
    }

    /// <summary>"Blue / Red" from a saved short Lüscher result, in the interface language; null when its codes are missing.</summary>
    public static string? BriefSummary(string? firstCode, string? secondCode) =>
        NameOfCode(firstCode) is { } first && NameOfCode(secondCode) is { } second ? $"{first} / {second}" : null;

    private static int IndexOfCode(string? code) =>
        Array.FindIndex(Order, value => string.Equals(value.Code, code, StringComparison.OrdinalIgnoreCase));

    private static string ExplanationAt(int index, ColourMeaningType meaningType)
    {
        string language = AppStrings.Language.ToLowerInvariant();
        if (language == "ru")
        {
            return (meaningType == ColourMeaningType.Wanted ? WantedRu : UnwantedRu)[index].Explaination!;
        }

        (string[] wanted, string[] unwanted) = Explanations.TryGetValue(language, out var texts) ? texts : Explanations["en"];
        return meaningType == ColourMeaningType.Wanted ? wanted[index] : unwanted[index];
    }

    /// <summary>The explanations in the order of <see cref="Order"/>: the chosen colour, then the rejected one. Russian lives in the domain.</summary>
    private static readonly Dictionary<string, (string[] Wanted, string[] Unwanted)> Explanations = new()
    {
        ["en"] = (
            [
                "Negativity, rejection of pleasure, and aggression fill your mind and body. You feel hostile and may burst into rage at any moment. You are close to destroying yourself or your relationships.",
                "You are emotionally aroused. Your mood is elevated. You strive for achievement and success. You may be pushy, even aggressive.",
                "You seek harmony, trust, understanding, and empathy. You feel emotionally comfortable, calm, gentle, and dreamy. You are open to talking with friends.",
                "You are flirtatious and seek some kind of romantic or sexual intrigue. You want to please, gain support, or receive a compliment. Your mood is even but not calm.",
                "Optimism fills your soul and makes your heart beat faster. You are relaxed and full of dreams of good fortune. You are ready for change and full release from ties or obligations.",
                "You are tired and seek rest and emotional stability. You are psychologically exhausted and hungry for supportive relationships. Deep down you fear something and do not feel safe. You need sensual satisfaction.",
                "You are self-confident, even overconfident. This is a peak of your strength and self-respect. You can do much and want to dominate in communication. Or conversely, you have taken a defensive stance.",
                "You are looking for a shoulder to lean on. You want to hide from everything heavy in your life and find emotional peace and refuge. You mimic and mask your true feelings with feigned indifference."
            ],
            [
                "Outwardly you are calm and confident. However, you have driven aggression deep inside and moved onto denial and self-punishment.",
                "You are constantly irritated and overstimulated. You are under deep stress. Sometimes you feel drained or even exhausted.",
                "You are anxious. A close relationship may have recently ended. You feel lonely and upset.",
                "You want to stay unnoticed and hide from excessive attention. Modesty, control of feelings, and restraint characterize you right now.",
                "You are disappointed to the point of despair. You are distrustful and suspicious. Your emotional state is unstable: ups and sharp drops.",
                "You are tense like a drawn string. You deny all your emotional and physical needs. You run from weakness, restricting yourself in everything. You need sensual satisfaction.",
                "You are frustrated by lack of attention and respect from your partner. You feel humiliated, hurt, wounded, and drained. You have no strength left to resist.",
                "You are proactive as never before. You are fully engaged in the here-and-now. You are sociable, moderately cheerful, and resourceful. You have a goal and feel confident calm about tomorrow. You seem to have found your purpose."
            ]),
        ["fr"] = (
            [
                "La négativité, le refus du plaisir et l'agressivité remplissent votre esprit et votre corps. Vous vous sentez hostile et pouvez exploser de colère à tout moment. Vous êtes près de vous détruire ou de détruire vos relations.",
                "Vous êtes émotionnellement stimulé. Votre humeur est haute. Vous recherchez la réussite et le succès. Vous pouvez être insistant, voire agressif.",
                "Vous recherchez l'harmonie, la confiance, la compréhension et l'empathie. Vous vous sentez à l'aise, calme, doux et rêveur. Vous êtes ouvert aux conversations avec vos amis.",
                "Vous êtes d'humeur séductrice et cherchez une forme d'intrigue romantique ou sensuelle. Vous voulez plaire, être soutenu ou recevoir un compliment. Votre humeur est égale, mais pas calme.",
                "L'optimisme remplit votre âme et fait battre votre cœur plus vite. Vous êtes détendu et plein de rêves de chance. Vous êtes prêt au changement et à vous libérer complètement des liens et des obligations.",
                "Vous êtes fatigué et cherchez le repos et la stabilité émotionnelle. Vous êtes épuisé psychologiquement et avez soif de relations qui vous soutiennent. Au fond, vous craignez quelque chose et ne vous sentez pas en sécurité. Vous avez besoin de satisfactions sensorielles.",
                "Vous êtes sûr de vous, voire trop. C'est un sommet de votre force et de votre estime de vous. Vous pouvez beaucoup et voulez dominer dans les échanges. Ou au contraire, vous avez pris une position défensive.",
                "Vous cherchez une épaule sur laquelle vous appuyer. Vous voulez vous cacher de tout ce qui est lourd dans votre vie et trouver la paix et un refuge. Vous masquez vos vrais sentiments derrière une indifférence feinte."
            ],
            [
                "En apparence, vous êtes calme et sûr de vous. Mais vous avez refoulé l'agressivité au fond de vous et êtes passé au déni et à l'autopunition.",
                "Vous êtes constamment irrité et surstimulé. Vous êtes sous un stress profond. Parfois, vous vous sentez vidé, voire épuisé.",
                "Vous êtes anxieux. Une relation proche s'est peut-être terminée récemment. Vous vous sentez seul et contrarié.",
                "Vous voulez passer inaperçu et vous cacher d'une attention excessive. La modestie, le contrôle de vos émotions et la retenue vous caractérisent en ce moment.",
                "Vous êtes déçu jusqu'au désespoir. Vous êtes méfiant et soupçonneux. Votre état émotionnel est instable : des hauts et des chutes brusques.",
                "Vous êtes tendu comme une corde. Vous niez tous vos besoins émotionnels et physiques. Vous fuyez la faiblesse en vous restreignant en tout. Vous avez besoin de satisfactions sensorielles.",
                "Vous souffrez du manque d'attention et de respect de votre partenaire. Vous vous sentez humilié, blessé et vidé. Il ne vous reste plus de force pour résister.",
                "Vous êtes plus actif que jamais. Vous êtes pleinement dans l'ici et maintenant. Vous êtes sociable, modérément joyeux et plein de ressources. Vous avez un but et regardez demain avec un calme confiant. Vous semblez avoir trouvé votre voie."
            ]),
        ["de"] = (
            [
                "Negativität, die Ablehnung von Genuss und Aggression erfüllen deinen Kopf und Körper. Du fühlst dich feindselig und könntest jeden Moment vor Wut explodieren. Du bist nah daran, dich selbst oder deine Beziehungen zu zerstören.",
                "Du bist emotional erregt. Deine Stimmung ist gehoben. Du strebst nach Leistung und Erfolg. Du kannst fordernd, sogar aggressiv sein.",
                "Du suchst Harmonie, Vertrauen, Verständnis und Mitgefühl. Du fühlst dich wohl, ruhig, sanft und verträumt. Du bist offen für Gespräche mit Freunden.",
                "Du bist kokett und suchst eine Art romantisches oder sinnliches Abenteuer. Du willst gefallen, Unterstützung bekommen oder ein Kompliment hören. Deine Stimmung ist ausgeglichen, aber nicht ruhig.",
                "Optimismus erfüllt dich und lässt dein Herz schneller schlagen. Du bist entspannt und voller Träume vom Glück. Du bist bereit für Veränderung und dafür, dich ganz von Bindungen und Verpflichtungen zu lösen.",
                "Du bist müde und suchst Ruhe und emotionale Stabilität. Du bist seelisch erschöpft und sehnst dich nach Beziehungen, die dich tragen. Tief im Inneren fürchtest du etwas und fühlst dich nicht sicher. Du brauchst sinnliche Zufriedenheit.",
                "Du bist selbstsicher, sogar übermäßig. Das ist ein Höhepunkt deiner Kraft und Selbstachtung. Du kannst viel und willst im Gespräch dominieren. Oder umgekehrt: Du bist in die Verteidigung gegangen.",
                "Du suchst eine Schulter zum Anlehnen. Du willst dich vor allem Schweren in deinem Leben verstecken und seelische Ruhe und Zuflucht finden. Du verbirgst deine wahren Gefühle hinter gespielter Gleichgültigkeit."
            ],
            [
                "Äußerlich bist du ruhig und selbstsicher. Doch du hast die Aggression tief nach innen verdrängt und bist zu Verleugnung und Selbstbestrafung übergegangen.",
                "Du bist ständig gereizt und überreizt. Du stehst unter tiefem Stress. Manchmal fühlst du dich ausgelaugt oder sogar erschöpft.",
                "Du bist ängstlich. Eine enge Beziehung ist vielleicht vor Kurzem zu Ende gegangen. Du fühlst dich einsam und bedrückt.",
                "Du willst unbemerkt bleiben und dich vor zu viel Aufmerksamkeit verstecken. Bescheidenheit, Kontrolle der Gefühle und Zurückhaltung prägen dich gerade.",
                "Du bist bis zur Verzweiflung enttäuscht. Du bist misstrauisch und argwöhnisch. Dein Gefühlszustand ist instabil: Höhen und steile Abstürze.",
                "Du bist gespannt wie eine Saite. Du verleugnest all deine seelischen und körperlichen Bedürfnisse. Du fliehst vor Schwäche und schränkst dich in allem ein. Du brauchst sinnliche Zufriedenheit.",
                "Dich frustriert der Mangel an Aufmerksamkeit und Respekt von deinem Partner. Du fühlst dich gedemütigt, verletzt und ausgelaugt. Du hast keine Kraft mehr, dich zu wehren.",
                "Du bist so tatkräftig wie nie zuvor. Du bist ganz im Hier und Jetzt. Du bist gesellig, maßvoll fröhlich und einfallsreich. Du hast ein Ziel und siehst dem Morgen mit ruhiger Zuversicht entgegen. Du scheinst deine Bestimmung gefunden zu haben."
            ]),
        ["es"] = (
            [
                "La negatividad, el rechazo del placer y la agresividad llenan tu mente y tu cuerpo. Te sientes hostil y puedes estallar de rabia en cualquier momento. Estás cerca de destruirte a ti mismo o tus relaciones.",
                "Estás emocionalmente activado. Tu ánimo está alto. Buscas logros y éxito. Puedes ser insistente, incluso agresivo.",
                "Buscas armonía, confianza, comprensión y empatía. Te sientes a gusto, tranquilo, suave y soñador. Estás abierto a hablar con tus amigos.",
                "Estás coqueto y buscas algún tipo de intriga romántica o sensual. Quieres gustar, recibir apoyo o un cumplido. Tu ánimo es estable, pero no tranquilo.",
                "El optimismo llena tu alma y hace latir más rápido tu corazón. Estás relajado y lleno de sueños de buena suerte. Estás listo para el cambio y para liberarte del todo de ataduras y obligaciones.",
                "Estás cansado y buscas descanso y estabilidad emocional. Estás agotado psicológicamente y con ganas de relaciones que te sostengan. En el fondo temes algo y no te sientes seguro. Necesitas satisfacción sensorial.",
                "Tienes confianza en ti mismo, incluso demasiada. Es un punto alto de tu fuerza y tu autoestima. Puedes mucho y quieres dominar en la conversación. O, al contrario, te has puesto a la defensiva.",
                "Buscas un hombro en el que apoyarte. Quieres esconderte de todo lo pesado de tu vida y encontrar paz y refugio. Ocultas tus verdaderos sentimientos tras una indiferencia fingida."
            ],
            [
                "Por fuera estás tranquilo y seguro. Sin embargo, has empujado la agresividad muy adentro y has pasado a la negación y el autocastigo.",
                "Estás irritado y sobreestimulado todo el tiempo. Estás bajo un estrés profundo. A veces te sientes vacío o incluso agotado.",
                "Sientes ansiedad. Puede que una relación cercana haya terminado hace poco. Te sientes solo y disgustado.",
                "Quieres pasar desapercibido y esconderte de la atención excesiva. Te caracterizan ahora la modestia, el control de tus emociones y la contención.",
                "Estás decepcionado hasta la desesperación. Eres desconfiado y receloso. Tu estado emocional es inestable: subidas y caídas bruscas.",
                "Estás tenso como una cuerda. Niegas todas tus necesidades emocionales y físicas. Huyes de la debilidad restringiéndote en todo. Necesitas satisfacción sensorial.",
                "Te frustra la falta de atención y respeto de tu pareja. Te sientes humillado, herido y vacío. No te quedan fuerzas para resistir.",
                "Estás más activo que nunca. Estás plenamente en el aquí y ahora. Eres sociable, moderadamente alegre y lleno de recursos. Tienes una meta y miras el mañana con una calma segura. Parece que has encontrado tu propósito."
            ]),
        ["pt"] = (
            [
                "Negatividade, rejeição do prazer e agressividade enchem sua mente e seu corpo. Você se sente hostil e pode explodir de raiva a qualquer momento. Você está perto de destruir a si mesmo ou seus relacionamentos.",
                "Você está emocionalmente agitado. Seu humor está elevado. Você busca conquistas e sucesso. Pode ser insistente, até agressivo.",
                "Você busca harmonia, confiança, compreensão e empatia. Sente-se à vontade, calmo, suave e sonhador. Está aberto a conversar com os amigos.",
                "Você está sedutor e busca algum tipo de envolvimento romântico ou sensual. Quer agradar, ter apoio ou receber um elogio. Seu humor está estável, mas não calmo.",
                "O otimismo enche sua alma e faz seu coração bater mais rápido. Você está relaxado e cheio de sonhos de boa sorte. Está pronto para mudanças e para se libertar por completo de laços e obrigações.",
                "Você está cansado e busca descanso e estabilidade emocional. Está esgotado psicologicamente e com sede de relacionamentos que deem apoio. No fundo, teme algo e não se sente seguro. Precisa de satisfação sensorial.",
                "Você está autoconfiante, até demais. É um auge da sua força e do seu respeito próprio. Você pode muito e quer dominar nas conversas. Ou, ao contrário, assumiu uma postura defensiva.",
                "Você procura um ombro para se apoiar. Quer se esconder de tudo o que pesa na sua vida e encontrar paz e refúgio. Esconde seus verdadeiros sentimentos atrás de uma indiferença fingida."
            ],
            [
                "Por fora você está calmo e confiante. Mas empurrou a agressividade para dentro e passou à negação e à autopunição.",
                "Você está constantemente irritado e sobrecarregado de estímulos. Está sob estresse profundo. Às vezes se sente esvaziado ou até exausto.",
                "Você está ansioso. Talvez um relacionamento próximo tenha terminado há pouco. Você se sente sozinho e chateado.",
                "Você quer passar despercebido e se esconder do excesso de atenção. Modéstia, controle das emoções e contenção caracterizam você agora.",
                "Você está decepcionado a ponto de se desesperar. Está desconfiado e suspeitoso. Seu estado emocional está instável: altos e quedas bruscas.",
                "Você está tenso como uma corda esticada. Nega todas as suas necessidades emocionais e físicas. Foge da fraqueza restringindo-se em tudo. Precisa de satisfação sensorial.",
                "Você está frustrado com a falta de atenção e respeito do seu parceiro. Sente-se humilhado, magoado e esvaziado. Não tem mais forças para resistir.",
                "Você está mais ativo do que nunca. Está totalmente no aqui e agora. É sociável, moderadamente alegre e cheio de recursos. Tem um objetivo e olha para o amanhã com uma calma confiante. Parece ter encontrado seu propósito."
            ]),
        ["zh"] = (
            [
                "消极、拒绝快乐和攻击性充满了你的头脑和身体。你感到充满敌意，随时可能爆发怒火。你正接近于毁掉自己或你的人际关系。",
                "你情绪高涨，心情愉快。你追求成就和成功。你可能会显得咄咄逼人，甚至具有攻击性。",
                "你在寻求和谐、信任、理解和共情。你感到情绪舒适、平静、温和，带着几分梦幻。你乐于和朋友交谈。",
                "你带着几分轻佻，在寻找某种浪漫或感官上的吸引。你想讨人喜欢、获得支持或得到一句赞美。你的情绪平稳，但并不平静。",
                "乐观充满你的内心，让你的心跳加快。你很放松，满怀对好运的憧憬。你已准备好迎接变化，彻底摆脱束缚和义务。",
                "你很疲惫，在寻求休息和情绪上的稳定。你在心理上筋疲力尽，渴望能给你支持的关系。内心深处你在害怕什么，感到不安全。你需要感官上的满足。",
                "你很自信，甚至过于自信。这是你力量和自尊的高峰。你能做很多事，想在交流中占据主导。或者相反，你采取了防御的姿态。",
                "你在寻找一个可以依靠的肩膀。你想躲开生活中一切沉重的事，找到内心的平静和避风港。你用假装的冷漠掩饰自己真实的感受。"
            ],
            [
                "表面上你平静而自信。但你把攻击性深深压在心底，转向了否认和自我惩罚。",
                "你经常烦躁、过度兴奋。你正承受着很深的压力。有时你感到被掏空，甚至筋疲力尽。",
                "你感到焦虑。也许一段亲密关系最近结束了。你感到孤独和难过。",
                "你想不被注意，躲开过多的关注。此刻的你谦逊、克制，控制着自己的情绪。",
                "你失望到了绝望的地步。你多疑、不信任他人。你的情绪不稳定：时高时低，起伏剧烈。",
                "你像一根绷紧的弦。你否认自己所有情感和身体上的需要。你逃避软弱，在一切事情上约束自己。你需要感官上的满足。",
                "伴侣缺少对你的关注和尊重，让你感到沮丧。你感到被羞辱、受伤和被掏空。你已没有力气再抵抗。",
                "你前所未有地积极主动。你完全活在此时此地。你善于交际、适度开朗、富有办法。你有目标，对明天怀着自信的平静。你似乎找到了自己的使命。"
            ])
    };
}
