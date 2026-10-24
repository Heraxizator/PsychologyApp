using PsychologyApp.Domain.Exposure;

namespace PsychologyApp.Application.Exposure;

public static partial class ExposureContent
{
    private static readonly string[] LessonsEs =
    [
        "Daba menos miedo de lo que esperaba",
        "Lo logré, aun con ansiedad",
        "Nadie notó nada",
        "La ansiedad subió y bajó por sí sola",
        "Aunque sea incómodo, puedo atravesarlo",
        "No necesité mi red de seguridad"
    ];

    private static readonly ExposureKindContent[] KindsEs =
    [
        new(
            FearKind.Social,
            "Las personas y su juicio",
            "Hablar en público, llamar por teléfono, conocer gente, comer delante de otros",
            [
                "Todos verán que estoy nervioso",
                "Diré una tontería y se reirán de mí",
                "Pensarán mal de mí",
                "Me sonrojaré y todos lo verán",
                "Me quedaré en blanco y no podré seguir",
                "Me rechazarán"
            ],
            [
                "Las llamadas telefónicas",
                "Las presentaciones y reuniones",
                "Conocer y hablar con desconocidos",
                "Comer o escribir delante de otros",
                "Pedir ayuda o devolver algo",
                "Decir que no"
            ],
            [
                "Ensayar lo que voy a decir",
                "Evitar el contacto visual",
                "Hablar bajo y poco",
                "Quedarme al lado de alguien conocido",
                "Esconder las manos o la cara",
                "Irme antes",
                "Mirar el móvil",
                "Beber alcohol para darme valor"
            ],
            [
                new("Preguntar a un dependiente dónde encontrar algo", 25),
                new("Saludar primero a un vecino", 30),
                new("Llamar para pedir cita con el médico", 40),
                new("Devolver un artículo en una tienda", 50),
                new("Hacer una pregunta en una reunión o en clase", 60),
                new("Iniciar una conversación con un desconocido", 65),
                new("Decir que no a la petición de un compañero", 70),
                new("Dar una charla breve ante un grupo", 85)
            ]),
        new(
            FearKind.Panic,
            "Ataques de pánico y sensaciones del cuerpo",
            "Corazón acelerado, falta de aire, mareo",
            [
                "Me dará un infarto",
                "Me voy a asfixiar",
                "Me voy a desmayar",
                "Me volveré loco o perderé el control",
                "Me sentiré mal y nadie me ayudará",
                "Haré el ridículo delante de todos"
            ],
            [
                "El transporte público y el metro",
                "Las colas y las multitudes",
                "Estar solo lejos de casa",
                "El ejercicio y subir escaleras",
                "El calor y los lugares cargados",
                "El café"
            ],
            [
                "Llevar agua o una pastilla",
                "Tomarme el pulso",
                "Sentarme cerca de la salida",
                "Salir solo acompañado",
                "Distraerme con el móvil",
                "Agarrarme a una barandilla o a la pared",
                "Vigilar mi respiración",
                "Irme en cuanto empieza"
            ],
            [
                new("Subir tres pisos a paso rápido", 30, ProvokesBodySensations: true),
                new("Tomar una taza de café", 35, ProvokesBodySensations: true),
                new("Hacer cola en una tienda", 40),
                new("Ir una parada en autobús", 50),
                new("Caminar solo a 15 minutos de casa", 55),
                new("Ir tres estaciones de metro sin agua en el bolso", 70),
                new("Sentarme en medio de una fila en el cine", 75),
                new("Ir en metro en hora punta", 85)
            ]),
        new(
            FearKind.Specific,
            "Un miedo concreto",
            "Alturas, volar, perros, médicos, sangre, ascensores",
            [
                "Pasará algo terrible",
                "No soportaré el miedo",
                "Perderé el control",
                "Me harán daño o me morderán",
                "Me caeré",
                "Me quedaré atrapado y no podré salir"
            ],
            [
                "Las alturas y los balcones",
                "Los ascensores y los espacios cerrados",
                "Los animales",
                "Los médicos, las inyecciones, la sangre",
                "Volar",
                "Los puentes y los túneles"
            ],
            [
                "Cerrar los ojos o apartar la mirada",
                "Agarrarme a alguien",
                "Ir solo acompañado",
                "Planear la salida de antemano",
                "Distraerme con música",
                "Tomar un calmante"
            ],
            [
                new("Mirar fotos de lo que me da miedo", 20),
                new("Ver un vídeo sobre ello", 30),
                new("Estar cerca a una distancia segura", 40),
                new("Acercarme durante un minuto", 55),
                new("Quedarme muy cerca 5 minutos", 65),
                new("Tocarlo o entrar (ascensor, balcón)", 75),
                new("Repetirlo solo, sin compañía", 85),
                new("Hacerlo en un lugar desconocido", 90)
            ]),
        new(
            FearKind.Uncertainty,
            "La incertidumbre y el «¿y si pasa algo?»",
            "Preocuparse, comprobar, buscar garantías",
            [
                "Pasará algo malo si no lo compruebo",
                "No soporto no saber algo con certeza",
                "Le pasará algo a un ser querido",
                "Me equivocaré y será un desastre",
                "Enfermaré gravemente"
            ],
            [
                "Decidir sin tener toda la información",
                "Enviar mensajes sin revisarlos",
                "No saber dónde están mis seres queridos",
                "Las situaciones nuevas e imprevisibles",
                "Leer sobre enfermedades"
            ],
            [
                "Comprobar muchas veces",
                "Buscar en internet",
                "Preguntar a otros si todo está bien",
                "Llamar a mis seres queridos para quedarme tranquilo",
                "Planearlo todo al detalle",
                "Aplazar las decisiones"
            ],
            [
                new("Enviar un mensaje después de leerlo una sola vez", 25),
                new("Elegir un plato en una cafetería en un minuto", 30),
                new("No llamar a un ser querido que se retrasa 30 minutos", 50),
                new("Enviar un correo de trabajo sin revisarlo", 55),
                new("No buscar un síntoma en internet durante un día", 60),
                new("Pasar un día libre sin planes", 65),
                new("No preguntar a nadie si decidí bien", 70),
                new("Salir de casa tras comprobar la puerta una sola vez", 80)
            ]),
        new(
            FearKind.Other,
            "Otra cosa",
            "Describe tu miedo con tus propias palabras",
            [
                "Pasará algo malo",
                "No podré con ello",
                "Perderé el control",
                "Me juzgarán"
            ],
            [],
            [
                "Evitar la situación",
                "Ir solo acompañado",
                "Irme antes",
                "Distraerme con el móvil",
                "Planear la salida de antemano"
            ],
            [])
    ];
}
