using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentPt
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("O que é", what),
        new("Quando ajuda", when),
        new("Como é a prática", how),
        new("Exemplo", example)
    ];

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["Uma briga com alguém próximo", "Algo no trabalho", "Um conflito com meu chefe", "Os estudos ou uma prova", "A saúde", "Dinheiro", "A solidão", "Um fracasso", "Um término", "A relação com meus pais", "Os filhos", "Uma mudança de casa", "O futuro", "Críticas a mim"];

    private static readonly string[] BackupOptions =
        ["Pedir ajuda", "Esperar e tentar de novo", "Encontrar outro caminho", "Cobrar menos de mim", "Fazer uma pausa", "Conversar com alguém próximo", "Procurar um especialista", "Arrumar um trabalho extra", "Fazer um curso", "Dividir a tarefa em etapas", "Adiar a decisão", "Começar pequeno"];

    private static readonly string[] HarshThoughts =
        ["\"Vou estragar tudo\"", "\"Vão me julgar\"", "\"Não dou conta\"", "\"Ninguém precisa de mim\"", "\"É um desastre\"", "\"Nada vai dar certo\"", "\"Sou pior que os outros\"", "\"Tudo sempre dá errado\"", "\"A culpa é minha\"", "\"Tem algo errado comigo\"", "\"Isso nunca vai acabar\"", "\"Tenho que ser perfeito\""];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Prática", "Spin",
        [
            "1. Descreva o episódio que incomoda você",
            "2. Dê nome ao sentimento principal",
            "3. Avalie o sentimento de 0 a 10"
        ],
        "O Spin de Zivorad Slavinski ajuda a reduzir a carga emocional de uma lembrança dolorosa.",
        TechniqueUiKind.Entry, "Técnica nº 1", "26.01.2023", "Spin", "Reduzir a carga emocional em 3 passos", "Momento difícil", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Episódio", "Ex.: a conversa de ontem que não sai da cabeça", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentimento principal", "Mágoa, medo, raiva…", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Intensidade do sentimento", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Uma técnica simples de neutralização: você nomeia o episódio, o sentimento e a intensidade. Muitas vezes, só perceber já reduz a carga.",
            "Quando uma cena ou um pensamento fica girando e não solta a ansiedade ou o ressentimento.",
            "Escreva o episódio e o sentimento e avalie a intensidade com sinceridade. Repita outro dia se precisar.",
            "Episódio: \"Um colega ignorou minha ideia\". Sentimento: mágoa. Nota: 8 → depois 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "Prática", "Comparar a importância",
        [
            "1. Diga qual é o problema",
            "2. O que era importante antes nessa área?",
            "3. O que é importante agora?",
            "4. O que vai ser importante no futuro?",
            "5. Compare as três respostas e anote o que mudou"
        ],
        "Comparar a importância ao longo do tempo ajuda a colocar um problema no contexto da sua vida.",
        TechniqueUiKind.Entry, "Técnica nº 2", "26.01.2023", "Comparar a importância", "Quando o problema parece grande demais", "Tamanho do problema", "PNL", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problema", "O que parece insuportável agora?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Era importante antes", "Ex.: estudos, relacionamentos, carreira", Suggestions: ["Os estudos", "A família", "Os amigos", "A saúde", "A carreira", "Os hobbies", "O esporte", "O amor", "Viagens", "A liberdade", "O reconhecimento", "O lar"]),
            new TechniqueEntrySeed("É importante agora", "O que mais ocupa sua atenção?", Suggestions: ["O trabalho", "A família", "A saúde", "Dinheiro", "Os relacionamentos", "O descanso", "Os filhos", "Os amigos", "Os estudos", "O sono", "Paz de espírito", "Crescimento pessoal"]),
            new TechniqueEntrySeed("Vai ser importante no futuro", "Para onde você quer ir?", Suggestions: ["Estabilidade", "A família", "A liberdade", "A saúde", "Crescer", "As pessoas que eu amo", "Uma casa própria", "Um trabalho que eu ame", "Viagens", "Calma", "Os filhos", "Uma reserva de dinheiro"]),
            new TechniqueEntrySeed("O que mudou?", "O problema ainda domina tudo?", Suggestions: ["O problema é só uma parte da vida", "Há mais coisas importantes do que parecia", "É uma fase passageira", "Ficou menos intenso", "Estou mais calmo", "Vejo que já dei conta antes", "Surgiu esperança", "Nada mudou ainda"])
        ],
        Theory(
            "A técnica mostra que o peso de um problema muda com o tempo e raramente fica no centro para sempre.",
            "Quando uma dificuldade ocupa a tela inteira e parece permanente.",
            "Preencha os quatro campos e compare: outros valores costumam diminuir o problema.",
            "Problema: \"Estou sem emprego\". Antes: os estudos. Agora: procurar emprego. Futuro: estabilidade. Percepção: uma fase, não a vida inteira."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "Prática", "Polaridades",
        [
            "1. Escreva o que você gosta na situação",
            "2. Escreva o que você não gosta",
            "3. Compare os pares e perceba o equilíbrio"
        ],
        "O método das polaridades ajuda a ver os dois lados e a reduzir a tensão interna.",
        TechniqueUiKind.Polarity, "Técnica nº 3", "26.01.2023", "Polaridades", "Ver os dois lados de uma situação", "Lados diferentes", "Zivorad Slavinski", 5, "Contrast",
        null,
        Theory(
            "Toda situação tem lados agradáveis e desagradáveis. Nomear os dois reduz a fixação em um só.",
            "Quando você só vê o negativo ou só o positivo e se sente travado.",
            "Adicione pares de \"gosto / não gosto\" sobre um tema. Compare as listas sem julgar o que é certo.",
            "Trabalho: gosto do salário, não gosto do horário. Casa: gosto do aconchego, não gosto do barulho dos vizinhos."),
        "O que você não gosta", "O que você gosta");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "Prática", "Folha de papel",
        [
            "1. Escreva o pensamento no papel (ou no campo abaixo)",
            "2. Leia em voz alta uma vez",
            "3. Dobre, rasgue ou solte simbolicamente a folha"
        ],
        "Escrever os pensamentos no papel reduz a carga emocional deles.",
        TechniqueUiKind.Paper, "Técnica nº 4", "26.01.2023", "Folha de papel", "Quando os pensamentos ficam girando", "Pensamentos", "Psique", 3, "Description",
        [new TechniqueEntrySeed("Pensamento", "Ex.: \"Vou estragar a apresentação\"", Suggestions: HarshThoughts)],
        Theory(
            "Escrever tira o pensamento da cabeça. Destruir a folha simboliza deixar ir.",
            "Quando a mesma frase gira por horas e atrapalha o sono ou a concentração.",
            "Descarregue o pensamento, leia e depois destrua a folha — de verdade ou na imaginação.",
            "Pensamento: \"Vão me achar incompetente\". Depois de escrever e queimar a folha, a ansiedade costuma diminuir."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "Prática", "50 anos depois",
        [
            "1. Diga qual é o problema",
            "2. Avalie a importância dele daqui a 50 anos, de 0 a 10"
        ],
        "Levar um problema para frente no tempo reduz a importância que ele parece ter.",
        TechniqueUiKind.Entry, "Técnica nº 5", "30.01.2023", "50 anos depois", "Levar o problema para o futuro", "Tamanho do problema", "PNL", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problema", "O que parece um desastre hoje?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Importância daqui a 50 anos", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Imaginar um acontecimento lá no futuro costuma tirar a força dele — você o vê com mais distância.",
            "Quando um contratempo parece o fim do mundo, mesmo sendo passageiro.",
            "Diga qual é o problema e avalie quanto ele vai importar daqui a cinquenta anos. A nota costuma ser menor.",
            "Problema: \"Não me chamaram para a reunião\". Daqui a 50 anos: 1–2 de 10."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "Prática", "Mancha branca",
        [
            "1. Lembre o episódio que incomoda",
            "2. Imagine-o como uma mancha branca sem detalhes",
            "3. Mantenha a mancha até a tensão diminuir"
        ],
        "Tirar os detalhes de uma lembrança costuma reduzir a carga emocional dela.",
        TechniqueUiKind.Entry, "Técnica nº 6", "30.01.2023", "Mancha branca", "Suavizar uma lembrança dolorosa", "Momento difícil", "PNL", 3, "BlurOn",
        [new TechniqueEntrySeed("Episódio", "Em poucas palavras: o que aconteceu e o que mais dói", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Os detalhes alimentam o sentimento. A mancha branca tira os gatilhos e dá uma pausa ao sistema nervoso.",
            "Quando uma imagem vívida do passado vem carregada de emoção.",
            "Lembre o episódio e troque a imagem por uma simples mancha branca por 30–60 segundos.",
            "No lugar do rosto de quem magoou você — uma mancha branca. A respiração se acalma e a mente volta ao agora."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "Prática", "Modificação da experiência",
        [
            "1. Escolha uma área para trabalhar",
            "2. Descreva sua atitude em relação a ela",
            "3. Diga quais crenças estão ligadas a ela",
            "4. Como a atitude muda quando você pensa em experiências passadas?",
            "5. Descreva uma experiência negativa",
            "6. Avalie a experiência negativa (−10…10)",
            "7. Que experiência positiva poderia substituí-la?",
            "8. Como sua atitude mudaria com essa experiência?",
            "9. Avalie a nova experiência (−10…10)"
        ],
        "A EMO ajuda a substituir uma experiência limitante por outra que apoie mais.",
        TechniqueUiKind.Entry, "Técnica nº 7", "08.02.2023", "Modificação da experiência", "Reescrever uma experiência limitante", "Momento difícil", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Área", "Relacionamentos, trabalho, saúde…", Suggestions: ["Os relacionamentos", "O trabalho", "A saúde", "Dinheiro", "Falar em público", "A família", "Os estudos", "A aparência", "A amizade", "A criatividade", "O esporte", "A carreira"]),
            new TechniqueEntrySeed("Atitude em relação à área", "Como você se sente em relação a isso agora?", Suggestions: ["Ansiedade", "Insegurança", "Irritação", "Indiferença", "Interesse", "Cansaço", "Medo", "Vergonha", "Esperança", "Raiva", "Confusão", "Calma"]),
            new TechniqueEntrySeed("Crenças ligadas", "Ex.: \"Não vou conseguir\"", Suggestions: ["\"Não vou conseguir\"", "\"Não sou bom o bastante\"", "\"Não vão me aceitar\"", "\"Nada vai mudar\"", "\"Tenho que ser perfeito\"", "\"Não tenho sorte\"", "\"Não mereço\"", "\"Os outros são melhores\"", "\"Errar não é permitido\"", "\"Estou velho demais para isso\""]),
            new TechniqueEntrySeed("Atitude e experiência passada", "O que muda quando você lembra do passado?", Suggestions: ["Fica mais pesado", "Fica mais calmo", "Nada muda", "Vem a raiva", "Vem a tristeza", "Vem o medo", "Lembro que dei conta", "Quero evitar"]),
            new TechniqueEntrySeed("Experiência negativa", "Um episódio específico ou um padrão"),
            new TechniqueEntrySeed("Nota da experiência negativa", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Substituição positiva", "Que experiência de apoio é possível?", Suggestions: ["Uma vez em que consegui", "Uma vez em que me apoiaram", "Uma vez em que dei conta sozinho", "Uma vez em que me elogiaram", "Uma vez em que ajudei alguém", "Uma vez em que aprendi algo novo", "Uma vez em que não desisti", "Uma vez em que me aceitaram como sou"]),
            new TechniqueEntrySeed("Atitude com o positivo", "Como mudaria seu olhar sobre a área?", Suggestions: ["Surge esperança", "Mais calma", "Quero tentar de novo", "Fica interessante", "Surge confiança", "Fica mais leve", "Sinto força", "Ainda é difícil acreditar"]),
            new TechniqueEntrySeed("Nota da nova experiência", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "As crenças muitas vezes se apoiam em uma única experiência negativa marcante. Substituí-la de forma consciente pode mudar sua atitude.",
            "Quando \"sempre foi assim\" e a mudança parece impossível.",
            "Preencha os campos em ordem. Compare as duas notas — a ideia é que a segunda seja maior.",
            "Área: falar em público. Negativa: uma prova em que fui mal (−8). Substituição: uma boa apresentação (+6)."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "Prática", "Repita isso",
        [
            "1. Escreva a frase que incomoda você",
            "2. Repita em voz alta como se a ouvisse pela primeira vez",
            "3. Continue repetindo até a emoção diminuir"
        ],
        "Repetir a frase de um problema \"como se fosse a primeira vez\" reduz a intensidade emocional.",
        TechniqueUiKind.Copied, "Técnica nº 8", "21.01.2025", "Repita isso", "Ouvir o problema como se fosse a primeira vez", "Tamanho do problema", "PNL", 2, "Repeat",
        [new TechniqueEntrySeed("Frase", "Tenho medo de perder meu emprego", Suggestions: HarshThoughts)],
        Theory(
            "Uma frase conhecida perde a carga quando é dita devagar e em tom neutro, sem drama.",
            "Quando um pensamento soa como uma sentença na sua cabeça.",
            "Escreva a frase e repita 5–10 vezes com pausas, percebendo as sensações do corpo.",
            "\"Ninguém precisa de mim\" — depois de sete repetições, costuma soar absurdo e mais leve."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "Prática", "Plano B",
        [
            "1. Diga qual é o problema",
            "2. Escreva três alternativas realistas",
            "3. Perceba como a ansiedade muda quando existem saídas"
        ],
        "Ter alternativas reduz a importância exagerada de um único resultado.",
        TechniqueUiKind.Entry, "Técnica nº 9", "21.01.2025", "Plano B", "Diminuir a pressão com um plano B", "Tamanho do problema", "PNL", 3, "Backup",
        [
            new TechniqueEntrySeed("Problema", "E se o plano principal falhar?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Alternativa 1", "Ex.: pegar um projeto como freelancer", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Alternativa 2", "Ex.: me capacitar para uma função parecida", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Alternativa 3", "Ex.: cortar gastos por um tempo", Suggestions: BackupOptions)
        ],
        Theory(
            "Quando tudo parece tudo ou nada, a mente trava. Alternativas devolvem a sensação de escolha.",
            "Antes de entrevistas, provas ou situações em que falhar parece uma catástrofe.",
            "Diga qual é o problema e três planos B possíveis — mesmo que modestos.",
            "Problema: \"Não vou conseguir a vaga\". Plano B: estágio, indicações, outra cidade."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "Prática", "Diminua",
        [
            "1. Descreva o que incomoda você",
            "2. Imagine isso como um objeto à sua frente",
            "3. Diminua e afaste o objeto até o interesse sumir"
        ],
        "Diminuir e afastar visualmente um problema reduz o interesse emocional por ele.",
        TechniqueUiKind.Entry, "Técnica nº 10", "22.01.2025", "Diminua", "Deixar o problema pequeno e distante", "Tamanho do problema", "PNL", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("O que incomoda você", "Ex.: um conflito que tira seu sono", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "O tamanho e a distância de uma imagem afetam a importância dela. Um objeto pequeno e distante dificilmente mexe com os nervos.",
            "Quando um problema parece enorme e bem na sua frente.",
            "Crie uma imagem, diminua aos poucos e empurre até o horizonte por 10–20 segundos.",
            "Uma briga vira um pontinho. A respiração fica mais leve e os pensamentos clareiam."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "Prática", "Teste",
        [
            "1. Descreva aquilo em que você fica girando",
            "2. Pare de pensar nisso de propósito por 1–2 minutos",
            "3. Perceba o que mudou no corpo e no humor"
        ],
        "Atenção demais infla a importância. Uma pausa curta testa o quanto o pensamento controla você.",
        TechniqueUiKind.Entry, "Técnica nº 11", "22.01.2025", "Teste", "Ver o que acontece se você soltar", "Tamanho do problema", "PNL", 2, "FactCheck",
        [new TechniqueEntrySeed("Aquilo em que você fica girando", "Que pensamento volta sempre?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Se um pensamento fosse vital para sobreviver, soltá-lo pareceria perigoso. O experimento costuma mostrar que nada catastrófico acontece.",
            "Quando você não consegue parar de remoer o mesmo assunto.",
            "Dê nome ao pensamento e leve a atenção com calma para a respiração ou para os sons ao seu redor.",
            "Pensamento: \"E se eu estiver doente\". Depois de uma pausa, a ansiedade costuma cair de 7 para 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "Prática", "Posição de observador",
        [
            "1. Descreva a situação que incomoda você",
            "2. Escreva os sentimentos em primeira pessoa",
            "3. Olhe a situação como um observador neutro",
            "4. Avalie a tensão depois de mudar de posição"
        ],
        "Mudar a posição de percepção ajuda a se afastar de um pico emocional e ver a situação de forma mais ampla.",
        TechniqueUiKind.Entry, "Técnica nº 12", "12.06.2026", "Posição de observador", "Se afastar das emoções e ver a situação de fora", "Momento difícil", "PNL", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situação", "O que aconteceu ou o que você fica repassando na cabeça?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentimentos em primeira pessoa", "O que você sente estando dentro da situação?", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("O que o observador vê", "O que um observador de fora, calmo, perceberia?", Suggestions: ["Duas pessoas discutindo", "Uma pessoa cansada e chateada", "Uma situação comum de trabalho", "Cada um defende seu lado", "Em uma semana isso vai estar esquecido", "As pessoas se entenderam mal", "Uma pessoa se esforçando muito", "Desagradável, mas não perigoso", "Cada um tem seu ponto de vista", "A pessoa precisa descansar"]),
            new TechniqueEntrySeed("Tensão agora", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Quando você está dentro de um episódio, as emoções parecem inevitáveis. A posição de observador cria distância sem negar a experiência.",
            "Quando uma situação sobrecarrega você e argumentos racionais não ajudam.",
            "Descreva a situação e os sentimentos e depois reescreva em terceira pessoa — de forma neutra, sem julgamento.",
            "Discussão: de dentro — \"não me respeitam\". Observador: \"duas pessoas falam alto\". Tensão: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "Prática", "Âncora de recurso",
        [
            "1. Lembre um momento em que você se sentiu confiante ou calmo",
            "2. Descreva as sensações do corpo",
            "3. Escolha um gesto simples como âncora",
            "4. Avalie a força do estado de recurso"
        ],
        "Uma âncora liga um gesto do corpo a um estado de recurso para você voltar a ter apoio mais rápido.",
        TechniqueUiKind.Entry, "Técnica nº 13", "12.06.2026", "Âncora de recurso", "Recuperar o apoio com um gesto e uma lembrança", "Momento difícil", "PNL", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Lembrança de recurso", "Quando você se sentiu forte, calmo ou alegre?", Suggestions: ["Uma apresentação que deu certo", "Férias na praia", "O abraço de alguém próximo", "Terminar algo difícil", "Uma caminhada na natureza", "Passar em uma prova", "O nascimento de um filho", "Vencer uma competição", "Uma noite com amigos", "Um elogio do meu chefe", "O primeiro dia em um emprego novo", "Uma manhã sem pressa"]),
            new TechniqueEntrySeed("Sensações do corpo", "Onde você sente isso? Calor, leveza, firmeza…", Suggestions: ["Calor no peito", "Leveza", "Ombros abertos", "Respiração tranquila", "Pernas firmes", "Um sorriso", "Um coração calmo", "As costas retas", "Energia nas mãos", "A barriga relaxada"]),
            new TechniqueEntrySeed("Gesto ou âncora", "Ex.: fechar a mão, tocar o pulso", Suggestions: ["Fechar a mão", "Tocar o pulso", "Juntar o polegar e o indicador", "A mão no coração", "Uma respiração profunda", "Apertar o lóbulo da orelha", "Abrir os ombros", "Acariciar a palma da mão"]),
            new TechniqueEntrySeed("Força do estado", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "O cérebro liga rapidamente um gesto a um estado se você repetir várias vezes o par \"lembrança + gesto\".",
            "Antes de algo estressante ou quando falta apoio interno.",
            "Lembre o episódio de recurso, intensifique as sensações do corpo e fixe-as com um gesto simples.",
            "Recurso: uma apresentação que deu certo. Corpo: calor no peito. Âncora: toque no polegar. Força: 8."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "Prática", "Ancoragem 5-4-3-2-1",
        [
            "1. Diga 5 coisas que você vê",
            "2. Diga 4 coisas que você sente com o corpo",
            "3. Diga 3 sons ao seu redor",
            "4. Diga 2 cheiros",
            "5. Diga 1 sabor ou uma sensação agradável"
        ],
        "A ancoragem pelos sentidos leva a atenção dos pensamentos ansiosos para o momento presente.",
        TechniqueUiKind.Entry, "Técnica nº 14", "12.06.2026", "Ancoragem 5-4-3-2-1", "Voltar ao presente pelos sentidos", "Momento difícil", "Psique", 3, "Nature",
        [
            new TechniqueEntrySeed("5 coisas que vejo", "Ex.: janela, xícara, livro, luminária, mão", Suggestions: ["Janela", "Mesa", "Celular", "Computador", "Notebook", "Monitor", "Teclado", "Xícara", "Luminária", "Mão", "Porta", "Livro", "Cadeira", "Parede", "Quadro", "Planta", "Relógio", "Armário", "Céu", "Carro"]),
            new TechniqueEntrySeed("4 coisas que sinto com o corpo", "Ex.: a cadeira, o chão, a roupa, a temperatura do ar", Suggestions: ["Cadeira", "O chão sob os pés", "A roupa", "O frescor", "O calor", "O celular na mão", "O encosto da cadeira", "O tecido na pele", "A respiração no peito", "O vento", "O mouse", "As chaves", "O travesseiro", "O relógio no pulso"]),
            new TechniqueEntrySeed("3 sons", "Ex.: a rua, o relógio, sua respiração", Suggestions: ["A rua", "Relógio", "A respiração", "Vozes", "A geladeira", "O vento", "O silêncio", "Computador", "Os carros", "Os pássaros", "Música", "O ar-condicionado", "Passos", "A chuva"]),
            new TechniqueEntrySeed("2 cheiros", "Ex.: café, ar fresco", Suggestions: ["Café", "Ar fresco", "Sabonete", "Comida", "Perfume", "Quase nada", "Chá", "Pão", "Livros", "Flores", "Creme para as mãos", "Madeira"]),
            new TechniqueEntrySeed("1 sabor ou sensação agradável", "Ex.: água na boca, um tecido macio", Suggestions: ["Água", "Chá", "Um tecido macio", "As palmas quentes", "A boca fresca", "Café", "Um chiclete de menta", "Chocolate", "Uma maçã", "Um cobertor quentinho"])
        ],
        Theory(
            "A ansiedade costuma puxar você para o passado ou para o futuro. Contar pelos sentidos traz você de volta ao aqui e agora.",
            "Em momentos de pânico, ansiedade forte, tontura ou sensação de estar desligado da realidade.",
            "Passe pelas etapas devagar, dizendo sensações concretas. Respire com calma entre um item e outro.",
            "5 vejo → 4 sinto → 3 ouço → a respiração se acalma, os pensamentos desaceleram."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "Prática", "Respiração quadrada",
        [
            "1. Avalie a tensão antes da prática",
            "2. Inspire contando até 4",
            "3. Segure contando até 4",
            "4. Expire contando até 4",
            "5. Segure contando até 4 — repita 4 ciclos e avalie a tensão depois"
        ],
        "Respirar com ritmo acalma o sistema nervoso e reduz a tensão do corpo.",
        TechniqueUiKind.Entry, "Técnica nº 15", "04.07.2026", "Respiração quadrada", "Acalmar o corpo com o ritmo da respiração", "Momento difícil", "Psique", 3, "Air",
        [
            new TechniqueEntrySeed("Tensão antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Anotações", "O que você notou no corpo ou na respiração durante a prática?", Suggestions: ["Os ombros baixaram", "A respiração ficou mais lenta", "Fiquei mais aquecido", "Foi difícil segurar a pausa", "Os pensamentos me distraíram", "Fiquei mais calmo", "Fiquei tonto", "O coração desacelerou", "Deu vontade de bocejar", "Nada de especial"]),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "A respiração lenta e ritmada reduz a atividade do sistema nervoso simpático e ajuda o corpo a sair do modo de luta ou fuga.",
            "Em momentos de ansiedade, pânico, coração acelerado, insônia ou sobrecarga.",
            "Sente-se confortavelmente e respire pelo nariz: 4 inspirando, 4 segurando, 4 expirando, 4 segurando. Repita 4 ciclos.",
            "Ansiedade 7/10 → depois de 4 ciclos 4/10. Os ombros baixam e o pulso desacelera."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "Prática", "Um pequeno passo",
        [
            "1. Descreva o que pesa ou impede você de começar",
            "2. Escolha o menor passo realista para hoje",
            "3. Escreva quando você vai fazer",
            "4. Avalie sua energia depois de escrever o passo"
        ],
        "Ativação comportamental: uma pequena ação devolve a sensação de movimento e controle.",
        TechniqueUiKind.Entry, "Técnica nº 16", "04.07.2026", "Um pequeno passo", "Retomar o impulso com uma ação mínima", "Momento difícil", "TCC", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("O que pesa", "O que parece difícil, chato ou impossível agora?", Suggestions: ["Cansaço", "Sem energia", "Coisa demais para fazer", "Tédio", "Medo de começar", "Nada parece importar", "Dormi mal", "Sem ânimo", "A solidão", "Procrastinação", "Dor de cabeça", "Não sei por onde começar"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("O menor passo hoje", "Ex.: levantar, lavar o rosto, mandar uma mensagem", Suggestions: ["Sair da cama", "Lavar o rosto", "Beber um pouco de água", "Mandar uma mensagem", "Sair por 5 minutos", "Arrumar uma coisa", "Abrir uma janela", "Me alongar por 2 minutos", "Tomar banho", "Comer alguma coisa", "Ligar para alguém próximo", "Abrir o arquivo de que preciso", "Escrever um plano para o dia", "Lavar uma xícara"]),
            new TechniqueEntrySeed("Quando você vai fazer", "Um horário ou momento específico do dia", Suggestions: ["Agora mesmo", "Daqui a 15 minutos", "Daqui a uma hora", "De manhã", "Depois do café da manhã", "Depois do almoço", "À noite", "Antes de dormir", "Amanhã de manhã", "No fim de semana"]),
            new TechniqueEntrySeed("Energia depois", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Na apatia, o cérebro espera a motivação para agir. Um pequeno passo cria motivação pela ação, e não o contrário.",
            "Quando você está sem energia, tudo parece sem sentido ou é difícil começar o dia.",
            "Escolha um passo de 2–5 minutos sem plano perfeito. Escreva o passo e o horário — isso já é movimento.",
            "Arrumar o apartamento → levar uma xícara para a cozinha às 10:00. Energia: 2 → 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "Prática", "Registro de pensamentos",
        [
            "1. Descreva a situação",
            "2. Escreva o pensamento automático",
            "3. Avalie a emoção",
            "4. Formule um pensamento alternativo mais equilibrado",
            "5. Avalie a emoção de novo"
        ],
        "Um registro estruturado da TCC ajuda a ver a ligação entre situação, pensamento e emoção.",
        TechniqueUiKind.Entry, "Técnica nº 17", "04.07.2026", "Registro de pensamentos", "Desmontar um pensamento e reduzir a carga emocional", "Momento difícil", "TCC", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situação", "O que aconteceu? Onde e quando?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Pensamento automático", "O que você pensou naquele momento?", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("Emoção", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Pensamento alternativo", "O que mais pode ser verdade? Um olhar mais equilibrado", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("Emoção agora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Pensamentos automáticos muitas vezes soam como fatos, embora sejam interpretações. Escrever torna esses pensamentos visíveis e possíveis de testar.",
            "Depois de um conflito, uma crítica, um fracasso, ou quando um pensamento fica girando.",
            "Descreva a situação e o pensamento, avalie a emoção e depois encontre uma alternativa — não positividade forçada, mas algo mais preciso.",
            "Situação: não responderam à mensagem. Pensamento: ninguém precisa de mim. Alternativa: a pessoa está ocupada. Emoção: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "Prática", "Palavras gentis para si",
        [
            "1. Escreva pelo que você está se criticando",
            "2. Imagine que um amigo próximo dissesse isso — o que você diria a ele?",
            "3. Formule a mesma frase de apoio para você",
            "4. Avalie o acolhimento ou o alívio"
        ],
        "A autocompaixão reduz a autocrítica dura e ajuda a se recuperar depois de um erro.",
        TechniqueUiKind.Entry, "Técnica nº 18", "04.07.2026", "Palavras gentis para si", "Suavizar a autocrítica com apoio", "Momento difícil", "TCC", 5, "Favorite",
        [
            new TechniqueEntrySeed("Pelo que você se critica", "O que você diz a si mesmo ou do que se culpa?", Suggestions: ["Um erro no trabalho", "Não terminei", "Fui grosso com alguém próximo", "Me comparo com os outros", "Adiei de novo", "Minha aparência", "Meu peso", "Falei o que não devia", "Não sou um pai ou mãe bom o bastante", "Ganho pouco", "Quebrei uma promessa", "Sensível demais"]),
            new TechniqueEntrySeed("O que você diria a um amigo", "Como você apoiaria alguém próximo nessa situação?", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("Frase para você", "A mesma gentileza, voltada para você", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("Acolhimento / alívio", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "A autocrítica raramente motiva — mais vezes alimenta a vergonha e a paralisia. A autocompaixão não tira a responsabilidade, mas dá apoio.",
            "Depois de um erro, ao se comparar com os outros ou ao se sentir insuficiente.",
            "Escreva a crítica e depois responda a si mesmo como responderia a um amigo — com respeito e sem minimizar.",
            "Crítica: estraguei tudo de novo. Para um amigo: você tentou, isso tem conserto. Para mim: posso aprender com isso. Acolhimento: 3 → 6."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "Prática", "Onde está no corpo?",
        [
            "1. Avalie sua tensão agora",
            "2. Passe a atenção devagar pelo corpo, da cabeça aos pés",
            "3. Encontre o lugar onde a sensação é mais forte",
            "4. Descreva: como é, grande ou pequena, quente ou fria",
            "5. Dê nome ao sentimento que vem junto",
            "6. Faça 5 expirações lentas como se cada uma passasse por esse lugar e avalie a tensão de novo"
        ],
        "Escaneamento corporal: perceba onde um sentimento mora, dê nome a ele e deixe o corpo suavizar sem lutar contra a sensação.",
        TechniqueUiKind.Entry, "Técnica nº 19", "16.10.2026", "Onde está no corpo?", "Perceber onde um sentimento mora no corpo e suavizar a tensão", "Momento difícil", "Mindfulness", 4, "Accessibility",
        [
            new TechniqueEntrySeed("Tensão agora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Em que parte do corpo você sente?", "Por exemplo: peito, garganta, ombros", Suggestions: ["Cabeça", "Testa", "Olhos", "Mandíbula", "Garganta", "Pescoço", "Ombros", "Peito", "Coração", "Barriga", "Costas", "Lombar", "Braços", "Mãos", "Pernas", "O corpo todo"]),
            new TechniqueEntrySeed("Como é a sensação?", "Por exemplo: aperto, peso, um nó", Suggestions: ["Aperto", "Peso", "Um nó", "Pressão", "Tensão", "Calor", "Frio", "Tremor", "Formigamento", "Latejamento", "Vazio", "Dormência", "Dor", "Difícil de descrever"]),
            new TechniqueEntrySeed("Que sentimento vem junto?", "Por exemplo: ansiedade, mágoa, cansaço", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Os sentimentos ecoam no corpo: a ansiedade como aperto no peito, a mágoa como um nó na garganta, o cansaço como ombros pesados. Quando percebemos a sensação e damos nome ao sentimento, a tensão costuma diminuir. Essa é a base do escaneamento corporal nos programas de mindfulness.",
            "Quando você se sente ansioso ou pesado, mas não sabe dizer exatamente o que sente; quando o corpo fica tenso depois de uma conversa ou um dia difícil. A prática não explica doenças nem substitui o médico: se uma sensação for nova, forte ou não passar, procure um médico.",
            "Sente-se confortavelmente e leve a atenção devagar do alto da cabeça até os pés. Pare onde a sensação for mais intensa. Não tente mudá-la nem lutar contra ela: descreva e dê nome ao sentimento. Depois faça algumas expirações lentas como se cada uma passasse por esse lugar.",
            "Tensão 6. Onde: peito. Sensação: aperto, como um punho. Sentimento: ansiedade antes de uma conversa. Depois de 5 expirações o aperto está mais suave, tensão 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "Prática", "Uma lembrança tomou conta",
        [
            "1. Avalie a intensidade",
            "2. Diga a si mesmo: \"Isto é uma lembrança. Aquilo foi antes, e agora é hoje\"",
            "3. Diga onde você está e que hora do dia é",
            "4. Sinta o apoio sob os pés, feche e abra as mãos",
            "5. Diga três coisas que você vê",
            "6. Cinco expirações lentas, água fria, e avalie de novo"
        ],
        "Ajuda em um flashback: voltar da lembrança para o presente pelo tempo, pelo lugar, pelo corpo e pelos sentidos.",
        TechniqueUiKind.Entry, "Técnica nº 20", "16.10.2026", "Uma lembrança tomou conta", "Voltar ao presente quando uma lembrança difícil toma conta", "Momento difícil", "Trauma", 3, "MyLocation",
        [
            new TechniqueEntrySeed("Intensidade", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Onde você está e que hora do dia é", "Por exemplo: em casa, à noite", Suggestions: ["Em casa", "No trabalho", "Na rua", "No transporte", "Na casa de alguém", "Manhã", "Tarde", "Noite", "Madrugada"]),
            new TechniqueEntrySeed("Três coisas que você vê", "Por exemplo: janela, xícara, luminária", Suggestions: ["Janela", "Mesa", "Celular", "Xícara", "Luminária", "Porta", "Cadeira", "Parede", "Quadro", "Planta", "Relógio", "Céu"]),
            new TechniqueEntrySeed("Intensidade agora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Um flashback é uma lembrança de trauma vivida como se estivesse acontecendo de novo: com os mesmos sentimentos e reações do corpo. O cérebro confunde \"antes\" e \"agora\". O tempo, o lugar, o apoio do corpo e o que você vê e ouve ajudam a voltar ao presente.",
            "Quando a lembrança de um acontecimento difícil toma conta, com imagens que surgem de repente e a sensação de \"estou lá de novo\". Flashbacks frequentes são um sinal comum de reação pós-traumática e respondem bem ao tratamento com um especialista em trauma.",
            "Não tente elaborar a lembrança: agora a única tarefa é voltar. Diga a si mesmo onde está e que dia é, pressione os pés no chão, diga o que vê, solte o ar devagar, use o frio.",
            "Intensidade 8. \"Aquilo foi antes; agora é 2026, estou em casa, é noite.\" Pés no chão; vejo uma janela, uma xícara, uma luminária. Água fria. Agora 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "Prática", "Reescrever um pesadelo",
        [
            "1. Escolha um pesadelo que se repete, não o mais difícil para começar",
            "2. Descreva em poucas palavras, sem detalhes pesados",
            "3. Decida o que mudar nele: o final, o lugar, você mesmo, alguém por perto",
            "4. Escreva a nova versão do sonho inteira, até um final calmo",
            "5. Todo dia, por 10 a 20 minutos, imagine a nova versão em detalhes",
            "6. Depois de uma ou duas semanas, veja como os pesadelos mudaram"
        ],
        "Ensaio por imagens: um pesadelo recorrente é reescrito acordado, e o cérebro começa a sonhar a nova versão.",
        TechniqueUiKind.Entry, "Técnica nº 21", "16.10.2026", "Reescrever um pesadelo", "Tornar um pesadelo recorrente mais raro e mais fraco", "O sono", "Trauma", 15, "NightsStay",
        [
            new TechniqueEntrySeed("Quão pesado é esse pesadelo", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Sobre o que é o pesadelo, em poucas palavras", "Uma ou duas frases, sem detalhes pesados", Suggestions: ["Alguém está me perseguindo", "Não consigo fugir", "Estou de volta onde foi assustador", "Estou perdendo alguém próximo", "Não consigo gritar", "Estou caindo", "Estou sendo atacado", "Estou atrasado e não chego a tempo"]),
            new TechniqueEntrySeed("O que você muda", "O que quiser: o sonho é seu", Suggestions: ["O final", "O lugar", "Fico mais forte", "Aparece alguém para ajudar", "Consigo falar e gritar", "O perigo desaparece", "Tudo fica seguro", "Percebo que é um sonho"]),
            new TechniqueEntrySeed("A nova versão do sonho", "Descreva inteira, do começo até um final calmo", Suggestions: ["Percebo que é um sonho", "Aparece alguém que me protege", "Me vejo em um lugar seguro", "Vou embora com calma", "Tudo termina bem"]),
            new TechniqueEntrySeed("Quando vou ensaiar a nova versão", "Todo dia, de 10 a 20 minutos", Suggestions: ["De manhã", "Durante o dia, em uma pausa", "À noite, umas duas horas antes de dormir", "Todo dia no mesmo horário"]),
            new TechniqueEntrySeed("Quão pesado está agora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Pesadelos recorrentes depois de um trauma ou estresse se mantêm por hábito: o cérebro repete o mesmo enredo várias vezes. Na terapia de ensaio por imagens, o enredo é reescrito acordado, e a nova versão é ensaiada mentalmente todos os dias. Pesquisas mostram que isso torna os pesadelos mais raros e mais fracos e melhora o sono.",
            "Com pesadelos recorrentes que dão medo de dormir. Se o pesadelo repete diretamente um trauma e até uma descrição curta traz muito pavor, faça isso junto com um especialista em trauma.",
            "Comece com um pesadelo mais leve. Não escreva as partes assustadoras em detalhes: umas duas frases bastam. Mude o sonho como quiser; a nova versão não precisa ser realista. Ensaie durante o dia, não logo antes de dormir, de 10 a 20 minutos por dia.",
            "Pesadelo: alguém me persegue e não consigo fugir. Mudança: percebo que é um sonho, me viro, e é só uma sombra; volto para casa com calma. Ensaio isso todo dia à tarde. Depois de duas semanas, o pesadelo aparece menos."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Balloons = new(
        "Prática", "Estoure os balões",
        [
            "1. Avalie sua tensão",
            "2. Estoure os balões com um toque, sem pressa",
            "3. A cada estouro, solte o ar devagar",
            "4. Jogue a rodada até o fim",
            "5. Avalie sua tensão depois"
        ],
        "Um jogo curto leva a atenção dos pensamentos ansiosos para uma ação simples e para as sensações.",
        TechniqueUiKind.Entry, "Técnica nº 22", "26.10.2026", "Estoure os balões", "Uma pausa curta para soltar a tensão", "Momento difícil", "Psique", 3, "BubbleChart",
        [
            new TechniqueEntrySeed("Tensão antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "O que mudou enquanto você estourava balões?", Suggestions: ["Me sinto mais leve", "Minha mente descansou", "Deu vontade de sorrir", "Minha respiração se acalmou", "A irritação passou", "Minhas mãos relaxaram", "Nada mudou", "Quero mais uma rodada"]),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Quando a ansiedade ou a irritação estão no auge, é difícil parar os pensamentos pela força de vontade. Uma ação simples, que pede atenção dos olhos e das mãos, muda o foco do cérebro por alguns minutos e deixa a tensão baixar. Não é um tratamento, mas um jeito de atravessar um minuto difícil.",
            "Quando a irritação, a tensão ou a ansiedade tomam conta e você precisa de uma pausa curta. Se o jogo só ajudar a fugir de um problema que vale a pena resolver, volte a ele quando estiver mais calmo.",
            "Sem pressa: não é preciso acertar todos os balões. Solte o ar devagar a cada estouro. A rodada termina sozinha; não existe jogo sem fim.",
            "Tensão 7/10 depois de uma discussão. Uma rodada de 20 balões, uma expiração a cada estouro. Tensão 4/10, e consegui pensar com calma no que responder."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition CyclicSigh = new(
        "Prática", "Suspiro cíclico",
        [
            "1. Avalie sua tensão",
            "2. Inspire pelo nariz e, no final, faça mais uma inspiração curta",
            "3. Expire devagar pela boca, por mais tempo do que inspirou",
            "4. Respire no ritmo do círculo por cerca de três minutos",
            "5. Avalie sua tensão depois"
        ],
        "Uma inspiração dupla e uma expiração longa acalmam o corpo rápido, sem prender a respiração.",
        TechniqueUiKind.Entry, "Técnica nº 23", "26.10.2026", "Suspiro cíclico", "Uma inspiração dupla e uma expiração longa para acalmar o corpo", "Momento difícil", "Psique", 3, "Air",
        [
            new TechniqueEntrySeed("Tensão antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "O que mudou depois da respiração?", Suggestions: ["Meu corpo está mais calmo", "Meus ombros relaxaram", "Meu coração desacelerou", "Meus pensamentos desaceleraram", "Fiquei um pouco tonto", "Nada mudou", "Quero repetir"]),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "A segunda inspiração curta abre mais os pulmões, e a expiração longa aciona o freio do sistema nervoso: o coração desacelera e o corpo se acalma. Num estudo de 2023, cinco minutos por dia dessa respiração melhoraram o humor mais do que a meditação de atenção plena. Não é um tratamento, mas um jeito rápido de baixar a tensão.",
            "Quando a ansiedade, a raiva ou a tensão tomam conta e você precisa acalmar o corpo em poucos minutos. Também serve antes de uma conversa difícil ou antes de dormir.",
            "Respire com suavidade, sem forçar: a segunda inspiração é curta, a expiração lenta e sem esforço. Se ficar tonto, volte à respiração normal; isso passa. O círculo termina sozinho depois de três minutos.",
            "Tensão 6/10 antes de uma ligação. Três minutos de inspiração dupla e expiração longa. Tensão 3/10, e minha voz ficou mais firme."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition MuscleRelax = new(
        "Prática", "Contrair e soltar",
        [
            "1. Avalie sua tensão",
            "2. Contraia um grupo de músculos por cinco segundos, com metade da força",
            "3. Solte e perceba o relaxamento por quinze segundos",
            "4. Passe por cinco grupos: mãos, ombros, rosto, barriga, pernas",
            "5. Avalie sua tensão depois"
        ],
        "Contrair os músculos primeiro e depois soltar ajuda o corpo a sentir o que é relaxar.",
        TechniqueUiKind.Entry, "Técnica nº 24", "26.10.2026", "Contrair e soltar", "Um relaxamento muscular curto para um corpo tenso", "Momento difícil", "Psique", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Tensão antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "Em que parte do corpo ficou mais leve?", Suggestions: ["Meus ombros relaxaram", "Minha mandíbula se soltou", "Minhas mãos esquentaram", "Ficou mais fácil respirar", "Deu sono", "Ainda sobrou alguma tensão", "Nada mudou"]),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "O relaxamento muscular progressivo é um dos jeitos mais estudados de reduzir a ansiedade e a tensão. Contrair um músculo e depois soltá-lo cria um contraste que ajuda a perceber o relaxamento e, com o tempo, aprender a provocá-lo. Não é um tratamento, mas uma habilidade que funciona melhor com a prática.",
            "Quando o corpo está tenso de estresse, quando é difícil ficar parado ou pegar no sono. Combina bem com a noite, antes de dormir.",
            "Contraia com metade da força, sem dor. Se alguma parte do corpo dói ou está machucada, não a contraia; apenas observe. Ao soltar, respire com calma e não tenha pressa de passar ao próximo grupo.",
            "Tensão 7/10 à noite depois do trabalho, ombros encolhidos. Cinco grupos: mãos, ombros, rosto, barriga, pernas. Tensão 4/10, os ombros relaxaram."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BusyMind = new(
        "Prática", "Ocupar a mente",
        [
            "1. Avalie sua tensão",
            "2. Pegue uma tarefa e faça mentalmente ou em voz alta",
            "3. Se os pensamentos voltarem à preocupação, volte com calma à tarefa",
            "4. Faça cinco tarefas",
            "5. Avalie sua tensão depois"
        ],
        "Tarefas simples para a mente tiram a atenção dos pensamentos que andam em círculos.",
        TechniqueUiKind.Entry, "Técnica nº 25", "26.10.2026", "Ocupar a mente", "Tarefas simples para quebrar o ciclo de pensamentos", "Momento difícil", "Psique", 3, "Extension",
        [
            new TechniqueEntrySeed("Tensão antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "O que mudou enquanto você estava ocupado com as tarefas?", Suggestions: ["Os pensamentos me largaram", "Fiquei mais calmo", "Foi difícil me concentrar", "Deu vontade de fazer algo", "A ansiedade voltou", "Nada mudou", "Quero mais tarefas"]),
            new TechniqueEntrySeed("Tensão depois", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Quando os pensamentos andam em círculos, pará-los com força de vontade é quase impossível. Uma tarefa que ocupa a mente (contar, lembrar, listar) não deixa espaço para a ruminação por um tempo. É uma habilidade de distração da terapia comportamental dialética: não resolve o problema, mas ajuda a atravessar um momento difícil.",
            "Quando pensamentos intrusivos tomam conta, antes de dormir ou depois de uma briga, quando a conversa continua na cabeça.",
            "Faça as tarefas para valer: quanto mais atenção elas pedirem, melhor. Se o problema realmente precisa ser resolvido, volte a ele quando estiver mais calmo.",
            "Depois de uma briga, a conversa fica girando na minha cabeça, tensão 7/10. Cinco tarefas: animais que vivem na água, contar de trás para frente a partir de 100 de 7 em 7, objetos de cada cor ao meu redor. Tensão 4/10."),
        null, null);
}
