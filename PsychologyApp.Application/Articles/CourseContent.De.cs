using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public static partial class ArticleContent
{
    private static readonly Course[] CoursesDe =
    [
        new(
            CourseIds.Sleep,
            "Schlafen ohne Tabletten",
            "Sieben Tage kognitive Verhaltenstherapie bei Schlaflosigkeit, der am besten untersuchte Weg zurück zum Schlaf",
            "Bedtime",
            "isi",
            "ISI-7",
            [
                new(
                    "Wie Schlaf funktioniert",
                    4,
                    [
                        P("Schlaf lässt sich nicht durch Anstrengung „einschalten“. Zwei Kräfte bereiten ihn vor: der Schlafdruck, der sich den ganzen Tag über im Wachsein aufbaut, und die innere Uhr, die uns jeden Tag zu denselben Zeiten müde und wach macht."),
                        P("Bei Schlaflosigkeit geraten beide Kräfte durcheinander: Man geht früher ins Bett, um „nachzuholen“, liegt lange wach, schläft tagsüber, steht zu unterschiedlichen Zeiten auf. Der Schlafdruck hat keine Zeit, sich aufzubauen, und die Uhr kommt durcheinander."),
                        Cycle("Der Kreislauf der Schlaflosigkeit", "Eine schlechte Nacht", "Sorge: „Ich werde wieder nicht schlafen“", "Krampfhaft versuchen einzuschlafen", "Anspannung vertreibt den Schlaf"),
                        P("Dieser Kurs beruht auf der kognitiven Verhaltenstherapie bei Schlaflosigkeit (KVT-I). Medizinische Leitlinien empfehlen sie an erster Stelle, noch vor Schlaftabletten: Ihre Wirkung hält nach dem Kurs an."),
                        Warning("Wann du zuerst zum Arzt gehen solltest"),
                        List(
                            "Lautes Schnarchen, Atempausen im Schlaf, starke Tagesmüdigkeit",
                            "Unangenehme Empfindungen in den Beinen am Abend, die dich dazu bringen, dich zu bewegen",
                            "Schlaflosigkeit zusammen mit gedrückter Stimmung seit mehr als zwei Wochen",
                            "Du fährst Auto oder bedienst Maschinen und bist tagsüber sehr müde")
                    ],
                    "Notiere jeden Morgen: wann du ins Bett gegangen bist, ungefähr wann du eingeschlafen bist, wie oft du aufgewacht bist, wann du aufgestanden bist und wie du von 0 bis 10 geschlafen hast. Nachts keine Uhr, ungefähr, nach Gefühl.",
                    "Wann gehst du gewöhnlich ins Bett und stehst auf? Was stört deinen Schlaf am meisten?"),
                new(
                    "Das Bett ist zum Schlafen da",
                    4,
                    [
                        P("Wenn du oft wach im Bett liegst, dir dort Sorgen machst oder aufs Handy schaust, verbindet das Gehirn das Bett mit Wachsein. Deshalb kannst du im Sessel einnicken und hellwach sein, sobald du dich hinlegst."),
                        H("Regeln, die die Verbindung wiederherstellen"),
                        List(
                            "Geh nur ins Bett, wenn du schläfrig bist, nicht einfach müde",
                            "Wenn du etwa 20 Minuten nicht schlafen kannst, steh auf und tu bei gedämpftem Licht etwas Ruhiges. Komm zurück, wenn du schläfrig bist",
                            "Das Bett ist nur für Schlaf und Intimität: kein Handy, keine Serien, keine Arbeit",
                            "Steh jeden Tag zur gleichen Zeit auf, auch nach einer schlechten Nacht und am Wochenende",
                            "Mach keinen Mittagsschlaf, und wenn es wirklich sein muss, nicht länger als 20 Minuten und vor 15 Uhr"),
                        Quiz("Die Nacht war schlecht. Was ist morgens besser?", 1, "Eine feste Aufstehzeit ist der wichtigste Anker des Schlafs. Wenn du ausschläfst, ist der Schlafdruck am Abend geringer, und auch die nächste Nacht wird schwer.", "Ausschlafen, um sich zu erholen", "Zur gewohnten Zeit aufstehen", "Aufstehen und tagsüber einen ausgiebigen Mittagsschlaf machen")
                    ],
                    "Wähle eine Aufstehzeit, die du an allen sieben Tagen einhalten kannst, und steh zu dieser Zeit auf. Wenn du etwa 20 Minuten nicht schlafen kannst, steh auf.",
                    "Welche Aufstehzeit hast du gewählt? Was war an der Regel „aufstehen, wenn du nicht schlafen kannst“ schwer?"),
                new(
                    "Das Schlaffenster",
                    5,
                    [
                        P("Je länger man wach im Bett liegt, desto flacher und unterbrochener wird der Schlaf. Eine Technik namens Schlafkompression hilft: Die Zeit im Bett wird an die tatsächliche Schlafdauer angenähert."),
                        H("Wie du das Fenster berechnest"),
                        List(
                            "Berechne aus deinen Notizen der letzten Tage, wie lange du im Durchschnitt schläfst",
                            "Rechne 30 Minuten dazu: Das ist deine Zeit im Bett, aber nicht weniger als sechs Stunden",
                            "Zähle sie von deiner gewählten Aufstehzeit zurück: So ergibt sich deine Zubettgehzeit",
                            "Geh nicht vor dieser Zeit ins Bett, auch wenn du es sehr möchtest"),
                        P("In den ersten Tagen wirst du müde sein: Das heißt, der Schlafdruck baut sich auf. Wenn du nach einer Woche die meiste Zeit im Bett schläfst, kann das Fenster um 15 Minuten verlängert werden."),
                        Warning("Vorsicht"),
                        P("Komprimiere den Schlaf nicht, wenn du eine bipolare Störung oder Epilepsie hast, schwanger bist oder wenn Müdigkeit das Autofahren oder die Arbeit gefährlich macht. Besprich deinen Schlaf in diesen Fällen mit einer Ärztin oder einem Arzt.")
                    ],
                    "Berechne dein Schlaffenster und halte es bis zum Ende des Kurses ein: nicht vor deiner Zubettgehzeit ins Bett, zur gleichen Zeit aufstehen.",
                    "Wie sieht dein Schlaffenster aus? Wie lief die erste Nacht damit?"),
                new(
                    "Ein Abend, der auf den Schlaf vorbereitet",
                    4,
                    [
                        P("Schlaf kommt leichter, wenn Körper und Geist Zeit haben, langsamer zu werden. Die Stunde vor dem Schlafengehen ist die Brücke vom Tag zum Schlaf."),
                        H("Was hilft"),
                        List(
                            "Eine Stunde vor dem Schlafengehen: gedämpftes Licht und ruhige Tätigkeiten ohne Bildschirme",
                            "Kaffee, starker Tee und Energydrinks nicht später als 14 Uhr",
                            "Alkohol hilft beim Einschlafen, macht den Schlaf aber flach und weckt gegen Morgen",
                            "Ein schweres Abendessen 2–3 Stunden vor dem Schlafengehen",
                            "Morgens mindestens 15 Minuten Tageslicht: Es stellt die innere Uhr"),
                        H("Entspannen vor dem Schlafen"),
                        P("Langsames Atmen mit langer Ausatmung senkt die Anspannung. Probier es jetzt."),
                        Breathe("Auf 4 einatmen, auf 6 ausatmen")
                    ],
                    "Gestalte dein eigenes Ritual für die letzte Stunde vor dem Schlafengehen und mach es jeden Abend. Bring das Handy aus dem Schlafzimmer, oder zumindest aus dem Bett.",
                    "Was gehört zu deinem Abendritual? Was hat sich als schwer erwiesen?"),
                new(
                    "Gedanken, die wach halten",
                    4,
                    [
                        P("Der Kopf fängt oft gerade im Bett an zu arbeiten: Tagsüber war keine Zeit für Gedanken. Deshalb hilft es, ihnen früher Zeit zu geben."),
                        H("Zeit für Gedanken"),
                        List(
                            "Nimm dir abends, ein paar Stunden vor dem Schlafengehen, 15 Minuten",
                            "Schreib alles auf, was dich beschäftigt, und zu jedem Punkt einen nächsten Schritt oder „im Moment kann ich nichts tun“",
                            "Wenn der Gedanke im Bett zurückkommt, sag dir: „Das habe ich aufgeschrieben, darum kümmere ich mich morgen“"),
                        Quiz("Im Bett denkst du: „Wenn ich nicht schlafe, vermassle ich morgen alles.“ Was ist näher an der Wahrheit?", 1, "Der Gedanke übertreibt. Nach einer schlechten Nacht ist es schwerer, aber die meisten Menschen erledigen ihre Aufgaben trotzdem. Je stärker die Angst vor morgen, desto schwerer ist das Einschlafen.", "Es stimmt, das Scheitern ist unvermeidlich", "Nach einer schlechten Nacht ist es schwerer, aber das Wichtigste schaffe ich meistens", "Besser gar nicht ins Bett gehen"),
                        Practice(TechniqueId.Observer, "Beobachterposition")
                    ],
                    "Nimm dir jeden Abend 15 Minuten, um aufzuschreiben, was dich beschäftigt, und nimm diese Gedanken nicht mit ins Bett.",
                    "Welche Gedanken kommen nachts am häufigsten? Hat das Aufschreiben geholfen?"),
                new(
                    "Aufhören, gegen den Schlaf zu kämpfen",
                    4,
                    [
                        P("Je mehr du versuchst zu schlafen, desto schlechter klappt es. Der Schlaf ist wie ein scheuer Vogel: Er kommt, wenn niemand ihn jagt."),
                        H("Was hilft, die Kontrolle loszulassen"),
                        List(
                            "Stell die Uhr außer Sichtweite: „Noch fünf Stunden“ zu zählen macht wach",
                            "Erlaube dir, nicht zu schlafen: „Ich liege hier und ruhe mich aus, der Schlaf kommt, wann er kommt“",
                            "Eine schlechte Nacht ist keine Katastrophe: Der Körper erholt sich auch nach unvollständigem Schlaf",
                            "Sag wegen einer schlechten Nacht keine Pläne ab: So bestimmt die Schlaflosigkeit nicht dein Leben"),
                        P("Vergleiche deine Notizen der ersten Tage mit jetzt: Wie haben sich die Einschlafzeit, die Zahl der Aufwachphasen und die Bewertung des Schlafs verändert?")
                    ],
                    "Halte die Uhr nachts außer Sichtweite. Lebe den Tag nach einer schlechten Nacht wie gewohnt.",
                    "Was hat sich laut deinen Notizen in diesen Tagen an deinem Schlaf verändert?"),
                new(
                    "Bilanz und Plan",
                    4,
                    [
                        P("In einer Woche hast du den Kern der KVT bei Schlaflosigkeit durchlaufen. Der Schlaf erholt sich allmählich: Viele Menschen bemerken in zwei bis vier Wochen eine Besserung, wenn sie sich an die Regeln halten."),
                        H("Was du beibehalten solltest"),
                        List(
                            "Eine Aufstehzeit an jedem Tag",
                            "Nur schläfrig ins Bett gehen und aufstehen, wenn du nicht schlafen kannst",
                            "Wenn du den größten Teil der Nacht schläfst, das Fenster um 15 Minuten pro Woche verlängern",
                            "Das Abendritual und die Zeit für Gedanken"),
                        H("Wenn die Schlaflosigkeit zurückkommt"),
                        P("Nach Stress, Krankheit oder Reisen kann der Schlaf wieder aus dem Takt geraten. Das ist normal: Kehr zu den Regeln dieses Kurses zurück, und er erholt sich schneller als beim ersten Mal."),
                        P("Mach den ISI-7-Index noch einmal und vergleiche ihn mit deinem Ergebnis vom Anfang. Wenn es keine Besserung gibt, geh zu einer Ärztin oder einem Arzt: Es gibt Schlafspezialisten, die KVT-I anbieten."),
                        TestsTab("ISI-7-Index", "isi")
                    ],
                    "Schreib deinen Plan auf, falls der Schlaf wieder aus dem Takt gerät.",
                    "Was hat dir im Kurs am meisten geholfen? Was wirst du beibehalten?")
            ]),
        new(
            CourseIds.Anxiety,
            "Angst im Griff",
            "Sechs Tage: verstehen, wie Angst funktioniert, den Körper beruhigen, beängstigende Gedanken prüfen und einen ersten Schritt auf die Angst zu machen",
            "Psychology",
            "gad7",
            "GAD-7",
            [
                new(
                    "Wie Angst funktioniert",
                    4,
                    [
                        P("Angst ist ein Alarm, der vor Gefahr warnen soll. Schwierig wird es, wenn er zu oft, zu laut losgeht und lange nicht verstummt."),
                        Cycle("Der Angstkreislauf", "Ein beängstigender Gedanke", "Angst im Körper", "Vermeiden oder ein Sicherheitsnetz", "Erleichterung für einen Moment", "Das Gehirn lernt: „Es war gefährlich“"),
                        P("Die Angst wird nicht von der Furcht selbst am Leben gehalten, sondern von dem, was wir tun, um sie nicht zu spüren: meiden, nachprüfen, uns rückversichern, Gedanken wälzen. Schritt für Schritt bearbeitet der Kurs jedes Glied dieses Kreislaufs."),
                        P("Zu Beginn des Kurses lohnt es sich, die Skala GAD-7 auszufüllen: Am Ende vergleichst du das Ergebnis."),
                        TestsTab("Skala GAD-7", "gad7")
                    ],
                    "Achte heute dreimal, wenn du Angst spürst, darauf: was passiert ist, was der Gedanke war, was der Körper gespürt hat und was du getan hast.",
                    "Wann und wo zeigt sich die Angst am häufigsten?"),
                new(
                    "Den Körper beruhigen",
                    4,
                    [
                        P("Wenn die Angst stark ist, kann man schwer mit Gedanken streiten. Zuerst hilft es, den Körper zu beruhigen: Eine langsame Ausatmung aktiviert das System, das die Kampf-oder-Flucht-Reaktion bremst."),
                        Breathe("Auf 4 einatmen, auf 6 ausatmen"),
                        P("Wenn die Angst stark ist und sich alles unwirklich anfühlt, hol deine Aufmerksamkeit über die Sinne in die Gegenwart zurück."),
                        Practice(TechniqueId.Grounding, "Erdung 5-4-3-2-1"),
                        P("Wichtig: Atmen ist kein Mittel, die Angst um jeden Preis loszuwerden. Es hilft, den Höhepunkt zu überstehen, nicht, Situationen zu meiden.")
                    ],
                    "Zweimal am Tag zwei Minuten langsam atmen, auch wenn du ruhig bist: So ist die Fähigkeit da, wenn du sie brauchst.",
                    "Was hast du in deinem Körper bemerkt, als du langsam geatmet hast?"),
                new(
                    "Einen beängstigenden Gedanken prüfen",
                    5,
                    [
                        P("Ängstliche Gedanken klingen wie Tatsachen: „Ich blamiere mich“, „es passiert etwas Schreckliches“. Aber es sind Vorhersagen, und man kann sie prüfen."),
                        H("Fragen an den Gedanken"),
                        List(
                            "Welche Fakten sprechen für diesen Gedanken, und welche dagegen?",
                            "Was ist in ähnlichen Fällen früher passiert?",
                            "Was ist am wahrscheinlichsten, und nicht am beängstigendsten?",
                            "Wenn das Schlimmste eintritt, wie werde ich damit fertig?",
                            "Was würde ich einem Freund mit diesem Gedanken sagen?"),
                        Quiz("„Ich werde es bestimmt vermasseln.“ Welche Denkfalle ist das?", 1, "Das ist Wahrsagen: Der Gedanke behandelt etwas, das noch nicht passiert ist, als Tatsache.", "Gedankenlesen", "Wahrsagen", "Das Positive abwerten"),
                        Practice(TechniqueId.ThoughtRecord, "Gedankenprotokoll")
                    ],
                    "Prüfe heute einmal einen ängstlichen Gedanken mit einem Gedankenprotokoll.",
                    "Welchen Gedanken hast du geprüft? Wie sehr hast du ihm vorher und nachher geglaubt?"),
                new(
                    "Sorgenzeit",
                    4,
                    [
                        P("Sich Sorgen zu machen fühlt sich nützlich an: „Wenn ich mehr nachdenke, bin ich vorbereitet.“ Tatsächlich führt Grübeln selten zu einer Lösung und hält die Angst hoch."),
                        Quiz("Wie unterscheidest du Lösen vom Kreisen?", 0, "Lösen bewegt sich auf einen Schritt zu: Was kann ich tun, und wann. Kreisen bringt neue „Was, wenn“ hervor.", "Lösen endet mit einem Schritt, Kreisen mit einem neuen „Was, wenn“", "Kreisen dauert länger", "Gar nicht"),
                        H("Wie du Sorgen verschiebst"),
                        List(
                            "Wähle 15–20 Minuten am Tag, nicht vor dem Schlafengehen",
                            "Wenn eine Sorge zu einer anderen Zeit kommt, schreib das Thema in einer Zeile auf und sag dir: „Darüber denke ich in meiner Sorgenzeit nach“",
                            "Geh in deiner Sorgenzeit die Liste durch: Plane einen Schritt für das, was du tun kannst, lass los, was du nicht tun kannst",
                            "Oft wirkt das Thema zur vereinbarten Zeit nicht mehr wichtig"),
                        Practice(TechniqueId.Observer, "Beobachterposition")
                    ],
                    "Leg dir eine Sorgenzeit fest und verschiebe ängstliche Gedanken auf sie.",
                    "Ist es dir gelungen, Sorgen zu verschieben? Was ist mit den Themen bis zur vereinbarten Zeit passiert?"),
                new(
                    "Sicherheitsnetze und Vermeidung",
                    4,
                    [
                        P("Am stärksten hält die Vermeidung die Angst am Leben. Sie kann offen sein (ich gehe nicht hin, ich rufe nicht an) oder versteckt: Sicherheitsnetze, mit denen man eine Situation übersteht."),
                        Check("Habe ich Sicherheitsnetze?", 2, "Du hast mehrere Sicherheitsnetze. Sie halten die Angst am Leben: Das Gehirn glaubt, alles sei nur dank ihnen gut gegangen. Versuch, in einer leichten Situation auf eines zu verzichten.", "Du hast wenige Sicherheitsnetze. Achte auch darauf, was du ganz meidest.", "Ich habe „für alle Fälle“ eine Tablette, Wasser oder ein geladenes Handy dabei", "Ich gehe nur in Begleitung an schwierige Orte", "Ich setze mich in die Nähe des Ausgangs", "Ich lenke mich ab, um die Angst nicht zu bemerken", "Ich prüfe oder frage viele Male nach"),
                        P("Der Ausweg ist, dem Gefürchteten schrittweise zu begegnen und auf Sicherheitsnetze zu verzichten. Dann lernt das Gehirn aus Erfahrung, dass das Befürchtete nicht eintritt oder erträglich ist.")
                    ],
                    "Mach eine Liste von drei bis fünf Situationen, die du meidest, von der leichtesten bis zur schwierigsten.",
                    "Welche Situation von der Liste bist du bereit, als Erstes zu versuchen?"),
                new(
                    "Der erste Schritt und ein Plan",
                    4,
                    [
                        P("Der wirksamste Weg, Angst zu schwächen, ist die Exposition: der Angst schrittweise begegnen. Du musst nicht gleich zum Beängstigendsten gehen: Fang mit einem leichten Schritt an und prüfe eine Vorhersage."),
                        List(
                            "Schreib vor dem Schritt auf, was du befürchtest und wie sicher du bist, dass es eintritt",
                            "Mach den Schritt ohne Sicherheitsnetze und bleib in der Situation, bis du die Vorhersage geprüft hast",
                            "Schreib danach auf, was wirklich passiert ist"),
                        Ladder("Angstleiter"),
                        H("Was du beibehalten solltest"),
                        List(
                            "Langsames Atmen auf dem Höhepunkt der Angst",
                            "Gedanken prüfen und die Sorgenzeit",
                            "Alle paar Tage Schritte auf der Angstleiter"),
                        P("Füll die Skala GAD-7 noch einmal aus und vergleiche mit dem Anfang. Wenn die Angst stark ist oder dein Leben stört, wende dich an eine Psychotherapeutin: KVT bei Angst ist gut untersucht."),
                        TestsTab("Skala GAD-7", "gad7")
                    ],
                    "Mach den ersten Schritt auf deiner Angstleiter und schreib auf, was passiert ist.",
                    "Was hattest du vorhergesagt, und was ist wirklich passiert?")
            ]),
        new(
            CourseIds.Mood,
            "Raus aus der Antriebslosigkeit",
            "Sechs Tage Verhaltensaktivierung: kleine Schritte, um Kraft und das, was Freude macht, zurückzuholen",
            "Cloud",
            "phq9",
            "PHQ-9",
            [
                new(
                    "Der Kreislauf der Untätigkeit",
                    4,
                    [
                        P("Wenn die Stimmung bei null ist, ist es natürlich, weniger zu tun: keine Kraft, nichts reizt. Aber je weniger wir tun, desto weniger gibt es im Leben, was Freude und das Gefühl „ich habe es geschafft“ bringt, und die Stimmung sinkt noch tiefer."),
                        Cycle("Der Kreislauf der Untätigkeit", "Keine Kraft", "Ich tue weniger", "Weniger Freude und Erfolge", "Noch weniger Kraft"),
                        P("Verhaltensaktivierung durchbricht diesen Kreislauf von der anderen Seite: nicht warten, bis du Lust hast, sondern kleine Schritte machen, nach denen Kraft und Lust allmählich zurückkehren. Sie ist eine der am besten untersuchten Hilfen bei Depression."),
                        P("Füll zu Beginn des Kurses die Skala PHQ-9 aus: Am Ende vergleichst du das Ergebnis."),
                        TestsTab("Skala PHQ-9", "phq9"),
                        Warning("Wenn Gedanken an den Tod auftauchen"),
                        P("Wenn du Gedanken hast, dass es besser wäre, wenn es dich nicht gäbe, bleib damit nicht allein: Dieser Kurs ersetzt nicht die Hilfe einer Fachperson."),
                        Crisis("Ich brauche jetzt Hilfe")
                    ],
                    "Achte heute und morgen darauf, was du tust, und bewerte deine Stimmung mehrmals am Tag von 0 bis 10.",
                    "Wie verlaufen deine Tage gerade? Was hast du in letzter Zeit aufgehört zu tun?"),
                new(
                    "Was die Stimmung hebt",
                    4,
                    [
                        P("Schau dir deine Beobachtungen an: In welchen Momenten war die Stimmung etwas besser? Oft ist es etwas Kleines: ein Gespräch, ein Spaziergang, eine Dusche, eine erledigte Aufgabe."),
                        H("Zwei Arten nützlicher Tätigkeiten"),
                        List(
                            "Angenehme: was Freude bringt, auch wenn sie schwach ist — Musik, gutes Essen, Sonne",
                            "„Ich habe es geschafft“-Tätigkeiten: was ein Gefühl des Erreichten hinterlässt — abwaschen, eine E-Mail beantworten"),
                        P("Bei Depression ist die Freude an angenehmen Dingen schwächer als früher. Das ist normal: Wichtig ist, es zu tun, und nicht auf die alte Freude zu warten.")
                    ],
                    "Mach eine Liste mit zehn Tätigkeiten: fünf angenehmen und fünf „Ich habe es geschafft“. Je kleiner und einfacher, desto besser.",
                    "Was steht auf deiner Liste? Gab es etwas, das deine Stimmung ein wenig gehoben hat?"),
                new(
                    "Kleine Schritte",
                    4,
                    [
                        P("Große Pläne schrecken in der Antriebslosigkeit ab und werden nicht umgesetzt. Es hilft, Dinge zu zerlegen: nicht „die Wohnung putzen“, sondern „drei Sachen vom Tisch räumen“."),
                        List(
                            "Wähle für heute eine Tätigkeit von der Liste",
                            "Zerlege sie bis zu einem Schritt, der fünf Minuten dauert",
                            "Leg Zeit und Ort fest: „nach dem Frühstück, in der Küche“",
                            "Notiere danach, wie sich deine Stimmung verändert hat"),
                        Practice(TechniqueId.SmallStep, "Ein kleiner Schritt")
                    ],
                    "Mach jeden Tag zur festgelegten Zeit eine angenehme Tätigkeit und eine „Ich habe es geschafft“-Tätigkeit.",
                    "Was hast du geschafft? Wie hat sich deine Stimmung danach verändert?"),
                new(
                    "„Ich habe keine Lust“",
                    4,
                    [
                        P("Das Haupthindernis ist der Gedanke „Ich habe keine Lust, ich mache es, wenn ich Kraft habe“. In der Antriebslosigkeit kommt die Lust nach dem Handeln, nicht davor."),
                        Quiz("Keine Kraft, und für heute ist ein Spaziergang geplant. Was ist besser?", 1, "In der Antriebslosigkeit kommt die Lust nach dem Handeln zurück. Selbst fünf Minuten geben Körper und Stimmung das Signal „ich habe etwas getan“.", "Warten, bis ich Lust habe", "Mindestens fünf Minuten hinausgehen", "Aufs Wochenende verschieben"),
                        H("Andere bremsende Gedanken"),
                        List(
                            "„Es bringt nichts“: Prüf hinterher — ist irgendetwas auch nur ein bisschen anders?",
                            "„Das ist zu wenig“: Genau aus kleinen Schritten besteht der Weg hinaus",
                            "„Früher habe ich mehr geschafft“: Vergleich dich mit gestern, nicht mit deinem früheren Ich"),
                        Practice(TechniqueId.SelfCompassion, "Freundliche Worte an dich selbst")
                    ],
                    "Wenn du keine Lust hast, mach wenigstens eine Fünf-Minuten-Version dessen, was du geplant hast.",
                    "Welcher Gedanke hält dich am häufigsten vom Anfangen ab? Was hat dir geholfen, ihn zu umgehen?"),
                new(
                    "Die Menschen um dich herum",
                    4,
                    [
                        P("In der Antriebslosigkeit möchte man sich von allen abschotten. Doch der Kontakt mit Menschen ist eines der stärksten Mittel gegen gedrückte Stimmung, auch wenn er gerade nicht so viel Freude macht wie früher."),
                        List(
                            "Schreib einem Menschen, mit dem du eine Weile keinen Kontakt hattest",
                            "Ein kurzes Treffen oder ein Anruf ist besser als keines",
                            "Du musst nicht alles erklären: „Es ist gerade schwer, lass uns einfach spazieren gehen“",
                            "Wenn die Kraft fehlt, wähle Menschen, mit denen es leicht ist"),
                        Practice(TechniqueId.SmallStep, "Ein kleiner Schritt")
                    ],
                    "Melde dich in den nächsten zwei Tagen bei mindestens einem Menschen.",
                    "Bei wem hast du dich gemeldet? Wie war es?"),
                new(
                    "Was mir wichtig ist",
                    4,
                    [
                        P("Damit die Schritte nicht zur Pflicht werden, hilft es, sie mit dem zu verbinden, was dir wichtig ist: nahestehende Menschen, Gesundheit, Arbeit, Kreativität, Natur."),
                        List(
                            "Wähle einen Bereich, der dir wichtig ist",
                            "Überleg, wer du in diesem Bereich sein möchtest",
                            "Wähle für die Woche einen kleinen Schritt in diese Richtung"),
                        H("Was du beibehalten solltest"),
                        List(
                            "Jeden Tag eine angenehme und eine „Ich habe es geschafft“-Tätigkeit",
                            "Kleine Schritte statt großer Pläne",
                            "Kontakt mit Menschen"),
                        P("Füll die Skala PHQ-9 noch einmal aus und vergleiche mit dem Anfang. Wenn das Ergebnis hoch ist oder sich nicht verändert, wende dich an eine Ärztin oder Psychotherapeutin: Depression lässt sich sehr gut behandeln."),
                        TestsTab("Skala PHQ-9", "phq9"),
                        Crisis("Ich brauche jetzt Hilfe")
                    ],
                    "Mach diese Woche einen Schritt in einem Bereich, der dir wichtig ist.",
                    "Was ist dir wichtig? Welchen Schritt hast du gewählt?")
            ]),
        new(
            CourseIds.Trauma,
            "Nach einem Trauma",
            "Fünf Tage über das, was man sicher allein tun kann: die eigene Reaktion verstehen, mit Flashbacks und Albträumen zurechtkommen, eine Fachperson finden",
            "Healing",
            "pcl5",
            "PCL-5",
            [
                new(
                    "Eine normale Reaktion auf ein unnormales Ereignis",
                    4,
                    [
                        P("Nach einem schweren Ereignis haben fast alle aufdringliche Erinnerungen, Albträume, Angst, Taubheit, Reizbarkeit. Das ist keine Schwäche, sondern die Reaktion eines Gehirns, das dich davor schützen will, dass es wieder passiert."),
                        P("Dieser Kurs verarbeitet das Trauma nicht: Das braucht eine Therapie mit einer Fachperson. Er hilft dir zu verstehen, was mit dir passiert, mit schweren Momenten zurechtzukommen und vorbereitet zu einer Fachperson zu gehen. Du musst dich in diesem Kurs nicht an das Ereignis selbst erinnern oder es beschreiben."),
                        P("Füll zu Beginn die Skala PCL-5 aus: Am Ende vergleichst du das Ergebnis."),
                        TestsTab("Skala PCL-5", "pcl5"),
                        Warning("Wenn die Gefahr andauert"),
                        P("Wenn du gerade bedroht wirst oder Gedanken aufgetaucht sind, dir etwas anzutun, kommt die Sicherheit zuerst."),
                        Crisis("Ich brauche jetzt Hilfe")
                    ],
                    "Achte darauf, wann es im Laufe des Tages schwerer wird, ohne Details des Ereignisses: die Zeit, der Ort, was um dich herum war.",
                    "Welche dieser Reaktionen erkennst du bei dir?"),
                new(
                    "In die Gegenwart zurückkommen",
                    4,
                    [
                        P("Ein Flashback ist eine Erinnerung, die erlebt wird, als geschähe sie jetzt. Zeit, Ort, der Halt des Körpers und die Sinne helfen zurückzukommen."),
                        Practice(TechniqueId.Flashback, "Eine Erinnerung hat mich überwältigt"),
                        Practice(TechniqueId.Grounding, "Erdung 5-4-3-2-1"),
                        H("Dein eigenes Erdungsset"),
                        List(
                            "Ein Satz: „Das ist eine Erinnerung. Jetzt ist eine andere Zeit, ich bin sicher“",
                            "Etwas, das du bei dir trägst: ein Kiesel, ein Ring, etwas mit einem Duft",
                            "Ein Mensch, dem du schreiben kannst",
                            "Etwas Kaltes: Wasser, Eis"),
                        Breathe("Die Ausatmung länger als die Einatmung")
                    ],
                    "Stell dein Erdungsset zusammen und probier die Übung „Eine Erinnerung hat mich überwältigt“ in einem ruhigen Moment aus, damit du sie vorher kennst.",
                    "Was gehört zu deinem Erdungsset?"),
                new(
                    "Schlaf und Albträume",
                    4,
                    [
                        P("Nach einem Trauma leidet der Schlaf oft: Das Einschlafen fällt schwer, der Schlaf ist flach, Albträume kommen. Schlafmangel macht die Tage ängstlicher und gereizter."),
                        List(
                            "Eine Aufstehzeit an jedem Tag",
                            "Licht am Morgen, ein ruhiger Abend ohne Nachrichten",
                            "Wenn dich ein Albtraum weckt: Licht, wo bin ich und welcher Tag ist heute, Wasser, langsame Ausatmungen",
                            "Alkohol verstärkt Albträume in der zweiten Nachthälfte"),
                        P("Ein wiederkehrender Albtraum lässt sich umschreiben: Imagery Rehearsal macht Albträume seltener und schwächer. Fang mit einem leichteren an."),
                        Practice(TechniqueId.Nightmare, "Einen Albtraum umschreiben")
                    ],
                    "Halte eine feste Aufstehzeit ein. Wenn du wiederkehrende Albträume hast, versuch einen davon umzuschreiben, angefangen mit einem leichteren.",
                    "Wie schläfst du gerade? Was hilft nach einer schweren Nacht?"),
                new(
                    "Unterstützung und Menschen",
                    4,
                    [
                        P("Ein Trauma entfernt Menschen oft von anderen und von dem, was früher wichtig war. Die Unterstützung nahestehender Menschen ist einer der stärksten Faktoren der Erholung."),
                        List(
                            "Du musst nicht über das Ereignis sprechen: Ihr könnt einfach zusammen sein",
                            "Sag deinen Angehörigen, wie sie dir in einem schweren Moment helfen können",
                            "Hol allmählich zurück, was dir Halt gab: Spaziergänge, Bewegung, Dinge, die du gern gemacht hast",
                            "Betäube Gefühle nicht mit Alkohol: Er behindert die Erholung"),
                        P("Erinnerungsreize zu meiden ist verständlich, aber mit der Zeit engt es das Leben ein und hält das Trauma „heiß“. Daran arbeitest du besser mit einer Fachperson, als dich allein zu zwingen."),
                        P("Was das Ereignis an deinem Denken über dich verändert hat — Schuld, „mit mir stimmt etwas nicht“, „man kann niemandem trauen“ —, lässt sich im Chat bearbeiten, ohne zu erzählen, was passiert ist."),
                        TraumaChat("Im Chat daran arbeiten"),
                        Practice(TechniqueId.SelfCompassion, "Freundliche Worte an dich selbst")
                    ],
                    "Tu eine Sache, die dir früher Halt gegeben hat, und verbring Zeit mit jemandem, dem du vertraust.",
                    "Wer und was gibt dir gerade Halt?"),
                new(
                    "Der Weg zu einer Fachperson",
                    5,
                    [
                        P("Posttraumatische Symptome sprechen gut auf traumafokussierte Psychotherapie an: prolongierte Exposition, kognitive Therapie, EMDR, schriftliche Exposition. Sie braucht meist 8–16 Sitzungen."),
                        See(ArticleIds.TraumaTreatment, "Wie Trauma behandelt wird und wie du eine Fachperson auswählst"),
                        H("Wie du dich vorbereitest"),
                        List(
                            "Füll die Skala PCL-5 noch einmal aus und vergleiche mit dem Anfang",
                            "Exportiere eine Zusammenfassung aus der App für die Fachperson: „Profil → Optionen → Daten und Sicherung“",
                            "Schreib deine Fragen auf: welche Methode, wie viele Sitzungen, wie der Fortschritt gemessen wird",
                            "Du musst beim ersten Treffen nicht alles über das Ereignis erzählen"),
                        TestsTab("Skala PCL-5", "pcl5"),
                        Crisis("Ich brauche jetzt Hilfe")
                    ],
                    "Such dir ein oder zwei Fachpersonen, die mit Trauma arbeiten, und vereinbare einen Beratungstermin.",
                    "Was war im Kurs am nützlichsten? Worüber möchtest du mit der Fachperson sprechen?")
            ])
    ];
}
