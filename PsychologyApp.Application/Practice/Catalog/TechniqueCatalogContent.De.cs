using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentDe
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("Was es ist", what),
        new("Wann es hilft", when),
        new("Wie die Übung abläuft", how),
        new("Beispiel", example)
    ];

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["Streit mit einem nahen Menschen", "Etwas auf der Arbeit", "Konflikt mit meinem Chef", "Studium oder eine Prüfung", "Gesundheit", "Geld", "Einsamkeit", "Ein Misserfolg", "Eine Trennung", "Beziehung zu meinen Eltern", "Kinder", "Ein Umzug", "Die Zukunft", "Kritik an mir"];

    private static readonly string[] BackupOptions =
        ["Um Hilfe bitten", "Abwarten und es erneut versuchen", "Einen anderen Weg finden", "Weniger von mir verlangen", "Eine Pause machen", "Mit einem nahen Menschen darüber reden", "Zu einer Fachperson gehen", "Einen Nebenjob finden", "Einen Kurs machen", "Die Aufgabe in Schritte teilen", "Die Entscheidung verschieben", "Klein anfangen"];

    private static readonly string[] HarshThoughts =
        ["„Ich mache alles kaputt“", "„Sie werden mich verurteilen“", "„Ich schaffe das nicht“", "„Niemand braucht mich“", "„Das ist eine Katastrophe“", "„Nichts wird klappen“", "„Ich bin schlechter als andere“", "„Immer geht alles schief“", "„Es ist meine Schuld“", "„Mit mir stimmt etwas nicht“", "„Das hört nie auf“", "„Ich muss perfekt sein“"];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Übung", "Spin",
        [
            "1. Beschreibe die Situation, die dich belastet",
            "2. Benenne das Hauptgefühl",
            "3. Bewerte das Gefühl von 0 bis 10"
        ],
        "Der Spin von Zivorad Slavinski hilft, die emotionale Ladung einer schmerzhaften Erinnerung zu senken.",
        TechniqueUiKind.Entry, "Technik Nr. 1", "26.01.2023", "Spin", "Emotionale Ladung in 3 Schritten senken", "Schwerer Moment", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Situation", "Z. B. das Gespräch von gestern, das dich nicht loslässt", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Hauptgefühl", "Kränkung, Angst, Wut…", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Stärke des Gefühls", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Eine einfache Neutralisierungstechnik: Du benennst die Situation, das Gefühl und seine Stärke. Oft senkt schon das Bewusstmachen die Ladung.",
            "Wenn eine Szene oder ein Gedanke kreist und Angst oder Groll nicht loslässt.",
            "Schreib die Situation und das Gefühl auf und bewerte die Stärke ehrlich. Wiederhole es bei Bedarf an einem anderen Tag.",
            "Situation: „Ein Kollege hat meine Idee ignoriert“. Gefühl: Kränkung. Bewertung: 8 → später 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "Übung", "Wichtigkeit vergleichen",
        [
            "1. Benenne das Problem",
            "2. Was war in diesem Bereich früher wichtig?",
            "3. Was ist jetzt wichtig?",
            "4. Was wird in Zukunft wichtig sein?",
            "5. Vergleiche die drei Antworten und notiere, was sich verschoben hat"
        ],
        "Wichtigkeit über die Zeit zu vergleichen hilft, ein Problem in den Zusammenhang deines Lebens zu stellen.",
        TechniqueUiKind.Entry, "Technik Nr. 2", "26.01.2023", "Wichtigkeit vergleichen", "Wenn das Problem zu groß wirkt", "Größe des Problems", "NLP", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problem", "Was fühlt sich gerade unerträglich an?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("War früher wichtig", "Z. B. Studium, Beziehungen, Karriere", Suggestions: ["Studium", "Familie", "Freunde", "Gesundheit", "Karriere", "Hobbys", "Sport", "Liebe", "Reisen", "Freiheit", "Anerkennung", "Zuhause"]),
            new TechniqueEntrySeed("Ist jetzt wichtig", "Was nimmt die meiste Aufmerksamkeit ein?", Suggestions: ["Arbeit", "Familie", "Gesundheit", "Geld", "Beziehungen", "Erholung", "Kinder", "Freunde", "Studium", "Schlaf", "Innere Ruhe", "Persönliche Entwicklung"]),
            new TechniqueEntrySeed("Wird in Zukunft wichtig sein", "Worauf steuerst du zu?", Suggestions: ["Stabilität", "Familie", "Freiheit", "Gesundheit", "Wachstum", "Menschen, die ich liebe", "Ein eigenes Zuhause", "Arbeit, die ich liebe", "Reisen", "Ruhe", "Kinder", "Ersparnisse"]),
            new TechniqueEntrySeed("Was hat sich verändert?", "Beherrscht das Problem noch alles?", Suggestions: ["Das Problem ist nur ein Teil des Lebens", "Es ist mehr wichtig, als es schien", "Es ist eine vorübergehende Phase", "Es fühlt sich weniger scharf an", "Ich bin ruhiger", "Ich sehe, dass ich schon früher zurechtgekommen bin", "Hoffnung ist da", "Noch hat sich nichts verändert"])
        ],
        Theory(
            "Die Technik zeigt, dass sich das Gewicht eines Problems mit der Zeit verändert und selten für immer im Mittelpunkt bleibt.",
            "Wenn eine Schwierigkeit alles ausfüllt und sich dauerhaft anfühlt.",
            "Fülle die vier Felder aus und vergleiche: Andere Werte lassen das Problem oft schrumpfen.",
            "Problem: „Keine Arbeit“. Früher: Studium. Jetzt: Jobsuche. Zukunft: Stabilität. Erkenntnis: eine Phase, nicht das ganze Leben."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "Übung", "Polaritäten",
        [
            "1. Schreib auf, was dir an der Situation gefällt",
            "2. Schreib auf, was dir nicht gefällt",
            "3. Vergleiche die Paare und bemerke das Gleichgewicht"
        ],
        "Die Polaritätenmethode hilft, beide Seiten zu sehen und innere Anspannung zu senken.",
        TechniqueUiKind.Polarity, "Technik Nr. 3", "26.01.2023", "Polaritäten", "Beide Seiten einer Situation sehen", "Verschiedene Seiten", "Zivorad Slavinski", 5, "Contrast",
        null,
        Theory(
            "Jede Situation hat angenehme und unangenehme Seiten. Beide zu benennen löst die einseitige Fixierung.",
            "Wenn du nur das Negative oder nur das Positive siehst und feststeckst.",
            "Füge zu einem Thema Paare aus „gefällt mir“ und „gefällt mir nicht“ hinzu. Vergleiche die Listen, ohne zu urteilen, was richtig ist.",
            "Arbeit: Das Gehalt gefällt mir, die Arbeitszeiten nicht. Zuhause: Die Gemütlichkeit gefällt mir, der Lärm der Nachbarn nicht."),
        "Was dir nicht gefällt", "Was dir gefällt");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "Übung", "Blatt Papier",
        [
            "1. Schreib den Gedanken auf Papier (oder in das Feld unten)",
            "2. Lies ihn einmal laut vor",
            "3. Falte, zerreiße oder lass das Blatt symbolisch los"
        ],
        "Gedanken auf Papier zu schreiben senkt ihre emotionale Ladung.",
        TechniqueUiKind.Paper, "Technik Nr. 4", "26.01.2023", "Blatt Papier", "Wenn Gedanken kreisen", "Gedanken", "Psyche", 3, "Description",
        [new TechniqueEntrySeed("Gedanke", "Z. B. „Ich vermassle die Präsentation“", Suggestions: HarshThoughts)],
        Theory(
            "Schreiben holt den Gedanken aus dem Kopf. Das Blatt zu vernichten steht für das Loslassen.",
            "Wenn derselbe Satz stundenlang kreist und Schlaf oder Konzentration blockiert.",
            "Lade den Gedanken ab, lies ihn und vernichte dann das Blatt — wirklich oder in der Vorstellung.",
            "Gedanke: „Sie werden mich für inkompetent halten“. Nach dem Aufschreiben und Verbrennen des Blattes lässt die Angst oft nach."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "Übung", "50 Jahre später",
        [
            "1. Benenne das Problem",
            "2. Bewerte seine Wichtigkeit in 50 Jahren von 0 bis 10"
        ],
        "Ein Problem in die Zukunft zu verschieben senkt seine gefühlte Wichtigkeit.",
        TechniqueUiKind.Entry, "Technik Nr. 5", "30.01.2023", "50 Jahre später", "Das Problem in die Zukunft verschieben", "Größe des Problems", "NLP", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problem", "Was fühlt sich heute wie eine Katastrophe an?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Wichtigkeit in 50 Jahren", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Sich ein Ereignis weit in der Zukunft vorzustellen nimmt ihm oft die Schärfe — du siehst es aus größerem Abstand.",
            "Wenn sich ein Rückschlag wie das Ende der Welt anfühlt, obwohl er vorübergehend ist.",
            "Benenne das Problem und bewerte, wie wichtig es in fünfzig Jahren sein wird. Der Wert ist meist niedriger.",
            "Problem: „Ich wurde nicht zum Treffen eingeladen“. In 50 Jahren: 1–2 von 10."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "Übung", "Weißer Fleck",
        [
            "1. Erinnere dich an die belastende Situation",
            "2. Stell sie dir als weißen Fleck ohne Details vor",
            "3. Halte den Fleck, bis die Anspannung nachlässt"
        ],
        "Details aus einer Erinnerung zu entfernen senkt oft ihre emotionale Ladung.",
        TechniqueUiKind.Entry, "Technik Nr. 6", "30.01.2023", "Weißer Fleck", "Eine schmerzhafte Erinnerung abmildern", "Schwerer Moment", "NLP", 3, "BlurOn",
        [new TechniqueEntrySeed("Situation", "Kurz: was passiert ist und was am meisten wehtut", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Details nähren das Gefühl. Ein weißer Fleck entfernt die Auslöser und gönnt deinem Nervensystem eine Pause.",
            "Wenn dich ein lebhaftes Bild aus der Vergangenheit mit Gefühlen überrollt.",
            "Erinnere dich an die Situation und ersetze das Bild dann 30–60 Sekunden lang durch einen schlichten weißen Fleck.",
            "Statt des Gesichts der Person, die dich verletzt hat — ein weißer Fleck. Der Atem beruhigt sich, der Kopf kehrt ins Jetzt zurück."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "Übung", "Erfahrung verändern",
        [
            "1. Wähle einen Bereich, an dem du arbeiten willst",
            "2. Beschreibe deine Haltung dazu",
            "3. Benenne die damit verbundenen Überzeugungen",
            "4. Wie verändert sich die Haltung, wenn du an frühere Erfahrungen denkst?",
            "5. Beschreibe eine negative Erfahrung",
            "6. Bewerte die negative Erfahrung (−10…10)",
            "7. Welche positive Erfahrung könnte sie ersetzen?",
            "8. Wie würde sich deine Haltung mit dieser Erfahrung verändern?",
            "9. Bewerte die neue Erfahrung (−10…10)"
        ],
        "EMO hilft, eine einschränkende Erfahrung durch eine unterstützendere zu ersetzen.",
        TechniqueUiKind.Entry, "Technik Nr. 7", "08.02.2023", "Erfahrung verändern", "Eine einschränkende Erfahrung umschreiben", "Schwerer Moment", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Bereich", "Beziehungen, Arbeit, Gesundheit…", Suggestions: ["Beziehungen", "Arbeit", "Gesundheit", "Geld", "Öffentliches Sprechen", "Familie", "Studium", "Aussehen", "Freundschaft", "Kreativität", "Sport", "Karriere"]),
            new TechniqueEntrySeed("Haltung zu diesem Bereich", "Wie empfindest du es jetzt?", Suggestions: ["Angst", "Selbstzweifel", "Gereiztheit", "Gleichgültigkeit", "Interesse", "Müdigkeit", "Furcht", "Scham", "Hoffnung", "Wut", "Verwirrung", "Ruhe"]),
            new TechniqueEntrySeed("Verbundene Überzeugungen", "Z. B. „Ich schaffe es nicht“", Suggestions: ["„Ich schaffe es nicht“", "„Ich bin nicht gut genug“", "„Man wird mich nicht akzeptieren“", "„Nichts wird sich ändern“", "„Ich muss perfekt sein“", "„Ich habe kein Glück“", "„Ich verdiene es nicht“", "„Andere sind besser“", "„Fehler sind verboten“", "„Dafür bin ich zu alt“"]),
            new TechniqueEntrySeed("Haltung und frühere Erfahrung", "Was verändert sich, wenn du an früher denkst?", Suggestions: ["Es wird schwerer", "Es wird ruhiger", "Nichts verändert sich", "Wut kommt hoch", "Traurigkeit kommt hoch", "Angst kommt hoch", "Ich erinnere mich, dass ich es geschafft habe", "Ich will es vermeiden"]),
            new TechniqueEntrySeed("Negative Erfahrung", "Eine bestimmte Situation oder ein Muster"),
            new TechniqueEntrySeed("Bewertung der negativen Erfahrung", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Positiver Ersatz", "Welche unterstützende Erfahrung ist möglich?", Suggestions: ["Ein Mal, als es mir gelungen ist", "Ein Mal, als ich unterstützt wurde", "Ein Mal, als ich es allein geschafft habe", "Ein Mal, als ich gelobt wurde", "Ein Mal, als ich jemandem geholfen habe", "Ein Mal, als ich etwas Neues gelernt habe", "Ein Mal, als ich nicht aufgegeben habe", "Ein Mal, als ich so angenommen wurde, wie ich bin"]),
            new TechniqueEntrySeed("Haltung mit dem Positiven", "Wie würde sich dein Blick auf den Bereich verändern?", Suggestions: ["Hoffnung kommt auf", "Mehr Ruhe", "Ich will es noch einmal versuchen", "Es wird interessant", "Zuversicht kommt auf", "Es wird leichter", "Ich spüre Kraft", "Noch schwer zu glauben"]),
            new TechniqueEntrySeed("Bewertung der neuen Erfahrung", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "Überzeugungen beruhen oft auf einer einzigen eindrücklichen negativen Erfahrung. Ein bewusster Ersatz kann deine Haltung verändern.",
            "Wenn es „schon immer so war“ und Veränderung unmöglich scheint.",
            "Geh die Felder der Reihe nach durch. Vergleiche die beiden Bewertungen — die zweite sollte höher sein.",
            "Bereich: öffentliches Sprechen. Negativ: durchgefallene Prüfung (−8). Ersatz: ein gelungener Vortrag (+6)."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "Übung", "Wiederhol es",
        [
            "1. Schreib die Formulierung auf, die dich belastet",
            "2. Wiederhole sie laut, als hörtest du sie zum ersten Mal",
            "3. Wiederhole weiter, bis das Gefühl nachlässt"
        ],
        "Einen Problemsatz „wie zum ersten Mal“ zu wiederholen senkt die emotionale Intensität.",
        TechniqueUiKind.Copied, "Technik Nr. 8", "21.01.2025", "Wiederhol es", "Das Problem wie zum ersten Mal hören", "Größe des Problems", "NLP", 2, "Repeat",
        [new TechniqueEntrySeed("Formulierung", "Ich habe Angst, meinen Job zu verlieren", Suggestions: HarshThoughts)],
        Theory(
            "Ein vertrauter Satz verliert seine Ladung, wenn man ihn langsam und neutral spricht, ohne Drama.",
            "Wenn ein Gedanke in deinem Kopf wie ein Urteil klingt.",
            "Schreib den Satz auf und wiederhole ihn dann 5–10 Mal mit Pausen; achte dabei auf die Körperempfindungen.",
            "„Niemand braucht mich“ — nach sieben Wiederholungen klingt es oft absurd und leichter."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "Übung", "Plan B",
        [
            "1. Benenne das Problem",
            "2. Schreib drei realistische Ausweichmöglichkeiten auf",
            "3. Bemerke, wie sich die Angst verändert, wenn es Auswege gibt"
        ],
        "Ausweichmöglichkeiten senken die übersteigerte Bedeutung eines einzigen Ausgangs.",
        TechniqueUiKind.Entry, "Technik Nr. 9", "21.01.2025", "Plan B", "Mit einem Plan B den Druck senken", "Größe des Problems", "NLP", 3, "Backup",
        [
            new TechniqueEntrySeed("Problem", "Was, wenn der Hauptplan scheitert?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Ausweichmöglichkeit 1", "Z. B. ein freiberufliches Projekt annehmen", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Ausweichmöglichkeit 2", "Z. B. eine Weiterbildung für einen verwandten Beruf", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Ausweichmöglichkeit 3", "Z. B. vorübergehend Ausgaben senken", Suggestions: BackupOptions)
        ],
        Theory(
            "Wenn es sich nach Alles-oder-nichts anfühlt, verkrampft der Kopf. Alternativen geben das Gefühl der Wahl zurück.",
            "Vor Vorstellungsgesprächen, Prüfungen oder Ereignissen, bei denen ein Scheitern katastrophal wirkt.",
            "Benenne das Problem und drei plausible Pläne B — auch bescheidene.",
            "Problem: „Ich bekomme die Stelle nicht“. Plan B: Praktikum, Empfehlungen, eine andere Stadt."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "Übung", "Mach es klein",
        [
            "1. Beschreibe, was dich belastet",
            "2. Stell es dir als Gegenstand vor dir vor",
            "3. Verkleinere es und schieb es weg, bis das Interesse nachlässt"
        ],
        "Visuelles Verkleinern und Abstand senken das emotionale Interesse an einem Problem.",
        TechniqueUiKind.Entry, "Technik Nr. 10", "22.01.2025", "Mach es klein", "Das Problem klein und fern machen", "Größe des Problems", "NLP", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("Was dich belastet", "Z. B. ein Konflikt, der dich wach hält", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Größe und Entfernung eines Bildes beeinflussen seine Bedeutung. Ein kleiner, ferner Gegenstand zerrt selten an den Nerven.",
            "Wenn ein Problem riesig wirkt und direkt vor dir steht.",
            "Erzeuge ein Bild, verkleinere es nach und nach und schieb es 10–20 Sekunden lang bis zum Horizont.",
            "Ein Streit wird zu einem winzigen Punkt. Der Atem wird leichter, die Gedanken klarer."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "Übung", "Prüf es",
        [
            "1. Beschreibe, worum deine Gedanken kreisen",
            "2. Hör bewusst für 1–2 Minuten auf, daran zu denken",
            "3. Bemerke, was sich in Körper und Stimmung verändert hat"
        ],
        "Zu viel Aufmerksamkeit bläht die Bedeutung auf. Eine kurze Pause prüft, wie sehr dich der Gedanke steuert.",
        TechniqueUiKind.Entry, "Technik Nr. 11", "22.01.2025", "Prüf es", "Sehen, was passiert, wenn du loslässt", "Größe des Problems", "NLP", 2, "FactCheck",
        [new TechniqueEntrySeed("Worum deine Gedanken kreisen", "Welcher Gedanke kommt immer wieder?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Wäre ein Gedanke überlebenswichtig, würde sich Loslassen gefährlich anfühlen. Das Experiment zeigt meist, dass nichts Katastrophales passiert.",
            "Wenn du nicht aufhören kannst, dasselbe Thema durchzukauen.",
            "Benenne den Gedanken und lenke die Aufmerksamkeit dann sanft auf den Atem oder die Geräusche um dich.",
            "Gedanke: „Was, wenn ich krank bin“. Nach einer Pause sinkt die Angst oft von 7 auf 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "Übung", "Beobachterposition",
        [
            "1. Beschreibe die Situation, die dich belastet",
            "2. Schreib die Gefühle in der Ich-Form auf",
            "3. Betrachte die Situation als neutraler Beobachter",
            "4. Bewerte die Anspannung nach dem Positionswechsel"
        ],
        "Ein Wechsel der Wahrnehmungsposition hilft, von einem emotionalen Höhepunkt zurückzutreten und die Situation weiter zu sehen.",
        TechniqueUiKind.Entry, "Technik Nr. 12", "12.06.2026", "Beobachterposition", "Von den Gefühlen zurücktreten und die Situation von außen sehen", "Schwerer Moment", "NLP", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situation", "Was ist passiert, oder was spielst du im Kopf immer wieder durch?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Gefühle in der Ich-Form", "Was fühlst du, wenn du mitten in der Situation bist?", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Was der Beobachter sieht", "Was würde ein ruhiger Beobachter von außen bemerken?", Suggestions: ["Zwei Menschen streiten", "Ein müder, aufgebrachter Mensch", "Eine gewöhnliche Arbeitssituation", "Jeder verteidigt seine Sicht", "In einer Woche ist das vergessen", "Die Menschen haben sich missverstanden", "Ein Mensch, der sich sehr bemüht", "Unangenehm, aber nicht gefährlich", "Jeder hat seine eigene Sicht", "Der Mensch braucht Ruhe"]),
            new TechniqueEntrySeed("Anspannung jetzt", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Mitten in einer Situation wirken Gefühle unausweichlich. Die Beobachterposition schafft Abstand, ohne das Erleben zu leugnen.",
            "Wenn dich eine Situation überwältigt und vernünftige Argumente nicht helfen.",
            "Beschreibe die Situation und die Gefühle und schreib sie dann in der dritten Person neu — neutral, ohne Urteil.",
            "Streit: von innen — „man respektiert mich nicht“. Beobachter: „zwei Menschen sprechen laut“. Anspannung: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "Übung", "Ressourcenanker",
        [
            "1. Erinnere dich an einen Moment, in dem du dich sicher oder ruhig gefühlt hast",
            "2. Beschreibe die Körperempfindungen",
            "3. Wähle eine einfache Ankergeste",
            "4. Bewerte die Stärke des Ressourcenzustands"
        ],
        "Ein Anker verbindet eine Körpergeste mit einem Ressourcenzustand, damit du schneller zu innerem Halt zurückfindest.",
        TechniqueUiKind.Entry, "Technik Nr. 13", "12.06.2026", "Ressourcenanker", "Halt durch Geste und Erinnerung zurückgewinnen", "Schwerer Moment", "NLP", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Ressourcenerinnerung", "Wann hast du dich stark, ruhig oder froh gefühlt?", Suggestions: ["Ein gelungener Vortrag", "Ein Urlaub am Meer", "Eine Umarmung von einem nahen Menschen", "Etwas Schweres geschafft", "Ein Spaziergang in der Natur", "Eine bestandene Prüfung", "Die Geburt eines Kindes", "Ein Sieg im Wettkampf", "Ein Abend mit Freunden", "Lob von meiner Chefin oder meinem Chef", "Der erste Tag in einem neuen Job", "Ein Morgen ohne Eile"]),
            new TechniqueEntrySeed("Körperempfindungen", "Wo spürst du es? Wärme, Leichtigkeit, Halt…", Suggestions: ["Wärme in der Brust", "Leichtigkeit", "Offene Schultern", "Gleichmäßiger Atem", "Feste Beine", "Ein Lächeln", "Ein ruhiges Herz", "Ein gerader Rücken", "Energie in den Händen", "Ein entspannter Bauch"]),
            new TechniqueEntrySeed("Geste oder Anker", "Z. B. Faust ballen, Handgelenk berühren", Suggestions: ["Eine Faust ballen", "Das Handgelenk berühren", "Daumen und Zeigefinger zusammenführen", "Hand aufs Herz", "Ein tiefer Atemzug", "Ein Ohrläppchen drücken", "Die Schultern öffnen", "Über die Handfläche streichen"]),
            new TechniqueEntrySeed("Stärke des Zustands", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Das Gehirn verknüpft eine Geste schnell mit einem Zustand, wenn du das Paar „Erinnerung + Geste“ mehrmals wiederholst.",
            "Vor einem stressigen Ereignis oder wenn dir innerer Halt fehlt.",
            "Erinnere dich an die Ressourcensituation, verstärke die Körperempfindungen und verankere sie mit einer einfachen Geste.",
            "Ressource: ein gelungener Vortrag. Körper: Wärme in der Brust. Anker: Daumenberührung. Stärke: 8."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "Übung", "Erdung 5-4-3-2-1",
        [
            "1. Nenne 5 Dinge, die du siehst",
            "2. Nenne 4 Dinge, die du mit dem Körper spürst",
            "3. Nenne 3 Geräusche um dich herum",
            "4. Nenne 2 Gerüche",
            "5. Nenne 1 Geschmack oder eine angenehme Empfindung"
        ],
        "Sinnliche Erdung lenkt die Aufmerksamkeit von ängstlichen Gedanken auf den gegenwärtigen Moment.",
        TechniqueUiKind.Entry, "Technik Nr. 14", "12.06.2026", "Erdung 5-4-3-2-1", "Über die Sinne in die Gegenwart zurückkehren", "Schwerer Moment", "Psyche", 3, "Nature",
        [
            new TechniqueEntrySeed("5 Dinge, die ich sehe", "Z. B. Fenster, Tasse, Buch, Lampe, Hand", Suggestions: ["Fenster", "Tisch", "Handy", "Computer", "Laptop", "Bildschirm", "Tastatur", "Tasse", "Lampe", "Hand", "Tür", "Buch", "Stuhl", "Wand", "Bild", "Pflanze", "Uhr", "Schrank", "Himmel", "Auto"]),
            new TechniqueEntrySeed("4 Dinge, die ich mit dem Körper spüre", "Z. B. Stuhl, Boden, Kleidung, Lufttemperatur", Suggestions: ["Stuhl", "Boden unter den Füßen", "Kleidung", "Kühle", "Wärme", "Handy in der Hand", "Stuhllehne", "Stoff auf der Haut", "Atem in der Brust", "Wind", "Computermaus", "Schlüssel", "Kissen", "Uhr am Handgelenk"]),
            new TechniqueEntrySeed("3 Geräusche", "Z. B. Straße, Uhr, dein Atem", Suggestions: ["Straße", "Uhr", "Atmen", "Stimmen", "Kühlschrank", "Wind", "Stille", "Computer", "Autos", "Vögel", "Musik", "Klimaanlage", "Schritte", "Regen"]),
            new TechniqueEntrySeed("2 Gerüche", "Z. B. Kaffee, frische Luft", Suggestions: ["Kaffee", "Frische Luft", "Seife", "Essen", "Parfüm", "Fast nichts", "Tee", "Brot", "Bücher", "Blumen", "Handcreme", "Holz"]),
            new TechniqueEntrySeed("1 Geschmack oder angenehme Empfindung", "Z. B. Wasser im Mund, weicher Stoff", Suggestions: ["Wasser", "Tee", "Weicher Stoff", "Warme Handflächen", "Frischer Mund", "Kaffee", "Minzkaugummi", "Schokolade", "Ein Apfel", "Eine warme Decke"])
        ],
        Theory(
            "Angst zieht dich oft in die Vergangenheit oder Zukunft. Das Zählen über die Sinne bringt dich zurück ins Hier und Jetzt.",
            "Bei Panik, starker Angst, Schwindel oder dem Gefühl, von der Wirklichkeit abgelöst zu sein.",
            "Geh die Schritte langsam durch und nenne konkrete Empfindungen. Atme zwischen den Punkten gleichmäßig.",
            "5 sehen → 4 spüren → 3 hören → der Atem wird ruhig, die Gedanken langsamer."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "Übung", "Box-Atmung",
        [
            "1. Bewerte die Anspannung vor der Übung",
            "2. Atme auf 4 Zählzeiten ein",
            "3. Halte 4 Zählzeiten",
            "4. Atme auf 4 Zählzeiten aus",
            "5. Halte 4 Zählzeiten — wiederhole 4 Zyklen und bewerte danach die Anspannung"
        ],
        "Rhythmisches Atmen beruhigt das Nervensystem und senkt die körperliche Anspannung.",
        TechniqueUiKind.Entry, "Technik Nr. 15", "04.07.2026", "Box-Atmung", "Den Körper mit dem Atemrhythmus beruhigen", "Schwerer Moment", "Psyche", 3, "Air",
        [
            new TechniqueEntrySeed("Anspannung vorher", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notizen", "Was hast du während der Übung in Körper oder Atem bemerkt?", Suggestions: ["Die Schultern sind gesunken", "Der Atem wurde langsamer", "Mir wurde wärmer", "Die Pause war schwer zu halten", "Gedanken haben mich abgelenkt", "Ich wurde ruhiger", "Mir wurde schwindelig", "Das Herz wurde langsamer", "Ich musste gähnen", "Nichts Besonderes"]),
            new TechniqueEntrySeed("Anspannung danach", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Langsames, rhythmisches Atmen senkt die Aktivität des Sympathikus und hilft dem Körper, aus dem Kampf-oder-Flucht-Modus herauszukommen.",
            "Bei Angst, Panik, Herzrasen, Schlaflosigkeit oder dem Gefühl, überfordert zu sein.",
            "Setz dich bequem hin und atme durch die Nase: 4 ein, 4 halten, 4 aus, 4 halten. Wiederhole 4 Zyklen.",
            "Angst 7/10 → nach 4 Zyklen 4/10. Die Schultern sinken, der Puls wird langsamer."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "Übung", "Ein kleiner Schritt",
        [
            "1. Beschreibe, was schwer wirkt oder dich am Anfangen hindert",
            "2. Wähle den kleinsten realistischen Schritt für heute",
            "3. Schreib auf, wann du ihn machst",
            "4. Bewerte deine Energie, nachdem du den Schritt aufgeschrieben hast"
        ],
        "Verhaltensaktivierung: Eine kleine Handlung gibt das Gefühl von Bewegung und Kontrolle zurück.",
        TechniqueUiKind.Entry, "Technik Nr. 16", "04.07.2026", "Ein kleiner Schritt", "Mit einer kleinen Handlung wieder in Schwung kommen", "Schwerer Moment", "KVT", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("Was schwer wirkt", "Was erscheint gerade schwer, langweilig oder unmöglich?", Suggestions: ["Müdigkeit", "Keine Energie", "Zu viel zu tun", "Langeweile", "Angst anzufangen", "Nichts scheint wichtig", "Schlecht geschlafen", "Keine Lust", "Einsamkeit", "Aufschieben", "Kopfschmerzen", "Ich weiß nicht, wo ich anfangen soll"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Kleinster Schritt heute", "Z. B. aufstehen, Gesicht waschen, eine Nachricht schreiben", Suggestions: ["Aus dem Bett aufstehen", "Mein Gesicht waschen", "Etwas Wasser trinken", "Eine Nachricht schreiben", "5 Minuten rausgehen", "Eine Sache aufräumen", "Ein Fenster öffnen", "2 Minuten dehnen", "Duschen", "Etwas essen", "Einen nahen Menschen anrufen", "Die Datei öffnen, die ich brauche", "Einen Plan für den Tag schreiben", "Eine Tasse spülen"]),
            new TechniqueEntrySeed("Wann du es machst", "Eine bestimmte Uhrzeit oder ein Moment am Tag", Suggestions: ["Jetzt sofort", "In 15 Minuten", "In einer Stunde", "Am Morgen", "Nach dem Frühstück", "Nach dem Mittagessen", "Am Abend", "Vor dem Schlafen", "Morgen früh", "Am Wochenende"]),
            new TechniqueEntrySeed("Energie danach", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "In der Antriebslosigkeit wartet das Gehirn auf Motivation, um zu handeln. Ein kleiner Schritt erzeugt Motivation durch Handeln — nicht umgekehrt.",
            "Wenn du keine Energie hast, alles sinnlos wirkt oder der Start in den Tag schwerfällt.",
            "Wähle einen Schritt von 2–5 Minuten, ohne perfekten Plan. Schreib ihn und die Uhrzeit auf — das ist schon Bewegung.",
            "Wohnung aufräumen → um 10:00 eine Tasse in die Küche bringen. Energie: 2 → 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "Übung", "Gedankenprotokoll",
        [
            "1. Beschreibe die Situation",
            "2. Schreib den automatischen Gedanken auf",
            "3. Bewerte das Gefühl",
            "4. Formuliere einen ausgewogeneren alternativen Gedanken",
            "5. Bewerte das Gefühl noch einmal"
        ],
        "Ein strukturiertes KVT-Protokoll hilft, den Zusammenhang zwischen Situation, Gedanke und Gefühl zu sehen.",
        TechniqueUiKind.Entry, "Technik Nr. 17", "04.07.2026", "Gedankenprotokoll", "Einen Gedanken auseinandernehmen und die emotionale Ladung senken", "Schwerer Moment", "KVT", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situation", "Was ist passiert? Wo und wann?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Automatischer Gedanke", "Was hast du in dem Moment gedacht?", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("Gefühl", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Alternativer Gedanke", "Was könnte sonst noch stimmen? Ein ausgewogenerer Blick", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("Gefühl jetzt", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Automatische Gedanken klingen oft wie Tatsachen, obwohl sie Deutungen sind. Aufgeschrieben werden sie sichtbar und überprüfbar.",
            "Nach einem Konflikt, Kritik, einem Misserfolg oder wenn ein Gedanke kreist.",
            "Beschreibe Situation und Gedanken, bewerte das Gefühl und finde dann eine Alternative — kein erzwungenes Positivdenken, sondern etwas Zutreffenderes.",
            "Situation: keine Antwort auf eine Nachricht. Gedanke: Niemand braucht mich. Alternative: Die Person ist beschäftigt. Gefühl: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "Übung", "Freundliche Worte an dich selbst",
        [
            "1. Schreib auf, wofür du dich kritisierst",
            "2. Stell dir vor, ein enger Freund sagt das — was würdest du ihm antworten?",
            "3. Formuliere denselben unterstützenden Satz für dich selbst",
            "4. Bewerte Wärme oder Erleichterung"
        ],
        "Selbstmitgefühl mildert harte Selbstkritik und hilft, sich nach einem Fehler zu erholen.",
        TechniqueUiKind.Entry, "Technik Nr. 18", "04.07.2026", "Freundliche Worte an dich selbst", "Selbstkritik durch Unterstützung abmildern", "Schwerer Moment", "KVT", 5, "Favorite",
        [
            new TechniqueEntrySeed("Wofür du dich kritisierst", "Was sagst du dir, oder was wirfst du dir vor?", Suggestions: ["Ein Fehler auf der Arbeit", "Nicht fertig geworden", "Einen nahen Menschen angefahren", "Mich mit anderen vergleichen", "Schon wieder etwas aufgeschoben", "Mein Aussehen", "Mein Gewicht", "Das Falsche gesagt", "Kein guter Elternteil", "Zu wenig verdienen", "Ein Versprechen gebrochen", "Zu empfindlich"]),
            new TechniqueEntrySeed("Was du einem Freund sagen würdest", "Wie würdest du einen nahen Menschen in dieser Situation unterstützen?", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("Satz an dich selbst", "Dieselbe Freundlichkeit, an dich gerichtet", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("Wärme / Erleichterung", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Selbstkritik motiviert selten — öfter nährt sie Scham und Lähmung. Selbstmitgefühl nimmt die Verantwortung nicht weg, gibt aber Halt.",
            "Nach einem Fehler, beim Vergleichen mit anderen oder wenn du dich nicht gut genug fühlst.",
            "Schreib die Kritik auf und antworte dir dann wie einem Freund — respektvoll und ohne abzuwiegeln.",
            "Kritik: Ich habe wieder alles kaputt gemacht. An einen Freund: Du hast es versucht, das lässt sich reparieren. An mich: Ich kann daraus lernen. Wärme: 3 → 6."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "Übung", "Wo sitzt es im Körper?",
        [
            "1. Bewerte deine Anspannung jetzt",
            "2. Lass deine Aufmerksamkeit langsam vom Kopf bis zu den Füßen durch den Körper wandern",
            "3. Finde die Stelle, an der die Empfindung am stärksten ist",
            "4. Beschreibe sie: wie sie ist, groß oder klein, warm oder kalt",
            "5. Benenne das Gefühl, das dazugehört",
            "6. Atme 5-mal langsam aus, als ginge jeder Atemzug durch diese Stelle, und bewerte die Anspannung erneut"
        ],
        "Body-Scan: bemerken, wo ein Gefühl sitzt, es benennen und den Körper weich werden lassen, ohne gegen die Empfindung anzukämpfen.",
        TechniqueUiKind.Entry, "Technik Nr. 19", "16.10.2026", "Wo sitzt es im Körper?", "Bemerken, wo ein Gefühl im Körper sitzt, und die Anspannung lösen", "Schwerer Moment", "Achtsamkeit", 4, "Accessibility",
        [
            new TechniqueEntrySeed("Anspannung jetzt", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Wo im Körper spürst du es?", "Zum Beispiel: Brust, Hals, Schultern", Suggestions: ["Kopf", "Stirn", "Augen", "Kiefer", "Hals", "Nacken", "Schultern", "Brust", "Herz", "Bauch", "Rücken", "Unterer Rücken", "Arme", "Hände", "Beine", "Der ganze Körper"]),
            new TechniqueEntrySeed("Wie fühlt sich die Empfindung an?", "Zum Beispiel: Enge, Schwere, ein Kloß", Suggestions: ["Enge", "Schwere", "Ein Kloß", "Druck", "Anspannung", "Hitze", "Kälte", "Zittern", "Kribbeln", "Pochen", "Leere", "Taubheit", "Schmerz", "Schwer zu beschreiben"]),
            new TechniqueEntrySeed("Welches Gefühl gehört dazu?", "Zum Beispiel: Angst, Kränkung, Müdigkeit", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Anspannung danach", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Gefühle hallen im Körper wider: Angst als Enge in der Brust, Kränkung als Kloß im Hals, Müdigkeit als schwere Schultern. Wenn wir die Empfindung bemerken und das Gefühl benennen, lässt die Anspannung meist nach. Das ist der Kern des Body-Scans in Achtsamkeitsprogrammen.",
            "Wenn du dich ängstlich oder schwer fühlst, aber nicht sagen kannst, was genau; wenn der Körper nach einem schweren Gespräch oder Tag angespannt ist. Die Übung erklärt keine Krankheiten und ersetzt keinen Arzt: Wenn eine Empfindung neu oder stark ist oder nicht vergeht, geh zu einer Ärztin oder einem Arzt.",
            "Setz dich bequem hin und lass die Aufmerksamkeit langsam vom Scheitel bis zu den Füßen wandern. Halte dort an, wo die Empfindung am deutlichsten ist. Verändere sie nicht und kämpfe nicht dagegen: Beschreibe sie und benenne das Gefühl. Atme dann ein paarmal langsam aus, als ginge jeder Atemzug durch diese Stelle.",
            "Anspannung 6. Wo: Brust. Empfindung: Enge, wie eine Faust. Gefühl: Angst vor einem Gespräch. Nach 5 Ausatmungen ist die Enge weicher, Anspannung 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "Übung", "Eine Erinnerung hat mich überwältigt",
        [
            "1. Bewerte, wie stark es ist",
            "2. Sag dir: „Das ist eine Erinnerung. Das war damals, und jetzt ist heute“",
            "3. Benenne, wo du bist und welche Tageszeit es ist",
            "4. Spür den Halt unter den Füßen, ball die Fäuste und lass sie wieder los",
            "5. Nenne drei Dinge, die du siehst",
            "6. Fünf langsame Ausatmungen, kaltes Wasser, und dann erneut bewerten"
        ],
        "Hilfe bei einem Flashback: über Zeit, Ort, Körper und Sinne aus der Erinnerung in die Gegenwart zurückkommen.",
        TechniqueUiKind.Entry, "Technik Nr. 20", "16.10.2026", "Eine Erinnerung hat mich überwältigt", "In die Gegenwart zurückkommen, wenn eine schwere Erinnerung überwältigt", "Schwerer Moment", "Trauma", 3, "MyLocation",
        [
            new TechniqueEntrySeed("Wie stark es ist", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Wo du bist und welche Tageszeit", "Zum Beispiel: zu Hause, abends", Suggestions: ["Zu Hause", "Auf der Arbeit", "Draußen", "Unterwegs in Bus oder Bahn", "Zu Besuch", "Morgen", "Nachmittag", "Abend", "Nacht"]),
            new TechniqueEntrySeed("Drei Dinge, die du siehst", "Zum Beispiel: Fenster, Tasse, Lampe", Suggestions: ["Fenster", "Tisch", "Handy", "Tasse", "Lampe", "Tür", "Stuhl", "Wand", "Bild", "Pflanze", "Uhr", "Himmel"]),
            new TechniqueEntrySeed("Wie stark es jetzt ist", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Ein Flashback ist eine Traumaerinnerung, die so erlebt wird, als geschähe es wieder: mit denselben Gefühlen und Körperreaktionen. Das Gehirn verwechselt „damals“ und „jetzt“. Zeit, Ort, der Halt des Körpers und das, was du siehst und hörst, helfen dir zurück in die Gegenwart.",
            "Wenn die Erinnerung an ein schweres Ereignis überwältigt, mit aufblitzenden Bildern und dem Gefühl „ich bin wieder dort“. Häufige Flashbacks sind ein typisches Zeichen einer posttraumatischen Reaktion und lassen sich mit einer Traumafachperson gut behandeln.",
            "Versuch nicht, die Erinnerung aufzuarbeiten: Jetzt ist die einzige Aufgabe, zurückzukommen. Sag dir, wo du bist und welcher Tag ist, drück die Füße auf den Boden, benenne, was du siehst, atme langsam aus, nutze Kälte.",
            "Stärke 8. „Das war damals; jetzt ist 2026, ich bin zu Hause, es ist Abend.“ Füße auf dem Boden; ich sehe ein Fenster, eine Tasse, eine Lampe. Kaltes Wasser. Jetzt 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "Übung", "Einen Albtraum umschreiben",
        [
            "1. Wähle einen wiederkehrenden Albtraum, für den Anfang nicht den schwersten",
            "2. Beschreibe ihn kurz, ohne belastende Details",
            "3. Entscheide, was du daran änderst: das Ende, den Ort, dich selbst, jemanden in der Nähe",
            "4. Schreib die neue Version des Traums vollständig auf, bis zu einem ruhigen Ende",
            "5. Stell dir die neue Version jeden Tag 10 bis 20 Minuten lang genau vor",
            "6. Schau nach ein bis zwei Wochen, wie sich die Albträume verändert haben"
        ],
        "Imagery Rehearsal: Ein wiederkehrender Albtraum wird im Wachzustand umgeschrieben, und das Gehirn beginnt, die neue Version zu träumen.",
        TechniqueUiKind.Entry, "Technik Nr. 21", "16.10.2026", "Einen Albtraum umschreiben", "Einen wiederkehrenden Albtraum seltener und schwächer machen", "Schlaf", "Trauma", 15, "NightsStay",
        [
            new TechniqueEntrySeed("Wie belastend dieser Albtraum ist", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Worum es im Albtraum geht, kurz", "Ein oder zwei Sätze, ohne belastende Details", Suggestions: ["Jemand verfolgt mich", "Ich komme nicht weg", "Ich bin wieder dort, wo es beängstigend war", "Ich verliere einen nahen Menschen", "Ich kann nicht schreien", "Ich falle", "Ich werde angegriffen", "Ich bin zu spät und schaffe es nicht"]),
            new TechniqueEntrySeed("Was du änderst", "Alles, was du willst: Es ist dein Traum", Suggestions: ["Das Ende", "Der Ort", "Ich werde stärker", "Jemand kommt zu Hilfe", "Ich kann sprechen und rufen", "Die Gefahr verschwindet", "Alles wird sicher", "Ich merke, dass es ein Traum ist"]),
            new TechniqueEntrySeed("Die neue Version des Traums", "Beschreibe sie vollständig, vom Anfang bis zu einem ruhigen Ende", Suggestions: ["Ich merke, dass es ein Traum ist", "Jemand, der mich beschützt, taucht auf", "Ich bin an einem sicheren Ort", "Ich gehe ruhig weg", "Alles geht gut aus"]),
            new TechniqueEntrySeed("Wann ich die neue Version übe", "Jeden Tag, 10 bis 20 Minuten", Suggestions: ["Am Morgen", "Tagsüber, in einer Pause", "Abends, ein paar Stunden vor dem Schlafen", "Jeden Tag zur selben Zeit"]),
            new TechniqueEntrySeed("Wie belastend es jetzt ist", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Wiederkehrende Albträume nach Trauma oder Stress werden durch Gewohnheit aufrechterhalten: Das Gehirn spielt immer wieder dieselbe Handlung ab. In der Imagery Rehearsal Therapy wird die Handlung im Wachzustand umgeschrieben und die neue Version jeden Tag im Kopf geübt. Studien zeigen, dass Albträume dadurch seltener und schwächer werden und der Schlaf besser wird.",
            "Bei wiederkehrenden Albträumen, die dir Angst vor dem Einschlafen machen. Wenn der Albtraum ein Trauma direkt wiederholt und schon eine kurze Beschreibung starkes Entsetzen auslöst, mach die Übung gemeinsam mit einer Traumafachperson.",
            "Beginne mit einem leichteren Albtraum. Schreib die beängstigenden Teile nicht ausführlich auf: Ein paar Sätze reichen. Verändere den Traum, wie du willst; die neue Version muss nicht realistisch sein. Übe sie tagsüber, nicht direkt vor dem Schlafen, 10 bis 20 Minuten am Tag.",
            "Albtraum: Jemand verfolgt mich und ich komme nicht weg. Änderung: Ich merke, dass es ein Traum ist, drehe mich um, und es ist nur ein Schatten; ich gehe ruhig nach Hause. Ich übe es jeden Nachmittag. Nach zwei Wochen kommt der Albtraum seltener."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Balloons = new(
        "Übung", "Ballons platzen lassen",
        [
            "1. Bewerte deine Anspannung",
            "2. Lass die Ballons mit einem Tippen platzen, ohne Eile",
            "3. Atme bei jedem Platzen langsam aus",
            "4. Spiel die Runde zu Ende",
            "5. Bewerte deine Anspannung danach"
        ],
        "Ein kurzes Spiel lenkt die Aufmerksamkeit von ängstlichen Gedanken auf eine einfache Handlung und Empfindungen.",
        TechniqueUiKind.Entry, "Technik Nr. 22", "26.10.2026", "Ballons platzen lassen", "Eine kurze Pause, um Anspannung rauszulassen", "Schwerer Moment", "Psyche", 3, "BubbleChart",
        [
            new TechniqueEntrySeed("Anspannung vorher", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notizen", "Was hat sich verändert, während du Ballons platzen ließt?", Suggestions: ["Ich fühle mich leichter", "Meine Gedanken haben Pause gemacht", "Ich musste lächeln", "Mein Atem wurde ruhiger", "Der Ärger ist verflogen", "Meine Hände haben sich entspannt", "Nichts hat sich verändert", "Ich möchte noch eine Runde"]),
            new TechniqueEntrySeed("Anspannung danach", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Wenn Angst oder Ärger am stärksten sind, lassen sich Gedanken mit Willenskraft kaum stoppen. Eine einfache Handlung, die Augen und Hände braucht, lenkt das Gehirn für ein paar Minuten um und lässt die Anspannung sinken. Das ist keine Behandlung, sondern ein Weg durch eine akute Minute.",
            "Wenn Ärger, Anspannung oder Angst überhandnehmen und du eine kurze Pause brauchst. Wenn das Spiel dir nur hilft, einem Problem auszuweichen, das sich zu lösen lohnt, komm darauf zurück, sobald du ruhiger bist.",
            "Lass dir Zeit: Du musst nicht jeden Ballon treffen. Atme bei jedem Platzen langsam aus. Die Runde endet von selbst; es gibt kein endloses Spiel.",
            "Anspannung 7/10 nach einem Streit. Eine Runde mit 20 Ballons, bei jedem Platzen ausatmen. Anspannung 4/10, und ich konnte in Ruhe überlegen, was ich antworte."),
        null, null);
}
