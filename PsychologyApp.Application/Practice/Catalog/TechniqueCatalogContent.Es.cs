using PsychologyApp.Application.Models.Practice;
using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Practice.Catalog;

internal static class TechniqueCatalogContentEs
{
    internal static TheorySection[] Theory(string what, string when, string how, string example) =>
    [
        new("Qué es", what),
        new("Cuándo ayuda", when),
        new("Cómo es la práctica", how),
        new("Ejemplo", example)
    ];

    /// <summary>Common topics for a "problem" field: a start to tap and then add a word or two, not a full answer.</summary>
    private static readonly string[] ProblemTopics =
        ["Una pelea con alguien cercano", "Algo en el trabajo", "Un conflicto con mi jefe", "Los estudios o un examen", "La salud", "El dinero", "La soledad", "Un fracaso", "Una ruptura", "La relación con mis padres", "Los hijos", "Una mudanza", "El futuro", "Las críticas hacia mí"];

    private static readonly string[] BackupOptions =
        ["Pedir ayuda", "Esperar y volver a intentarlo", "Buscar otro camino", "Exigirme menos", "Tomar un descanso", "Hablarlo con alguien cercano", "Ir a un especialista", "Buscar un trabajo extra", "Hacer un curso", "Dividir la tarea en pasos", "Aplazar la decisión", "Empezar por poco"];

    private static readonly string[] HarshThoughts =
        ["«Lo voy a arruinar todo»", "«Me van a juzgar»", "«No puedo con esto»", "«Nadie me necesita»", "«Es un desastre»", "«Nada va a salir bien»", "«Soy peor que los demás»", "«Todo siempre sale mal»", "«Es culpa mía»", "«Algo anda mal conmigo»", "«Esto nunca va a terminar»", "«Tengo que ser perfecto»"];

