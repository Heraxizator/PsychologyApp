using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

public static partial class ExposureContent
{
    private static readonly string[] LessonsPt =
    [
        "Foi menos assustador do que eu esperava",
        "Eu consegui, mesmo com ansiedade",
        "Ninguém percebeu nada",
        "A ansiedade subiu e desceu sozinha",
        "Mesmo que seja desconfortável, consigo passar por isso",
        "Não precisei da minha rede de segurança"
    ];

    private static readonly ExposureKindContent[] KindsPt =
    [
        new(
            FearKind.Social,
            "As pessoas e o julgamento delas",
            "Falar em público, telefonar, conhecer pessoas, comer na frente dos outros",
            [
                "Todos vão ver que estou nervoso",
                "Vou dizer uma bobagem e vão rir de mim",
                "Vão pensar mal de mim",
                "Vou ficar vermelho e todos vão ver",
                "Vou dar um branco e não vou conseguir continuar",
                "Vão me rejeitar"
            ],
            [
                "Ligações telefônicas",
                "Apresentações e reuniões",
                "Conhecer e conversar com estranhos",
                "Comer ou escrever na frente dos outros",
                "Pedir ajuda ou devolver algo",
                "Dizer não"
            ],
            [
                "Ensaiar o que vou dizer",
                "Evitar o contato visual",
                "Falar baixo e pouco",
                "Ficar perto de alguém conhecido",
                "Esconder as mãos ou o rosto",
                "Ir embora mais cedo",
                "Olhar o celular",
                "Beber álcool para criar coragem"
            ],
            [
                new("Perguntar a um vendedor onde encontrar algo", 25),
                new("Cumprimentar um vizinho primeiro", 30),
                new("Ligar para marcar uma consulta médica", 40),
                new("Devolver um produto em uma loja", 50),
                new("Fazer uma pergunta em uma reunião ou aula", 60),
                new("Puxar conversa com um desconhecido", 65),
                new("Dizer não ao pedido de um colega", 70),
                new("Fazer uma fala curta diante de um grupo", 85)
            ]),
        new(
            FearKind.Panic,
            "Ataques de pânico e sensações do corpo",
            "Coração disparado, falta de ar, tontura",
            [
                "Vou ter um infarto",
                "Vou sufocar",
                "Vou desmaiar",
                "Vou enlouquecer ou perder o controle",
                "Vou passar mal e ninguém vai me ajudar",
                "Vou passar vergonha na frente de todos"
            ],
            [
                "Transporte público e metrô",
                "Filas e multidões",
                "Ficar sozinho longe de casa",
                "Exercícios e subir escadas",
                "Calor e lugares abafados",
                "Café"
            ],
            [
                "Levar água ou um comprimido",
                "Medir meu pulso",
                "Sentar perto da saída",
                "Sair só acompanhado",
                "Me distrair com o celular",
                "Me segurar num corrimão ou na parede",
                "Vigiar minha respiração",
                "Ir embora assim que começa"
            ],
            [
                new("Subir três andares em passo rápido", 30, ProvokesBodySensations: true),
                new("Tomar uma xícara de café", 35, ProvokesBodySensations: true),
                new("Ficar na fila de uma loja", 40),
                new("Andar um ponto de ônibus", 50),
                new("Caminhar sozinho a 15 minutos de casa", 55),
                new("Andar três estações de metrô sem água na bolsa", 70),
                new("Sentar no meio de uma fileira no cinema", 75),
                new("Andar de metrô no horário de pico", 85)
            ]),
        new(
            FearKind.Specific,
            "Um medo específico",
            "Altura, avião, cães, médicos, sangue, elevadores",
            [
                "Algo terrível vai acontecer",
                "Não vou aguentar o medo",
                "Vou perder o controle",
                "Vou me machucar ou ser mordido",
                "Vou cair",
                "Vou ficar preso e não vou conseguir sair"
            ],
            [
                "Alturas e sacadas",
                "Elevadores e lugares fechados",
                "Animais",
                "Médicos, injeções, sangue",
                "Voar de avião",
                "Pontes e túneis"
            ],
            [
                "Fechar os olhos ou desviar o olhar",
                "Me segurar em alguém",
                "Ir só acompanhado",
                "Planejar a saída com antecedência",
                "Me distrair com música",
                "Tomar um calmante"
            ],
            [
                new("Ver fotos do que me dá medo", 20),
                new("Assistir a um vídeo sobre isso", 30),
                new("Ficar por perto a uma distância segura", 40),
                new("Chegar mais perto por um minuto", 55),
                new("Ficar bem perto por 5 minutos", 65),
                new("Tocar ou entrar (elevador, sacada)", 75),
                new("Fazer de novo sozinho, sem companhia", 85),
                new("Fazer em um lugar desconhecido", 90)
            ]),
        new(
            FearKind.Uncertainty,
            "A incerteza e o “e se acontecer alguma coisa?”",
            "Preocupar-se, verificar, buscar garantias",
            [
                "Algo ruim vai acontecer se eu não verificar",
                "Não suporto não ter certeza",
                "Vai acontecer alguma coisa com alguém que eu amo",
                "Vou errar e será um desastre",
                "Vou ficar gravemente doente"
            ],
            [
                "Decidir sem ter todas as informações",
                "Enviar mensagens sem revisar",
                "Não saber onde estão as pessoas que amo",
                "Situações novas e imprevisíveis",
                "Ler sobre doenças"
            ],
            [
                "Verificar muitas vezes",
                "Pesquisar na internet",
                "Perguntar aos outros se está tudo bem",
                "Ligar para quem amo para me tranquilizar",
                "Planejar tudo nos mínimos detalhes",
                "Adiar as decisões"
            ],
            [
                new("Enviar uma mensagem depois de ler uma só vez", 25),
                new("Escolher um prato no café em um minuto", 30),
                new("Não ligar para alguém querido que está 30 minutos atrasado", 50),
                new("Enviar um e-mail de trabalho sem revisar", 55),
                new("Não pesquisar um sintoma na internet por um dia", 60),
                new("Passar um dia de folga sem planos", 65),
                new("Não perguntar a ninguém se decidi certo", 70),
                new("Sair de casa depois de verificar a porta uma só vez", 80)
            ]),
        new(
            FearKind.Other,
            "Outra coisa",
            "Descreva seu medo com suas próprias palavras",
            [
                "Algo ruim vai acontecer",
                "Não vou dar conta",
                "Vou perder o controle",
                "Vão me julgar"
            ],
            [],
            [
                "Evitar a situação",
                "Ir só acompanhado",
                "Ir embora mais cedo",
                "Me distrair com o celular",
                "Planejar a saída com antecedência"
            ],
            [])
    ];
}
