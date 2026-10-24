using PsychologyApp.Domain.Practice;

namespace PsychologyApp.Application.Articles;

public static partial class ArticleContent
{
    private static readonly Course[] CoursesEs =
    [
        new(
            CourseIds.Sleep,
            "Dormir sin pastillas",
            "Siete días de terapia cognitivo-conductual del insomnio, la forma mejor estudiada de recuperar el sueño",
            "Bedtime",
            "isi",
            "ISI-7",
            [
                new(
                    "Cómo funciona el sueño",
                    4,
                    [
                        P("El sueño no se «enciende» a base de esfuerzo. Lo preparan dos fuerzas: la presión de sueño, que se acumula durante todo el día mientras estás despierto, y el reloj interno, que nos da sueño y nos despierta a las mismas horas cada día."),
                        P("En el insomnio, las dos fuerzas se desajustan: la persona se acuesta antes para «recuperar», pasa mucho rato despierta, duerme siesta, se levanta a horas distintas. La presión de sueño no tiene tiempo de acumularse y el reloj se confunde."),
                        Cycle("El ciclo del insomnio", "Una mala noche", "Preocupación: «otra vez no voy a dormir»", "Esforzarse por dormir", "La tensión aleja el sueño"),
                        P("Este curso se basa en la terapia cognitivo-conductual del insomnio (TCC-I). Las guías médicas la recomiendan en primer lugar, antes que las pastillas para dormir: su efecto dura después de terminar el curso."),
                        Warning("Cuándo consultar primero a un médico"),
                        List(
                            "Ronquidos fuertes, pausas en la respiración durante el sueño, mucho sueño durante el día",
                            "Sensaciones desagradables en las piernas por la tarde que te obligan a moverte",
                            "Insomnio junto con ánimo bajo durante más de dos semanas",
                            "Conduces o manejas maquinaria y tienes mucho sueño durante el día")
                    ],
                    "Cada mañana apunta: cuándo te acostaste, más o menos cuándo te dormiste, cuántas veces te despertaste, cuándo te levantaste y cómo dormiste de 0 a 10. Sin mirar el reloj por la noche, aproximadamente, a ojo.",
                    "¿A qué hora sueles acostarte y levantarte? ¿Qué es lo que más estorba tu sueño?"),
                new(
                    "La cama es para dormir",
                    4,
                    [
                        P("Si a menudo te quedas despierto en la cama, te preocupas o miras el móvil ahí, el cerebro asocia la cama con estar despierto. Por eso puedes dormirte en un sillón y estar completamente despierto en cuanto te acuestas."),
                        H("Reglas que recuperan la asociación"),
                        List(
                            "Acuéstate solo cuando tengas sueño, no simplemente cansancio",
                            "Si llevas unos 20 minutos sin dormir, levántate y haz algo tranquilo con luz tenue. Vuelve cuando tengas sueño",
                            "La cama es solo para dormir y la intimidad: sin móvil, sin series y sin trabajo",
                            "Levántate a la misma hora cada día, incluso después de una mala noche y los fines de semana",
                            "No duermas siesta y, si de verdad lo necesitas, no más de 20 minutos y antes de las 15:00"),
                        Quiz("La noche fue mala. ¿Qué es mejor por la mañana?", 1, "Una hora fija para levantarse es el ancla principal del sueño. Si te quedas en la cama más tiempo, por la noche habrá menos presión de sueño y la noche siguiente también será difícil.", "Quedarse durmiendo para recuperarse", "Levantarse a la hora de siempre", "Levantarse y echar una buena siesta durante el día")
                    ],
                    "Elige una hora para levantarte que puedas mantener los siete días y levántate a esa hora. Si llevas unos 20 minutos sin dormir, sal de la cama.",
                    "¿Qué hora de levantarte elegiste? ¿Qué te costó de la regla «levantarse si no duermes»?"),
                new(
                    "La ventana de sueño",
                    5,
                    [
                        P("Cuanto más tiempo pasa una persona despierta en la cama, más ligero y entrecortado se vuelve el sueño. Ayuda una técnica llamada compresión del sueño: el tiempo en la cama se acerca a lo que realmente duermes."),
                        H("Cómo calcular la ventana"),
                        List(
                            "Con tus notas de los últimos días, calcula cuánto duermes de media",
                            "Suma 30 minutos: ese es tu tiempo en la cama, pero nunca menos de seis horas",
                            "Cuéntalo hacia atrás desde la hora de levantarte que elegiste: así obtienes tu hora de acostarte",
                            "No te acuestes antes de esa hora, aunque tengas muchas ganas"),
                        P("Los primeros días tendrás sueño: significa que la presión de sueño se está acumulando. Pasada una semana, si duermes la mayor parte del tiempo en la cama, la ventana se puede ampliar 15 minutos."),
                        Warning("Ten cuidado"),
                        P("No comprimas el sueño si tienes trastorno bipolar o epilepsia, si estás embarazada o si el sueño hace que conducir o trabajar sea peligroso. En estos casos, habla del sueño con un médico.")
                    ],
                    "Calcula tu ventana de sueño y mantenla hasta el final del curso: nada de cama antes de tu hora de acostarte, levantarse a la misma hora.",
                    "¿Cuál es tu ventana de sueño? ¿Cómo fue la primera noche con ella?"),
                new(
                    "Una tarde que prepara para dormir",
                    4,
                    [
                        P("El sueño llega con más facilidad cuando el cuerpo y la mente tienen tiempo de bajar el ritmo. La hora antes de acostarse es el puente entre el día y el sueño."),
                        H("Qué ayuda"),
                        List(
                            "Una hora antes de acostarte: luz tenue y actividades tranquilas sin pantallas",
                            "Café, té fuerte y bebidas energéticas no más tarde de las 14:00",
                            "El alcohol ayuda a dormirse pero hace el sueño superficial y despierta de madrugada",
                            "Una cena pesada 2–3 horas antes de acostarte",
                            "Por la mañana, luz del día durante al menos 15 minutos: ajusta el reloj interno"),
                        H("Relajarse antes de dormir"),
                        P("La respiración lenta con una exhalación larga baja la activación. Pruébalo ahora."),
                        Breathe("Inhala contando 4, exhala contando 6")
                    ],
                    "Crea tu propio ritual para la última hora antes de acostarte y hazlo cada tarde. Saca el móvil del dormitorio o, al menos, de la cama.",
                    "¿Qué incluiste en tu ritual de la tarde? ¿Qué resultó difícil?"),
                new(
                    "Pensamientos que no dejan dormir",
                    4,
                    [
                        P("La mente suele ponerse a trabajar justo en la cama: durante el día no hubo tiempo para pensar. Por eso ayuda darles tiempo antes."),
                        H("Un tiempo para los pensamientos"),
                        List(
                            "Por la tarde, un par de horas antes de acostarte, reserva 15 minutos",
                            "Escribe todo lo que te preocupa y, para cada punto, un próximo paso o «por ahora no puedo hacer nada»",
                            "Si el pensamiento vuelve en la cama, dite: «ya lo he apuntado, me ocuparé de ello mañana»"),
                        Quiz("En la cama piensas: «Si no duermo, mañana me saldrá todo mal.» ¿Qué se acerca más a la verdad?", 1, "El pensamiento exagera. Después de una mala noche cuesta más, pero la mayoría de las personas sacan adelante sus tareas. Cuanto mayor es el miedo al día siguiente, más cuesta dormirse.", "Es verdad, el fracaso es inevitable", "Después de una mala noche cuesta más, pero normalmente saco adelante lo principal", "Mejor no acostarse"),
                        Practice(TechniqueId.Observer, "Posición de observador")
                    ],
                    "Cada tarde, reserva 15 minutos para escribir lo que te preocupa, y no te lleves esos pensamientos a la cama.",
                    "¿Qué pensamientos aparecen con más frecuencia por la noche? ¿Te ayudó escribirlos?"),
                new(
                    "Dejar de luchar contra el sueño",
                    4,
                    [
                        P("Cuanto más intentas dormir, peor sale. El sueño es como un pájaro tímido: llega cuando nadie lo persigue."),
                        H("Qué ayuda a soltar el control"),
                        List(
                            "Pon el reloj fuera de la vista: contar «quedan cinco horas» despierta",
                            "Permítete no dormir: «me quedo aquí tumbado descansando, el sueño vendrá cuando venga»",
                            "Una mala noche no es una catástrofe: el cuerpo se recupera incluso con un sueño incompleto",
                            "No canceles planes por una mala noche: así el insomnio no dirige tu vida"),
                        P("Compara tus notas de los primeros días con las de ahora: ¿cómo han cambiado el tiempo que tardas en dormirte, el número de despertares y la valoración del sueño?")
                    ],
                    "Por la noche, mantén el reloj fuera de la vista. Después de una mala noche, vive el día como siempre.",
                    "Según tus notas, ¿qué ha cambiado en tu sueño en estos días?"),
                new(
                    "Resumen y plan",
                    4,
                    [
                        P("En una semana has recorrido el núcleo de la TCC del insomnio. El sueño se recupera poco a poco: muchas personas notan una mejora en dos a cuatro semanas si mantienen las reglas."),
                        H("Qué seguir haciendo"),
                        List(
                            "Una misma hora de levantarse cada día",
                            "Acostarse solo con sueño y levantarse si no duermes",
                            "Si duermes la mayor parte de la noche, ampliar la ventana 15 minutos por semana",
                            "El ritual de la tarde y el tiempo para los pensamientos"),
                        H("Si el insomnio vuelve"),
                        P("Después de estrés, una enfermedad o un viaje, el sueño puede volver a desajustarse. Es normal: vuelve a las reglas de este curso y se recuperará más rápido que la primera vez."),
                        P("Vuelve a hacer el índice ISI-7 y compáralo con tu resultado del principio. Si no hay mejora, consulta a un médico: hay especialistas en sueño que aplican la TCC-I."),
                        TestsTab("Índice ISI-7", "isi")
                    ],
                    "Escribe tu plan para si el sueño vuelve a desajustarse.",
                    "¿Qué te ayudó más del curso? ¿Qué vas a seguir haciendo?")
            ]),
        new(
            CourseIds.Anxiety,
            "La ansiedad bajo control",
            "Seis días: entender cómo funciona la ansiedad, calmar el cuerpo, comprobar los pensamientos que asustan y dar un primer paso hacia el miedo",
            "Psychology",
            "gad7",
            "GAD-7",
            [
                new(
                    "Cómo funciona la ansiedad",
                    4,
                    [
                        P("La ansiedad es una alarma pensada para avisar del peligro. El problema empieza cuando suena demasiado a menudo, demasiado fuerte y tarda mucho en apagarse."),
                        Cycle("El ciclo de la ansiedad", "Un pensamiento que asusta", "Ansiedad en el cuerpo", "Evitación o una muleta", "Alivio por un momento", "El cerebro aprende: «era peligroso»"),
                        P("Lo que mantiene la ansiedad no es el miedo en sí, sino lo que hacemos para no sentirlo: evitar, volver a comprobar, buscar que nos tranquilicen, darle vueltas a los pensamientos. Paso a paso, el curso trabaja cada eslabón de este ciclo."),
                        P("Al principio del curso vale la pena hacer la escala GAD-7: al final compararás el resultado."),
                        TestsTab("Escala GAD-7", "gad7")
                    ],
                    "Hoy, tres veces, cuando sientas ansiedad, fíjate en: qué pasó, cuál fue el pensamiento, qué sintió el cuerpo y qué hiciste.",
                    "¿Cuándo y dónde aparece la ansiedad con más frecuencia?"),
                new(
                    "Calmar el cuerpo",
                    4,
                    [
                        P("Cuando la ansiedad es alta, cuesta discutir con los pensamientos. Primero ayuda calmar el cuerpo: una exhalación lenta activa el sistema que frena la reacción de lucha o huida."),
                        Breathe("Inhala contando 4, exhala contando 6"),
                        P("Si la ansiedad es fuerte y todo parece irreal, devuelve la atención al presente a través de los sentidos."),
                        Practice(TechniqueId.Grounding, "Anclaje 5-4-3-2-1"),
                        P("Importante: la respiración no es una forma de librarse de la ansiedad a toda costa. Ayuda a atravesar el pico, no a evitar situaciones.")
                    ],
                    "Dos veces al día, dos minutos de respiración lenta, incluso estando tranquilo: así la habilidad estará ahí cuando la necesites.",
                    "¿Qué notaste en el cuerpo al respirar despacio?"),
                new(
                    "Comprobar un pensamiento que asusta",
                    5,
                    [
                        P("Los pensamientos ansiosos suenan a hechos: «voy a hacer el ridículo», «va a pasar algo terrible». Pero son predicciones, y se pueden comprobar."),
                        H("Preguntas para hacerle al pensamiento"),
                        List(
                            "¿Qué hechos apoyan este pensamiento y cuáles van en contra?",
                            "¿Qué pasó en casos parecidos antes?",
                            "¿Qué es lo más probable, en lugar de lo más aterrador?",
                            "Si pasa lo peor, ¿cómo lo afrontaré?",
                            "¿Qué le diría a un amigo con este pensamiento?"),
                        Quiz("«Seguro que lo voy a estropear.» ¿Qué trampa del pensamiento es?", 1, "Es adivinar el futuro: el pensamiento trata como un hecho algo que todavía no ha pasado.", "Leer la mente", "Adivinar el futuro", "Descalificar lo positivo"),
                        Practice(TechniqueId.ThoughtRecord, "Registro de pensamientos")
                    ],
                    "Hoy, una vez, comprueba un pensamiento ansioso con un registro de pensamientos.",
                    "¿Qué pensamiento comprobaste? ¿Cuánto lo creías antes y después?"),
                new(
                    "El tiempo para preocuparse",
                    4,
                    [
                        P("Preocuparse parece útil: «si pienso más, estaré preparado». En realidad, darle vueltas rara vez lleva a una solución y mantiene alta la ansiedad."),
                        Quiz("¿Cómo distinguir resolver de dar vueltas?", 0, "Resolver avanza hacia un paso: qué puedo hacer y cuándo. Dar vueltas produce nuevos «y si».", "Resolver termina en un paso; dar vueltas, en un nuevo «y si»", "Dar vueltas lleva más tiempo", "No se puede"),
                        H("Cómo aplazar la preocupación"),
                        List(
                            "Elige 15–20 minutos al día, no antes de dormir",
                            "Cuando aparezca una preocupación en otro momento, apunta el tema en una línea y dite: «pensaré en esto en mi tiempo para preocuparme»",
                            "En tu tiempo para preocuparte, repasa la lista: planifica un paso para lo que puedes hacer y suelta lo que no",
                            "A menudo, a la hora fijada, el tema ya no parece importante"),
                        Practice(TechniqueId.Observer, "Posición de observador")
                    ],
                    "Fíjate un tiempo para preocuparte y aplaza a él los pensamientos ansiosos.",
                    "¿Conseguiste aplazar la preocupación? ¿Qué pasó con los temas a la hora fijada?"),
                new(
                    "Muletas y evitación",
                    4,
                    [
                        P("Lo que más mantiene la ansiedad es la evitación. Puede ser abierta (no voy, no llamo) u oculta: muletas que ayudan a atravesar una situación."),
                        Check("¿Tengo muletas?", 2, "Tienes varias muletas. Mantienen el miedo: el cerebro cree que todo salió bien solo gracias a ellas. Intenta renunciar a una en una situación fácil.", "Tienes pocas muletas. Fíjate también en lo que evitas por completo.", "Llevo una pastilla, agua o el móvil cargado «por si acaso»", "Solo voy acompañado a los lugares difíciles", "Me siento cerca de la salida", "Me distraigo para no notar la ansiedad", "Compruebo o vuelvo a preguntar muchas veces"),
                        P("La salida es encontrarse poco a poco con lo que temes y prescindir de las muletas. Así el cerebro aprende por experiencia que lo temido no ocurre o se puede soportar.")
                    ],
                    "Haz una lista de tres a cinco situaciones que evitas, de la más fácil a la más difícil.",
                    "¿Qué situación de la lista estás dispuesto a probar primero?"),
                new(
                    "El primer paso y un plan",
                    4,
                    [
                        P("La forma más eficaz de debilitar la ansiedad es la exposición: encontrarse poco a poco con el miedo. No hace falta ir directamente a lo más aterrador: empieza con un paso fácil y comprueba una predicción."),
                        List(
                            "Antes del paso, apunta qué temes y cuánto crees que va a pasar",
                            "Da el paso sin muletas y quédate en la situación hasta haber comprobado la predicción",
                            "Después, apunta qué pasó en realidad"),
                        Ladder("Escalera del miedo"),
                        H("Qué seguir haciendo"),
                        List(
                            "La respiración lenta en el pico de la ansiedad",
                            "Comprobar los pensamientos y el tiempo para preocuparse",
                            "Pasos en la escalera del miedo cada pocos días"),
                        P("Vuelve a hacer la escala GAD-7 y compárala con el principio. Si la ansiedad es fuerte o afecta a tu vida, acude a un psicoterapeuta: la TCC de la ansiedad está bien estudiada."),
                        TestsTab("Escala GAD-7", "gad7")
                    ],
                    "Da el primer paso en tu escalera del miedo y apunta qué pasó.",
                    "¿Qué habías predicho y qué pasó en realidad?")
            ]),
        new(
            CourseIds.Mood,
            "Salir de la apatía",
            "Seis días de activación conductual: pequeños pasos para recuperar la energía y lo que da alegría",
            "Cloud",
            "phq9",
            "PHQ-9",
            [
                new(
                    "El ciclo de la inactividad",
                    4,
                    [
                        P("Cuando el ánimo está a cero, es natural hacer menos: no hay fuerzas, nada apetece. Pero cuanto menos hacemos, menos hay en la vida que dé alegría y la sensación de «lo he conseguido», y el ánimo baja todavía más."),
                        Cycle("El ciclo de la inactividad", "Sin fuerzas", "Hago menos", "Menos alegría y logros", "Aún menos fuerzas"),
                        P("La activación conductual rompe este ciclo por el otro lado: no esperar a tener ganas, sino dar pequeños pasos después de los cuales las fuerzas y las ganas vuelven poco a poco. Es uno de los métodos de ayuda mejor estudiados para la depresión."),
                        P("Haz la escala PHQ-9 al principio del curso: al final compararás el resultado."),
                        TestsTab("Escala PHQ-9", "phq9"),
                        Warning("Si aparecen pensamientos sobre la muerte"),
                        P("Si tienes pensamientos de que estaría mejor que no existieras, no te quedes solo con ellos: este curso no sustituye la ayuda de un especialista."),
                        Crisis("Necesito ayuda ahora")
                    ],
                    "Hoy y mañana, fíjate en lo que haces y valora tu ánimo de 0 a 10 varias veces al día.",
                    "¿Cómo van tus días ahora? ¿Qué has dejado de hacer últimamente?"),
                new(
                    "Qué te sube el ánimo",
                    4,
                    [
                        P("Mira tus observaciones: ¿en qué momentos el ánimo estuvo un poco más alto? A menudo es algo pequeño: una conversación, un paseo, una ducha, una tarea hecha."),
                        H("Dos tipos de actividades útiles"),
                        List(
                            "Las agradables: lo que da placer, aunque sea poco — música, comida rica, el sol",
                            "Las de «lo he conseguido»: lo que deja sensación de logro — fregar los platos, contestar un correo"),
                        P("En la depresión, el placer de las cosas agradables es más débil que antes. Es normal: lo que importa es hacerlo, no esperar la alegría de antes.")
                    ],
                    "Haz una lista de diez actividades: cinco agradables y cinco de «lo he conseguido». Cuanto más pequeñas y sencillas, mejor.",
                    "¿Qué incluiste en tu lista? ¿Hubo algo que te subiera un poco el ánimo?"),
                new(
                    "Pasos pequeños",
                    4,
                    [
                        P("Los grandes planes asustan en la apatía y no se llevan a cabo. Ayuda dividir las cosas: no «limpiar el piso», sino «quitar tres cosas de la mesa»."),
                        List(
                            "Elige una actividad de la lista para hoy",
                            "Divídela hasta un paso que lleve cinco minutos",
                            "Fija un momento y un lugar: «después del desayuno, en la cocina»",
                            "Después, apunta cómo cambió tu ánimo"),
                        Practice(TechniqueId.SmallStep, "Un pequeño paso")
                    ],
                    "Cada día, haz una actividad agradable y una de «lo he conseguido» a la hora que fijaste.",
                    "¿Qué conseguiste hacer? ¿Cómo cambió tu ánimo después?"),
                new(
                    "«No me apetece»",
                    4,
                    [
                        P("El principal obstáculo es el pensamiento «no me apetece, lo haré cuando tenga fuerzas». En la apatía, las ganas llegan después de la acción, no antes."),
                        Quiz("No tienes fuerzas y hoy tenías planeado un paseo. ¿Qué es mejor?", 1, "En la apatía, las ganas vuelven después de la acción. Incluso cinco minutos le dan al cuerpo y al ánimo la señal «he hecho algo».", "Esperar a que me apetezca", "Salir al menos cinco minutos", "Dejarlo para el fin de semana"),
                        H("Otros pensamientos que frenan"),
                        List(
                            "«No sirve de nada»: compruébalo después — ¿ha cambiado algo, aunque sea un poco?",
                            "«Es demasiado poco»: precisamente de pasos pequeños está hecha la salida",
                            "«Antes hacía más»: compárate con ayer, no con quien eras antes"),
                        Practice(TechniqueId.SelfCompassion, "Palabras amables para ti")
                    ],
                    "Cuando no te apetezca, haz al menos una versión de cinco minutos de lo que planeaste.",
                    "¿Qué pensamiento te impide empezar con más frecuencia? ¿Qué te ayudó a sortearlo?"),
                new(
                    "La gente a tu alrededor",
                    4,
                    [
                        P("En la apatía apetece encerrarse y alejarse de todos. Pero el contacto con la gente es uno de los remedios más potentes contra el ánimo bajo, aunque ahora mismo no dé la alegría de antes."),
                        List(
                            "Escribe a una persona con la que hace tiempo que no hablas",
                            "Un encuentro corto o una llamada es mejor que nada",
                            "No tienes que explicarlo todo: «ahora mismo estoy pasando una mala racha, vamos solo a dar un paseo»",
                            "Si hay pocas fuerzas, elige a personas con las que estar sea fácil"),
                        Practice(TechniqueId.SmallStep, "Un pequeño paso")
                    ],
                    "Ponte en contacto con al menos una persona en los próximos dos días.",
                    "¿Con quién te pusiste en contacto? ¿Cómo fue?"),
                new(
                    "Lo que me importa",
                    4,
                    [
                        P("Para que los pasos no se conviertan en obligaciones, ayuda relacionarlos con lo que te importa: las personas cercanas, la salud, el trabajo, la creatividad, la naturaleza."),
                        List(
                            "Elige un ámbito que te importe",
                            "Piensa en quién quieres ser en él",
                            "Elige un pequeño paso en esa dirección para la semana"),
                        H("Qué seguir haciendo"),
                        List(
                            "Cada día, una actividad agradable y una de «lo he conseguido»",
                            "Pasos pequeños en lugar de grandes planes",
                            "El contacto con la gente"),
                        P("Vuelve a hacer la escala PHQ-9 y compárala con el principio. Si el resultado es alto o no cambia, acude a un médico o a un psicoterapeuta: la depresión se trata muy bien."),
                        TestsTab("Escala PHQ-9", "phq9"),
                        Crisis("Necesito ayuda ahora")
                    ],
                    "Esta semana, da un paso en un ámbito que te importe.",
                    "¿Qué te importa? ¿Qué paso elegiste?")
            ]),
        new(
            CourseIds.Trauma,
            "Después de un trauma",
            "Cinco días sobre lo que es seguro hacer por tu cuenta: entender tu reacción, manejar los flashbacks y las pesadillas, encontrar un especialista",
            "Healing",
            "pcl5",
            "PCL-5",
            [
                new(
                    "Una reacción normal ante un suceso anormal",
                    4,
                    [
                        P("Después de un suceso difícil, casi todo el mundo tiene recuerdos intrusivos, pesadillas, ansiedad, entumecimiento, irritabilidad. No es debilidad, sino la reacción de un cerebro que intenta protegerte de que vuelva a pasar."),
                        P("Este curso no procesará el trauma: eso requiere terapia con un especialista. Te ayudará a entender lo que te pasa, a manejar los momentos difíciles y a llegar preparado al especialista. En este curso no tienes que recordar ni describir el suceso en sí."),
                        P("Haz la escala PCL-5 al principio: al final compararás el resultado."),
                        TestsTab("Escala PCL-5", "pcl5"),
                        Warning("Si el peligro continúa"),
                        P("Si ahora mismo estás amenazado o han aparecido pensamientos de hacerte daño, la seguridad va primero."),
                        Crisis("Necesito ayuda ahora")
                    ],
                    "Fíjate en cuándo se hace más difícil durante el día, sin detalles del suceso: la hora, el lugar, lo que había alrededor.",
                    "¿Cuáles de estas reacciones reconoces en ti?"),
                new(
                    "Volver al presente",
                    4,
                    [
                        P("Un flashback es un recuerdo que se revive como si estuviera pasando ahora. El momento, el lugar, el apoyo del cuerpo y los sentidos ayudan a volver."),
                        Practice(TechniqueId.Flashback, "Un recuerdo me ha desbordado"),
                        Practice(TechniqueId.Grounding, "Anclaje 5-4-3-2-1"),
                        H("Tu propio kit de anclaje"),
                        List(
                            "Una frase: «Es un recuerdo. Ahora es otro momento, estoy a salvo»",
                            "Algo que llevar contigo: una piedrecita, un anillo, algo con olor",
                            "Una persona a la que puedas escribir",
                            "Algo frío: agua, hielo"),
                        Breathe("La exhalación más larga que la inhalación")
                    ],
                    "Prepara tu kit de anclaje y prueba la práctica «Un recuerdo me ha desbordado» en un momento tranquilo, para conocerla de antemano.",
                    "¿Qué incluiste en tu kit de anclaje?"),
                new(
                    "El sueño y las pesadillas",
                    4,
                    [
                        P("Después de un trauma, el sueño suele resentirse: cuesta dormirse, el sueño es ligero, llegan las pesadillas. La falta de sueño vuelve los días más ansiosos e irritables."),
                        List(
                            "Una misma hora de levantarse cada día",
                            "Luz por la mañana, una tarde tranquila sin noticias",
                            "Si una pesadilla te despierta: luz, dónde estoy y qué día es, agua, exhalaciones lentas",
                            "El alcohol aumenta las pesadillas en la segunda mitad de la noche"),
                        P("Una pesadilla que se repite se puede reescribir: el ensayo en imaginación hace que las pesadillas sean menos frecuentes y más débiles. Empieza por una más leve."),
                        Practice(TechniqueId.Nightmare, "Reescribir una pesadilla")
                    ],
                    "Mantén una misma hora de levantarte. Si tienes pesadillas que se repiten, prueba a reescribir una, empezando por una más leve.",
                    "¿Cómo duermes ahora? ¿Qué te ayuda después de una noche difícil?"),
                new(
                    "Apoyo y personas",
                    4,
                    [
                        P("El trauma suele alejar de los demás y de lo que antes importaba. El apoyo de las personas cercanas es uno de los factores más potentes de la recuperación."),
                        List(
                            "No tienes que hablar del suceso: podéis simplemente estar juntos",
                            "Diles a los tuyos cómo ayudarte en un momento difícil",
                            "Recupera poco a poco lo que te daba apoyo: paseos, movimiento, cosas que te gustaban",
                            "No anestesies las emociones con alcohol: entorpece la recuperación"),
                        P("Evitar lo que te recuerda el suceso es comprensible, pero con el tiempo estrecha la vida y mantiene el trauma «caliente». Es mejor trabajarlo con un especialista que obligarte en solitario."),
                        P("Lo que el suceso cambió en tu forma de pensar sobre ti —la culpa, «algo va mal en mí», «no se puede confiar en nadie»— se puede trabajar en el chat, sin volver a contar lo que pasó."),
                        TraumaChat("Trabajarlo en el chat"),
                        Practice(TechniqueId.SelfCompassion, "Palabras amables para ti")
                    ],
                    "Haz una cosa que antes te daba apoyo y pasa tiempo con alguien de confianza.",
                    "¿Quién y qué te da apoyo ahora?"),
                new(
                    "El camino hacia un especialista",
                    5,
                    [
                        P("Los síntomas postraumáticos responden bien a la psicoterapia centrada en el trauma: exposición prolongada, terapia cognitiva, EMDR, exposición escrita. Suele requerir 8–16 sesiones."),
                        See(ArticleIds.TraumaTreatment, "Cómo se trata el trauma y cómo elegir un especialista"),
                        H("Cómo prepararte"),
                        List(
                            "Vuelve a hacer la escala PCL-5 y compárala con el principio",
                            "Exporta un resumen de la aplicación para el especialista: «Perfil → Opciones → Datos y copia de seguridad»",
                            "Apunta tus preguntas: qué método, cuántas sesiones, cómo se medirá el progreso",
                            "No tienes que contarlo todo sobre el suceso en la primera cita"),
                        TestsTab("Escala PCL-5", "pcl5"),
                        Crisis("Necesito ayuda ahora")
                    ],
                    "Busca uno o dos especialistas que trabajen con trauma y pide una cita de consulta.",
                    "¿Qué fue lo más útil del curso? ¿De qué quieres hablar con el especialista?")
            ])
    ];
}
