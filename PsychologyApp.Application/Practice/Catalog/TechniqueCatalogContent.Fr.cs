using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentFr
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("Ce que c'est", what),
        new("Quand ça aide", when),
        new("Comment se déroule la pratique", how),
        new("Exemple", example)
    ];

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["Une dispute avec un proche", "Quelque chose au travail", "Un conflit avec mon chef", "Les études ou un examen", "La santé", "L'argent", "La solitude", "Un échec", "Une rupture", "La relation avec mes parents", "Les enfants", "Un déménagement", "L'avenir", "Les critiques à mon égard"];

    private static readonly string[] BackupOptions =
        ["Demander de l'aide", "Attendre et réessayer", "Trouver un autre moyen", "Être moins exigeant·e envers moi", "Faire une pause", "En parler à un proche", "Consulter un spécialiste", "Trouver un travail d'appoint", "Suivre une formation", "Découper la tâche en étapes", "Reporter la décision", "Commencer petit"];

    private static readonly string[] HarshThoughts =
        ["« Je vais tout gâcher »", "« On va me juger »", "« Je n'y arrive pas »", "« Personne n'a besoin de moi »", "« C'est une catastrophe »", "« Rien ne va marcher »", "« Je suis moins bien que les autres »", "« Tout va toujours de travers »", "« C'est de ma faute »", "« Quelque chose ne va pas chez moi »", "« Ça ne finira jamais »", "« Je dois être parfait·e »"];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Pratique", "Spin",
        [
            "1. Décrivez l'épisode qui vous préoccupe",
            "2. Nommez le sentiment principal",
            "3. Évaluez le sentiment de 0 à 10"
        ],
        "Le Spin de Zivorad Slavinski aide à réduire la charge émotionnelle d'un souvenir douloureux.",
        TechniqueUiKind.Entry, "Technique n° 1", "26.01.2023", "Spin", "Réduire la charge émotionnelle en 3 étapes", "Moment difficile", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Épisode", "Par ex. la conversation d'hier qui ne vous lâche pas", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentiment principal", "Blessure, peur, colère…", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Intensité du sentiment", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Une technique simple de neutralisation : vous nommez l'épisode, le sentiment et son intensité. Souvent, la prise de conscience suffit à réduire la charge.",
            "Quand une scène ou une pensée tourne en boucle et n'apaise ni l'anxiété ni la rancœur.",
            "Écrivez l'épisode et le sentiment, puis évaluez l'intensité honnêtement. Recommencez un autre jour si besoin.",
            "Épisode : « Un collègue a ignoré mon idée ». Sentiment : blessure. Note : 8 → plus tard 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "Pratique", "Comparer l'importance",
        [
            "1. Énoncez le problème",
            "2. Qu'est-ce qui comptait autrefois dans ce domaine ?",
            "3. Qu'est-ce qui compte maintenant ?",
            "4. Qu'est-ce qui comptera à l'avenir ?",
            "5. Comparez les trois réponses et notez ce qui a bougé"
        ],
        "Comparer l'importance dans le temps aide à replacer un problème dans le contexte de votre vie.",
        TechniqueUiKind.Entry, "Technique n° 2", "26.01.2023", "Comparer l'importance", "Quand le problème semble trop grand", "Ampleur du problème", "PNL", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problème", "Qu'est-ce qui semble insupportable en ce moment ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Comptait autrefois", "Par ex. les études, les relations, la carrière", Suggestions: ["Les études", "La famille", "Les amis", "La santé", "La carrière", "Les loisirs", "Le sport", "L'amour", "Les voyages", "La liberté", "La reconnaissance", "Le foyer"]),
            new TechniqueEntrySeed("Compte maintenant", "Qu'est-ce qui prend le plus votre attention ?", Suggestions: ["Le travail", "La famille", "La santé", "L'argent", "Les relations", "Le repos", "Les enfants", "Les amis", "Les études", "Le sommeil", "La tranquillité d'esprit", "Le développement personnel"]),
            new TechniqueEntrySeed("Comptera à l'avenir", "Vers quoi tendez-vous ?", Suggestions: ["La stabilité", "La famille", "La liberté", "La santé", "Progresser", "Les gens que j'aime", "Un chez-moi", "Un travail que j'aime", "Les voyages", "Le calme", "Les enfants", "Des économies"]),
            new TechniqueEntrySeed("Qu'est-ce qui a changé ?", "Le problème domine-t-il encore tout ?", Suggestions: ["Le problème n'est qu'une partie de la vie", "Plus de choses comptent qu'il n'y paraissait", "C'est une étape passagère", "C'est moins vif", "Je me sens plus calme", "Je vois que j'ai déjà fait face", "L'espoir est revenu", "Rien n'a encore changé"])
        ],
        Theory(
            "La technique montre que le poids d'un problème change avec le temps et reste rarement central pour toujours.",
            "Quand une difficulté occupe tout l'écran et semble permanente.",
            "Remplissez les quatre champs, puis comparez : d'autres valeurs réduisent souvent le problème.",
            "Problème : « Pas de travail ». Passé : les études. Présent : la recherche d'emploi. Avenir : la stabilité. Prise de conscience : une étape, pas toute la vie."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "Pratique", "Polarités",
        [
            "1. Écrivez ce qui vous plaît dans la situation",
            "2. Écrivez ce qui vous déplaît",
            "3. Comparez les paires et remarquez l'équilibre"
        ],
        "La méthode des polarités aide à voir les deux côtés et à réduire la tension intérieure.",
        TechniqueUiKind.Polarity, "Technique n° 3", "26.01.2023", "Polarités", "Voir les deux côtés d'une situation", "Différents côtés", "Zivorad Slavinski", 5, "Contrast",
        null,
        Theory(
            "Toute situation a des côtés attirants et désagréables. Les nommer tous les deux réduit la fixation sur un seul.",
            "Quand vous ne voyez que le négatif ou que le positif et vous sentez bloqué·e.",
            "Ajoutez des paires « me plaît / me déplaît » sur un thème. Comparez les listes sans juger qui a raison.",
            "Travail : le salaire me plaît, les horaires me déplaisent. Maison : le confort me plaît, le bruit des voisins me déplaît."),
        "Ce qui vous déplaît", "Ce qui vous plaît");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "Pratique", "Feuille de papier",
        [
            "1. Écrivez la pensée sur papier (ou dans le champ ci-dessous)",
            "2. Lisez-la une fois à voix haute",
            "3. Pliez, déchirez ou lâchez symboliquement la feuille"
        ],
        "Écrire ses pensées sur papier réduit leur charge émotionnelle.",
        TechniqueUiKind.Paper, "Technique n° 4", "26.01.2023", "Feuille de papier", "Quand les pensées tournent en boucle", "Pensées", "Psyché", 3, "Description",
        [new TechniqueEntrySeed("Pensée", "Par ex. « Je vais rater la présentation »", Suggestions: HarshThoughts)],
        Theory(
            "Écrire sort la pensée de votre tête. Détruire la feuille symbolise le lâcher-prise.",
            "Quand la même phrase tourne pendant des heures et bloque le sommeil ou la concentration.",
            "Déchargez la pensée, lisez-la, puis détruisez la feuille — réellement ou en imagination.",
            "Pensée : « Ils vont me trouver incompétent·e ». Après avoir écrit et brûlé la feuille, l'anxiété diminue souvent."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "Pratique", "50 ans plus tard",
        [
            "1. Énoncez le problème",
            "2. Évaluez son importance dans 50 ans, de 0 à 10"
        ],
        "Projeter un problème dans le temps réduit son importance ressentie.",
        TechniqueUiKind.Entry, "Technique n° 5", "30.01.2023", "50 ans plus tard", "Projeter le problème dans le temps", "Ampleur du problème", "PNL", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problème", "Qu'est-ce qui semble une catastrophe aujourd'hui ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Importance dans 50 ans", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Imaginer un événement loin dans l'avenir l'émousse souvent — vous le voyez avec plus de recul.",
            "Quand un revers semble la fin du monde alors qu'il est passager.",
            "Nommez le problème et évaluez son importance dans cinquante ans. La note est généralement plus basse.",
            "Problème : « Je n'ai pas été invité·e à la réunion ». Dans 50 ans : 1–2 sur 10."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "Pratique", "Tache blanche",
        [
            "1. Rappelez-vous l'épisode qui vous préoccupe",
            "2. Imaginez-le comme une tache blanche sans détails",
            "3. Gardez la tache jusqu'à ce que la tension retombe"
        ],
        "Retirer les détails d'un souvenir réduit souvent sa charge émotionnelle.",
        TechniqueUiKind.Entry, "Technique n° 6", "30.01.2023", "Tache blanche", "Adoucir un souvenir douloureux", "Moment difficile", "PNL", 3, "BlurOn",
        [new TechniqueEntrySeed("Épisode", "En bref : ce qui s'est passé et ce qui fait le plus mal", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Les détails nourrissent le sentiment. Une tache blanche retire les déclencheurs et offre une pause à votre système nerveux.",
            "Quand une image vive du passé vous submerge d'émotion.",
            "Rappelez-vous l'épisode, puis remplacez l'image par une simple tache blanche pendant 30 à 60 secondes.",
            "À la place du visage de la personne qui vous a blessé·e — une tache blanche. La respiration se calme, l'esprit revient au présent."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "Pratique", "Modification de l'expérience",
        [
            "1. Choisissez un domaine à travailler",
            "2. Décrivez votre attitude à son égard",
            "3. Nommez les croyances associées",
            "4. Comment l'attitude change-t-elle quand vous pensez à une expérience passée ?",
            "5. Décrivez une expérience négative",
            "6. Évaluez l'expérience négative (−10…10)",
            "7. Quelle expérience positive pourrait la remplacer ?",
            "8. Comment votre attitude changerait-elle avec cette expérience ?",
            "9. Évaluez la nouvelle expérience (−10…10)"
        ],
        "L'EMO aide à remplacer une expérience limitante par une expérience plus soutenante.",
        TechniqueUiKind.Entry, "Technique n° 7", "08.02.2023", "Modification de l'expérience", "Réécrire une expérience limitante", "Moment difficile", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Domaine", "Relations, travail, santé…", Suggestions: ["Les relations", "Le travail", "La santé", "L'argent", "Parler en public", "La famille", "Les études", "L'apparence", "L'amitié", "La créativité", "Le sport", "La carrière"]),
            new TechniqueEntrySeed("Attitude envers ce domaine", "Qu'en ressentez-vous maintenant ?", Suggestions: ["Anxiété", "Doute de soi", "Irritation", "Indifférence", "Intérêt", "Fatigue", "Peur", "Honte", "Espoir", "Colère", "Confusion", "Le calme"]),
            new TechniqueEntrySeed("Croyances associées", "Par ex. « Je n'y arriverai pas »", Suggestions: ["« Je n'y arriverai pas »", "« Je ne suis pas assez bien »", "« On ne m'acceptera pas »", "« Rien ne changera »", "« Je dois être parfait·e »", "« Je n'ai pas de chance »", "« Je ne le mérite pas »", "« Les autres sont meilleurs »", "« L'erreur est interdite »", "« Je suis trop vieux·vieille pour ça »"]),
            new TechniqueEntrySeed("Attitude et expérience passée", "Qu'est-ce qui change quand vous repensez au passé ?", Suggestions: ["C'est plus lourd", "C'est plus calme", "Rien ne change", "La colère monte", "La tristesse monte", "La peur monte", "Je me souviens que j'ai fait face", "J'ai envie de l'éviter"]),
            new TechniqueEntrySeed("Expérience négative", "Un épisode précis ou un schéma"),
            new TechniqueEntrySeed("Note de l'expérience négative", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Remplacement positif", "Quelle expérience soutenante est possible ?", Suggestions: ["Une fois où j'ai réussi", "Une fois où on m'a soutenu·e", "Une fois où je m'en suis sorti·e seul·e", "Une fois où on m'a félicité·e", "Une fois où j'ai aidé quelqu'un", "Une fois où j'ai appris quelque chose de nouveau", "Une fois où je n'ai pas abandonné", "Une fois où on m'a accepté·e tel·le que je suis"]),
            new TechniqueEntrySeed("Attitude avec le positif", "Comment votre regard sur ce domaine changerait-il ?", Suggestions: ["L'espoir apparaît", "Plus de calme", "J'ai envie de réessayer", "Ça devient intéressant", "La confiance apparaît", "C'est plus léger", "Je sens de la force", "Encore difficile à croire"]),
            new TechniqueEntrySeed("Note de la nouvelle expérience", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "Les croyances reposent souvent sur une seule expérience négative marquante. Un remplacement conscient peut faire évoluer votre attitude.",
            "Quand « ça a toujours été comme ça » et que le changement semble impossible.",
            "Remplissez les champs dans l'ordre. Comparez les deux notes — visez une seconde note plus haute.",
            "Domaine : parler en public. Négatif : examen raté (−8). Remplacement : une intervention réussie (+6)."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "Pratique", "Répétez-le",
        [
            "1. Écrivez la formulation qui vous préoccupe",
            "2. Répétez-la à voix haute comme si vous l'entendiez pour la première fois",
            "3. Continuez à répéter jusqu'à ce que l'émotion retombe"
        ],
        "Répéter l'énoncé d'un problème « comme pour la première fois » réduit l'intensité émotionnelle.",
        TechniqueUiKind.Copied, "Technique n° 8", "21.01.2025", "Répétez-le", "Entendre le problème comme pour la première fois", "Ampleur du problème", "PNL", 2, "Repeat",
        [new TechniqueEntrySeed("Formulation", "J'ai peur de perdre mon travail", Suggestions: HarshThoughts)],
        Theory(
            "Une phrase familière perd sa charge quand on la dit lentement et sur un ton neutre, sans drame.",
            "Quand une pensée résonne comme un verdict dans votre tête.",
            "Écrivez la phrase, puis répétez-la 5 à 10 fois avec des pauses, en observant les sensations du corps.",
            "« Personne n'a besoin de moi » — après sept répétitions, elle sonne souvent absurde et plus légère."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "Pratique", "Plan B",
        [
            "1. Nommez le problème",
            "2. Écrivez trois solutions de repli réalistes",
            "3. Remarquez comment l'anxiété change quand il existe des issues"
        ],
        "Les solutions de repli réduisent l'importance exagérée d'une seule issue.",
        TechniqueUiKind.Entry, "Technique n° 9", "21.01.2025", "Plan B", "Réduire l'enjeu avec un plan B", "Ampleur du problème", "PNL", 3, "Backup",
        [
            new TechniqueEntrySeed("Problème", "Et si le plan principal échoue ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Solution de repli 1", "Par ex. prendre une mission en freelance", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Solution de repli 2", "Par ex. se former à un métier proche", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Solution de repli 3", "Par ex. réduire temporairement les dépenses", Suggestions: BackupOptions)
        ],
        Theory(
            "Quand l'enjeu semble tout ou rien, l'esprit se crispe. Les alternatives redonnent un sentiment de choix.",
            "Avant un entretien, un examen ou un événement où l'échec semble catastrophique.",
            "Énoncez le problème et trois plans B plausibles — même modestes.",
            "Problème : « Je n'aurai pas le poste ». Plan B : un stage, des recommandations, une autre ville."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "Pratique", "Rendre petit",
        [
            "1. Décrivez ce qui vous préoccupe",
            "2. Imaginez-le comme un objet devant vous",
            "3. Rétrécissez-le et éloignez-le jusqu'à ce que l'intérêt s'estompe"
        ],
        "Rétrécir et éloigner visuellement un problème réduit l'intérêt émotionnel qu'il suscite.",
        TechniqueUiKind.Entry, "Technique n° 10", "22.01.2025", "Rendre petit", "Rendre le problème petit et lointain", "Ampleur du problème", "PNL", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("Ce qui vous préoccupe", "Par ex. un conflit qui vous empêche de dormir", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "La taille et la distance d'une image influencent son importance. Un petit objet lointain met rarement les nerfs à vif.",
            "Quand un problème semble énorme et juste sous vos yeux.",
            "Créez une image, rétrécissez-la peu à peu et repoussez-la vers l'horizon pendant 10 à 20 secondes.",
            "Une dispute devient un point minuscule. La respiration se détend, les pensées s'éclaircissent."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "Pratique", "Vérifier",
        [
            "1. Décrivez ce sur quoi vous tournez en boucle",
            "2. Arrêtez volontairement d'y penser pendant 1 à 2 minutes",
            "3. Remarquez ce qui a changé dans le corps et l'humeur"
        ],
        "Trop d'attention gonfle l'importance. Une courte pause permet de tester à quel point la pensée vous contrôle.",
        TechniqueUiKind.Entry, "Technique n° 11", "22.01.2025", "Vérifier", "Voir ce qui se passe si vous lâchez prise", "Ampleur du problème", "PNL", 2, "FactCheck",
        [new TechniqueEntrySeed("Ce sur quoi vous tournez en boucle", "Quelle pensée revient sans cesse ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Si une pensée était vitale pour survivre, la lâcher semblerait dangereux. L'expérience montre en général que rien de catastrophique n'arrive.",
            "Quand vous n'arrivez pas à arrêter de ressasser le même sujet.",
            "Nommez la pensée, puis déplacez doucement l'attention vers la respiration ou les sons autour de vous.",
            "Pensée : « Et si j'étais malade ». Après une pause, l'anxiété passe souvent de 7 à 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "Pratique", "Position d'observateur",
        [
            "1. Décrivez la situation qui vous préoccupe",
            "2. Écrivez vos sentiments à la première personne",
            "3. Regardez la situation en observateur neutre",
            "4. Évaluez la tension après avoir changé de position"
        ],
        "Changer de position de perception aide à prendre du recul face à un pic émotionnel et à voir la situation plus largement.",
        TechniqueUiKind.Entry, "Technique n° 12", "12.06.2026", "Position d'observateur", "Prendre du recul et voir la situation de l'extérieur", "Moment difficile", "PNL", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situation", "Que s'est-il passé ou que ressassez-vous ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentiments à la première personne", "Que ressentez-vous quand vous êtes dans la situation ?", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Ce que voit l'observateur", "Que remarquerait un observateur extérieur calme ?", Suggestions: ["Deux personnes qui se disputent", "Une personne fatiguée et contrariée", "Une situation de travail ordinaire", "Chacun défend son point de vue", "Dans une semaine, ce sera oublié", "Les gens se sont mal compris", "Une personne qui fait de gros efforts", "Désagréable, mais pas dangereux", "Chacun a son point de vue", "La personne a besoin de repos"]),
            new TechniqueEntrySeed("Tension maintenant", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Quand on est dans un épisode, les émotions semblent inévitables. La position d'observateur crée de la distance sans nier ce qu'on vit.",
            "Quand une situation vous submerge et que les arguments rationnels n'aident pas.",
            "Décrivez la situation et vos sentiments, puis réécrivez-la à la troisième personne — de façon neutre, sans jugement.",
            "Dispute : de l'intérieur — « on me manque de respect ». Observateur : « deux personnes parlent fort ». Tension : 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "Pratique", "Ancre de ressource",
        [
            "1. Rappelez-vous un moment où vous vous sentiez confiant·e ou calme",
            "2. Décrivez les sensations du corps",
            "3. Choisissez un geste d'ancrage simple",
            "4. Évaluez la force de l'état ressource"
        ],
        "Une ancre relie un geste du corps à un état ressource pour retrouver plus vite un appui.",
        TechniqueUiKind.Entry, "Technique n° 13", "12.06.2026", "Ancre de ressource", "Retrouver un appui par un geste et un souvenir", "Moment difficile", "PNL", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Souvenir ressource", "Quand vous êtes-vous senti·e fort·e, calme ou joyeux·se ?", Suggestions: ["Une intervention réussie", "Des vacances à la mer", "L'étreinte d'un proche", "Avoir terminé quelque chose de difficile", "Une promenade dans la nature", "Un examen réussi", "La naissance d'un enfant", "Une victoire en compétition", "Une soirée entre amis", "Les félicitations de mon chef", "Le premier jour dans un nouveau travail", "Un matin sans se presser"]),
            new TechniqueEntrySeed("Sensations du corps", "Où le ressentez-vous ? Chaleur, légèreté, ancrage…", Suggestions: ["De la chaleur dans la poitrine", "De la légèreté", "Les épaules ouvertes", "Une respiration régulière", "Des jambes solides", "Un sourire", "Un cœur calme", "Le dos droit", "De l'énergie dans les mains", "Le ventre détendu"]),
            new TechniqueEntrySeed("Geste ou ancre", "Par ex. serrer le poing, toucher le poignet", Suggestions: ["Serrer le poing", "Toucher le poignet", "Joindre le pouce et l'index", "La main sur le cœur", "Une inspiration profonde", "Pincer le lobe de l'oreille", "Ouvrir les épaules", "Caresser ma paume"]),
            new TechniqueEntrySeed("Force de l'état", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Le cerveau associe vite un geste à un état si vous répétez plusieurs fois la paire « souvenir + geste ».",
            "Avant un événement stressant ou quand vous manquez d'appui intérieur.",
            "Rappelez-vous l'épisode ressource, intensifiez les sensations du corps et fixez-les par un geste simple.",
            "Ressource : une intervention réussie. Corps : chaleur dans la poitrine. Ancre : toucher du pouce. Force : 8."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "Pratique", "Ancrage 5-4-3-2-1",
        [
            "1. Nommez 5 choses que vous voyez",
            "2. Nommez 4 choses que vous sentez avec votre corps",
            "3. Nommez 3 sons autour de vous",
            "4. Nommez 2 odeurs",
            "5. Nommez 1 goût ou une sensation agréable"
        ],
        "L'ancrage sensoriel déplace l'attention des pensées anxieuses vers le moment présent.",
        TechniqueUiKind.Entry, "Technique n° 14", "12.06.2026", "Ancrage 5-4-3-2-1", "Revenir au présent par les sens", "Moment difficile", "Psyché", 3, "Nature",
        [
            new TechniqueEntrySeed("5 choses que je vois", "Par ex. fenêtre, tasse, livre, lampe, main", Suggestions: ["Fenêtre", "Table", "Téléphone", "Ordinateur", "Ordinateur portable", "Écran", "Clavier", "Tasse", "Lampe", "Main", "Porte", "Livre", "Chaise", "Mur", "Tableau", "Plante", "Horloge", "Armoire", "Ciel", "Voiture"]),
            new TechniqueEntrySeed("4 choses que je sens avec mon corps", "Par ex. la chaise, le sol, les vêtements, la température de l'air", Suggestions: ["Chaise", "Le sol sous mes pieds", "Les vêtements", "La fraîcheur", "La chaleur", "Le téléphone dans ma main", "Le dossier de la chaise", "Le tissu sur ma peau", "Le souffle dans ma poitrine", "Le vent", "La souris", "Les clés", "L'oreiller", "La montre à mon poignet"]),
            new TechniqueEntrySeed("3 sons", "Par ex. la rue, l'horloge, votre respiration", Suggestions: ["La rue", "Horloge", "La respiration", "Des voix", "Le frigo", "Le vent", "Le silence", "Ordinateur", "Les voitures", "Les oiseaux", "La musique", "La climatisation", "Des pas", "La pluie"]),
            new TechniqueEntrySeed("2 odeurs", "Par ex. le café, l'air frais", Suggestions: ["Le café", "L'air frais", "Le savon", "La nourriture", "Le parfum", "Presque rien", "Le thé", "Le pain", "Les livres", "Les fleurs", "La crème pour les mains", "Le bois"]),
            new TechniqueEntrySeed("1 goût ou une sensation agréable", "Par ex. l'eau dans la bouche, un tissu doux", Suggestions: ["L'eau", "Le thé", "Un tissu doux", "Les paumes chaudes", "La bouche fraîche", "Le café", "Un chewing-gum à la menthe", "Du chocolat", "Une pomme", "Une couverture chaude"])
        ],
        Theory(
            "L'anxiété vous entraîne souvent vers le passé ou l'avenir. Compter par les sens vous ramène ici et maintenant.",
            "En cas de panique, de forte anxiété, de vertige ou de sensation d'être détaché·e de la réalité.",
            "Parcourez les étapes lentement en nommant des sensations concrètes. Respirez calmement entre chaque élément.",
            "5 je vois → 4 je sens → 3 j'entends → la respiration se calme, les pensées ralentissent."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "Pratique", "Respiration carrée",
        [
            "1. Évaluez la tension avant la pratique",
            "2. Inspirez sur 4 temps",
            "3. Retenez sur 4 temps",
            "4. Expirez sur 4 temps",
            "5. Retenez sur 4 temps — répétez 4 cycles et évaluez la tension ensuite"
        ],
        "Une respiration rythmée calme le système nerveux et réduit la tension du corps.",
        TechniqueUiKind.Entry, "Technique n° 15", "04.07.2026", "Respiration carrée", "Calmer le corps par le rythme du souffle", "Moment difficile", "Psyché", 3, "Air",
        [
            new TechniqueEntrySeed("Tension avant", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notes", "Qu'avez-vous remarqué dans votre corps ou votre respiration pendant la pratique ?", Suggestions: ["Les épaules se sont relâchées", "La respiration a ralenti", "J'ai eu plus chaud", "Difficile de tenir la pause", "Les pensées m'ont distrait·e", "Je me suis senti·e plus calme", "J'ai eu la tête qui tourne", "Le cœur a ralenti", "J'ai eu envie de bâiller", "Rien de spécial"]),
            new TechniqueEntrySeed("Tension après", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Une respiration lente et rythmée réduit l'activité du système nerveux sympathique et aide le corps à sortir du mode « combat ou fuite ».",
            "En cas d'anxiété, de panique, de cœur qui s'emballe, d'insomnie ou de sentiment d'être débordé·e.",
            "Installez-vous confortablement, respirez par le nez : 4 inspiration, 4 rétention, 4 expiration, 4 rétention. Répétez 4 cycles.",
            "Anxiété 7/10 → après 4 cycles 4/10. Les épaules se relâchent, le pouls ralentit."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "Pratique", "Un petit pas",
        [
            "1. Décrivez ce qui vous pèse ou vous empêche de commencer",
            "2. Choisissez le plus petit pas réaliste pour aujourd'hui",
            "3. Écrivez quand vous le ferez",
            "4. Évaluez votre énergie après avoir écrit le pas"
        ],
        "Activation comportementale : une petite action redonne un sentiment de mouvement et de contrôle.",
        TechniqueUiKind.Entry, "Technique n° 16", "04.07.2026", "Un petit pas", "Retrouver l'élan par une action minimale", "Moment difficile", "TCC", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("Ce qui pèse", "Qu'est-ce qui semble difficile, ennuyeux ou impossible en ce moment ?", Suggestions: ["Fatigue", "Pas d'énergie", "Trop de choses à faire", "L'ennui", "Peur de commencer", "Rien ne semble compter", "Mal dormi", "Pas le moral", "La solitude", "La procrastination", "Mal de tête", "Je ne sais pas par où commencer"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Le plus petit pas aujourd'hui", "Par ex. me lever, me laver le visage, envoyer un message", Suggestions: ["Sortir du lit", "Me laver le visage", "Boire un peu d'eau", "Envoyer un message", "Sortir 5 minutes", "Ranger une chose", "Ouvrir une fenêtre", "M'étirer 2 minutes", "Prendre une douche", "Manger quelque chose", "Appeler un proche", "Ouvrir le fichier dont j'ai besoin", "Écrire un plan pour la journée", "Laver une tasse"]),
            new TechniqueEntrySeed("Quand vous le ferez", "Une heure précise ou un moment de la journée", Suggestions: ["Tout de suite", "Dans 15 minutes", "Dans une heure", "Le matin", "Après le petit-déjeuner", "Après le déjeuner", "Le soir", "Avant de dormir", "Demain matin", "Le week-end"]),
            new TechniqueEntrySeed("Énergie après", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Dans l'apathie, le cerveau attend la motivation pour agir. Un petit pas crée la motivation par l'action, et non l'inverse.",
            "Quand vous n'avez pas d'énergie, que tout semble inutile ou que commencer la journée est difficile.",
            "Choisissez un pas de 2 à 5 minutes sans plan parfait. Écrivez-le avec l'heure — c'est déjà un mouvement.",
            "Ranger l'appartement → poser une tasse dans la cuisine à 10 h. Énergie : 2 → 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "Pratique", "Journal des pensées",
        [
            "1. Décrivez la situation",
            "2. Écrivez la pensée automatique",
            "3. Évaluez l'émotion",
            "4. Formulez une pensée alternative plus équilibrée",
            "5. Évaluez de nouveau l'émotion"
        ],
        "Un journal structuré de TCC aide à voir le lien entre situation, pensée et émotion.",
        TechniqueUiKind.Entry, "Technique n° 17", "04.07.2026", "Journal des pensées", "Décortiquer une pensée et réduire la charge émotionnelle", "Moment difficile", "TCC", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situation", "Que s'est-il passé ? Où et quand ?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Pensée automatique", "Qu'avez-vous pensé à ce moment-là ?", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("Émotion", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Pensée alternative", "Qu'est-ce qui pourrait aussi être vrai ? Un regard plus équilibré", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("Émotion maintenant", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Les pensées automatiques sonnent souvent comme des faits alors que ce sont des interprétations. Les écrire les rend visibles et vérifiables.",
            "Après un conflit, une critique, un échec, ou quand une pensée tourne en boucle.",
            "Décrivez la situation et la pensée, évaluez l'émotion, puis trouvez une alternative — pas une positivité forcée, mais quelque chose de plus juste.",
            "Situation : pas de réponse à un message. Pensée : personne n'a besoin de moi. Alternative : la personne est occupée. Émotion : 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "Pratique", "Des mots bienveillants pour soi",
        [
            "1. Écrivez ce que vous vous reprochez",
            "2. Imaginez qu'un ami proche dise cela — que lui répondriez-vous ?",
            "3. Formulez la même phrase de soutien pour vous-même",
            "4. Évaluez la chaleur ou le soulagement"
        ],
        "L'autocompassion réduit l'autocritique sévère et aide à se relever après une erreur.",
        TechniqueUiKind.Entry, "Technique n° 18", "04.07.2026", "Des mots bienveillants pour soi", "Adoucir l'autocritique par le soutien", "Moment difficile", "TCC", 5, "Favorite",
        [
            new TechniqueEntrySeed("Ce que vous vous reprochez", "Que vous dites-vous ou que vous reprochez-vous ?", Suggestions: ["Une erreur au travail", "Je n'ai pas fini", "Je me suis emporté·e contre un proche", "Je me compare aux autres", "Je repousse encore les choses", "Mon apparence", "Mon poids", "J'ai dit ce qu'il ne fallait pas", "Pas un assez bon parent", "Je gagne trop peu", "Je n'ai pas tenu une promesse", "Trop sensible"]),
            new TechniqueEntrySeed("Ce que vous diriez à un ami", "Comment soutiendriez-vous un proche dans cette situation ?", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("Phrase pour vous-même", "La même bienveillance, tournée vers vous", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("Chaleur / soulagement", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "L'autocritique motive rarement — elle nourrit plus souvent la honte et la paralysie. L'autocompassion n'enlève pas la responsabilité, elle apporte un soutien.",
            "Après une erreur, en vous comparant aux autres ou en vous sentant pas assez bien.",
            "Écrivez la critique, puis répondez-vous comme à un ami — avec respect et sans minimiser.",
            "Critique : j'ai encore tout gâché. À un ami : tu as essayé, ça se répare. À moi : je peux en tirer une leçon. Chaleur : 3 → 6."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "Pratique", "Où est-ce dans le corps ?",
        [
            "1. Évaluez votre tension maintenant",
            "2. Déplacez lentement votre attention dans le corps, de la tête aux pieds",
            "3. Trouvez l'endroit où la sensation est la plus forte",
            "4. Décrivez-la : à quoi elle ressemble, grande ou petite, chaude ou froide",
            "5. Nommez le sentiment qui l'accompagne",
            "6. Faites 5 expirations lentes comme si chacune traversait cet endroit, puis évaluez de nouveau la tension"
        ],
        "Scan corporel : remarquez où loge un sentiment, nommez-le et laissez le corps se détendre sans lutter contre la sensation.",
        TechniqueUiKind.Entry, "Technique n° 19", "16.10.2026", "Où est-ce dans le corps ?", "Remarquer où un sentiment loge dans le corps et adoucir la tension", "Moment difficile", "Pleine conscience", 4, "Accessibility",
        [
            new TechniqueEntrySeed("Tension maintenant", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Où le ressentez-vous dans le corps ?", "Par exemple : poitrine, gorge, épaules", Suggestions: ["Tête", "Front", "Yeux", "Mâchoire", "Gorge", "Cou", "Épaules", "Poitrine", "Cœur", "Ventre", "Dos", "Bas du dos", "Bras", "Mains", "Jambes", "Tout le corps"]),
            new TechniqueEntrySeed("À quoi ressemble la sensation ?", "Par exemple : serrement, lourdeur, une boule", Suggestions: ["Serrement", "Lourdeur", "Une boule", "Pression", "Tension", "Chaleur", "Froid", "Tremblement", "Picotements", "Pulsations", "Vide", "Engourdissement", "Douleur", "Difficile à décrire"]),
            new TechniqueEntrySeed("Quel sentiment l'accompagne ?", "Par exemple : anxiété, blessure, fatigue", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Tension après", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Les sentiments résonnent dans le corps : l'anxiété comme un serrement dans la poitrine, la blessure comme une boule dans la gorge, la fatigue comme des épaules lourdes. Quand on remarque la sensation et qu'on nomme le sentiment, la tension diminue en général. C'est le cœur du scan corporel dans les programmes de pleine conscience.",
            "Quand vous vous sentez anxieux·se ou lourd·e sans savoir exactement ce que vous ressentez ; quand le corps est tendu après une conversation ou une journée difficile. La pratique n'explique pas les maladies et ne remplace pas un médecin : si une sensation est nouvelle, forte ou ne passe pas, consultez un médecin.",
            "Installez-vous confortablement et déplacez lentement votre attention du sommet de la tête jusqu'aux pieds. Arrêtez-vous là où la sensation est la plus vive. Ne la changez pas et ne luttez pas : décrivez-la et nommez le sentiment. Puis faites quelques expirations lentes comme si chacune traversait cet endroit.",
            "Tension 6. Où : la poitrine. Sensation : un serrement, comme un poing. Sentiment : l'anxiété avant une conversation. Après 5 expirations, le serrement est plus doux, tension 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "Pratique", "Un souvenir a pris le dessus",
        [
            "1. Évaluez son intensité",
            "2. Dites-vous : « C'est un souvenir. C'était avant, et maintenant c'est aujourd'hui »",
            "3. Nommez où vous êtes et quel moment de la journée il est",
            "4. Sentez l'appui sous vos pieds, serrez et relâchez les poings",
            "5. Nommez trois choses que vous voyez",
            "6. Cinq expirations lentes, de l'eau froide, puis évaluez de nouveau"
        ],
        "Aide en cas de flash-back : revenir du souvenir au présent par le temps, le lieu, le corps et les sens.",
        TechniqueUiKind.Entry, "Technique n° 20", "16.10.2026", "Un souvenir a pris le dessus", "Revenir au présent quand un souvenir difficile prend le dessus", "Moment difficile", "Traumatisme", 3, "MyLocation",
        [
            new TechniqueEntrySeed("Son intensité", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Où vous êtes et le moment de la journée", "Par exemple : à la maison, le soir", Suggestions: ["À la maison", "Au travail", "Dehors", "Dans les transports", "Chez quelqu'un", "Matin", "Après-midi", "Soir", "Nuit"]),
            new TechniqueEntrySeed("Trois choses que vous voyez", "Par exemple : fenêtre, tasse, lampe", Suggestions: ["Fenêtre", "Table", "Téléphone", "Tasse", "Lampe", "Porte", "Chaise", "Mur", "Tableau", "Plante", "Horloge", "Ciel"]),
            new TechniqueEntrySeed("Son intensité maintenant", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Un flash-back est un souvenir de traumatisme vécu comme s'il se reproduisait : avec les mêmes sentiments et les mêmes réactions du corps. Le cerveau confond « avant » et « maintenant ». Le temps, le lieu, l'appui du corps et ce que vous voyez et entendez aident à revenir au présent.",
            "Quand le souvenir d'un événement difficile prend le dessus, avec des images qui surgissent et la sensation « j'y suis de nouveau ». Des flash-back fréquents sont un signe courant de réaction post-traumatique, et ils se soignent bien avec un spécialiste du traumatisme.",
            "N'essayez pas de travailler le souvenir : pour l'instant, la seule tâche est de revenir. Dites-vous où vous êtes et quel jour on est, appuyez vos pieds au sol, nommez ce que vous voyez, expirez lentement, utilisez le froid.",
            "Intensité 8. « C'était avant ; maintenant on est en 2026, je suis chez moi, c'est le soir. » Les pieds au sol ; je vois une fenêtre, une tasse, une lampe. De l'eau froide. Maintenant 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "Pratique", "Réécrire un cauchemar",
        [
            "1. Choisissez un cauchemar récurrent, pas le plus difficile pour commencer",
            "2. Décrivez-le brièvement, sans détails pénibles",
            "3. Décidez ce que vous y changez : la fin, le lieu, vous-même, quelqu'un à côté",
            "4. Écrivez la nouvelle version du rêve en entier, jusqu'à une fin calme",
            "5. Chaque jour, pendant 10 à 20 minutes, imaginez la nouvelle version en détail",
            "6. Au bout d'une ou deux semaines, voyez comment les cauchemars ont changé"
        ],
        "Répétition par imagerie mentale : un cauchemar récurrent est réécrit à l'état de veille, et le cerveau commence à rêver la nouvelle version.",
        TechniqueUiKind.Entry, "Technique n° 21", "16.10.2026", "Réécrire un cauchemar", "Rendre un cauchemar récurrent plus rare et plus faible", "Le sommeil", "Traumatisme", 15, "NightsStay",
        [
            new TechniqueEntrySeed("À quel point ce cauchemar est pénible", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("De quoi parle le cauchemar, en bref", "Une ou deux phrases, sans détails pénibles", Suggestions: ["Quelqu'un me poursuit", "Je n'arrive pas à m'enfuir", "Je suis de retour là où j'ai eu peur", "Je perds un proche", "Je n'arrive pas à crier", "Je tombe", "On m'attaque", "Je suis en retard et je n'y arrive pas"]),
            new TechniqueEntrySeed("Ce que vous changez", "Tout ce que vous voulez : c'est votre rêve", Suggestions: ["La fin", "Le lieu", "Je deviens plus fort·e", "Quelqu'un vient m'aider", "Je peux parler et crier", "Le danger disparaît", "Tout devient sûr", "Je remarque que c'est un rêve"]),
            new TechniqueEntrySeed("La nouvelle version du rêve", "Décrivez-la en entier, du début jusqu'à une fin calme", Suggestions: ["Je remarque que c'est un rêve", "Quelqu'un qui me protège apparaît", "Je me retrouve dans un endroit sûr", "Je m'éloigne calmement", "Tout finit bien"]),
            new TechniqueEntrySeed("Quand je répéterai la nouvelle version", "Chaque jour, 10 à 20 minutes", Suggestions: ["Le matin", "Dans la journée, pendant une pause", "Le soir, deux heures avant de dormir", "Chaque jour à la même heure"]),
            new TechniqueEntrySeed("À quel point c'est pénible maintenant", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Les cauchemars récurrents après un traumatisme ou un stress sont entretenus par l'habitude : le cerveau rejoue le même scénario encore et encore. Dans la thérapie par répétition d'imagerie mentale, le scénario est réécrit à l'état de veille, et la nouvelle version est répétée mentalement chaque jour. Les recherches montrent que cela rend les cauchemars plus rares et plus faibles, et le sommeil meilleur.",
            "En cas de cauchemars récurrents qui donnent peur de s'endormir. Si le cauchemar rejoue directement un traumatisme et que même une courte description provoque une forte terreur, faites-le avec un spécialiste du traumatisme.",
            "Commencez par un cauchemar plus léger. N'écrivez pas en détail les passages effrayants : deux phrases suffisent. Changez le rêve comme vous le souhaitez ; la nouvelle version n'a pas besoin d'être réaliste. Répétez-la dans la journée, pas juste avant de dormir, 10 à 20 minutes par jour.",
            "Cauchemar : quelqu'un me poursuit et je n'arrive pas à m'enfuir. Changement : je remarque que c'est un rêve, je me retourne, et ce n'est qu'une ombre ; je rentre calmement chez moi. Je le répète chaque après-midi. Au bout de deux semaines, le cauchemar revient moins souvent."),
        null, null);
}
