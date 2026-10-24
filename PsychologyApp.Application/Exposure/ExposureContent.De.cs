using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

public static partial class ExposureContent
{
    private static readonly string[] LessonsDe =
    [
        "Es war weniger schlimm, als ich erwartet hatte",
        "Ich habe es geschafft, auch mit Angst",
        "Niemand hat etwas bemerkt",
        "Die Angst stieg und fiel von selbst wieder",
        "Auch wenn es unangenehm ist, komme ich da durch",
        "Ich brauchte mein Sicherheitsnetz nicht"
    ];

    private static readonly ExposureKindContent[] KindsDe =
    [
        new(
            FearKind.Social,
            "Andere Menschen und ihr Urteil",
            "Sprechen vor anderen, telefonieren, Leute treffen, vor anderen essen",
            [
                "Alle werden sehen, dass ich nervös bin",
                "Ich sage etwas Dummes und werde ausgelacht",
                "Man wird schlecht über mich denken",
                "Ich werde rot und alle sehen es",
                "Ich habe einen Blackout und komme nicht weiter",
                "Ich werde abgelehnt"
            ],
            [
                "Telefonate",
                "Vorträge und Besprechungen",
                "Fremde kennenlernen und ansprechen",
                "Vor anderen essen oder schreiben",
                "Um Hilfe bitten oder etwas umtauschen",
                "Nein sagen"
            ],
            [
                "Einüben, was ich sagen werde",
                "Blickkontakt vermeiden",
                "Leise und kurz sprechen",
                "Neben jemandem bleiben, den ich kenne",
                "Hände oder Gesicht verbergen",
                "Früher gehen",
                "Aufs Handy schauen",
                "Mir mit Alkohol Mut antrinken"
            ],
            [
                new("Eine Verkäuferin fragen, wo etwas zu finden ist", 25),
                new("Einen Nachbarn zuerst grüßen", 30),
                new("Anrufen, um einen Arzttermin zu vereinbaren", 40),
                new("Einen Artikel im Geschäft zurückgeben", 50),
                new("In einer Besprechung oder im Kurs eine Frage stellen", 60),
                new("Ein Gespräch mit einem Fremden beginnen", 65),
                new("Die Bitte eines Kollegen ablehnen", 70),
                new("Einen kurzen Vortrag vor einer Gruppe halten", 85)
            ]),
        new(
            FearKind.Panic,
            "Panikattacken und Körperempfindungen",
            "Herzrasen, Atemnot, Schwindel",
            [
                "Ich bekomme einen Herzinfarkt",
                "Ich ersticke",
                "Ich werde ohnmächtig",
                "Ich werde verrückt oder verliere die Kontrolle",
                "Mir wird schlecht und niemand hilft",
                "Ich blamiere mich vor allen"
            ],
            [
                "Öffentliche Verkehrsmittel und die U-Bahn",
                "Warteschlangen und Menschenmengen",
                "Allein weit weg von zu Hause sein",
                "Sport und Treppensteigen",
                "Hitze und stickige Räume",
                "Kaffee"
            ],
            [
                "Wasser oder eine Tablette dabeihaben",
                "Meinen Puls prüfen",
                "In der Nähe des Ausgangs sitzen",
                "Nur in Begleitung hinausgehen",
                "Mich mit dem Handy ablenken",
                "Mich an einem Geländer oder einer Wand festhalten",
                "Auf meine Atmung achten",
                "Sofort gehen, wenn es anfängt"
            ],
            [
                new("Drei Stockwerke zügig hinaufsteigen", 30, ProvokesBodySensations: true),
                new("Eine Tasse Kaffee trinken", 35, ProvokesBodySensations: true),
                new("Im Geschäft an der Kasse anstehen", 40),
                new("Eine Station mit dem Bus fahren", 50),
                new("Allein 15 Minuten von zu Hause weggehen", 55),
                new("Drei U-Bahn-Stationen fahren, ohne Wasser in der Tasche", 70),
                new("Im Kino in der Mitte einer Reihe sitzen", 75),
                new("Im Berufsverkehr U-Bahn fahren", 85)
            ]),
        new(
            FearKind.Specific,
            "Eine bestimmte Angst",
            "Höhe, Fliegen, Hunde, Ärzte, Blut, Aufzüge",
            [
                "Es passiert etwas Schreckliches",
                "Ich halte die Angst nicht aus",
                "Ich verliere die Kontrolle",
                "Ich werde verletzt oder gebissen",
                "Ich falle",
                "Ich sitze fest und komme nicht heraus"
            ],
            [
                "Höhen und Balkone",
                "Aufzüge und enge Räume",
                "Tiere",
                "Ärzte, Spritzen, Blut",
                "Fliegen",
                "Brücken und Tunnel"
            ],
            [
                "Die Augen schließen oder wegschauen",
                "Mich an jemandem festhalten",
                "Nur in Begleitung hingehen",
                "Den Ausweg im Voraus planen",
                "Mich mit Musik ablenken",
                "Ein Beruhigungsmittel nehmen"
            ],
            [
                new("Fotos von dem ansehen, wovor ich Angst habe", 20),
                new("Ein Video darüber ansehen", 30),
                new("In sicherem Abstand in der Nähe stehen", 40),
                new("Für eine Minute näher herangehen", 55),
                new("5 Minuten ganz in der Nähe bleiben", 65),
                new("Es berühren oder hineingehen (Aufzug, Balkon)", 75),
                new("Es noch einmal allein tun, ohne Begleitung", 85),
                new("Es an einem unbekannten Ort tun", 90)
            ]),
        new(
            FearKind.Uncertainty,
            "Ungewissheit und „Was, wenn etwas passiert?“",
            "Sich sorgen, kontrollieren, Sicherheit suchen",
            [
                "Es passiert etwas Schlimmes, wenn ich nicht nachprüfe",
                "Ich ertrage es nicht, etwas nicht sicher zu wissen",
                "Einem geliebten Menschen stößt etwas zu",
                "Ich mache einen Fehler und es wird eine Katastrophe",
                "Ich werde schwer krank"
            ],
            [
                "Entscheidungen ohne vollständige Informationen",
                "Nachrichten senden, ohne sie noch einmal zu prüfen",
                "Nicht wissen, wo meine Angehörigen sind",
                "Neue und unvorhersehbare Situationen",
                "Über Krankheiten lesen"
            ],
            [
                "Mehrmals nachprüfen",
                "Im Internet suchen",
                "Andere fragen, ob alles in Ordnung ist",
                "Angehörige anrufen, um sicherzugehen",
                "Alles bis ins Detail planen",
                "Entscheidungen aufschieben"
            ],
            [
                new("Eine Nachricht nach einmaligem Lesen abschicken", 25),
                new("Im Café innerhalb einer Minute ein Gericht wählen", 30),
                new("Einen Angehörigen, der 30 Minuten zu spät ist, nicht anrufen", 50),
                new("Eine dienstliche E-Mail ohne erneutes Prüfen senden", 55),
                new("Einen Tag lang kein Symptom im Internet nachsehen", 60),
                new("Einen freien Tag ohne Plan verbringen", 65),
                new("Niemanden fragen, ob ich richtig entschieden habe", 70),
                new("Das Haus verlassen, nachdem ich die Tür nur einmal geprüft habe", 80)
            ]),
        new(
            FearKind.Other,
            "Etwas anderes",
            "Beschreibe deine Angst mit eigenen Worten",
            [
                "Es passiert etwas Schlimmes",
                "Ich schaffe es nicht",
                "Ich verliere die Kontrolle",
                "Man wird mich verurteilen"
            ],
            [],
            [
                "Die Situation meiden",
                "Nur in Begleitung hingehen",
                "Früher gehen",
                "Mich mit dem Handy ablenken",
                "Den Ausweg im Voraus planen"
            ],
            [])
    ];
}
