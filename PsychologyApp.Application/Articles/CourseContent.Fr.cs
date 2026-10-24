using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public static partial class ArticleContent
{
    private static readonly Course[] CoursesFr =
    [
        new(
            CourseIds.Sleep,
            "Dormir sans somnifères",
            "Sept jours de thérapie cognitive et comportementale de l'insomnie, la façon la mieux étudiée de retrouver le sommeil",
            "Bedtime",
            "isi",
            "ISI-7",
            [
                new(
                    "Comment fonctionne le sommeil",
                    4,
                    [
                        P("Le sommeil ne s'« allume » pas par l'effort. Deux forces le préparent : la pression de sommeil, qui s'accumule toute la journée pendant l'éveil, et l'horloge interne, qui nous rend somnolents et éveillés aux mêmes heures chaque jour."),
                        P("Dans l'insomnie, les deux forces sont déréglées : on se couche plus tôt pour « rattraper », on reste longtemps éveillé, on fait la sieste, on se lève à des heures différentes. La pression de sommeil n'a pas le temps de s'accumuler, et l'horloge s'embrouille."),
                        Cycle("Le cercle de l'insomnie", "Une mauvaise nuit", "L'inquiétude : « je ne vais encore pas dormir »", "S'efforcer de s'endormir", "La tension éloigne le sommeil"),
                        P("Ce cours repose sur la thérapie cognitive et comportementale de l'insomnie (TCC-I). Les recommandations médicales la placent en premier, avant les somnifères : son effet dure après la fin du cours."),
                        Warning("Quand consulter d'abord un médecin"),
                        List(
                            "Ronflements forts, pauses respiratoires pendant le sommeil, forte somnolence dans la journée",
                            "Des sensations désagréables dans les jambes le soir qui donnent envie de bouger",
                            "Une insomnie accompagnée d'une humeur basse depuis plus de deux semaines",
                            "Vous conduisez ou utilisez des machines et êtes très somnolent dans la journée")
                    ],
                    "Chaque matin, notez : quand vous vous êtes couché, à peu près quand vous vous êtes endormi, combien de fois vous vous êtes réveillé, quand vous vous êtes levé et comment vous avez dormi de 0 à 10. Pas de montre la nuit, approximativement, au ressenti.",
                    "À quelle heure vous couchez-vous et vous levez-vous d'habitude ? Qu'est-ce qui gêne le plus votre sommeil ?"),
                new(
                    "Le lit est fait pour dormir",
                    4,
                    [
                        P("Si vous restez souvent éveillé au lit, vous y inquiétez ou y regardez votre téléphone, le cerveau associe le lit à l'éveil. C'est pourquoi on peut s'assoupir dans un fauteuil et être parfaitement réveillé dès qu'on s'allonge."),
                        H("Les règles qui rétablissent le lien"),
                        List(
                            "N'allez au lit que lorsque vous avez sommeil, pas simplement quand vous êtes fatigué",
                            "Si vous ne dormez pas depuis environ 20 minutes, levez-vous et faites quelque chose de calme sous une lumière douce. Revenez quand vous avez sommeil",
                            "Le lit sert uniquement au sommeil et à l'intimité : pas de téléphone, de séries ni de travail",
                            "Levez-vous à la même heure chaque jour, même après une mauvaise nuit et le week-end",
                            "Ne faites pas de sieste, et si vraiment il le faut, pas plus de 20 minutes et avant 15 h"),
                        Quiz("La nuit a été mauvaise. Que vaut-il mieux faire le matin ?", 1, "Une heure de lever fixe est le principal point d'ancrage du sommeil. Si vous faites la grasse matinée, la pression de sommeil sera plus faible le soir, et la nuit suivante sera difficile elle aussi.", "Faire la grasse matinée pour récupérer", "Se lever à l'heure habituelle", "Se lever, puis faire une bonne sieste dans la journée")
                    ],
                    "Choisissez une heure de lever que vous pouvez tenir les sept jours, et levez-vous à cette heure. Si vous ne dormez pas depuis environ 20 minutes, sortez du lit.",
                    "Quelle heure de lever avez-vous choisie ? Qu'est-ce qui a été difficile avec la règle « se lever si on ne dort pas » ?"),
                new(
                    "La fenêtre de sommeil",
                    5,
                    [
                        P("Plus on reste éveillé longtemps au lit, plus le sommeil devient léger et haché. Une technique appelée compression du sommeil aide : le temps passé au lit est rapproché du temps réellement dormi."),
                        H("Comment calculer la fenêtre"),
                        List(
                            "À partir de vos notes des derniers jours, calculez combien de temps vous dormez en moyenne",
                            "Ajoutez 30 minutes : c'est votre temps au lit, mais pas moins de six heures",
                            "Comptez à rebours depuis l'heure de lever choisie : cela donne votre heure de coucher",
                            "Ne vous couchez pas avant cette heure, même si vous en avez très envie"),
                        P("Les premiers jours, vous aurez sommeil : cela signifie que la pression de sommeil s'accumule. Au bout d'une semaine, si vous dormez la plupart du temps passé au lit, la fenêtre peut être allongée de 15 minutes."),
                        Warning("Attention"),
                        P("Ne compressez pas le sommeil si vous avez un trouble bipolaire ou une épilepsie, si vous êtes enceinte, ou si la somnolence rend la conduite ou le travail dangereux. Dans ces cas, parlez de votre sommeil avec un médecin.")
                    ],
                    "Calculez votre fenêtre de sommeil et respectez-la jusqu'à la fin du cours : pas de lit avant l'heure de coucher, lever à la même heure.",
                    "Quelle est votre fenêtre de sommeil ? Comment s'est passée la première nuit avec elle ?"),
                new(
                    "Une soirée qui prépare au sommeil",
                    4,
                    [
                        P("Le sommeil vient plus facilement quand le corps et l'esprit ont le temps de ralentir. L'heure avant le coucher est le pont entre la journée et le sommeil."),
                        H("Ce qui aide"),
                        List(
                            "Une heure avant le coucher : lumière douce et activités calmes sans écrans",
                            "Café, thé fort et boissons énergisantes pas plus tard que 14 h",
                            "L'alcool aide à s'endormir mais rend le sommeil superficiel et réveille vers le matin",
                            "Un dîner copieux 2 à 3 heures avant le coucher",
                            "Le matin, la lumière du jour pendant au moins 15 minutes : elle règle l'horloge interne"),
                        H("Se détendre avant de dormir"),
                        P("Une respiration lente avec une longue expiration diminue l'activation. Essayez maintenant."),
                        Breathe("Inspirez sur 4, expirez sur 6")
                    ],
                    "Créez votre propre rituel pour la dernière heure avant le coucher et faites-le chaque soir. Sortez le téléphone de la chambre, ou au moins du lit.",
                    "Qu'avez-vous mis dans votre rituel du soir ? Qu'est-ce qui s'est révélé difficile ?"),
                new(
                    "Les pensées qui empêchent de dormir",
                    4,
                    [
                        P("L'esprit se met souvent à travailler justement au lit : dans la journée, il n'y avait pas de temps pour les pensées. Il aide donc de leur en donner plus tôt."),
                        H("Un temps pour les pensées"),
                        List(
                            "Le soir, deux heures environ avant le coucher, réservez 15 minutes",
                            "Écrivez tout ce qui vous préoccupe et, pour chaque point, une prochaine étape ou « rien à faire pour l'instant »",
                            "Si la pensée revient au lit, dites-vous : « je l'ai noté, je m'en occuperai demain »"),
                        Quiz("Au lit, vous pensez : « Si je ne dors pas, je vais tout rater demain. » Qu'est-ce qui est plus proche de la vérité ?", 1, "La pensée exagère. Après une mauvaise nuit, c'est plus dur, mais la plupart des gens accomplissent quand même leurs tâches. Plus la peur du lendemain est forte, plus il est difficile de s'endormir.", "C'est vrai, l'échec est inévitable", "Après une mauvaise nuit c'est plus dur, mais d'habitude je m'en sors pour l'essentiel", "Mieux vaut ne pas se coucher du tout"),
                        Practice(TechniqueId.Observer, "Position d'observateur")
                    ],
                    "Chaque soir, réservez 15 minutes pour écrire ce qui vous préoccupe, et n'emportez pas ces pensées au lit.",
                    "Quelles pensées viennent le plus souvent la nuit ? Les écrire vous a-t-il aidé ?"),
                new(
                    "Cesser de lutter contre le sommeil",
                    4,
                    [
                        P("Plus on essaie de dormir, pire c'est. Le sommeil est comme un oiseau farouche : il vient quand personne ne le poursuit."),
                        H("Ce qui aide à lâcher le contrôle"),
                        List(
                            "Mettez l'horloge hors de vue : compter « plus que cinq heures » réveille",
                            "Autorisez-vous à ne pas dormir : « je vais rester allongé et me reposer, le sommeil viendra quand il viendra »",
                            "Une mauvaise nuit n'est pas une catastrophe : le corps récupère même après un sommeil incomplet",
                            "N'annulez pas vos projets à cause d'une mauvaise nuit : ainsi l'insomnie ne dirige pas votre vie"),
                        P("Comparez vos notes des premiers jours avec aujourd'hui : comment ont changé le temps d'endormissement, le nombre de réveils et la note du sommeil ?")
                    ],
                    "Gardez l'horloge hors de vue la nuit. Après une mauvaise nuit, vivez la journée comme d'habitude.",
                    "Qu'est-ce qui a changé dans votre sommeil ces derniers jours, d'après vos notes ?"),
                new(
                    "Bilan et plan",
                    4,
                    [
                        P("En une semaine, vous avez parcouru le cœur de la TCC de l'insomnie. Le sommeil revient progressivement : beaucoup de personnes constatent une amélioration en deux à quatre semaines si elles respectent les règles."),
                        H("Ce qu'il faut continuer"),
                        List(
                            "Une seule heure de lever chaque jour",
                            "N'aller au lit qu'avec sommeil, et se lever si l'on ne dort pas",
                            "Si vous dormez la plus grande partie de la nuit, allonger la fenêtre de 15 minutes par semaine",
                            "Le rituel du soir et le temps pour les pensées"),
                        H("Si l'insomnie revient"),
                        P("Après un stress, une maladie ou un voyage, le sommeil peut de nouveau se dérégler. C'est normal : revenez aux règles de ce cours, et il se rétablira plus vite que la première fois."),
                        P("Refaites l'index ISI-7 et comparez avec votre résultat du début. S'il n'y a pas d'amélioration, consultez un médecin : il existe des spécialistes du sommeil qui pratiquent la TCC-I."),
                        TestsTab("Index ISI-7", "isi")
                    ],
                    "Écrivez votre plan pour le cas où le sommeil se dérèglerait de nouveau.",
                    "Qu'est-ce qui vous a le plus aidé dans le cours ? Qu'allez-vous continuer à faire ?")
            ]),
        new(
            CourseIds.Anxiety,
            "L'anxiété sous contrôle",
            "Six jours : comprendre comment fonctionne l'anxiété, calmer le corps, vérifier les pensées effrayantes et faire un premier pas vers la peur",
            "Psychology",
            "gad7",
            "GAD-7",
            [
                new(
                    "Comment fonctionne l'anxiété",
                    4,
                    [
                        P("L'anxiété est une alarme destinée à prévenir d'un danger. Les difficultés commencent quand elle sonne trop souvent, trop fort, et ne s'arrête pas pendant longtemps."),
                        Cycle("Le cercle de l'anxiété", "Une pensée effrayante", "L'anxiété dans le corps", "Évitement ou béquille", "Soulagement pour une minute", "Le cerveau retient : « c'était dangereux »"),
                        P("Ce qui entretient l'anxiété, ce n'est pas la peur elle-même, mais ce que nous faisons pour ne pas la ressentir : éviter, revérifier, chercher à être rassuré, ressasser. Pas à pas, le cours travaille chaque maillon de ce cercle."),
                        P("Au début du cours, il vaut la peine de remplir l'échelle GAD-7 : à la fin, vous comparerez le résultat."),
                        TestsTab("Échelle GAD-7", "gad7")
                    ],
                    "Trois fois aujourd'hui, quand vous vous sentez anxieux, notez : ce qui s'est passé, quelle était la pensée, ce que le corps ressentait et ce que vous avez fait.",
                    "Quand et où l'anxiété apparaît-elle le plus souvent ?"),
                new(
                    "Calmer le corps",
                    4,
                    [
                        P("Quand l'anxiété est forte, il est difficile de discuter avec ses pensées. D'abord, il aide de calmer le corps : une expiration lente active le système qui freine la réaction de lutte ou de fuite."),
                        Breathe("Inspirez sur 4, expirez sur 6"),
                        P("Si l'anxiété est forte et que tout semble irréel, ramenez votre attention au présent par les sens."),
                        Practice(TechniqueId.Grounding, "Ancrage 5-4-3-2-1"),
                        P("Important : la respiration n'est pas un moyen de se débarrasser de l'anxiété à tout prix. Elle aide à passer le pic, pas à éviter les situations.")
                    ],
                    "Deux fois par jour, deux minutes de respiration lente, même au calme : ainsi la compétence sera là quand vous en aurez besoin.",
                    "Qu'avez-vous remarqué dans votre corps en respirant lentement ?"),
                new(
                    "Vérifier une pensée effrayante",
                    5,
                    [
                        P("Les pensées anxieuses sonnent comme des faits : « je vais me ridiculiser », « il va arriver quelque chose de terrible ». Mais ce sont des prédictions, et on peut les vérifier."),
                        H("Les questions à poser à la pensée"),
                        List(
                            "Quels faits soutiennent cette pensée, et quels faits vont contre ?",
                            "Que s'est-il passé dans des cas semblables auparavant ?",
                            "Qu'est-ce qui est le plus probable, plutôt que le plus effrayant ?",
                            "Si le pire arrive, comment y ferai-je face ?",
                            "Que dirais-je à un ami qui aurait cette pensée ?"),
                        Quiz("« Je vais forcément tout rater. » Quel piège de pensée est-ce ?", 1, "C'est la prédiction de l'avenir : la pensée traite comme un fait ce qui n'est pas encore arrivé.", "La lecture de pensées", "La prédiction de l'avenir", "La disqualification du positif"),
                        Practice(TechniqueId.ThoughtRecord, "Journal des pensées")
                    ],
                    "Une fois aujourd'hui, vérifiez une pensée anxieuse avec le journal des pensées.",
                    "Quelle pensée avez-vous vérifiée ? Combien y croyiez-vous avant et après ?"),
                new(
                    "Le temps des soucis",
                    4,
                    [
                        P("S'inquiéter semble utile : « si je réfléchis davantage, je serai prêt ». En réalité, ressasser mène rarement à une solution et maintient l'anxiété élevée."),
                        Quiz("Comment distinguer résoudre et tourner en rond ?", 0, "Résoudre avance vers un pas : que puis-je faire, et quand. Tourner en rond produit de nouveaux « et si ».", "Résoudre finit par un pas, tourner en rond par un nouveau « et si »", "Tourner en rond prend plus de temps", "C'est impossible"),
                        H("Comment reporter les soucis"),
                        List(
                            "Choisissez 15 à 20 minutes par jour, pas avant le coucher",
                            "Quand un souci arrive à un autre moment, notez le sujet en une ligne et dites-vous : « j'y penserai pendant mon temps des soucis »",
                            "Pendant votre temps des soucis, parcourez la liste : prévoyez un pas pour ce que vous pouvez faire, lâchez ce que vous ne pouvez pas",
                            "Souvent, à l'heure prévue, le sujet ne semble plus important"),
                        Practice(TechniqueId.Observer, "Position d'observateur")
                    ],
                    "Fixez-vous un temps des soucis et reportez-y les pensées anxieuses.",
                    "Avez-vous réussi à reporter les soucis ? Que sont devenus les sujets à l'heure prévue ?"),
                new(
                    "Béquilles et évitement",
                    4,
                    [
                        P("Ce qui entretient le plus l'anxiété, c'est l'évitement. Il peut être ouvert (je n'y vais pas, je n'appelle pas) ou caché : des béquilles qui aident à traverser une situation."),
                        Check("Ai-je des béquilles ?", 2, "Vous avez plusieurs béquilles. Elles entretiennent la peur : le cerveau croit que tout s'est bien passé uniquement grâce à elles. Essayez d'en abandonner une dans une situation facile.", "Vous avez peu de béquilles. Remarquez aussi ce que vous évitez complètement.", "J'ai sur moi un comprimé, de l'eau ou un téléphone chargé « au cas où »", "Je ne vais dans les endroits difficiles qu'accompagné", "Je m'assois près de la sortie", "Je me distrais pour ne pas remarquer l'anxiété", "Je vérifie ou redemande de nombreuses fois"),
                        P("La sortie consiste à rencontrer progressivement ce qui fait peur et à se passer des béquilles. Le cerveau apprend alors par l'expérience que ce qu'on redoute n'arrive pas ou est supportable.")
                    ],
                    "Faites une liste de trois à cinq situations que vous évitez, de la plus facile à la plus difficile.",
                    "Quelle situation de la liste êtes-vous prêt à essayer en premier ?"),
                new(
                    "Le premier pas et un plan",
                    4,
                    [
                        P("Le moyen le plus efficace d'affaiblir l'anxiété est l'exposition : rencontrer progressivement la peur. Inutile d'aller tout de suite au plus effrayant : commencez par un pas facile et vérifiez une prédiction."),
                        List(
                            "Avant le pas, notez ce que vous craignez et à quel point vous êtes sûr que cela arrivera",
                            "Faites le pas sans béquilles et restez dans la situation jusqu'à avoir vérifié la prédiction",
                            "Ensuite, notez ce qui s'est réellement passé"),
                        Ladder("Échelle de la peur"),
                        H("Ce qu'il faut continuer"),
                        List(
                            "La respiration lente au pic de l'anxiété",
                            "La vérification des pensées et le temps des soucis",
                            "Des marches sur l'échelle de la peur tous les quelques jours"),
                        P("Refaites l'échelle GAD-7 et comparez avec le début. Si l'anxiété est forte ou gêne votre vie, consultez un psychothérapeute : la TCC de l'anxiété est bien étudiée."),
                        TestsTab("Échelle GAD-7", "gad7")
                    ],
                    "Faites le premier pas sur votre échelle de la peur et notez ce qui s'est passé.",
                    "Qu'aviez-vous prédit, et que s'est-il réellement passé ?")
            ]),
        new(
            CourseIds.Mood,
            "Sortir de l'apathie",
            "Six jours d'activation comportementale : de petits pas pour retrouver l'énergie et ce qui fait plaisir",
            "Cloud",
            "phq9",
            "PHQ-9",
            [
                new(
                    "Le cercle de l'inaction",
                    4,
                    [
                        P("Quand l'humeur est à zéro, il est naturel d'en faire moins : pas d'énergie, rien ne tente. Mais moins on en fait, moins il y a dans la vie de choses qui apportent de la joie et le sentiment « j'y suis arrivé », et l'humeur baisse encore."),
                        Cycle("Le cercle de l'inaction", "Pas d'énergie", "J'en fais moins", "Moins de joie et de réussites", "Encore moins d'énergie"),
                        P("L'activation comportementale brise ce cercle par l'autre bout : ne pas attendre d'en avoir envie, mais faire de petits pas après lesquels l'énergie et l'envie reviennent peu à peu. C'est l'une des méthodes d'aide les mieux étudiées contre la dépression."),
                        P("Remplissez l'échelle PHQ-9 au début du cours : à la fin, vous comparerez le résultat."),
                        TestsTab("Échelle PHQ-9", "phq9"),
                        Warning("Si des pensées de mort apparaissent"),
                        P("Si vous avez des pensées qu'il vaudrait mieux que vous ne soyez pas là, ne restez pas seul avec elles : ce cours ne remplace pas l'aide d'un spécialiste."),
                        Crisis("J'ai besoin d'aide maintenant")
                    ],
                    "Aujourd'hui et demain, notez ce que vous faites et évaluez votre humeur de 0 à 10 plusieurs fois par jour.",
                    "Comment se passent vos journées en ce moment ? Qu'avez-vous cessé de faire ces derniers temps ?"),
                new(
                    "Ce qui remonte l'humeur",
                    4,
                    [
                        P("Regardez vos observations : à quels moments l'humeur était-elle un peu plus haute ? Souvent, c'est quelque chose de petit : une conversation, une promenade, une douche, une tâche accomplie."),
                        H("Deux types d'activités utiles"),
                        List(
                            "Les agréables : ce qui apporte du plaisir, même faible — musique, bonne nourriture, soleil",
                            "Les « j'y suis arrivé » : ce qui laisse un sentiment d'accomplissement — faire la vaisselle, répondre à un e-mail"),
                        P("Dans la dépression, le plaisir des choses agréables est plus faible qu'avant. C'est normal : ce qui compte, c'est de faire, sans attendre la joie d'avant.")
                    ],
                    "Faites une liste de dix activités : cinq agréables et cinq « j'y suis arrivé ». Plus elles sont petites et simples, mieux c'est.",
                    "Qu'avez-vous mis dans votre liste ? Y a-t-il eu quelque chose qui a un peu remonté votre humeur ?"),
                new(
                    "De petits pas",
                    4,
                    [
                        P("Les grands projets font peur dans l'apathie et ne se réalisent pas. Il aide de découper : pas « ranger l'appartement » mais « enlever trois objets de la table »."),
                        List(
                            "Choisissez une activité de la liste pour aujourd'hui",
                            "Découpez-la jusqu'à un pas qui prend cinq minutes",
                            "Fixez un moment et un lieu : « après le petit-déjeuner, dans la cuisine »",
                            "Ensuite, notez comment votre humeur a changé"),
                        Practice(TechniqueId.SmallStep, "Un petit pas")
                    ],
                    "Chaque jour, faites une activité agréable et une activité « j'y suis arrivé » au moment prévu.",
                    "Qu'avez-vous réussi à faire ? Comment votre humeur a-t-elle changé ensuite ?"),
                new(
                    "« Je n'en ai pas envie »",
                    4,
                    [
                        P("Le principal obstacle est la pensée « je n'en ai pas envie, je le ferai quand j'aurai de l'énergie ». Dans l'apathie, l'envie vient après l'action, pas avant."),
                        Quiz("Pas d'énergie, et une promenade est prévue aujourd'hui. Que vaut-il mieux faire ?", 1, "Dans l'apathie, l'envie revient après l'action. Même cinq minutes envoient au corps et à l'humeur le signal « j'ai fait quelque chose ».", "Attendre d'en avoir envie", "Sortir au moins cinq minutes", "Reporter au week-end"),
                        H("D'autres pensées qui freinent"),
                        List(
                            "« Ça ne sert à rien » : vérifiez après coup — quelque chose a-t-il un peu changé ?",
                            "« C'est trop petit » : c'est justement de petits pas qu'est faite la sortie",
                            "« Avant, j'en faisais plus » : comparez-vous à hier, pas à vous d'avant"),
                        Practice(TechniqueId.SelfCompassion, "Des mots bienveillants pour soi")
                    ],
                    "Quand vous n'en avez pas envie, faites au moins une version de cinq minutes de ce que vous aviez prévu.",
                    "Quelle pensée vous empêche le plus souvent de commencer ? Qu'est-ce qui vous a aidé à la contourner ?"),
                new(
                    "Les gens autour de vous",
                    4,
                    [
                        P("Dans l'apathie, on veut se couper de tout le monde. Pourtant, le contact avec les autres est l'un des remèdes les plus puissants contre une humeur basse, même si en ce moment il n'apporte pas autant de joie qu'avant."),
                        List(
                            "Écrivez à une personne avec qui vous n'avez pas été en contact depuis un moment",
                            "Une courte rencontre ou un appel vaut mieux que rien",
                            "Vous n'êtes pas obligé de tout expliquer : « c'est difficile en ce moment, allons juste marcher »",
                            "Si l'énergie manque, choisissez des personnes avec qui c'est facile"),
                        Practice(TechniqueId.SmallStep, "Un petit pas")
                    ],
                    "Prenez contact avec au moins une personne dans les deux prochains jours.",
                    "Avec qui avez-vous pris contact ? Comment cela s'est-il passé ?"),
                new(
                    "Ce qui compte pour moi",
                    4,
                    [
                        P("Pour que les pas ne deviennent pas des corvées, il aide de les relier à ce qui compte pour vous : vos proches, la santé, le travail, la créativité, la nature."),
                        List(
                            "Choisissez un domaine qui compte pour vous",
                            "Réfléchissez à qui vous voulez être dans ce domaine",
                            "Choisissez un petit pas dans cette direction pour la semaine"),
                        H("Ce qu'il faut continuer"),
                        List(
                            "Chaque jour, une activité agréable et une « j'y suis arrivé »",
                            "De petits pas plutôt que de grands projets",
                            "Le contact avec les autres"),
                        P("Refaites l'échelle PHQ-9 et comparez avec le début. Si le résultat est élevé ou ne change pas, consultez un médecin ou un psychothérapeute : la dépression se soigne très bien."),
                        TestsTab("Échelle PHQ-9", "phq9"),
                        Crisis("J'ai besoin d'aide maintenant")
                    ],
                    "Faites un pas cette semaine dans un domaine qui compte pour vous.",
                    "Qu'est-ce qui compte pour vous ? Quel pas avez-vous choisi ?")
            ]),
        new(
            CourseIds.Trauma,
            "Après un traumatisme",
            "Cinq jours sur ce qu'on peut faire sans risque seul : comprendre sa réaction, faire face aux reviviscences et aux cauchemars, trouver un spécialiste",
            "Healing",
            "pcl5",
            "PCL-5",
            [
                new(
                    "Une réaction normale à un événement anormal",
                    4,
                    [
                        P("Après un événement difficile, presque tout le monde a des souvenirs envahissants, des cauchemars, de l'anxiété, de l'engourdissement, de l'irritabilité. Ce n'est pas une faiblesse, mais la réaction d'un cerveau qui essaie de vous protéger contre une répétition."),
                        P("Ce cours ne traitera pas le traumatisme : cela demande une thérapie avec un spécialiste. Il vous aidera à comprendre ce qui vous arrive, à faire face aux moments difficiles et à arriver préparé chez un spécialiste. Dans ce cours, vous n'avez pas besoin de vous rappeler ni de décrire l'événement lui-même."),
                        P("Remplissez l'échelle PCL-5 au début : à la fin, vous comparerez le résultat."),
                        TestsTab("Échelle PCL-5", "pcl5"),
                        Warning("Si le danger continue"),
                        P("Si vous êtes menacé en ce moment, ou si des pensées de vous faire du mal sont apparues, la sécurité passe en premier."),
                        Crisis("J'ai besoin d'aide maintenant")
                    ],
                    "Notez quand les choses deviennent plus difficiles dans la journée, sans détails de l'événement : l'heure, le lieu, ce qu'il y avait autour.",
                    "Lesquelles de ces réactions reconnaissez-vous chez vous ?"),
                new(
                    "Revenir au présent",
                    4,
                    [
                        P("Une reviviscence est un souvenir revécu comme s'il se produisait maintenant. Le moment, le lieu, l'appui du corps et les sens aident à revenir."),
                        Practice(TechniqueId.Flashback, "Un souvenir a pris le dessus"),
                        Practice(TechniqueId.Grounding, "Ancrage 5-4-3-2-1"),
                        H("Votre trousse d'ancrage"),
                        List(
                            "Une phrase : « C'est un souvenir. C'est un autre moment maintenant, je suis en sécurité »",
                            "Un objet à garder sur vous : un galet, une bague, quelque chose de parfumé",
                            "Une personne à qui vous pouvez écrire",
                            "Quelque chose de froid : de l'eau, de la glace"),
                        Breathe("L'expiration plus longue que l'inspiration")
                    ],
                    "Préparez votre trousse d'ancrage et essayez la pratique « Un souvenir a pris le dessus » à un moment calme, pour la connaître à l'avance.",
                    "Qu'avez-vous mis dans votre trousse d'ancrage ?"),
                new(
                    "Le sommeil et les cauchemars",
                    4,
                    [
                        P("Après un traumatisme, le sommeil souffre souvent : on a du mal à s'endormir, le sommeil est léger, des cauchemars viennent. Le manque de sommeil rend les journées plus anxieuses et plus irritables."),
                        List(
                            "Une seule heure de lever chaque jour",
                            "De la lumière le matin, une soirée calme sans les informations",
                            "Si un cauchemar vous réveille : lumière, où suis-je et quel jour on est, de l'eau, des expirations lentes",
                            "L'alcool renforce les cauchemars en seconde partie de nuit"),
                        P("Un cauchemar récurrent peut être réécrit : la répétition d'imagerie mentale rend les cauchemars plus rares et plus faibles. Commencez par un plus léger."),
                        Practice(TechniqueId.Nightmare, "Réécrire un cauchemar")
                    ],
                    "Gardez une seule heure de lever. Si vous avez des cauchemars récurrents, essayez d'en réécrire un, en commençant par un plus léger.",
                    "Comment dormez-vous en ce moment ? Qu'est-ce qui aide après une nuit difficile ?"),
                new(
                    "Le soutien et les autres",
                    4,
                    [
                        P("Le traumatisme éloigne souvent des autres et de ce qui comptait avant. Le soutien des proches est l'un des facteurs les plus puissants de rétablissement."),
                        List(
                            "Vous n'êtes pas obligé de parler de l'événement : vous pouvez simplement être ensemble",
                            "Dites à vos proches comment vous aider dans un moment difficile",
                            "Ramenez peu à peu ce qui vous soutenait : promenades, mouvement, choses que vous aimiez",
                            "N'anesthésiez pas vos émotions avec l'alcool : cela gêne le rétablissement"),
                        P("Éviter ce qui rappelle l'événement est compréhensible, mais avec le temps cela rétrécit la vie et garde le traumatisme « brûlant ». Il vaut mieux y travailler avec un spécialiste que se forcer seul."),
                        P("Ce que l'événement a changé dans votre façon de penser à vous — la culpabilité, « quelque chose ne va pas chez moi », « on ne peut faire confiance à personne » — peut être travaillé dans le chat, sans raconter ce qui s'est passé."),
                        TraumaChat("Y travailler dans le chat"),
                        Practice(TechniqueId.SelfCompassion, "Des mots bienveillants pour soi")
                    ],
                    "Faites une chose qui vous soutenait autrefois, et passez du temps avec quelqu'un de confiance.",
                    "Qui et qu'est-ce qui vous soutient en ce moment ?"),
                new(
                    "Le chemin vers un spécialiste",
                    5,
                    [
                        P("Les symptômes post-traumatiques répondent bien à la psychothérapie centrée sur le traumatisme : exposition prolongée, thérapie cognitive, EMDR, exposition écrite. Elle demande généralement 8 à 16 séances."),
                        See(ArticleIds.TraumaTreatment, "Comment on soigne un traumatisme et comment choisir un spécialiste"),
                        H("Comment se préparer"),
                        List(
                            "Refaites l'échelle PCL-5 et comparez avec le début",
                            "Exportez un résumé de l'application pour le spécialiste : « Profil → Paramètres → Données et sauvegarde »",
                            "Notez vos questions : quelle méthode, combien de séances, comment les progrès seront mesurés",
                            "Vous n'êtes pas obligé de tout raconter sur l'événement au premier rendez-vous"),
                        TestsTab("Échelle PCL-5", "pcl5"),
                        Crisis("J'ai besoin d'aide maintenant")
                    ],
                    "Trouvez un ou deux spécialistes qui travaillent avec le traumatisme et prenez rendez-vous pour une consultation.",
                    "Qu'est-ce qui a été le plus utile dans le cours ? De quoi voulez-vous parler avec le spécialiste ?")
            ])
    ];
}
