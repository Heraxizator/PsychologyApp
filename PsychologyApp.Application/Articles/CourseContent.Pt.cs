using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public static partial class ArticleContent
{
    private static readonly Course[] CoursesPt =
    [
        new(
            CourseIds.Sleep,
            "Dormir sem remédios",
            "Sete dias de terapia cognitivo-comportamental da insônia, o jeito mais estudado de recuperar o sono",
            "Bedtime",
            "isi",
            "ISI-7",
            [
                new(
                    "Como o sono funciona",
                    4,
                    [
                        P("O sono não se «liga» com esforço. Duas forças o preparam: a pressão do sono, que se acumula o dia todo enquanto você está acordado, e o relógio interno, que nos dá sono e nos deixa alertas nos mesmos horários todos os dias."),
                        P("Na insônia, as duas forças se desregulam: a pessoa deita mais cedo para «compensar», fica muito tempo acordada, cochila de dia, levanta em horários diferentes. A pressão do sono não tem tempo de se acumular, e o relógio se confunde."),
                        Cycle("O ciclo da insônia", "Uma noite ruim", "Preocupação: «de novo não vou dormir»", "Esforçar-se para dormir", "A tensão afasta o sono"),
                        P("Este curso se baseia na terapia cognitivo-comportamental da insônia (TCC-I). As diretrizes médicas a recomendam em primeiro lugar, antes dos remédios para dormir: o efeito dura depois que o curso termina."),
                        Warning("Quando procurar primeiro um médico"),
                        List(
                            "Ronco alto, pausas na respiração durante o sono, muita sonolência durante o dia",
                            "Sensações desagradáveis nas pernas à noite que dão vontade de se mexer",
                            "Insônia junto com humor baixo há mais de duas semanas",
                            "Você dirige ou opera máquinas e sente muito sono durante o dia")
                    ],
                    "Toda manhã, anote: quando você deitou, mais ou menos quando adormeceu, quantas vezes acordou, quando levantou e como dormiu de 0 a 10. Sem relógio à noite, mais ou menos, pela sensação.",
                    "Em que horário você costuma deitar e levantar? O que mais atrapalha o seu sono?"),
                new(
                    "A cama é para dormir",
                    4,
                    [
                        P("Se você costuma ficar acordado na cama, se preocupar ou olhar o celular ali, o cérebro associa a cama a estar acordado. É por isso que dá para cochilar numa poltrona e ficar totalmente desperto assim que se deita."),
                        H("Regras que restauram a associação"),
                        List(
                            "Vá para a cama só quando sentir sono, não apenas cansaço",
                            "Se você não consegue dormir por uns 20 minutos, levante e faça algo tranquilo com luz baixa. Volte quando sentir sono",
                            "A cama é só para dormir e para a intimidade: sem celular, séries ou trabalho",
                            "Levante no mesmo horário todos os dias, mesmo depois de uma noite ruim e nos fins de semana",
                            "Não cochile e, se precisar muito, não mais que 20 minutos e antes das 15h"),
                        Quiz("A noite foi ruim. O que é melhor de manhã?", 1, "Um horário fixo para levantar é a principal âncora do sono. Se você dormir até mais tarde, haverá menos pressão do sono à noite, e a próxima noite também será difícil.", "Dormir até mais tarde para se recuperar", "Levantar no horário de sempre", "Levantar e depois tirar um bom cochilo durante o dia")
                    ],
                    "Escolha um horário para levantar que você consiga manter nos sete dias, e levante nele. Se não conseguir dormir por uns 20 minutos, saia da cama.",
                    "Que horário para levantar você escolheu? O que foi difícil na regra «levantar se não conseguir dormir»?"),
                new(
                    "A janela de sono",
                    5,
                    [
                        P("Quanto mais tempo a pessoa fica acordada na cama, mais leve e picado o sono fica. Uma técnica chamada compressão do sono ajuda: o tempo na cama é aproximado de quanto você realmente dorme."),
                        H("Como calcular a janela"),
                        List(
                            "Com as anotações dos últimos dias, calcule quanto você dorme em média",
                            "Some 30 minutos: esse é o seu tempo na cama, mas não menos que seis horas",
                            "Conte para trás a partir do horário de levantar escolhido: assim você tem o horário de deitar",
                            "Não deite antes desse horário, mesmo que tenha muita vontade"),
                        P("Nos primeiros dias você vai sentir sono: isso quer dizer que a pressão do sono está se acumulando. Depois de uma semana, se você dormir a maior parte do tempo na cama, a janela pode ser ampliada em 15 minutos."),
                        Warning("Atenção"),
                        P("Não comprima o sono se você tem transtorno bipolar ou epilepsia, se está grávida, ou se a sonolência torna dirigir ou trabalhar perigoso. Nesses casos, converse sobre o sono com um médico.")
                    ],
                    "Calcule sua janela de sono e mantenha-a até o fim do curso: nada de cama antes do horário de deitar, levantar no mesmo horário.",
                    "Qual é a sua janela de sono? Como foi a primeira noite com ela?"),
                new(
                    "Uma noite que prepara para o sono",
                    4,
                    [
                        P("O sono vem mais fácil quando o corpo e a mente têm tempo de desacelerar. A hora antes de deitar é a ponte entre o dia e o sono."),
                        H("O que ajuda"),
                        List(
                            "Uma hora antes de deitar: luz baixa e atividades tranquilas sem telas",
                            "Café, chá forte e energéticos no máximo até as 14h",
                            "O álcool ajuda a adormecer, mas deixa o sono superficial e faz acordar de madrugada",
                            "Um jantar pesado 2–3 horas antes de deitar",
                            "De manhã, luz do dia por pelo menos 15 minutos: ela acerta o relógio interno"),
                        H("Relaxar antes de dormir"),
                        P("A respiração lenta com uma expiração longa diminui a ativação. Experimente agora."),
                        Breathe("Inspire contando 4, expire contando 6")
                    ],
                    "Crie seu próprio ritual para a última hora antes de deitar e faça-o todas as noites. Tire o celular do quarto, ou pelo menos da cama.",
                    "O que entrou no seu ritual da noite? O que se mostrou difícil?"),
                new(
                    "Pensamentos que não deixam dormir",
                    4,
                    [
                        P("A mente muitas vezes começa a trabalhar justamente na cama: durante o dia não houve tempo para pensar. Por isso ajuda dar tempo a esses pensamentos mais cedo."),
                        H("Um tempo para os pensamentos"),
                        List(
                            "À noite, umas duas horas antes de deitar, reserve 15 minutos",
                            "Escreva tudo o que preocupa você e, para cada item, um próximo passo ou «por enquanto não há o que fazer»",
                            "Se o pensamento voltar na cama, diga a si mesmo: «já anotei isso, cuido disso amanhã»"),
                        Quiz("Na cama você pensa: «Se eu não dormir, amanhã vou estragar tudo.» O que está mais perto da verdade?", 1, "O pensamento exagera. Depois de uma noite ruim fica mais difícil, mas a maioria das pessoas ainda dá conta das tarefas. Quanto maior o medo do dia seguinte, mais difícil adormecer.", "É verdade, o fracasso é inevitável", "Depois de uma noite ruim é mais difícil, mas geralmente dou conta do principal", "Melhor nem ir para a cama"),
                        Practice(TechniqueId.Observer, "Posição de observador")
                    ],
                    "Toda noite, reserve 15 minutos para escrever o que preocupa você, e não leve esses pensamentos para a cama.",
                    "Que pensamentos aparecem com mais frequência à noite? Escrevê-los ajudou?"),
                new(
                    "Parar de lutar contra o sono",
                    4,
                    [
                        P("Quanto mais você tenta dormir, pior fica. O sono é como um pássaro arisco: vem quando ninguém está correndo atrás dele."),
                        H("O que ajuda a soltar o controle"),
                        List(
                            "Tire o relógio da vista: contar «faltam cinco horas» desperta",
                            "Permita-se não dormir: «vou ficar aqui deitado descansando, o sono vem quando vier»",
                            "Uma noite ruim não é uma catástrofe: o corpo se recupera mesmo depois de um sono incompleto",
                            "Não cancele planos por causa de uma noite ruim: assim a insônia não comanda a sua vida"),
                        P("Compare suas anotações dos primeiros dias com as de agora: como mudaram o tempo para adormecer, o número de despertares e a nota do sono?")
                    ],
                    "À noite, mantenha o relógio fora da vista. Depois de uma noite ruim, viva o dia normalmente.",
                    "Segundo suas anotações, o que mudou no seu sono nesses dias?"),
                new(
                    "Resumo e plano",
                    4,
                    [
                        P("Em uma semana você passou pelo núcleo da TCC da insônia. O sono se recupera aos poucos: muitas pessoas percebem melhora em duas a quatro semanas se seguirem as regras."),
                        H("O que continuar fazendo"),
                        List(
                            "Um mesmo horário para levantar todos os dias",
                            "Ir para a cama só com sono e levantar se não conseguir dormir",
                            "Se você dorme a maior parte da noite, ampliar a janela em 15 minutos por semana",
                            "O ritual da noite e o tempo para os pensamentos"),
                        H("Se a insônia voltar"),
                        P("Depois de estresse, doença ou viagem, o sono pode sair dos trilhos de novo. É normal: volte às regras deste curso, e ele vai se recuperar mais rápido do que da primeira vez."),
                        P("Refaça o índice ISI-7 e compare com o seu resultado do início. Se não houver melhora, procure um médico: existem especialistas em sono que aplicam a TCC-I."),
                        TestsTab("Índice ISI-7", "isi")
                    ],
                    "Escreva seu plano para o caso de o sono sair dos trilhos de novo.",
                    "O que mais ajudou no curso? O que você vai continuar fazendo?")
            ]),
        new(
            CourseIds.Anxiety,
            "A ansiedade sob controle",
            "Seis dias: entender como a ansiedade funciona, acalmar o corpo, conferir pensamentos assustadores e dar um primeiro passo em direção ao medo",
            "Psychology",
            "gad7",
            "GAD-7",
            [
                new(
                    "Como a ansiedade funciona",
                    4,
                    [
                        P("A ansiedade é um alarme feito para avisar do perigo. O problema começa quando ele dispara com frequência demais, alto demais, e demora a se calar."),
                        Cycle("O ciclo da ansiedade", "Um pensamento assustador", "Ansiedade no corpo", "Evitação ou uma muleta", "Alívio por um instante", "O cérebro aprende: «era perigoso»"),
                        P("O que mantém a ansiedade não é o medo em si, mas o que fazemos para não senti-lo: evitar, reconferir, pedir que nos tranquilizem, remoer pensamentos. Passo a passo, o curso trabalha cada elo desse ciclo."),
                        P("No início do curso, vale a pena fazer a escala GAD-7: no fim você vai comparar o resultado."),
                        TestsTab("Escala GAD-7", "gad7")
                    ],
                    "Hoje, três vezes, quando sentir ansiedade, repare: o que aconteceu, qual foi o pensamento, o que o corpo sentiu e o que você fez.",
                    "Quando e onde a ansiedade aparece com mais frequência?"),
                new(
                    "Acalmar o corpo",
                    4,
                    [
                        P("Quando a ansiedade está alta, é difícil discutir com os pensamentos. Primeiro ajuda acalmar o corpo: uma expiração lenta aciona o sistema que freia a reação de luta ou fuga."),
                        Breathe("Inspire contando 4, expire contando 6"),
                        P("Se a ansiedade está forte e tudo parece irreal, traga a atenção de volta ao presente pelos sentidos."),
                        Practice(TechniqueId.Grounding, "Ancoragem 5-4-3-2-1"),
                        P("Importante: a respiração não é um jeito de se livrar da ansiedade a qualquer custo. Ela ajuda a atravessar o pico, não a evitar situações.")
                    ],
                    "Duas vezes por dia, dois minutos de respiração lenta, mesmo com calma: assim a habilidade estará lá quando você precisar.",
                    "O que você notou no corpo ao respirar devagar?"),
                new(
                    "Conferir um pensamento assustador",
                    5,
                    [
                        P("Pensamentos ansiosos soam como fatos: «vou passar vergonha», «vai acontecer algo terrível». Mas são previsões, e dá para conferi-las."),
                        H("Perguntas para fazer ao pensamento"),
                        List(
                            "Que fatos apoiam esse pensamento e quais vão contra?",
                            "O que aconteceu em casos parecidos antes?",
                            "O que é mais provável, e não mais assustador?",
                            "Se o pior acontecer, como vou lidar com isso?",
                            "O que eu diria a um amigo com esse pensamento?"),
                        Quiz("«Com certeza vou estragar tudo.» Que armadilha do pensamento é essa?", 1, "É adivinhar o futuro: o pensamento trata como fato algo que ainda não aconteceu.", "Ler a mente", "Adivinhar o futuro", "Desqualificar o positivo"),
                        Practice(TechniqueId.ThoughtRecord, "Registro de pensamentos")
                    ],
                    "Hoje, uma vez, confira um pensamento ansioso com um registro de pensamentos.",
                    "Que pensamento você conferiu? Quanto você acreditava nele antes e depois?"),
                new(
                    "O horário da preocupação",
                    4,
                    [
                        P("Preocupar-se parece útil: «se eu pensar mais, vou estar preparado». Na verdade, remoer raramente leva a uma solução e mantém a ansiedade alta."),
                        Quiz("Como distinguir resolver de andar em círculos?", 0, "Resolver avança para um passo: o que posso fazer, e quando. Andar em círculos produz novos «e se».", "Resolver termina num passo; andar em círculos, num novo «e se»", "Andar em círculos leva mais tempo", "Não dá"),
                        H("Como adiar a preocupação"),
                        List(
                            "Escolha 15–20 minutos por dia, não antes de dormir",
                            "Quando a preocupação vier em outra hora, anote o assunto numa linha e diga a si mesmo: «vou pensar nisso no meu horário da preocupação»",
                            "No seu horário da preocupação, repasse a lista: planeje um passo para o que você pode fazer, solte o que não pode",
                            "Muitas vezes, na hora marcada, o assunto já não parece importante"),
                        Practice(TechniqueId.Observer, "Posição de observador")
                    ],
                    "Defina um horário da preocupação e adie para ele os pensamentos ansiosos.",
                    "Você conseguiu adiar a preocupação? O que aconteceu com os assuntos na hora marcada?"),
                new(
                    "Muletas e evitação",
                    4,
                    [
                        P("O que mais mantém a ansiedade é a evitação. Ela pode ser aberta (não vou, não ligo) ou escondida: muletas que ajudam a atravessar uma situação."),
                        Check("Eu tenho muletas?", 2, "Você tem várias muletas. Elas mantêm o medo: o cérebro acha que tudo deu certo só graças a elas. Tente abrir mão de uma numa situação fácil.", "Você tem poucas muletas. Repare também no que você evita por completo.", "Levo um remédio, água ou o celular carregado «por via das dúvidas»", "Só vou acompanhado a lugares difíceis", "Sento perto da saída", "Me distraio para não perceber a ansiedade", "Confiro ou pergunto de novo muitas vezes"),
                        P("A saída é encontrar aos poucos o que você teme e dispensar as muletas. Assim o cérebro aprende pela experiência que o temido não acontece ou é suportável.")
                    ],
                    "Faça uma lista de três a cinco situações que você evita, da mais fácil à mais difícil.",
                    "Que situação da lista você está pronto para tentar primeiro?"),
                new(
                    "O primeiro passo e um plano",
                    4,
                    [
                        P("O jeito mais eficaz de enfraquecer a ansiedade é a exposição: encontrar o medo aos poucos. Não é preciso ir direto ao mais assustador: comece com um passo fácil e confira uma previsão."),
                        List(
                            "Antes do passo, anote o que você teme e o quanto tem certeza de que vai acontecer",
                            "Dê o passo sem muletas e fique na situação até ter conferido a previsão",
                            "Depois, anote o que realmente aconteceu"),
                        Ladder("Escada do medo"),
                        H("O que continuar fazendo"),
                        List(
                            "A respiração lenta no pico da ansiedade",
                            "Conferir pensamentos e o horário da preocupação",
                            "Degraus na escada do medo a cada poucos dias"),
                        P("Refaça a escala GAD-7 e compare com o início. Se a ansiedade estiver forte ou atrapalhar a vida, procure um psicoterapeuta: a TCC da ansiedade é bem estudada."),
                        TestsTab("Escala GAD-7", "gad7")
                    ],
                    "Dê o primeiro passo na sua escada do medo e anote o que aconteceu.",
                    "O que você tinha previsto, e o que realmente aconteceu?")
            ]),
        new(
            CourseIds.Mood,
            "Sair da apatia",
            "Seis dias de ativação comportamental: pequenos passos para trazer de volta a energia e o que dá alegria",
            "Cloud",
            "phq9",
            "PHQ-9",
            [
                new(
                    "O ciclo da inatividade",
                    4,
                    [
                        P("Quando o humor está no zero, é natural fazer menos: não há forças, nada atrai. Mas quanto menos fazemos, menos há na vida que traga alegria e a sensação de «eu consegui», e o humor cai ainda mais."),
                        Cycle("O ciclo da inatividade", "Sem forças", "Faço menos", "Menos alegria e conquistas", "Ainda menos forças"),
                        P("A ativação comportamental quebra esse ciclo pelo outro lado: não esperar a vontade, mas dar pequenos passos depois dos quais as forças e a vontade voltam aos poucos. É um dos métodos de ajuda mais estudados para a depressão."),
                        P("Faça a escala PHQ-9 no início do curso: no fim você vai comparar o resultado."),
                        TestsTab("Escala PHQ-9", "phq9"),
                        Warning("Se surgirem pensamentos sobre a morte"),
                        P("Se você tem pensamentos de que seria melhor se você não existisse, não fique sozinho com eles: este curso não substitui a ajuda de um especialista."),
                        Crisis("Preciso de ajuda agora")
                    ],
                    "Hoje e amanhã, repare no que você faz e dê uma nota ao seu humor de 0 a 10 várias vezes ao dia.",
                    "Como estão seus dias agora? O que você parou de fazer ultimamente?"),
                new(
                    "O que levanta o humor",
                    4,
                    [
                        P("Olhe suas observações: em que momentos o humor estava um pouco mais alto? Muitas vezes é algo pequeno: uma conversa, uma caminhada, um banho, uma tarefa feita."),
                        H("Dois tipos de atividades úteis"),
                        List(
                            "As agradáveis: o que dá prazer, mesmo que fraco — música, comida gostosa, sol",
                            "As de «eu consegui»: o que deixa a sensação de realização — lavar a louça, responder um e-mail"),
                        P("Na depressão, o prazer das coisas agradáveis é mais fraco do que antes. É normal: o que importa é fazer, e não esperar a alegria de antes.")
                    ],
                    "Faça uma lista de dez atividades: cinco agradáveis e cinco de «eu consegui». Quanto menores e mais simples, melhor.",
                    "O que entrou na sua lista? Houve algo que levantou um pouco o seu humor?"),
                new(
                    "Pequenos passos",
                    4,
                    [
                        P("Planos grandes assustam na apatia e não saem do papel. Ajuda dividir as coisas: não «limpar a casa», mas «tirar três coisas da mesa»."),
                        List(
                            "Escolha uma atividade da lista para hoje",
                            "Divida-a até chegar a um passo de cinco minutos",
                            "Defina um horário e um lugar: «depois do café da manhã, na cozinha»",
                            "Depois, anote como seu humor mudou"),
                        Practice(TechniqueId.SmallStep, "Um pequeno passo")
                    ],
                    "Todos os dias, faça uma atividade agradável e uma de «eu consegui» no horário que você definiu.",
                    "O que você conseguiu fazer? Como seu humor mudou depois?"),
                new(
                    "«Não estou com vontade»",
                    4,
                    [
                        P("O principal obstáculo é o pensamento «não estou com vontade, vou fazer quando tiver forças». Na apatia, a vontade vem depois da ação, não antes."),
                        Quiz("Sem forças, e hoje está planejada uma caminhada. O que é melhor?", 1, "Na apatia, a vontade volta depois da ação. Mesmo cinco minutos dão ao corpo e ao humor o sinal «eu fiz alguma coisa».", "Esperar até ter vontade", "Sair pelo menos por cinco minutos", "Deixar para o fim de semana"),
                        H("Outros pensamentos que travam"),
                        List(
                            "«Não adianta nada»: confira depois — algo ficou nem que seja um pouco diferente?",
                            "«É pouco demais»: é justamente de pequenos passos que se faz a saída",
                            "«Antes eu fazia mais»: compare-se com ontem, não com quem você era antes"),
                        Practice(TechniqueId.SelfCompassion, "Palavras gentis para si")
                    ],
                    "Quando não estiver com vontade, faça pelo menos uma versão de cinco minutos do que planejou.",
                    "Que pensamento mais impede você de começar? O que ajudou a contorná-lo?"),
                new(
                    "As pessoas ao seu redor",
                    4,
                    [
                        P("Na apatia dá vontade de se fechar para todo mundo. Mas o contato com as pessoas é um dos remédios mais fortes contra o humor baixo, mesmo que agora não traga a alegria de antes."),
                        List(
                            "Escreva para uma pessoa com quem você não fala há algum tempo",
                            "Um encontro curto ou uma ligação é melhor do que nada",
                            "Você não precisa explicar tudo: «está difícil agora, vamos só dar uma volta»",
                            "Se as forças estiverem baixas, escolha pessoas com quem é fácil estar"),
                        Practice(TechniqueId.SmallStep, "Um pequeno passo")
                    ],
                    "Entre em contato com pelo menos uma pessoa nos próximos dois dias.",
                    "Com quem você entrou em contato? Como foi?"),
                new(
                    "O que importa para mim",
                    4,
                    [
                        P("Para que os passos não virem obrigações, ajuda ligá-los ao que importa para você: as pessoas próximas, a saúde, o trabalho, a criatividade, a natureza."),
                        List(
                            "Escolha uma área que importa para você",
                            "Pense em quem você quer ser nela",
                            "Escolha um pequeno passo nessa direção para a semana"),
                        H("O que continuar fazendo"),
                        List(
                            "Todos os dias, uma atividade agradável e uma de «eu consegui»",
                            "Pequenos passos em vez de grandes planos",
                            "O contato com as pessoas"),
                        P("Refaça a escala PHQ-9 e compare com o início. Se o resultado estiver alto ou não mudar, procure um médico ou psicoterapeuta: a depressão tem tratamento muito bom."),
                        TestsTab("Escala PHQ-9", "phq9"),
                        Crisis("Preciso de ajuda agora")
                    ],
                    "Dê um passo nesta semana numa área que importa para você.",
                    "O que importa para você? Que passo você escolheu?")
            ]),
        new(
            CourseIds.Trauma,
            "Depois de um trauma",
            "Cinco dias sobre o que é seguro fazer sozinho: entender sua reação, lidar com flashbacks e pesadelos, encontrar um especialista",
            "Healing",
            "pcl5",
            "PCL-5",
            [
                new(
                    "Uma reação normal a um acontecimento anormal",
                    4,
                    [
                        P("Depois de um acontecimento difícil, quase todo mundo tem lembranças invasivas, pesadelos, ansiedade, entorpecimento, irritação. Não é fraqueza, e sim a reação de um cérebro tentando proteger você de que aconteça de novo."),
                        P("Este curso não vai processar o trauma: isso exige terapia com um especialista. Ele vai ajudar você a entender o que está acontecendo, a lidar com os momentos difíceis e a chegar preparado ao especialista. Neste curso, você não precisa lembrar nem descrever o acontecimento em si."),
                        P("Faça a escala PCL-5 no início: no fim você vai comparar o resultado."),
                        TestsTab("Escala PCL-5", "pcl5"),
                        Warning("Se o perigo continua"),
                        P("Se você está sob ameaça agora, ou se surgiram pensamentos de se machucar, a segurança vem primeiro."),
                        Crisis("Preciso de ajuda agora")
                    ],
                    "Repare quando as coisas ficam mais difíceis durante o dia, sem detalhes do acontecimento: o horário, o lugar, o que havia ao redor.",
                    "Quais dessas reações você reconhece em si?"),
                new(
                    "Voltar ao presente",
                    4,
                    [
                        P("Um flashback é uma lembrança revivida como se estivesse acontecendo agora. O momento, o lugar, o apoio do corpo e os sentidos ajudam a voltar."),
                        Practice(TechniqueId.Flashback, "Uma lembrança tomou conta"),
                        Practice(TechniqueId.Grounding, "Ancoragem 5-4-3-2-1"),
                        H("Seu kit de ancoragem"),
                        List(
                            "Uma frase: «É uma lembrança. Agora é outro momento, estou seguro»",
                            "Algo para levar com você: uma pedrinha, um anel, algo com cheiro",
                            "Uma pessoa para quem você pode mandar mensagem",
                            "Algo frio: água, gelo"),
                        Breathe("A expiração mais longa que a inspiração")
                    ],
                    "Monte seu kit de ancoragem e experimente a prática «Uma lembrança tomou conta» num momento tranquilo, para conhecê-la antes.",
                    "O que entrou no seu kit de ancoragem?"),
                new(
                    "O sono e os pesadelos",
                    4,
                    [
                        P("Depois de um trauma, o sono costuma sofrer: é difícil adormecer, o sono é leve, vêm os pesadelos. A falta de sono deixa os dias mais ansiosos e irritados."),
                        List(
                            "Um mesmo horário para levantar todos os dias",
                            "Luz de manhã, uma noite tranquila sem notícias",
                            "Se um pesadelo acordar você: luz, onde estou e que dia é hoje, água, expirações lentas",
                            "O álcool aumenta os pesadelos na segunda metade da noite"),
                        P("Um pesadelo que se repete pode ser reescrito: o ensaio por imagens torna os pesadelos menos frequentes e mais fracos. Comece por um mais leve."),
                        Practice(TechniqueId.Nightmare, "Reescrever um pesadelo")
                    ],
                    "Mantenha um mesmo horário para levantar. Se você tem pesadelos que se repetem, tente reescrever um deles, começando por um mais leve.",
                    "Como você está dormindo agora? O que ajuda depois de uma noite difícil?"),
                new(
                    "Apoio e pessoas",
                    4,
                    [
                        P("O trauma muitas vezes afasta as pessoas dos outros e do que importava antes. O apoio das pessoas próximas é um dos fatores mais fortes de recuperação."),
                        List(
                            "Você não precisa falar do acontecimento: dá para simplesmente estar junto",
                            "Diga às pessoas próximas como ajudar você num momento difícil",
                            "Traga de volta aos poucos o que dava apoio: caminhadas, movimento, coisas de que você gostava",
                            "Não anestesie as emoções com álcool: isso atrapalha a recuperação"),
                        P("Evitar o que lembra o acontecimento é compreensível, mas com o tempo isso estreita a vida e mantém o trauma «quente». É melhor trabalhar isso com um especialista do que se forçar sozinho."),
                        P("O que o acontecimento mudou no seu jeito de pensar sobre si — a culpa, «tem algo errado comigo», «não dá para confiar em ninguém» — pode ser trabalhado no chat, sem recontar o que aconteceu."),
                        TraumaChat("Trabalhar isso no chat"),
                        Practice(TechniqueId.SelfCompassion, "Palavras gentis para si")
                    ],
                    "Faça uma coisa que antes dava apoio a você, e passe um tempo com alguém de confiança.",
                    "Quem e o que dá apoio a você agora?"),
                new(
                    "O caminho até um especialista",
                    5,
                    [
                        P("Os sintomas pós-traumáticos respondem bem à psicoterapia focada no trauma: exposição prolongada, terapia cognitiva, EMDR, exposição escrita. Geralmente leva 8–16 sessões."),
                        See(ArticleIds.TraumaTreatment, "Como o trauma é tratado e como escolher um especialista"),
                        H("Como se preparar"),
                        List(
                            "Refaça a escala PCL-5 e compare com o início",
                            "Exporte um resumo do aplicativo para o especialista: «Perfil → Opções → Dados e backup»",
                            "Anote suas perguntas: qual método, quantas sessões, como o progresso será medido",
                            "Você não precisa contar tudo sobre o acontecimento no primeiro encontro"),
                        TestsTab("Escala PCL-5", "pcl5"),
                        Crisis("Preciso de ajuda agora")
                    ],
                    "Encontre um ou dois especialistas que trabalhem com trauma e marque uma consulta.",
                    "O que foi mais útil no curso? Sobre o que você quer conversar com o especialista?")
            ])
    ];
}
