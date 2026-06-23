namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string BeckScore(int ball) => ball switch
    {
        <= 9 => T("0-9 - нет депрессивных симптомов", "0-9 - no depressive symptoms"),
        <= 15 => T("10-15 - лёгкая депрессия", "10-15 - mild depression"),
        <= 19 => T("16-19 - умеренная депрессия", "16-19 - moderate depression"),
        <= 29 => T("20-29 - выраженная депрессия (средней тяжести)", "20-29 - marked depression (moderate severity)"),
        _ => T("30-63 – тяжелая депрессия", "30-63 - severe depression")
    };

    public static string BeckScoreDetail(int ball) => ball switch
    {
        <= 9 => T(
            "Симптомы в пределах нормы. Поддерживайте режим сна и регулярную активность.",
            "Symptoms are within normal range. Keep sleep and regular activity."),
        <= 15 => T(
            "Лёгкое снижение настроения. Помогают прогулки, дневник мыслей и короткие практики.",
            "Mild low mood. Walks, thought journaling, and short practices can help."),
        <= 19 => T(
            "Умеренная депрессия. Имеет смысл обсудить состояние со специалистом и добавить ежедневные практики.",
            "Moderate depression. Consider talking to a professional and daily practices."),
        <= 29 => T(
            "Выраженные симптомы. Рекомендуем обратиться к психологу или врачу и не оставаться с этим наедине.",
            "Marked symptoms. Please consult a psychologist or doctor and seek support."),
        _ => T(
            "Тяжёлая депрессия. Важно как можно скорее получить профессиональную помощь.",
            "Severe depression. Professional help is strongly recommended.")
    };

    public static string HeckHessScore(int ball) =>
        ball <= 24
            ? T("0-24 - невысокий уровень невротизации", "0-24 - low neuroticism")
            : T("25-40 - высокий уровень невротизации", "25-40 - high neuroticism");

    public static string HeckHessScoreDetail(int ball) =>
        ball <= 24
            ? T(
                "Эмоциональная реактивность в пределах нормы. Продолжайте отслеживать стресс и отдых.",
                "Emotional reactivity is within normal range. Keep monitoring stress and rest.")
            : T(
                "Повышенная невротизация. Практики на баланс противоположностей и сравнение важностей могут снизить напряжение.",
                "Elevated neuroticism. Polarity and importance-comparison practices may ease tension.");

    public static string HaerScore(int ball) =>
        ball <= 29
            ? T("0-28 - невысокий уровень психопатии", "0-28 - low psychopathy")
            : T("29-40 - высокий уровень психопатии", "29-40 - high psychopathy");

    public static string HaerScoreDetail(int ball) =>
        ball <= 29
            ? T(
                "Показатель в низком диапазоне. Это скрининг, а не диагноз — ориентируйтесь на самочувствие.",
                "Score is in the low range. This is a screening tool, not a diagnosis.")
            : T(
                "Повышенные показатели. Полезно работать с прошлым опытом и переосмыслением убеждений.",
                "Elevated score. Working with past experience and beliefs may help.");

    public static string PochebutScore(int ball) => ball switch
    {
        <= 10 => T("0-10 - низкий уровень агрессивности", "0-10 - low aggressiveness"),
        <= 24 => T("11-24 - средний уровень агрессивности", "11-24 - moderate aggressiveness"),
        _ => T("25-40 - высокий уровень агрессивности", "25-40 - high aggressiveness")
    };

    public static string PochebutScoreDetail(int ball) => ball switch
    {
        <= 10 => T(
            "Агрессивные реакции редки. Сохраняйте навыки саморегуляции.",
            "Aggressive reactions are rare. Keep your self-regulation habits."),
        <= 24 => T(
            "Умеренная агрессивность. Помогает пауза, дыхание и переформулировка ситуации.",
            "Moderate aggressiveness. Pause, breathing, and reframing the situation help."),
        _ => T(
            "Высокая агрессивность. Практики на снижение важности и дистанцирование от триггера особенно уместны.",
            "High aggressiveness. Practices that lower importance and add distance from triggers are especially useful.")
    };

    public static string TestRecommendationReasonBeck(int score) =>
        score >= 10
            ? T("При депрессивных симптомах «Крутилка» помогает снизить заряд болезненных воспоминаний.", "For depressive symptoms, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonHeckHess(int score) =>
        score >= 25
            ? T("При высокой невротизации полярности помогают увидеть обе стороны ситуации.", "With high neuroticism, polarities help see both sides.")
            : T("Сравнение важностей укрепляет ощущение перспективы.", "Comparing importance strengthens perspective.");

    public static string TestRecommendationReasonHaer(int score) =>
        score >= 29
            ? T("«50 лет спустя» отдаляет проблему во времени.", "\"50 years later\" moves the problem forward in time.")
            : T("Модификация опыта помогает пересмотреть убеждения.", "Experience modification helps revisit beliefs.");

    public static string TestRecommendationReasonPochebut(int score) =>
        score >= 25
            ? T("«Уменьши это» визуально снижает значимость триггера.", "\"Shrink it\" visually lowers the trigger's importance.")
            : T("«Проверь это» помогает отпустить зацикленную мысль.", "\"Check it\" helps release a looping thought.");

    public static string Gad7Score(int ball) => ball switch
    {
        <= 4 => T("0-4 — минимальная тревога", "0-4 — minimal anxiety"),
        <= 9 => T("5-9 — лёгкая тревога", "5-9 — mild anxiety"),
        <= 14 => T("10-14 — умеренная тревога", "10-14 — moderate anxiety"),
        _ => T("15-21 — выраженная тревога", "15-21 — severe anxiety")
    };

    public static string Gad7ScoreDetail(int ball) => ball switch
    {
        <= 4 => T(
            "Тревожные симптомы в пределах нормы. Продолжайте отслеживать стресс и отдых.",
            "Anxiety symptoms are within normal range. Keep monitoring stress and rest."),
        <= 9 => T(
            "Лёгкая тревога. Помогают дыхание, прогулки и короткие практики заземления.",
            "Mild anxiety. Breathing, walks, and short grounding practices help."),
        <= 14 => T(
            "Умеренная тревога. Имеет смысл обсудить состояние со специалистом.",
            "Moderate anxiety. Consider discussing your state with a professional."),
        _ => T(
            "Выраженная тревога. Рекомендуем обратиться к психологу или врачу.",
            "Severe anxiety. Please consult a psychologist or doctor.")
    };

    public static string K10Score(int ball) => ball switch
    {
        <= 15 => T("10-15 — низкий дистресс", "10-15 — low distress"),
        <= 21 => T("16-21 — умеренный дистресс", "16-21 — mild distress"),
        <= 29 => T("22-29 — выраженный дистресс", "22-29 — moderate distress"),
        _ => T("30-50 — высокий дистресс", "30-50 — high distress")
    };

    public static string K10ScoreDetail(int ball) => ball switch
    {
        <= 15 => T(
            "Психологический дистресс в низком диапазоне. Поддерживайте режим и регулярную активность.",
            "Psychological distress is in the low range. Maintain routine and regular activity."),
        <= 21 => T(
            "Умеренный дистресс. Практики на переосмысление опыта и выгрузку мыслей могут помочь.",
            "Mild distress. Experience reframing and thought journaling practices may help."),
        <= 29 => T(
            "Выраженный дистресс. Важно не оставаться с этим наедине — обратитесь за поддержкой.",
            "Moderate distress. Do not face this alone — seek support."),
        _ => T(
            "Высокий дистресс. Рекомендуем как можно скорее получить профессиональную помощь.",
            "High distress. Professional help is strongly recommended.")
    };

    public static string Who5Score(int ball) => ball switch
    {
        <= 12 => T("0-12 — низкое благополучие", "0-12 — low well-being"),
        <= 18 => T("13-18 — удовлетворительное благополучие", "13-18 — fair well-being"),
        _ => T("19-25 — хорошее благополучие", "19-25 — good well-being")
    };

    public static string Who5ScoreDetail(int ball) => ball switch
    {
        <= 12 => T(
            "Субъективное благополучие снижено. Помогают регулярный сон, движение и выгрузка мыслей на бумагу.",
            "Subjective well-being is low. Regular sleep, movement, and journaling thoughts can help."),
        <= 18 => T(
            "Благополучие на среднем уровне. Укрепляйте привычки, которые дают энергию и интерес к делам.",
            "Well-being is at a moderate level. Strengthen habits that bring energy and interest."),
        _ => T(
            "Хороший уровень благополучия. Продолжайте практики, которые поддерживают это состояние.",
            "Good level of well-being. Continue practices that support this state.")
    };

    public static string TestRecommendationReasonGad7(int score) =>
        score >= 10
            ? T("При тревоге полярности помогают увидеть обе стороны ситуации.", "With anxiety, polarities help see both sides of a situation.")
            : T("Сравнение важностей укрепляет ощущение перспективы.", "Comparing importance strengthens perspective.");

    public static string TestRecommendationReasonK10(int score) =>
        score >= 22
            ? T("При высоком дистрессе «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With high distress, Spin helps lower the charge of painful memories.")
            : score >= 16
                ? T("Модификация опыта помогает пересмотреть тяжёлые переживания.", "Experience modification helps revisit difficult experiences.")
                : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonWho5(int score) =>
        score <= 12
            ? T("При низком благополучии полезно выгружать мысли на бумагу.", "With low well-being, writing thoughts on paper is helpful.")
            : T("Модификация опыта поддерживает ощущение смысла и ресурса.", "Experience modification supports a sense of meaning and resource.");

    public static string Phq9Score(int ball) => ball switch
    {
        <= 4 => T("0-4 — минимальная депрессия", "0-4 — minimal depression"),
        <= 9 => T("5-9 — лёгкая депрессия", "5-9 — mild depression"),
        <= 14 => T("10-14 — умеренная депрессия", "10-14 — moderate depression"),
        <= 19 => T("15-19 — умеренно тяжёлая депрессия", "15-19 — moderately severe depression"),
        _ => T("20-27 — тяжёлая депрессия", "20-27 — severe depression")
    };

    public static string Phq9ScoreDetail(int ball) => ball switch
    {
        <= 4 => T(
            "Депрессивные симптомы в пределах нормы. Поддерживайте режим и активность.",
            "Depressive symptoms are within normal range. Maintain routine and activity."),
        <= 9 => T(
            "Лёгкая депрессия. Помогают сон, движение и выгрузка мыслей на бумагу.",
            "Mild depression. Sleep, movement, and journaling thoughts can help."),
        <= 14 => T(
            "Умеренная депрессия. Имеет смысл обсудить состояние со специалистом.",
            "Moderate depression. Consider discussing your state with a professional."),
        <= 19 => T(
            "Умеренно тяжёлая депрессия. Рекомендуем обратиться к психологу или врачу.",
            "Moderately severe depression. Please consult a psychologist or doctor."),
        _ => T(
            "Тяжёлая депрессия. Важно как можно скорее получить профессиональную помощь.",
            "Severe depression. Professional help is urgently recommended.")
    };

    public static string IsiScore(int ball) => ball switch
    {
        <= 7 => T("0-7 — норма", "0-7 — no significant insomnia"),
        <= 14 => T("8-14 — субпороговая бессонница", "8-14 — subthreshold insomnia"),
        <= 21 => T("15-21 — умеренная бессонница", "15-21 — moderate insomnia"),
        _ => T("22-28 — тяжёлая бессонница", "22-28 — severe insomnia")
    };

    public static string IsiScoreDetail(int ball) => ball switch
    {
        <= 7 => T(
            "Значимых нарушений сна нет. Сохраняйте стабильный режим и гигиену сна.",
            "No clinically significant insomnia. Keep a stable sleep schedule and sleep hygiene."),
        <= 14 => T(
            "Субпороговая бессонница. Помогают ритуалы перед сном и снижение стимуляции вечером.",
            "Subthreshold insomnia. Bedtime rituals and reducing evening stimulation help."),
        <= 21 => T(
            "Умеренная бессонница. Имеет смысл обсудить сон со специалистом.",
            "Moderate insomnia. Consider discussing sleep with a professional."),
        _ => T(
            "Тяжёлая бессонница. Рекомендуем обратиться к врачу или специалисту по сну.",
            "Severe insomnia. Please consult a doctor or sleep specialist.")
    };

    public static string EssScore(int ball) => ball switch
    {
        <= 10 => T("0-10 — норма", "0-10 — normal"),
        <= 12 => T("11-12 — лёгкая сонливость", "11-12 — mild sleepiness"),
        <= 15 => T("13-15 — умеренная сонливость", "13-15 — moderate sleepiness"),
        _ => T("16-24 — выраженная сонливость", "16-24 — severe sleepiness")
    };

    public static string EssScoreDetail(int ball) => ball switch
    {
        <= 10 => T(
            "Дневная сонливость в пределах нормы. Поддерживайте регулярный сон.",
            "Daytime sleepiness is within normal range. Maintain regular sleep."),
        <= 12 => T(
            "Лёгкая дневная сонливость. Проверьте режим сна и время отхода ко сну.",
            "Mild daytime sleepiness. Review your sleep schedule and bedtime."),
        <= 15 => T(
            "Умеренная дневная сонливость. Имеет смысл обсудить это с врачом.",
            "Moderate daytime sleepiness. Consider discussing this with a doctor."),
        _ => T(
            "Выраженная дневная сонливость. Рекомендуем обратиться к врачу.",
            "Severe daytime sleepiness. Medical consultation is recommended.")
    };

    public static string TestRecommendationReasonPhq9(int score) =>
        score >= 10
            ? T("При депрессивных симптомах «Крутилка» помогает снизить заряд болезненных воспоминаний.", "For depressive symptoms, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonIsi(int score) =>
        score >= 22
            ? T("При тяжёлой бессоннице «Крутилка» помогает снизить заряд тревожных мыслей.", "With severe insomnia, Spin helps lower the charge of anxious thoughts.")
            : score >= 15
                ? T("Модификация опыта помогает пересмотреть тяжёлые переживания, мешающие сну.", "Experience modification helps revisit difficult experiences that disrupt sleep.")
                : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonEss(int score) =>
        score >= 16
            ? T("При выраженной сонливости «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With severe sleepiness, Spin helps lower the charge of painful memories.")
            : score >= 11
                ? T("Модификация опыта помогает пересмотреть привычки, влияющие на бодрость.", "Experience modification helps revisit habits that affect alertness.")
                : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string Phq15Score(int ball) => ball switch
    {
        <= 4 => T("0-4 — минимальная соматическая нагрузка", "0-4 — minimal somatic burden"),
        <= 9 => T("5-9 — низкая соматическая нагрузка", "5-9 — low somatic burden"),
        <= 14 => T("10-14 — умеренная соматическая нагрузка", "10-14 — moderate somatic burden"),
        _ => T("15-30 — высокая соматическая нагрузка", "15-30 — high somatic burden")
    };

    public static string Phq15ScoreDetail(int ball) => ball switch
    {
        <= 4 => T(
            "Соматические симптомы в пределах нормы. Поддерживайте режим и физическую активность.",
            "Somatic symptoms are within normal range. Maintain routine and physical activity."),
        <= 9 => T(
            "Низкая соматическая нагрузка. Отслеживайте сон, стресс и нагрузку.",
            "Low somatic burden. Monitor sleep, stress, and workload."),
        <= 14 => T(
            "Умеренная соматическая нагрузка. Имеет смысл обсудить жалобы с врачом.",
            "Moderate somatic burden. Consider discussing symptoms with a doctor."),
        _ => T(
            "Высокая соматическая нагрузка. Рекомендуем обратиться к врачу для обследования.",
            "High somatic burden. Medical evaluation is recommended.")
    };

    public static string ScoffScore(int ball) => ball switch
    {
        <= 1 => T("0-1 — отрицательный скрининг", "0-1 — negative screen"),
        _ => T("2-5 — положительный скрининг", "2-5 — positive screen")
    };

    public static string ScoffScoreDetail(int ball) => ball switch
    {
        <= 1 => T(
            "Признаков расстройства пищевого поведения по скринингу не выявлено.",
            "No signs of an eating disorder on this screen."),
        _ => T(
            "Положительный скрининг. Рекомендуем обратиться к специалисту по пищевому поведению.",
            "Positive screen. Please consult an eating-disorder specialist.")
    };

    public static string SwlsScore(int ball) => ball switch
    {
        <= 9 => T("5-9 — крайне низкая удовлетворённость", "5-9 — extremely dissatisfied"),
        <= 14 => T("10-14 — низкая удовлетворённость", "10-14 — dissatisfied"),
        <= 19 => T("15-19 — слегка низкая удовлетворённость", "15-19 — slightly dissatisfied"),
        20 => T("20 — нейтральная удовлетворённость", "20 — neutral"),
        <= 25 => T("21-25 — слегка высокая удовлетворённость", "21-25 — slightly satisfied"),
        <= 30 => T("26-30 — высокая удовлетворённость", "26-30 — satisfied"),
        _ => T("31-35 — очень высокая удовлетворённость", "31-35 — extremely satisfied")
    };

    public static string SwlsScoreDetail(int ball) => ball switch
    {
        <= 9 => T(
            "Крайне низкая удовлетворённость жизнью. Полезно выгружать мысли и обсудить состояние со специалистом.",
            "Extremely low life satisfaction. Journaling and speaking with a professional may help."),
        <= 14 => T(
            "Низкая удовлетворённость жизнью. Помогают регулярный сон, движение и осмысленные цели.",
            "Low life satisfaction. Regular sleep, movement, and meaningful goals help."),
        <= 19 => T(
            "Удовлетворённость жизнью ниже среднего. Укрепляйте привычки, которые дают ощущение смысла.",
            "Life satisfaction is below average. Strengthen habits that bring a sense of meaning."),
        20 => T(
            "Нейтральная удовлетворённость жизнью. Есть пространство для улучшения повседневных практик.",
            "Neutral life satisfaction. There is room to improve everyday practices."),
        <= 25 => T(
            "Удовлетворённость жизнью слегка выше среднего. Продолжайте практики, которые вас поддерживают.",
            "Life satisfaction is slightly above average. Continue practices that support you."),
        <= 30 => T(
            "Высокая удовлетворённость жизнью. Поддерживайте то, что помогает сохранять это состояние.",
            "High life satisfaction. Maintain what helps you keep this state."),
        _ => T(
            "Очень высокая удовлетворённость жизнью. Продолжайте заботиться о ресурсах и балансе.",
            "Very high life satisfaction. Keep caring for your resources and balance.")
    };

    public static string TestRecommendationReasonPhq15(int score) =>
        score >= 15
            ? T("При высокой соматической нагрузке «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With high somatic burden, Spin helps lower the charge of painful memories.")
            : score >= 10
                ? T("Модификация опыта помогает пересмотреть связь тела и переживаний.", "Experience modification helps revisit the link between body and emotions.")
                : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonScoff(int score) =>
        score >= 2
            ? T("При признаках РПП «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With signs of an eating disorder, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonSwls(int score) =>
        score <= 20
            ? T("При низкой удовлетворённости жизнью полезно выгружать мысли на бумагу.", "With low life satisfaction, writing thoughts on paper is helpful.")
            : T("Модификация опыта поддерживает ощущение смысла и ресурса.", "Experience modification supports a sense of meaning and resource.");

}