    internal static readonly BuiltInTechniqueDefinition Spin = new(
        "Práctica", "Spin",
        [
            "1. Describe el episodio que te preocupa",
            "2. Nombra el sentimiento principal",
            "3. Valora el sentimiento de 0 a 10"
        ],
        "El Spin de Zivorad Slavinski ayuda a reducir la carga emocional de un recuerdo doloroso.",
        TechniqueUiKind.Entry, "Técnica n.º 1", "26.01.2023", "Spin", "Reducir la carga emocional en 3 pasos", "Momento difícil", "Zivorad Slavinski", 5, "Autorenew",
        [
            new TechniqueEntrySeed("Episodio", "P. ej., la conversación de ayer que no puedes soltar", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentimiento principal", "Dolor, miedo, enfado…", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Intensidad del sentimiento", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Una técnica sencilla de neutralización: nombras el episodio, el sentimiento y su intensidad. A menudo, solo darte cuenta ya reduce la carga.",
            "Cuando una escena o un pensamiento da vueltas y no suelta la ansiedad o el resentimiento.",
            "Escribe el episodio y el sentimiento, y valora la intensidad con sinceridad. Repítelo otro día si hace falta.",
            "Episodio: «Un compañero ignoró mi idea». Sentimiento: dolor. Valoración: 8 → después 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Comparison = new(
        "Práctica", "Comparar la importancia",
        [
            "1. Plantea el problema",
            "2. ¿Qué era importante antes en este ámbito?",
            "3. ¿Qué es importante ahora?",
            "4. ¿Qué será importante en el futuro?",
            "5. Compara las tres respuestas y anota qué cambió"
        ],
        "Comparar la importancia a lo largo del tiempo ayuda a situar un problema en el contexto de tu vida.",
        TechniqueUiKind.Entry, "Técnica n.º 2", "26.01.2023", "Comparar la importancia", "Cuando el problema parece demasiado grande", "Tamaño del problema", "PNL", 7, "CompareArrows",
        [
            new TechniqueEntrySeed("Problema", "¿Qué te parece insoportable ahora mismo?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Era importante antes", "P. ej., estudios, relaciones, carrera", Suggestions: ["Los estudios", "La familia", "Los amigos", "La salud", "La carrera", "Las aficiones", "El deporte", "El amor", "Los viajes", "La libertad", "El reconocimiento", "El hogar"]),
            new TechniqueEntrySeed("Es importante ahora", "¿Qué se lleva más tu atención?", Suggestions: ["El trabajo", "La familia", "La salud", "El dinero", "Las relaciones", "El descanso", "Los hijos", "Los amigos", "Los estudios", "El sueño", "La tranquilidad", "El crecimiento personal"]),
            new TechniqueEntrySeed("Será importante en el futuro", "¿Hacia dónde vas?", Suggestions: ["La estabilidad", "La familia", "La libertad", "La salud", "Crecer", "La gente que quiero", "Una casa propia", "Un trabajo que me guste", "Los viajes", "La calma", "Los hijos", "Ahorros"]),
            new TechniqueEntrySeed("¿Qué cambió?", "¿El problema sigue dominándolo todo?", Suggestions: ["El problema es solo una parte de la vida", "Hay más cosas importantes de lo que parecía", "Es una etapa pasajera", "Se siente menos intenso", "Me siento más tranquilo", "Veo que ya salí adelante antes", "Apareció la esperanza", "Todavía no ha cambiado nada"])
        ],
        Theory(
            "La técnica muestra que el peso de un problema cambia con el tiempo y rara vez sigue siendo central para siempre.",
            "Cuando una dificultad ocupa toda la pantalla y parece permanente.",
            "Rellena los cuatro campos y compara: otros valores suelen hacer el problema más pequeño.",
            "Problema: «No tengo trabajo». Antes: los estudios. Ahora: buscar trabajo. Futuro: estabilidad. Conclusión: una etapa, no toda la vida."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Polarity = new(
        "Práctica", "Polaridades",
        [
            "1. Escribe lo que te gusta de la situación",
            "2. Escribe lo que no te gusta",
            "3. Compara las parejas y fíjate en el equilibrio"
        ],
        "El método de las polaridades ayuda a ver los dos lados y a reducir la tensión interna.",
        TechniqueUiKind.Polarity, "Técnica n.º 3", "26.01.2023", "Polaridades", "Ver los dos lados de una situación", "Distintos lados", "Zivorad Slavinski", 5, "Contrast",
        null,
        Theory(
            "Toda situación tiene lados atractivos y desagradables. Nombrar ambos reduce la fijación en uno solo.",
            "Cuando solo ves lo negativo o solo lo positivo y te sientes atascado.",
            "Añade parejas de «me gusta / no me gusta» sobre un tema. Compara las listas sin juzgar qué es lo correcto.",
            "Trabajo: me gusta el sueldo, no me gusta el horario. Casa: me gusta lo acogedora, no me gusta el ruido de los vecinos."),
        "Lo que no te gusta", "Lo que te gusta");

    internal static readonly BuiltInTechniqueDefinition Paper = new(
        "Práctica", "Hoja de papel",
        [
            "1. Escribe el pensamiento en papel (o en el campo de abajo)",
            "2. Léelo una vez en voz alta",
            "3. Dobla, rompe o suelta simbólicamente la hoja"
        ],
        "Escribir los pensamientos en papel reduce su carga emocional.",
        TechniqueUiKind.Paper, "Técnica n.º 4", "26.01.2023", "Hoja de papel", "Cuando los pensamientos dan vueltas", "Pensamientos", "Psique", 3, "Description",
        [new TechniqueEntrySeed("Pensamiento", "P. ej., «Voy a arruinar la presentación»", Suggestions: HarshThoughts)],
        Theory(
            "Escribir saca el pensamiento de tu cabeza. Destruir la hoja simboliza soltarlo.",
            "Cuando la misma frase da vueltas durante horas y te impide dormir o concentrarte.",
            "Descarga el pensamiento, léelo y luego destruye la hoja, de verdad o en tu imaginación.",
            "Pensamiento: «Van a pensar que soy incompetente». Tras escribirlo y quemar la hoja, la ansiedad suele bajar."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Future = new(
        "Práctica", "50 años después",
        [
            "1. Plantea el problema",
            "2. Valora su importancia dentro de 50 años, de 0 a 10"
        ],
        "Llevar un problema hacia adelante en el tiempo reduce su importancia subjetiva.",
        TechniqueUiKind.Entry, "Técnica n.º 5", "30.01.2023", "50 años después", "Llevar el problema al futuro", "Tamaño del problema", "PNL", 2, "Schedule",
        [
            new TechniqueEntrySeed("Problema", "¿Qué te parece hoy un desastre?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Importancia dentro de 50 años", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Imaginar un suceso muy lejos en el futuro suele suavizarlo: lo ves con más perspectiva.",
            "Cuando un revés parece el fin del mundo aunque sea temporal.",
            "Nombra el problema y valora cuánto importará dentro de cincuenta años. La puntuación suele ser más baja.",
            "Problema: «No me invitaron a la reunión». Dentro de 50 años: 1–2 de 10."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Hack = new(
        "Práctica", "Mancha blanca",
        [
            "1. Recuerda el episodio que te preocupa",
            "2. Imagínalo como una mancha blanca sin detalles",
            "3. Mantén la mancha hasta que la tensión baje"
        ],
        "Quitar los detalles de un recuerdo suele reducir su carga emocional.",
        TechniqueUiKind.Entry, "Técnica n.º 6", "30.01.2023", "Mancha blanca", "Suavizar un recuerdo doloroso", "Momento difícil", "PNL", 3, "BlurOn",
        [new TechniqueEntrySeed("Episodio", "En pocas palabras: qué pasó y qué duele más", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Los detalles alimentan el sentimiento. Una mancha blanca quita los desencadenantes y le da una pausa a tu sistema nervioso.",
            "Cuando una imagen vívida del pasado te golpea con emoción.",
            "Recuerda el episodio y luego sustituye la imagen por una simple mancha blanca durante 30–60 segundos.",
            "En lugar de la cara de quien te hizo daño, una mancha blanca. La respiración se calma y la mente vuelve al presente."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Experience = new(
        "Práctica", "Modificación de la experiencia",
        [
            "1. Elige un ámbito para trabajar",
            "2. Describe tu actitud hacia él",
            "3. Nombra las creencias relacionadas",
            "4. ¿Cómo cambia la actitud al pensar en experiencias pasadas?",
            "5. Describe una experiencia negativa",
            "6. Valora la experiencia negativa (−10…10)",
            "7. ¿Qué experiencia positiva podría sustituirla?",
            "8. ¿Cómo cambiaría tu actitud con esa experiencia?",
            "9. Valora la nueva experiencia (−10…10)"
        ],
        "La EMO ayuda a sustituir una experiencia limitante por otra que te sostenga más.",
        TechniqueUiKind.Entry, "Técnica n.º 7", "08.02.2023", "Modificación de la experiencia", "Reescribir una experiencia limitante", "Momento difícil", "Filip Slavinski", 15, "Psychology",
        [
            new TechniqueEntrySeed("Ámbito", "Relaciones, trabajo, salud…", Suggestions: ["Las relaciones", "El trabajo", "La salud", "El dinero", "Hablar en público", "La familia", "Los estudios", "La apariencia", "La amistad", "La creatividad", "El deporte", "La carrera"]),
            new TechniqueEntrySeed("Actitud hacia el ámbito", "¿Cómo lo sientes ahora?", Suggestions: ["Ansiedad", "Inseguridad", "Irritación", "Indiferencia", "Interés", "Cansancio", "Miedo", "Vergüenza", "Esperanza", "Enfado", "Confusión", "La calma"]),
            new TechniqueEntrySeed("Creencias relacionadas", "P. ej., «No lo voy a lograr»", Suggestions: ["«No lo voy a lograr»", "«No soy suficiente»", "«No me van a aceptar»", "«Nada va a cambiar»", "«Tengo que ser perfecto»", "«No tengo suerte»", "«No me lo merezco»", "«Los demás son mejores»", "«No puedo equivocarme»", "«Ya soy mayor para esto»"]),
            new TechniqueEntrySeed("Actitud y experiencia pasada", "¿Qué cambia cuando recuerdas el pasado?", Suggestions: ["Se hace más pesado", "Se vuelve más tranquilo", "No cambia nada", "Surge el enfado", "Surge la tristeza", "Surge el miedo", "Recuerdo que salí adelante", "Quiero evitarlo"]),
            new TechniqueEntrySeed("Experiencia negativa", "Un episodio concreto o un patrón"),
            new TechniqueEntrySeed("Valoración de la experiencia negativa", "", EntryFieldKind.RatingNeg10To10),
            new TechniqueEntrySeed("Sustituto positivo", "¿Qué experiencia de apoyo es posible?", Suggestions: ["Una vez que lo logré", "Una vez que me apoyaron", "Una vez que salí adelante solo", "Una vez que me elogiaron", "Una vez que ayudé a alguien", "Una vez que aprendí algo nuevo", "Una vez que no me rendí", "Una vez que me aceptaron como soy"]),
            new TechniqueEntrySeed("Actitud con lo positivo", "¿Cómo cambiaría tu forma de ver el ámbito?", Suggestions: ["Aparece la esperanza", "Más calma", "Quiero volver a intentarlo", "Se vuelve interesante", "Aparece la confianza", "Se hace más ligero", "Siento fuerza", "Todavía cuesta creerlo"]),
            new TechniqueEntrySeed("Valoración de la nueva experiencia", "", EntryFieldKind.RatingNeg10To10)
        ],
        Theory(
            "Las creencias suelen apoyarse en una sola experiencia negativa muy viva. Sustituirla de forma consciente puede cambiar tu actitud.",
            "Cuando «siempre ha sido así» y el cambio parece imposible.",
            "Recorre los campos en orden. Compara las dos valoraciones: busca que la segunda sea más alta.",
            "Ámbito: hablar en público. Negativa: un examen suspendido (−8). Sustituto: una buena exposición (+6)."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Copied = new(
        "Práctica", "Repítelo",
        [
            "1. Escribe la frase que te preocupa",
            "2. Repítela en voz alta como si la oyeras por primera vez",
            "3. Sigue repitiéndola hasta que la emoción baje"
        ],
        "Repetir la frase de un problema «como por primera vez» reduce la intensidad emocional.",
        TechniqueUiKind.Copied, "Técnica n.º 8", "21.01.2025", "Repítelo", "Oír el problema como por primera vez", "Tamaño del problema", "PNL", 2, "Repeat",
        [new TechniqueEntrySeed("Frase", "Tengo miedo de perder mi trabajo", Suggestions: HarshThoughts)],
        Theory(
            "Una frase conocida pierde carga cuando se dice despacio y con tono neutro, sin dramatismo.",
            "Cuando un pensamiento suena como un veredicto en tu cabeza.",
            "Escribe la frase y repítela 5–10 veces con pausas, fijándote en las sensaciones del cuerpo.",
            "«Nadie me necesita»: tras siete repeticiones suele sonar absurda y más ligera."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Extend = new(
        "Práctica", "Plan B",
        [
            "1. Nombra el problema",
            "2. Escribe tres alternativas realistas",
            "3. Fíjate en cómo cambia la ansiedad cuando hay salidas"
        ],
        "Las alternativas reducen la importancia exagerada de un único resultado.",
        TechniqueUiKind.Entry, "Técnica n.º 9", "21.01.2025", "Plan B", "Bajar la presión con un plan B", "Tamaño del problema", "PNL", 3, "Backup",
        [
            new TechniqueEntrySeed("Problema", "¿Y si falla el plan principal?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Alternativa 1", "P. ej., aceptar un proyecto como autónomo", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Alternativa 2", "P. ej., formarme para un puesto parecido", Suggestions: BackupOptions),
            new TechniqueEntrySeed("Alternativa 3", "P. ej., recortar gastos por un tiempo", Suggestions: BackupOptions)
        ],
        Theory(
            "Cuando todo parece a todo o nada, la mente se tensa. Las alternativas devuelven la sensación de poder elegir.",
            "Antes de entrevistas, exámenes o situaciones en las que fallar parece una catástrofe.",
            "Plantea el problema y tres planes B posibles, aunque sean modestos.",
            "Problema: «No me van a dar el puesto». Plan B: unas prácticas, recomendaciones, otra ciudad."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Resize = new(
        "Práctica", "Hazlo pequeño",
        [
            "1. Describe lo que te preocupa",
            "2. Imagínalo como un objeto delante de ti",
            "3. Encógelo y aléjalo hasta que pierda interés"
        ],
        "Encoger y alejar visualmente un problema reduce el interés emocional que despierta.",
        TechniqueUiKind.Entry, "Técnica n.º 10", "22.01.2025", "Hazlo pequeño", "Hacer el problema pequeño y lejano", "Tamaño del problema", "PNL", 2, "ZoomOutMap",
        [new TechniqueEntrySeed("Lo que te preocupa", "P. ej., un conflicto que no te deja dormir", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "El tamaño y la distancia de una imagen influyen en su importancia. Un objeto pequeño y lejano rara vez te altera.",
            "Cuando un problema parece enorme y lo tienes justo delante.",
            "Crea una imagen, encógela poco a poco y llévala hasta el horizonte durante 10–20 segundos.",
            "Una pelea se convierte en un punto diminuto. La respiración se suaviza y los pensamientos se aclaran."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Check = new(
        "Práctica", "Compruébalo",
        [
            "1. Describe aquello a lo que le das vueltas",
            "2. Deja de pensar en ello a propósito durante 1–2 minutos",
            "3. Fíjate en qué cambió en el cuerpo y el ánimo"
        ],
        "Demasiada atención infla la importancia. Una pausa breve pone a prueba cuánto te controla el pensamiento.",
        TechniqueUiKind.Entry, "Técnica n.º 11", "22.01.2025", "Compruébalo", "Ver qué pasa si lo sueltas", "Tamaño del problema", "PNL", 2, "FactCheck",
        [new TechniqueEntrySeed("Aquello a lo que le das vueltas", "¿Qué pensamiento vuelve una y otra vez?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem)],
        Theory(
            "Si un pensamiento fuera vital para sobrevivir, soltarlo parecería peligroso. El experimento suele mostrar que no pasa nada catastrófico.",
            "Cuando no puedes dejar de darle vueltas al mismo tema.",
            "Nombra el pensamiento y lleva con suavidad la atención a la respiración o a los sonidos que te rodean.",
            "Pensamiento: «¿Y si estoy enfermo?». Tras una pausa, la ansiedad suele bajar de 7 a 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Observer = new(
        "Práctica", "Posición de observador",
        [
            "1. Describe la situación que te preocupa",
            "2. Escribe tus sentimientos en primera persona",
            "3. Mira la situación como un observador neutral",
            "4. Valora la tensión después de cambiar de posición"
        ],
        "Cambiar de posición perceptiva ayuda a tomar distancia de un pico emocional y ver la situación con más amplitud.",
        TechniqueUiKind.Entry, "Técnica n.º 12", "12.06.2026", "Posición de observador", "Tomar distancia de las emociones y ver la situación desde fuera", "Momento difícil", "PNL", 4, "Visibility",
        [
            new TechniqueEntrySeed("Situación", "¿Qué pasó o qué repasas una y otra vez en tu cabeza?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Sentimientos en primera persona", "¿Qué sientes estando dentro de la situación?", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Lo que ve el observador", "¿Qué notaría un observador externo y tranquilo?", Suggestions: ["Dos personas discutiendo", "Una persona cansada y alterada", "Una situación normal de trabajo", "Cada uno defiende su postura", "En una semana estará olvidado", "Las personas no se entendieron", "Una persona que se esfuerza mucho", "Desagradable, pero no peligroso", "Cada uno tiene su punto de vista", "La persona necesita descansar"]),
            new TechniqueEntrySeed("Tensión ahora", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "Cuando estás dentro de un episodio, las emociones parecen inevitables. La posición de observador crea distancia sin negar lo que vives.",
            "Cuando una situación te desborda y los argumentos racionales no ayudan.",
            "Describe la situación y tus sentimientos y luego reescríbela en tercera persona, de forma neutral y sin juzgar.",
            "Discusión: desde dentro, «no me respetan». Observador: «dos personas hablan en voz alta». Tensión: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Anchor = new(
        "Práctica", "Ancla de recursos",
        [
            "1. Recuerda un momento en que te sentiste seguro o tranquilo",
            "2. Describe las sensaciones del cuerpo",
            "3. Elige un gesto sencillo como ancla",
            "4. Valora la fuerza del estado de recursos"
        ],
        "Un ancla une un gesto del cuerpo a un estado de recursos para que puedas recuperar el apoyo más rápido.",
        TechniqueUiKind.Entry, "Técnica n.º 13", "12.06.2026", "Ancla de recursos", "Recuperar apoyo con un gesto y un recuerdo", "Momento difícil", "PNL", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Recuerdo de recursos", "¿Cuándo te sentiste fuerte, tranquilo o alegre?", Suggestions: ["Una exposición que salió bien", "Unas vacaciones en el mar", "El abrazo de alguien cercano", "Terminar algo difícil", "Un paseo por la naturaleza", "Aprobar un examen", "El nacimiento de un hijo", "Ganar una competición", "Una tarde con amigos", "Un elogio de mi jefe", "El primer día en un trabajo nuevo", "Una mañana sin prisas"]),
            new TechniqueEntrySeed("Sensaciones del cuerpo", "¿Dónde lo sientes? Calor, ligereza, arraigo…", Suggestions: ["Calor en el pecho", "Ligereza", "Los hombros abiertos", "Respiración regular", "Piernas firmes", "Una sonrisa", "Un corazón tranquilo", "La espalda recta", "Energía en las manos", "El vientre relajado"]),
            new TechniqueEntrySeed("Gesto o ancla", "P. ej., cerrar el puño, tocar la muñeca", Suggestions: ["Cerrar el puño", "Tocar la muñeca", "Unir el pulgar y el índice", "La mano en el corazón", "Una respiración profunda", "Apretar el lóbulo de la oreja", "Abrir los hombros", "Acariciar la palma"]),
            new TechniqueEntrySeed("Fuerza del estado", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "El cerebro une rápido un gesto con un estado si repites varias veces el par «recuerdo + gesto».",
            "Antes de algo estresante o cuando te falta apoyo interior.",
            "Recuerda el episodio de recursos, intensifica las sensaciones del cuerpo y fíjalas con un gesto sencillo.",
            "Recurso: una exposición que salió bien. Cuerpo: calor en el pecho. Ancla: tocar el pulgar. Fuerza: 8."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Grounding = new(
        "Práctica", "Anclaje 5-4-3-2-1",
        [
            "1. Nombra 5 cosas que ves",
            "2. Nombra 4 cosas que sientes con el cuerpo",
            "3. Nombra 3 sonidos a tu alrededor",
            "4. Nombra 2 olores",
            "5. Nombra 1 sabor o una sensación agradable"
        ],
        "El anclaje sensorial lleva la atención de los pensamientos ansiosos al momento presente.",
        TechniqueUiKind.Entry, "Técnica n.º 14", "12.06.2026", "Anclaje 5-4-3-2-1", "Volver al presente a través de los sentidos", "Momento difícil", "Psique", 3, "Nature",
        [
            new TechniqueEntrySeed("5 cosas que veo", "P. ej., ventana, taza, libro, lámpara, mano", Suggestions: ["Ventana", "Mesa", "Teléfono", "Ordenador", "Portátil", "Pantalla", "Teclado", "Taza", "Lámpara", "Mano", "Puerta", "Libro", "Silla", "Pared", "Cuadro", "Planta", "Reloj", "Armario", "Cielo", "Coche"]),
            new TechniqueEntrySeed("4 cosas que siento con el cuerpo", "P. ej., la silla, el suelo, la ropa, la temperatura del aire", Suggestions: ["Silla", "El suelo bajo mis pies", "La ropa", "El fresco", "El calor", "El teléfono en la mano", "El respaldo de la silla", "La tela sobre la piel", "La respiración en el pecho", "El viento", "El ratón", "Las llaves", "La almohada", "El reloj en la muñeca"]),
            new TechniqueEntrySeed("3 sonidos", "P. ej., la calle, el reloj, tu respiración", Suggestions: ["La calle", "Reloj", "La respiración", "Voces", "La nevera", "El viento", "El silencio", "Ordenador", "Los coches", "Los pájaros", "La música", "El aire acondicionado", "Pasos", "La lluvia"]),
            new TechniqueEntrySeed("2 olores", "P. ej., café, aire fresco", Suggestions: ["Café", "Aire fresco", "Jabón", "Comida", "Perfume", "Casi nada", "Té", "Pan", "Libros", "Flores", "Crema de manos", "Madera"]),
            new TechniqueEntrySeed("1 sabor o una sensación agradable", "P. ej., agua en la boca, una tela suave", Suggestions: ["Agua", "Té", "Una tela suave", "Las palmas calientes", "La boca fresca", "Café", "Un chicle de menta", "Chocolate", "Una manzana", "Una manta calentita"])
        ],
        Theory(
            "La ansiedad suele arrastrarte al pasado o al futuro. Contar a través de los sentidos te devuelve al aquí y ahora.",
            "En momentos de pánico, ansiedad fuerte, mareo o sensación de estar desconectado de la realidad.",
            "Recorre los pasos despacio, nombrando sensaciones concretas. Respira con calma entre uno y otro.",
            "5 veo → 4 siento → 3 oigo → la respiración se calma, los pensamientos se frenan."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Breathing = new(
        "Práctica", "Respiración cuadrada",
        [
            "1. Valora la tensión antes de la práctica",
            "2. Inhala contando 4",
            "3. Mantén contando 4",
            "4. Exhala contando 4",
            "5. Mantén contando 4; repite 4 ciclos y valora la tensión después"
        ],
        "Respirar con ritmo calma el sistema nervioso y reduce la tensión del cuerpo.",
        TechniqueUiKind.Entry, "Técnica n.º 15", "04.07.2026", "Respiración cuadrada", "Calmar el cuerpo con el ritmo de la respiración", "Momento difícil", "Psique", 3, "Air",
        [
            new TechniqueEntrySeed("Tensión antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "¿Qué notaste en el cuerpo o la respiración durante la práctica?", Suggestions: ["Bajaron los hombros", "La respiración se hizo más lenta", "Sentí más calor", "Me costó mantener la pausa", "Me distrajeron los pensamientos", "Me sentí más tranquilo", "Me mareé", "El corazón se calmó", "Me dieron ganas de bostezar", "Nada en especial"]),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "La respiración lenta y rítmica reduce la actividad del sistema nervioso simpático y ayuda al cuerpo a salir del modo de lucha o huida.",
            "En momentos de ansiedad, pánico, corazón acelerado, insomnio o sensación de desborde.",
            "Siéntate cómodamente y respira por la nariz: 4 inhalar, 4 mantener, 4 exhalar, 4 mantener. Repite 4 ciclos.",
            "Ansiedad 7/10 → tras 4 ciclos 4/10. Los hombros bajan y el pulso se calma."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SmallStep = new(
        "Práctica", "Un pequeño paso",
        [
            "1. Describe lo que te pesa o te impide empezar",
            "2. Elige el paso más pequeño y realista para hoy",
            "3. Escribe cuándo lo harás",
            "4. Valora tu energía después de escribir el paso"
        ],
        "Activación conductual: una pequeña acción devuelve la sensación de movimiento y control.",
        TechniqueUiKind.Entry, "Técnica n.º 16", "04.07.2026", "Un pequeño paso", "Recuperar el impulso con una acción mínima", "Momento difícil", "TCC", 5, "DirectionsWalk",
        [
            new TechniqueEntrySeed("Lo que pesa", "¿Qué te parece difícil, aburrido o imposible ahora mismo?", Suggestions: ["Cansancio", "Sin energía", "Demasiadas cosas por hacer", "Aburrimiento", "Miedo a empezar", "Nada parece importar", "Dormí mal", "Sin ánimo", "La soledad", "Procrastinación", "Dolor de cabeza", "No sé por dónde empezar"], Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("El paso más pequeño hoy", "P. ej., levantarme, lavarme la cara, enviar un mensaje", Suggestions: ["Levantarme de la cama", "Lavarme la cara", "Beber un poco de agua", "Enviar un mensaje", "Salir 5 minutos", "Ordenar una cosa", "Abrir una ventana", "Estirarme 2 minutos", "Darme una ducha", "Comer algo", "Llamar a alguien cercano", "Abrir el archivo que necesito", "Escribir un plan para el día", "Lavar una taza"]),
            new TechniqueEntrySeed("Cuándo lo harás", "Una hora concreta o un momento del día", Suggestions: ["Ahora mismo", "En 15 minutos", "En una hora", "Por la mañana", "Después del desayuno", "Después de comer", "Por la tarde", "Antes de dormir", "Mañana por la mañana", "El fin de semana"]),
            new TechniqueEntrySeed("Energía después", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "En la apatía, el cerebro espera a tener motivación para actuar. Un pequeño paso crea la motivación a través de la acción, y no al revés.",
            "Cuando no tienes energía, todo parece inútil o te cuesta empezar el día.",
            "Elige un paso de 2–5 minutos sin un plan perfecto. Escríbelo junto con la hora: eso ya es movimiento.",
            "Limpiar el piso → llevar una taza a la cocina a las 10:00. Energía: 2 → 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition ThoughtRecord = new(
        "Práctica", "Registro de pensamientos",
        [
            "1. Describe la situación",
            "2. Escribe el pensamiento automático",
            "3. Valora la emoción",
            "4. Formula un pensamiento alternativo más equilibrado",
            "5. Valora otra vez la emoción"
        ],
        "Un registro estructurado de TCC ayuda a ver la relación entre situación, pensamiento y emoción.",
        TechniqueUiKind.Entry, "Técnica n.º 17", "04.07.2026", "Registro de pensamientos", "Desmontar un pensamiento y reducir la carga emocional", "Momento difícil", "TCC", 7, "EditNote",
        [
            new TechniqueEntrySeed("Situación", "¿Qué pasó? ¿Dónde y cuándo?", Suggestions: ProblemTopics, Role: EntryFieldRole.Problem),
            new TechniqueEntrySeed("Pensamiento automático", "¿Qué pensaste en ese momento?", Suggestions: HarshThoughts),
            new TechniqueEntrySeed("Emoción", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Pensamiento alternativo", "¿Qué más podría ser cierto? Una mirada más equilibrada", Role: EntryFieldRole.Reframe),
            new TechniqueEntrySeed("Emoción ahora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Los pensamientos automáticos suelen sonar a hechos, aunque son interpretaciones. Escribirlos los hace visibles y comprobables.",
            "Después de un conflicto, una crítica, un fracaso o cuando un pensamiento no deja de dar vueltas.",
            "Describe la situación y el pensamiento, valora la emoción y busca una alternativa: no positividad forzada, sino algo más ajustado a la realidad.",
            "Situación: no responden a un mensaje. Pensamiento: nadie me necesita. Alternativa: está ocupado. Emoción: 8 → 5."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition SelfCompassion = new(
        "Práctica", "Palabras amables para ti",
        [
            "1. Escribe qué te estás criticando",
            "2. Imagina que un amigo cercano dijera esto: ¿qué le dirías?",
            "3. Formula la misma frase de apoyo para ti",
            "4. Valora la calidez o el alivio"
        ],
        "La autocompasión reduce la autocrítica dura y ayuda a recuperarte después de un error.",
        TechniqueUiKind.Entry, "Técnica n.º 18", "04.07.2026", "Palabras amables para ti", "Suavizar la autocrítica con apoyo", "Momento difícil", "TCC", 5, "Favorite",
        [
            new TechniqueEntrySeed("Lo que te criticas", "¿Qué te dices o de qué te culpas?", Suggestions: ["Un error en el trabajo", "No lo terminé", "Le grité a alguien cercano", "Me comparo con los demás", "Lo vuelvo a dejar para después", "Mi apariencia", "Mi peso", "Dije algo que no debía", "No soy buen padre o madre", "Gano muy poco", "No cumplí una promesa", "Soy demasiado sensible"]),
            new TechniqueEntrySeed("Lo que le dirías a un amigo", "¿Cómo apoyarías a alguien cercano en esta situación?", Role: EntryFieldRole.Kindness),
            new TechniqueEntrySeed("Frase para ti", "La misma amabilidad, dirigida a ti", Role: EntryFieldRole.SelfKindness),
            new TechniqueEntrySeed("Calidez / alivio", "", EntryFieldKind.Rating0To10)
        ],
        Theory(
            "La autocrítica rara vez motiva: más a menudo alimenta la vergüenza y la parálisis. La autocompasión no quita la responsabilidad, pero da apoyo.",
            "Después de un error, al compararte con otros o al sentir que no eres suficiente.",
            "Escribe la crítica y respóndete como lo harías con un amigo: con respeto y sin quitarle importancia.",
            "Crítica: otra vez lo arruiné todo. A un amigo: lo intentaste, tiene arreglo. A mí: puedo aprender de esto. Calidez: 3 → 6."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BodyScan = new(
        "Práctica", "¿Dónde está en el cuerpo?",
        [
            "1. Valora tu tensión ahora",
            "2. Recorre despacio el cuerpo con la atención, de la cabeza a los pies",
            "3. Encuentra el lugar donde la sensación es más fuerte",
            "4. Descríbela: cómo es, grande o pequeña, cálida o fría",
            "5. Nombra el sentimiento que la acompaña",
            "6. Haz 5 exhalaciones lentas como si cada una pasara por ese lugar, y valora otra vez la tensión"
        ],
        "Escaneo corporal: nota dónde vive un sentimiento, nómbralo y deja que el cuerpo se suavice sin pelear con la sensación.",
        TechniqueUiKind.Entry, "Técnica n.º 19", "16.10.2026", "¿Dónde está en el cuerpo?", "Notar dónde vive un sentimiento en el cuerpo y suavizar la tensión", "Momento difícil", "Mindfulness", 4, "Accessibility",
        [
            new TechniqueEntrySeed("Tensión ahora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("¿En qué parte del cuerpo lo sientes?", "Por ejemplo: pecho, garganta, hombros", Suggestions: ["Cabeza", "Frente", "Ojos", "Mandíbula", "Garganta", "Cuello", "Hombros", "Pecho", "Corazón", "Estómago", "Espalda", "Zona lumbar", "Brazos", "Manos", "Piernas", "Todo el cuerpo"]),
            new TechniqueEntrySeed("¿Cómo es la sensación?", "Por ejemplo: opresión, pesadez, un nudo", Suggestions: ["Opresión", "Pesadez", "Un nudo", "Presión", "Tensión", "Calor", "Frío", "Temblor", "Hormigueo", "Palpitación", "Vacío", "Entumecimiento", "Dolor", "Difícil de describir"]),
            new TechniqueEntrySeed("¿Qué sentimiento la acompaña?", "Por ejemplo: ansiedad, dolor emocional, cansancio", Role: EntryFieldRole.Emotion),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Los sentimientos resuenan en el cuerpo: la ansiedad como opresión en el pecho, el dolor emocional como un nudo en la garganta, el cansancio como hombros pesados. Cuando notamos la sensación y nombramos el sentimiento, la tensión suele bajar. Esto es la base del escaneo corporal en los programas de mindfulness.",
            "Cuando te sientes ansioso o pesado pero no sabes qué sientes exactamente; cuando el cuerpo está tenso tras una conversación o un día difícil. La práctica no explica enfermedades ni sustituye al médico: si una sensación es nueva, fuerte o no se va, consulta a un médico.",
            "Siéntate cómodamente y lleva despacio la atención desde la coronilla hasta los pies. Detente donde la sensación sea más intensa. No la cambies ni luches contra ella: descríbela y nombra el sentimiento. Luego haz unas exhalaciones lentas como si cada una pasara por ese lugar.",
            "Tensión 6. Dónde: el pecho. Sensación: opresión, como un puño. Sentimiento: ansiedad antes de una conversación. Tras 5 exhalaciones la opresión es más suave, tensión 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Flashback = new(
        "Práctica", "Un recuerdo me ha desbordado",
        [
            "1. Valora lo fuerte que es",
            "2. Dite: «Esto es un recuerdo. Eso fue entonces, y ahora es hoy»",
            "3. Nombra dónde estás y qué momento del día es",
            "4. Siente el apoyo bajo los pies, aprieta y suelta los puños",
            "5. Nombra tres cosas que ves",
            "6. Cinco exhalaciones lentas, agua fría, y valora otra vez"
        ],
        "Ayuda ante un recuerdo intrusivo: volver del recuerdo al presente a través del tiempo, el lugar, el cuerpo y los sentidos.",
        TechniqueUiKind.Entry, "Técnica n.º 20", "16.10.2026", "Un recuerdo me ha desbordado", "Volver al presente cuando un recuerdo difícil te desborda", "Momento difícil", "Trauma", 3, "MyLocation",
        [
            new TechniqueEntrySeed("Lo fuerte que es", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Dónde estás y qué momento del día es", "Por ejemplo: en casa, por la noche", Suggestions: ["En casa", "En el trabajo", "En la calle", "En el transporte", "De visita", "Mañana", "Tarde", "Anochecer", "Noche"]),
            new TechniqueEntrySeed("Tres cosas que ves", "Por ejemplo: ventana, taza, lámpara", Suggestions: ["Ventana", "Mesa", "Teléfono", "Taza", "Lámpara", "Puerta", "Silla", "Pared", "Cuadro", "Planta", "Reloj", "Cielo"]),
            new TechniqueEntrySeed("Lo fuerte que es ahora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Un recuerdo intrusivo (flashback) es un recuerdo de un trauma que se vive como si estuviera pasando de nuevo: con los mismos sentimientos y reacciones del cuerpo. El cerebro confunde «entonces» y «ahora». El tiempo, el lugar, el apoyo del cuerpo y lo que ves y oyes ayudan a volver al presente.",
            "Cuando el recuerdo de algo difícil te desborda, con imágenes que irrumpen y la sensación de «estoy otra vez allí». Los recuerdos intrusivos frecuentes son una señal habitual de reacción postraumática y responden bien al tratamiento con un especialista en trauma.",
            "No intentes trabajar el recuerdo: ahora la única tarea es volver. Dite dónde estás y qué día es, presiona los pies contra el suelo, nombra lo que ves, exhala despacio, usa el frío.",
            "Intensidad 8. «Eso fue entonces; ahora es 2026, estoy en casa, es de noche». Los pies en el suelo; veo una ventana, una taza, una lámpara. Agua fría. Ahora 4."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Nightmare = new(
        "Práctica", "Reescribir una pesadilla",
        [
            "1. Elige una pesadilla que se repite, no la más difícil para empezar",
            "2. Descríbela brevemente, sin detalles duros",
            "3. Decide qué cambias en ella: el final, el lugar, a ti mismo, a alguien cerca",
            "4. Escribe la nueva versión del sueño completa, hasta un final tranquilo",
            "5. Cada día, durante 10 a 20 minutos, imagina la nueva versión con detalle",
            "6. Tras una o dos semanas, mira cómo han cambiado las pesadillas"
        ],
        "Ensayo en imaginación: una pesadilla recurrente se reescribe estando despierto, y el cerebro empieza a soñar la nueva versión.",
        TechniqueUiKind.Entry, "Técnica n.º 21", "16.10.2026", "Reescribir una pesadilla", "Hacer que una pesadilla recurrente sea menos frecuente y menos intensa", "El sueño", "Trauma", 15, "NightsStay",
        [
            new TechniqueEntrySeed("Lo dura que es esta pesadilla", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("De qué trata la pesadilla, en pocas palabras", "Una o dos frases, sin detalles duros", Suggestions: ["Alguien me persigue", "No puedo escapar", "Estoy otra vez donde tuve miedo", "Pierdo a alguien cercano", "No puedo gritar", "Me caigo", "Me atacan", "Llego tarde y no lo consigo"]),
            new TechniqueEntrySeed("Lo que cambias", "Lo que quieras: es tu sueño", Suggestions: ["El final", "El lugar", "Me vuelvo más fuerte", "Aparece alguien que me ayuda", "Puedo hablar y gritar", "El peligro desaparece", "Todo se vuelve seguro", "Me doy cuenta de que es un sueño"]),
            new TechniqueEntrySeed("La nueva versión del sueño", "Descríbela completa, desde el principio hasta un final tranquilo", Suggestions: ["Me doy cuenta de que es un sueño", "Aparece alguien que me protege", "Me encuentro en un lugar seguro", "Me alejo con calma", "Todo termina bien"]),
            new TechniqueEntrySeed("Cuándo ensayaré la nueva versión", "Cada día, de 10 a 20 minutos", Suggestions: ["Por la mañana", "Durante el día, en un descanso", "Por la noche, un par de horas antes de dormir", "Cada día a la misma hora"]),
            new TechniqueEntrySeed("Lo dura que es ahora", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Las pesadillas que se repiten tras un trauma o un estrés se mantienen por costumbre: el cerebro reproduce la misma trama una y otra vez. En la terapia de ensayo en imaginación, la trama se reescribe estando despierto y la nueva versión se ensaya mentalmente cada día. Los estudios muestran que así las pesadillas se vuelven menos frecuentes e intensas y el sueño mejora.",
            "Con pesadillas que se repiten y te dan miedo a dormirte. Si la pesadilla repite directamente un trauma y hasta una descripción breve te provoca mucho horror, hazlo junto con un especialista en trauma.",
            "Empieza por una pesadilla más leve. No escribas con detalle las partes que dan miedo: con un par de frases basta. Cambia el sueño como quieras; la nueva versión no tiene que ser realista. Ensáyala durante el día, no justo antes de dormir, de 10 a 20 minutos al día.",
            "Pesadilla: alguien me persigue y no puedo escapar. Cambio: me doy cuenta de que es un sueño, me doy la vuelta y solo es una sombra; vuelvo a casa con calma. Lo ensayo cada tarde. Tras dos semanas, la pesadilla aparece menos."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition Balloons = new(
        "Práctica", "Revienta los globos",
        [
            "1. Valora tu tensión",
            "2. Revienta los globos con un toque, sin prisa",
            "3. Con cada globo, suelta el aire despacio",
            "4. Juega la ronda hasta el final",
            "5. Valora tu tensión después"
        ],
        "Un juego breve lleva la atención de los pensamientos ansiosos a una acción sencilla y a las sensaciones.",
        TechniqueUiKind.Entry, "Técnica n.º 22", "26.10.2026", "Revienta los globos", "Una pausa breve para soltar la tensión", "Momento difícil", "Psique", 3, "BubbleChart",
        [
            new TechniqueEntrySeed("Tensión antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "¿Qué cambió mientras reventabas globos?", Suggestions: ["Me siento más ligero", "Mi mente descansó", "Me dieron ganas de sonreír", "Mi respiración se calmó", "La irritación pasó", "Mis manos se relajaron", "No cambió nada", "Quiero otra ronda"]),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Cuando la ansiedad o la irritación están en su punto más alto, cuesta detener los pensamientos con fuerza de voluntad. Una acción sencilla que pide atención de ojos y manos cambia de foco al cerebro durante unos minutos y deja que la tensión baje. No es un tratamiento, sino una forma de atravesar un minuto difícil.",
            "Cuando la irritación, la tensión o la ansiedad te desbordan y necesitas una pausa breve. Si el juego solo te ayuda a evitar un problema que vale la pena resolver, vuelve a él cuando estés más tranquilo.",
            "Sin prisa: no hace falta acertar a cada globo. Suelta el aire despacio con cada uno. La ronda termina sola; no hay juego infinito.",
            "Tensión 7/10 después de una discusión. Una ronda de 20 globos, una espiración con cada uno. Tensión 4/10, y pude pensar con calma qué responder."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition CyclicSigh = new(
        "Práctica", "Suspiro cíclico",
        [
            "1. Valora tu tensión",
            "2. Inhala por la nariz y, al final, añade otra inhalación corta",
            "3. Exhala despacio por la boca, más tiempo del que inhalaste",
            "4. Respira al ritmo del círculo unos tres minutos",
            "5. Valora tu tensión después"
        ],
        "Una doble inhalación y una exhalación larga calman el cuerpo rápido, sin retener el aire.",
        TechniqueUiKind.Entry, "Técnica n.º 23", "26.10.2026", "Suspiro cíclico", "Una doble inhalación y una exhalación larga para calmar el cuerpo", "Momento difícil", "Psique", 3, "Air",
        [
            new TechniqueEntrySeed("Tensión antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "¿Qué cambió después de respirar?", Suggestions: ["Mi cuerpo está más tranquilo", "Mis hombros bajaron", "Mi corazón se calmó", "Mis pensamientos se calmaron", "Me mareé un poco", "No cambió nada", "Quiero repetirlo"]),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "La segunda inhalación corta abre más los pulmones, y la exhalación larga activa el freno del sistema nervioso: el corazón se ralentiza y el cuerpo se calma. En un estudio de 2023, cinco minutos al día de esta respiración mejoraban el ánimo más que la meditación de atención plena. No es un tratamiento, sino una forma rápida de bajar la tensión.",
            "Cuando la ansiedad, el enfado o la tensión te desbordan y necesitas calmar el cuerpo en un par de minutos. También sirve antes de una conversación difícil o antes de dormir.",
            "Respira con suavidad, sin forzar: la segunda inhalación es corta y la exhalación lenta y sin esfuerzo. Si te mareas, vuelve a respirar con normalidad; se pasará. El círculo termina solo a los tres minutos.",
            "Tensión 6/10 antes de una llamada. Tres minutos de doble inhalación y exhalación larga. Tensión 3/10, y mi voz sonaba más tranquila."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition MuscleRelax = new(
        "Práctica", "Tensar y soltar",
        [
            "1. Valora tu tensión",
            "2. Tensa un grupo de músculos cinco segundos, a media fuerza",
            "3. Suelta y nota la relajación durante quince segundos",
            "4. Recorre cinco grupos: manos, hombros, cara, abdomen, piernas",
            "5. Valora tu tensión después"
        ],
        "Tensar primero los músculos y luego soltarlos ayuda al cuerpo a sentir qué es relajarse.",
        TechniqueUiKind.Entry, "Técnica n.º 24", "26.10.2026", "Tensar y soltar", "Una relajación muscular breve para un cuerpo tenso", "Momento difícil", "Psique", 4, "SelfImprovement",
        [
            new TechniqueEntrySeed("Tensión antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "¿En qué parte del cuerpo te sientes más ligero?", Suggestions: ["Se relajaron mis hombros", "Se aflojó mi mandíbula", "Se calentaron mis manos", "Respiro mejor", "Me dio sueño", "Todavía queda algo de tensión", "No cambió nada"]),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "La relajación muscular progresiva es una de las formas más estudiadas de reducir la ansiedad y la tensión. Tensar un músculo y luego soltarlo crea un contraste que ayuda a notar la relajación y, con el tiempo, a aprender a provocarla. No es un tratamiento, sino una habilidad que funciona mejor con la práctica.",
            "Cuando el cuerpo está tenso por el estrés, cuando cuesta quedarse quieto o dormirse. Va bien por la noche antes de dormir.",
            "Tensa a media fuerza, sin dolor. Si una parte del cuerpo duele o está lesionada, no la tenses: solo obsérvala. Al soltar, respira con calma y no te apresures a pasar al siguiente grupo.",
            "Tensión 7/10 por la noche después del trabajo, con los hombros encogidos. Cinco grupos: manos, hombros, cara, abdomen, piernas. Tensión 4/10, los hombros bajaron."),
        null, null);

    internal static readonly BuiltInTechniqueDefinition BusyMind = new(
        "Práctica", "Ocupar la mente",
        [
            "1. Valora tu tensión",
            "2. Toma una tarea y hazla mentalmente o en voz alta",
            "3. Si tus pensamientos vuelven a la preocupación, regresa con suavidad a la tarea",
            "4. Haz cinco tareas",
            "5. Valora tu tensión después"
        ],
        "Tareas sencillas para la mente apartan la atención de los pensamientos que dan vueltas.",
        TechniqueUiKind.Entry, "Técnica n.º 25", "26.10.2026", "Ocupar la mente", "Tareas sencillas para romper el círculo de pensamientos", "Momento difícil", "Psique", 3, "Extension",
        [
            new TechniqueEntrySeed("Tensión antes", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityBefore),
            new TechniqueEntrySeed("Notas", "¿Qué cambió mientras estabas ocupado con las tareas?", Suggestions: ["Los pensamientos me soltaron", "Estoy más tranquilo", "Me costó concentrarme", "Me dieron ganas de hacer algo", "La ansiedad volvió", "No cambió nada", "Quiero más tareas"]),
            new TechniqueEntrySeed("Tensión después", "", EntryFieldKind.Rating0To10, Role: EntryFieldRole.IntensityAfter)
        ],
        Theory(
            "Cuando los pensamientos dan vueltas, detenerlos con fuerza de voluntad es casi imposible. Una tarea que ocupa la mente (contar, recordar, enumerar) no deja espacio a la rumiación durante un rato. Es una habilidad de distracción de la terapia dialéctica conductual: no resuelve el problema, pero ayuda a pasar un momento difícil.",
            "Cuando te invaden pensamientos intrusivos, antes de dormir o después de una discusión, cuando la conversación sigue en tu cabeza.",
            "Haz las tareas en serio: cuanta más atención te pidan, mejor. Si el problema de verdad hay que resolverlo, vuelve a él cuando estés más tranquilo.",
            "Después de una discusión, la conversación sigue dando vueltas en mi cabeza, tensión 7/10. Cinco tareas: animales que viven en el agua, contar hacia atrás desde 100 de 7 en 7, objetos de cada color a mi alrededor. Tensión 4/10."),
        null, null);
}
