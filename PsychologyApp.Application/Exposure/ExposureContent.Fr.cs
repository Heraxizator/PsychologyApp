using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

public static partial class ExposureContent
{
    private static readonly string[] LessonsFr =
    [
        "C'était moins effrayant que prévu",
        "J'ai fait face, même avec de l'anxiété",
        "Personne n'a rien remarqué",
        "L'anxiété est montée puis redescendue d'elle-même",
        "Même si c'est gênant, je peux le traverser",
        "Je n'ai pas eu besoin de ma béquille"
    ];

    private static readonly ExposureKindContent[] KindsFr =
    [
        new(
            FearKind.Social,
            "Les autres et leur jugement",
            "Prendre la parole, téléphoner, rencontrer des gens, manger devant d'autres",
            [
                "Tout le monde verra que je suis nerveux",
                "Je dirai une bêtise et on se moquera de moi",
                "On pensera du mal de moi",
                "Je vais rougir et tout le monde le verra",
                "Je vais avoir un trou et ne pourrai pas continuer",
                "On va me rejeter"
            ],
            [
                "Les appels téléphoniques",
                "Les présentations et les réunions",
                "Rencontrer des inconnus et leur parler",
                "Manger ou écrire devant d'autres",
                "Demander de l'aide ou rapporter un achat",
                "Dire non"
            ],
            [
                "Répéter ce que je vais dire",
                "Éviter le regard des autres",
                "Parler bas et brièvement",
                "Rester à côté de quelqu'un que je connais",
                "Cacher mes mains ou mon visage",
                "Partir plus tôt",
                "Regarder mon téléphone",
                "Boire de l'alcool pour me donner du courage"
            ],
            [
                new("Demander à un vendeur où trouver un article", 25),
                new("Saluer un voisin le premier", 30),
                new("Appeler pour prendre rendez-vous chez le médecin", 40),
                new("Rapporter un article en magasin", 50),
                new("Poser une question en réunion ou en cours", 60),
                new("Engager la conversation avec un inconnu", 65),
                new("Refuser la demande d'un collègue", 70),
                new("Faire un court exposé devant un groupe", 85)
            ]),
        new(
            FearKind.Panic,
            "Crises de panique et sensations du corps",
            "Cœur qui s'emballe, souffle court, vertiges",
            [
                "Je vais faire une crise cardiaque",
                "Je vais étouffer",
                "Je vais m'évanouir",
                "Je vais devenir fou ou perdre le contrôle",
                "Je vais me sentir mal et personne ne m'aidera",
                "Je vais me ridiculiser devant tout le monde"
            ],
            [
                "Les transports en commun et le métro",
                "Les files d'attente et la foule",
                "Être seul loin de chez moi",
                "L'effort physique et les escaliers",
                "La chaleur et les pièces mal aérées",
                "Le café"
            ],
            [
                "Garder de l'eau ou un comprimé sur moi",
                "Prendre mon pouls",
                "M'asseoir près de la sortie",
                "Ne sortir qu'accompagné",
                "Me distraire avec mon téléphone",
                "Me tenir à une rampe ou à un mur",
                "Surveiller ma respiration",
                "Partir dès que ça commence"
            ],
            [
                new("Monter trois étages d'un pas rapide", 30, ProvokesBodySensations: true),
                new("Boire une tasse de café", 35, ProvokesBodySensations: true),
                new("Faire la queue dans un magasin", 40),
                new("Prendre le bus pour un arrêt", 50),
                new("Marcher seul à 15 minutes de chez moi", 55),
                new("Faire trois stations de métro sans eau dans mon sac", 70),
                new("M'asseoir au milieu d'une rangée au cinéma", 75),
                new("Prendre le métro à l'heure de pointe", 85)
            ]),
        new(
            FearKind.Specific,
            "Une peur précise",
            "Le vide, l'avion, les chiens, les médecins, le sang, les ascenseurs",
            [
                "Quelque chose de terrible va arriver",
                "Je ne supporterai pas la peur",
                "Je vais perdre le contrôle",
                "Je vais être blessé ou mordu",
                "Je vais tomber",
                "Je serai coincé et ne pourrai pas sortir"
            ],
            [
                "Les hauteurs et les balcons",
                "Les ascenseurs et les espaces fermés",
                "Les animaux",
                "Les médecins, les piqûres, le sang",
                "L'avion",
                "Les ponts et les tunnels"
            ],
            [
                "Fermer les yeux ou détourner le regard",
                "Me tenir à quelqu'un",
                "N'y aller qu'accompagné",
                "Prévoir la sortie à l'avance",
                "Me distraire avec de la musique",
                "Prendre un calmant"
            ],
            [
                new("Regarder des photos de ce qui me fait peur", 20),
                new("Regarder une vidéo à ce sujet", 30),
                new("Me tenir à proximité, à une distance sûre", 40),
                new("M'approcher pendant une minute", 55),
                new("Rester tout près pendant 5 minutes", 65),
                new("Toucher ou entrer (ascenseur, balcon)", 75),
                new("Recommencer seul, sans personne", 85),
                new("Le faire dans un endroit inconnu", 90)
            ]),
        new(
            FearKind.Uncertainty,
            "L'incertitude et le « et si quelque chose arrivait »",
            "S'inquiéter, vérifier, chercher des garanties",
            [
                "Il arrivera un malheur si je ne vérifie pas",
                "Je ne supporte pas de ne pas savoir avec certitude",
                "Il arrivera quelque chose à un proche",
                "Je ferai une erreur et ce sera une catastrophe",
                "Je vais tomber gravement malade"
            ],
            [
                "Décider sans avoir toutes les informations",
                "Envoyer un message sans le relire",
                "Ne pas savoir où sont mes proches",
                "Les situations nouvelles et imprévisibles",
                "Lire des choses sur les maladies"
            ],
            [
                "Vérifier plusieurs fois",
                "Chercher sur internet",
                "Demander aux autres si tout va bien",
                "Appeler mes proches pour me rassurer",
                "Tout planifier dans le détail",
                "Repousser les décisions"
            ],
            [
                new("Envoyer un message après l'avoir lu une seule fois", 25),
                new("Choisir un plat au café en une minute", 30),
                new("Ne pas appeler un proche qui a 30 minutes de retard", 50),
                new("Envoyer un e-mail professionnel sans le relire", 55),
                new("Ne pas chercher un symptôme sur internet pendant une journée", 60),
                new("Passer un jour de repos sans programme", 65),
                new("Ne demander à personne si j'ai bien décidé", 70),
                new("Sortir de chez moi après n'avoir vérifié la porte qu'une fois", 80)
            ]),
        new(
            FearKind.Other,
            "Autre chose",
            "Décrivez votre peur avec vos propres mots",
            [
                "Quelque chose de grave va arriver",
                "Je ne m'en sortirai pas",
                "Je vais perdre le contrôle",
                "On va me juger"
            ],
            [],
            [
                "Éviter la situation",
                "N'y aller qu'accompagné",
                "Partir plus tôt",
                "Me distraire avec mon téléphone",
                "Prévoir la sortie à l'avance"
            ],
            [])
    ];
}
