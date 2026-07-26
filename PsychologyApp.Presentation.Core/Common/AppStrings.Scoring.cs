using PsychologyApp.Domain.Tests;

namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string BeckScore(BeckBand band) => band switch
    {
        BeckBand.None => T("0-9 - нет депрессивных симптомов", "0-9 - no depressive symptoms"),
        BeckBand.Mild => T("10-15 - лёгкая депрессия", "10-15 - mild depression"),
        BeckBand.Moderate => T("16-19 - умеренная депрессия", "16-19 - moderate depression"),
        BeckBand.Marked => T("20-29 - выраженная депрессия (средней тяжести)", "20-29 - marked depression (moderate severity)"),
        _ => T("30-63 – тяжелая депрессия", "30-63 - severe depression")
    };

    public static string BeckScoreDetail(BeckBand band) => band switch
    {
        BeckBand.None => T(
            "Симптомы в пределах нормы. Поддерживайте режим сна и регулярную активность.",
            "Symptoms are within normal range. Keep sleep and regular activity."),
        BeckBand.Mild => T(
            "Лёгкое снижение настроения. Помогают прогулки, дневник мыслей и короткие практики.",
            "Mild low mood. Walks, thought journaling, and short practices can help."),
        BeckBand.Moderate => T(
            "Умеренная депрессия. Имеет смысл обсудить состояние со специалистом и добавить ежедневные практики.",
            "Moderate depression. Consider talking to a professional and daily practices."),
        BeckBand.Marked => T(
            "Выраженные симптомы. Рекомендуем обратиться к психологу или врачу и не оставаться с этим наедине.",
            "Marked symptoms. Please consult a psychologist or doctor and seek support."),
        _ => T(
            "Тяжёлая депрессия. Важно как можно скорее получить профессиональную помощь.",
            "Severe depression. Professional help is strongly recommended.")
    };

    public static string HeckHessScore(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? T("0-24 - невысокий уровень невротизации", "0-24 - low neuroticism")
            : T("25-40 - высокий уровень невротизации", "25-40 - high neuroticism");

    public static string HeckHessScoreDetail(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? T(
                "Эмоциональная реактивность в пределах нормы. Продолжайте отслеживать стресс и отдых.",
                "Emotional reactivity is within normal range. Keep monitoring stress and rest.")
            : T(
                "Повышенная невротизация. Практики на баланс противоположностей и сравнение важностей могут снизить напряжение.",
                "Elevated neuroticism. Polarity and importance-comparison practices may ease tension.");

    public static string HaerScore(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? T("0-28 - невысокий уровень психопатии", "0-28 - low psychopathy")
            : T("29-40 - высокий уровень психопатии", "29-40 - high psychopathy");

    public static string HaerScoreDetail(BinarySeverityBand band) =>
        band == BinarySeverityBand.Low
            ? T(
                "Показатель в низком диапазоне. Это скрининг, а не диагноз — ориентируйтесь на самочувствие.",
                "Score is in the low range. This is a screening tool, not a diagnosis.")
            : T(
                "Повышенные показатели. Полезно работать с прошлым опытом и переосмыслением убеждений.",
                "Elevated score. Working with past experience and beliefs may help.");

    public static string PochebutScore(PochebutBand band) => band switch
    {
        PochebutBand.Low => T("0-10 - низкий уровень агрессивности", "0-10 - low aggressiveness"),
        PochebutBand.Moderate => T("11-24 - средний уровень агрессивности", "11-24 - moderate aggressiveness"),
        _ => T("25-40 - высокий уровень агрессивности", "25-40 - high aggressiveness")
    };

    public static string PochebutScoreDetail(PochebutBand band) => band switch
    {
        PochebutBand.Low => T(
            "Агрессивные реакции редки. Сохраняйте навыки саморегуляции.",
            "Aggressive reactions are rare. Keep your self-regulation habits."),
        PochebutBand.Moderate => T(
            "Умеренная агрессивность. Помогает пауза, дыхание и переформулировка ситуации.",
            "Moderate aggressiveness. Pause, breathing, and reframing the situation help."),
        _ => T(
            "Высокая агрессивность. Практики на снижение важности и дистанцирование от триггера особенно уместны.",
            "High aggressiveness. Practices that lower importance and add distance from triggers are especially useful.")
    };

    public static string TestRecommendationReasonBeck(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При депрессивных симптомах «Крутилка» помогает снизить заряд болезненных воспоминаний.", "For depressive symptoms, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonHeckHess(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При высокой невротизации полярности помогают увидеть обе стороны ситуации.", "With high neuroticism, polarities help see both sides.")
            : T("Сравнение важностей укрепляет ощущение перспективы.", "Comparing importance strengthens perspective.");

    public static string TestRecommendationReasonHaer(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("«50 лет спустя» отдаляет проблему во времени.", "\"50 years later\" moves the problem forward in time.")
            : T("Модификация опыта помогает пересмотреть убеждения.", "Experience modification helps revisit beliefs.");

    public static string TestRecommendationReasonPochebut(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("«Уменьши это» визуально снижает значимость триггера.", "\"Shrink it\" visually lowers the trigger's importance.")
            : T("«Проверь это» помогает отпустить зацикленную мысль.", "\"Check it\" helps release a looping thought.");

    public static string Gad7Score(Gad7Band band) => band switch
    {
        Gad7Band.Minimal => T("0-4 — минимальная тревога", "0-4 — minimal anxiety"),
        Gad7Band.Mild => T("5-9 — лёгкая тревога", "5-9 — mild anxiety"),
        Gad7Band.Moderate => T("10-14 — умеренная тревога", "10-14 — moderate anxiety"),
        _ => T("15-21 — выраженная тревога", "15-21 — severe anxiety")
    };

    public static string Gad7ScoreDetail(Gad7Band band) => band switch
    {
        Gad7Band.Minimal => T(
            "Тревожные симптомы в пределах нормы. Продолжайте отслеживать стресс и отдых.",
            "Anxiety symptoms are within normal range. Keep monitoring stress and rest."),
        Gad7Band.Mild => T(
            "Лёгкая тревога. Помогают дыхание, прогулки и короткие практики заземления.",
            "Mild anxiety. Breathing, walks, and short grounding practices help."),
        Gad7Band.Moderate => T(
            "Умеренная тревога. Имеет смысл обсудить состояние со специалистом.",
            "Moderate anxiety. Consider discussing your state with a professional."),
        _ => T(
            "Выраженная тревога. Рекомендуем обратиться к психологу или врачу.",
            "Severe anxiety. Please consult a psychologist or doctor.")
    };

    public static string K10Score(K10Band band) => band switch
    {
        K10Band.Low => T("10-15 — низкий дистресс", "10-15 — low distress"),
        K10Band.Mild => T("16-21 — умеренный дистресс", "16-21 — mild distress"),
        K10Band.Moderate => T("22-29 — выраженный дистресс", "22-29 — moderate distress"),
        _ => T("30-50 — высокий дистресс", "30-50 — high distress")
    };

    public static string K10ScoreDetail(K10Band band) => band switch
    {
        K10Band.Low => T(
            "Психологический дистресс в низком диапазоне. Поддерживайте режим и регулярную активность.",
            "Psychological distress is in the low range. Maintain routine and regular activity."),
        K10Band.Mild => T(
            "Умеренный дистресс. Практики на переосмысление опыта и выгрузку мыслей могут помочь.",
            "Mild distress. Experience reframing and thought journaling practices may help."),
        K10Band.Moderate => T(
            "Выраженный дистресс. Важно не оставаться с этим наедине — обратитесь за поддержкой.",
            "Moderate distress. Do not face this alone — seek support."),
        _ => T(
            "Высокий дистресс. Рекомендуем как можно скорее получить профессиональную помощь.",
            "High distress. Professional help is strongly recommended.")
    };

    public static string Who5Score(Who5Band band) => band switch
    {
        Who5Band.Low => T("0-12 — низкое благополучие", "0-12 — low well-being"),
        Who5Band.Fair => T("13-18 — удовлетворительное благополучие", "13-18 — fair well-being"),
        _ => T("19-25 — хорошее благополучие", "19-25 — good well-being")
    };

    public static string Who5ScoreDetail(Who5Band band) => band switch
    {
        Who5Band.Low => T(
            "Субъективное благополучие снижено. Помогают регулярный сон, движение и выгрузка мыслей на бумагу.",
            "Subjective well-being is low. Regular sleep, movement, and journaling thoughts can help."),
        Who5Band.Fair => T(
            "Благополучие на среднем уровне. Укрепляйте привычки, которые дают энергию и интерес к делам.",
            "Well-being is at a moderate level. Strengthen habits that bring energy and interest."),
        _ => T(
            "Хороший уровень благополучия. Продолжайте практики, которые поддерживают это состояние.",
            "Good level of well-being. Continue practices that support this state.")
    };

    public static string TestRecommendationReasonGad7(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При тревоге полярности помогают увидеть обе стороны ситуации.", "With anxiety, polarities help see both sides of a situation.")
            : T("Сравнение важностей укрепляет ощущение перспективы.", "Comparing importance strengthens perspective.");

    public static string TestRecommendationReasonK10(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При высоком дистрессе «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With high distress, Spin helps lower the charge of painful memories."),
        RecommendationLane.Mid => T("Модификация опыта помогает пересмотреть тяжёлые переживания.", "Experience modification helps revisit difficult experiences."),
        _ => T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.")
    };

    public static string TestRecommendationReasonWho5(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При низком благополучии полезно выгружать мысли на бумагу.", "With low well-being, writing thoughts on paper is helpful.")
            : T("Модификация опыта поддерживает ощущение смысла и ресурса.", "Experience modification supports a sense of meaning and resource.");

    public static string Phq9Score(Phq9Band band) => band switch
    {
        Phq9Band.Minimal => T("0-4 — минимальная депрессия", "0-4 — minimal depression"),
        Phq9Band.Mild => T("5-9 — лёгкая депрессия", "5-9 — mild depression"),
        Phq9Band.Moderate => T("10-14 — умеренная депрессия", "10-14 — moderate depression"),
        Phq9Band.ModeratelySevere => T("15-19 — умеренно тяжёлая депрессия", "15-19 — moderately severe depression"),
        _ => T("20-27 — тяжёлая депрессия", "20-27 — severe depression")
    };

    public static string Phq9ScoreDetail(Phq9Band band) => band switch
    {
        Phq9Band.Minimal => T(
            "Депрессивные симптомы в пределах нормы. Поддерживайте режим и активность.",
            "Depressive symptoms are within normal range. Maintain routine and activity."),
        Phq9Band.Mild => T(
            "Лёгкая депрессия. Помогают сон, движение и выгрузка мыслей на бумагу.",
            "Mild depression. Sleep, movement, and journaling thoughts can help."),
        Phq9Band.Moderate => T(
            "Умеренная депрессия. Имеет смысл обсудить состояние со специалистом.",
            "Moderate depression. Consider discussing your state with a professional."),
        Phq9Band.ModeratelySevere => T(
            "Умеренно тяжёлая депрессия. Рекомендуем обратиться к психологу или врачу.",
            "Moderately severe depression. Please consult a psychologist or doctor."),
        _ => T(
            "Тяжёлая депрессия. Важно как можно скорее получить профессиональную помощь.",
            "Severe depression. Professional help is urgently recommended.")
    };

    public static string IsiScore(IsiBand band) => band switch
    {
        IsiBand.None => T("0-7 — норма", "0-7 — no significant insomnia"),
        IsiBand.Subthreshold => T("8-14 — субпороговая бессонница", "8-14 — subthreshold insomnia"),
        IsiBand.Moderate => T("15-21 — умеренная бессонница", "15-21 — moderate insomnia"),
        _ => T("22-28 — тяжёлая бессонница", "22-28 — severe insomnia")
    };

    public static string IsiScoreDetail(IsiBand band) => band switch
    {
        IsiBand.None => T(
            "Значимых нарушений сна нет. Сохраняйте стабильный режим и гигиену сна.",
            "No clinically significant insomnia. Keep a stable sleep schedule and sleep hygiene."),
        IsiBand.Subthreshold => T(
            "Субпороговая бессонница. Помогают ритуалы перед сном и снижение стимуляции вечером.",
            "Subthreshold insomnia. Bedtime rituals and reducing evening stimulation help."),
        IsiBand.Moderate => T(
            "Умеренная бессонница. Имеет смысл обсудить сон со специалистом.",
            "Moderate insomnia. Consider discussing sleep with a professional."),
        _ => T(
            "Тяжёлая бессонница. Рекомендуем обратиться к врачу или специалисту по сну.",
            "Severe insomnia. Please consult a doctor or sleep specialist.")
    };

    public static string EssScore(EssBand band) => band switch
    {
        EssBand.Normal => T("0-10 — норма", "0-10 — normal"),
        EssBand.Mild => T("11-12 — лёгкая сонливость", "11-12 — mild sleepiness"),
        EssBand.Moderate => T("13-15 — умеренная сонливость", "13-15 — moderate sleepiness"),
        _ => T("16-24 — выраженная сонливость", "16-24 — severe sleepiness")
    };

    public static string EssScoreDetail(EssBand band) => band switch
    {
        EssBand.Normal => T(
            "Дневная сонливость в пределах нормы. Поддерживайте регулярный сон.",
            "Daytime sleepiness is within normal range. Maintain regular sleep."),
        EssBand.Mild => T(
            "Лёгкая дневная сонливость. Проверьте режим сна и время отхода ко сну.",
            "Mild daytime sleepiness. Review your sleep schedule and bedtime."),
        EssBand.Moderate => T(
            "Умеренная дневная сонливость. Имеет смысл обсудить это с врачом.",
            "Moderate daytime sleepiness. Consider discussing this with a doctor."),
        _ => T(
            "Выраженная дневная сонливость. Рекомендуем обратиться к врачу.",
            "Severe daytime sleepiness. Medical consultation is recommended.")
    };

    public static string TestRecommendationReasonPhq9(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При депрессивных симптомах «Крутилка» помогает снизить заряд болезненных воспоминаний.", "For depressive symptoms, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonIsi(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При тяжёлой бессоннице «Крутилка» помогает снизить заряд тревожных мыслей.", "With severe insomnia, Spin helps lower the charge of anxious thoughts."),
        RecommendationLane.Mid => T("Модификация опыта помогает пересмотреть тяжёлые переживания, мешающие сну.", "Experience modification helps revisit difficult experiences that disrupt sleep."),
        _ => T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.")
    };

    public static string TestRecommendationReasonEss(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При выраженной сонливости «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With severe sleepiness, Spin helps lower the charge of painful memories."),
        RecommendationLane.Mid => T("Модификация опыта помогает пересмотреть привычки, влияющие на бодрость.", "Experience modification helps revisit habits that affect alertness."),
        _ => T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.")
    };

    public static string Phq15Score(Phq15Band band) => band switch
    {
        Phq15Band.Minimal => T("0-4 — минимальная соматическая нагрузка", "0-4 — minimal somatic burden"),
        Phq15Band.Low => T("5-9 — низкая соматическая нагрузка", "5-9 — low somatic burden"),
        Phq15Band.Moderate => T("10-14 — умеренная соматическая нагрузка", "10-14 — moderate somatic burden"),
        _ => T("15-30 — высокая соматическая нагрузка", "15-30 — high somatic burden")
    };

    public static string Phq15ScoreDetail(Phq15Band band) => band switch
    {
        Phq15Band.Minimal => T(
            "Соматические симптомы в пределах нормы. Поддерживайте режим и физическую активность.",
            "Somatic symptoms are within normal range. Maintain routine and physical activity."),
        Phq15Band.Low => T(
            "Низкая соматическая нагрузка. Отслеживайте сон, стресс и нагрузку.",
            "Low somatic burden. Monitor sleep, stress, and workload."),
        Phq15Band.Moderate => T(
            "Умеренная соматическая нагрузка. Имеет смысл обсудить жалобы с врачом.",
            "Moderate somatic burden. Consider discussing symptoms with a doctor."),
        _ => T(
            "Высокая соматическая нагрузка. Рекомендуем обратиться к врачу для обследования.",
            "High somatic burden. Medical evaluation is recommended.")
    };

    public static string ScoffScore(ScoffBand band) =>
        band == ScoffBand.Negative
            ? T("0-1 — отрицательный скрининг", "0-1 — negative screen")
            : T("2-5 — положительный скрининг", "2-5 — positive screen");

    public static string ScoffScoreDetail(ScoffBand band) =>
        band == ScoffBand.Negative
            ? T(
                "Признаков расстройства пищевого поведения по скринингу не выявлено.",
                "No signs of an eating disorder on this screen.")
            : T(
                "Положительный скрининг. Рекомендуем обратиться к специалисту по пищевому поведению.",
                "Positive screen. Please consult an eating-disorder specialist.");

    public static string SwlsScore(SwlsBand band) => band switch
    {
        SwlsBand.ExtremelyDissatisfied => T("5-9 — крайне низкая удовлетворённость", "5-9 — extremely dissatisfied"),
        SwlsBand.Dissatisfied => T("10-14 — низкая удовлетворённость", "10-14 — dissatisfied"),
        SwlsBand.SlightlyDissatisfied => T("15-19 — слегка низкая удовлетворённость", "15-19 — slightly dissatisfied"),
        SwlsBand.Neutral => T("20 — нейтральная удовлетворённость", "20 — neutral"),
        SwlsBand.SlightlySatisfied => T("21-25 — слегка высокая удовлетворённость", "21-25 — slightly satisfied"),
        SwlsBand.Satisfied => T("26-30 — высокая удовлетворённость", "26-30 — satisfied"),
        _ => T("31-35 — очень высокая удовлетворённость", "31-35 — extremely satisfied")
    };

    public static string SwlsScoreDetail(SwlsBand band) => band switch
    {
        SwlsBand.ExtremelyDissatisfied => T(
            "Крайне низкая удовлетворённость жизнью. Полезно выгружать мысли и обсудить состояние со специалистом.",
            "Extremely low life satisfaction. Journaling and speaking with a professional may help."),
        SwlsBand.Dissatisfied => T(
            "Низкая удовлетворённость жизнью. Помогают регулярный сон, движение и осмысленные цели.",
            "Low life satisfaction. Regular sleep, movement, and meaningful goals help."),
        SwlsBand.SlightlyDissatisfied => T(
            "Удовлетворённость жизнью ниже среднего. Укрепляйте привычки, которые дают ощущение смысла.",
            "Life satisfaction is below average. Strengthen habits that bring a sense of meaning."),
        SwlsBand.Neutral => T(
            "Нейтральная удовлетворённость жизнью. Есть пространство для улучшения повседневных практик.",
            "Neutral life satisfaction. There is room to improve everyday practices."),
        SwlsBand.SlightlySatisfied => T(
            "Удовлетворённость жизнью слегка выше среднего. Продолжайте практики, которые вас поддерживают.",
            "Life satisfaction is slightly above average. Continue practices that support you."),
        SwlsBand.Satisfied => T(
            "Высокая удовлетворённость жизнью. Поддерживайте то, что помогает сохранять это состояние.",
            "High life satisfaction. Maintain what helps you keep this state."),
        _ => T(
            "Очень высокая удовлетворённость жизнью. Продолжайте заботиться о ресурсах и балансе.",
            "Very high life satisfaction. Keep caring for your resources and balance.")
    };

    public static string TestRecommendationReasonPhq15(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При высокой соматической нагрузке «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With high somatic burden, Spin helps lower the charge of painful memories."),
        RecommendationLane.Mid => T("Модификация опыта помогает пересмотреть связь тела и переживаний.", "Experience modification helps revisit the link between body and emotions."),
        _ => T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.")
    };

    public static string TestRecommendationReasonScoff(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При признаках РПП «Крутилка» помогает снизить заряд болезненных воспоминаний.", "With signs of an eating disorder, Spin helps lower the charge of painful memories.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonSwls(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При низкой удовлетворённости жизнью полезно выгружать мысли на бумагу.", "With low life satisfaction, writing thoughts on paper is helpful.")
            : T("Модификация опыта поддерживает ощущение смысла и ресурса.", "Experience modification supports a sense of meaning and resource.");

    public static string Pss10Score(Pss10Band band) => band switch
    {
        Pss10Band.Low => T("0–13 — низкий стресс", "0–13 — low stress"),
        Pss10Band.Moderate => T("14–26 — умеренный стресс", "14–26 — moderate stress"),
        _ => T("27–40 — высокий стресс", "27–40 — high stress")
    };

    public static string Pss10ScoreDetail(Pss10Band band) => band switch
    {
        Pss10Band.Low => T(
            "Воспринимаемый стресс в пределах нормы. Поддерживайте режим отдыха и восстановления.",
            "Perceived stress is within normal range. Maintain rest and recovery habits."),
        Pss10Band.Moderate => T(
            "Умеренный стресс. Дыхательные практики и заземление помогают снизить напряжение.",
            "Moderate stress. Breathing and grounding practices can ease tension."),
        _ => T(
            "Высокий воспринимаемый стресс. Имеет смысл снизить нагрузку и обратиться за поддержкой.",
            "High perceived stress. Consider reducing load and seeking support.")
    };

    public static string Phq2Score(ScreenBand band) =>
        band == ScreenBand.Negative
            ? T("0–2 — отрицательный скрининг", "0–2 — negative screen")
            : T("3–6 — положительный скрининг", "3–6 — positive screen");

    public static string Phq2ScoreDetail(ScreenBand band) =>
        band == ScreenBand.Negative
            ? T(
                "Симптомы депрессии не выражены. Продолжайте отслеживать настроение.",
                "Depressive symptoms are not prominent. Keep monitoring mood.")
            : T(
                "Положительный скрининг депрессии. Рассмотрите PHQ-9 или консультацию со специалистом.",
                "Positive depression screen. Consider PHQ-9 or a professional consultation.");

    public static string Gad2Score(ScreenBand band) =>
        band == ScreenBand.Negative
            ? T("0–2 — отрицательный скрининг", "0–2 — negative screen")
            : T("3–6 — положительный скрининг", "3–6 — positive screen");

    public static string Gad2ScoreDetail(ScreenBand band) =>
        band == ScreenBand.Negative
            ? T(
                "Симптомы тревоги не выражены. Продолжайте отслеживать состояние.",
                "Anxiety symptoms are not prominent. Keep monitoring how you feel.")
            : T(
                "Положительный скрининг тревоги. Рассмотрите GAD-7 или консультацию со специалистом.",
                "Positive anxiety screen. Consider GAD-7 or a professional consultation.");

    public static string HadsAnxietyScore(HadsBand band) => band switch
    {
        HadsBand.Normal => T("0–7 — норма", "0–7 — normal"),
        HadsBand.Borderline => T("8–10 — пограничный уровень", "8–10 — borderline"),
        _ => T("11–21 — выраженная тревога", "11–21 — elevated anxiety")
    };

    public static string HadsAnxietyScoreDetail(HadsBand band) => band switch
    {
        HadsBand.Normal => T(
            "Тревожные симптомы в пределах нормы.",
            "Anxiety symptoms are within normal range."),
        HadsBand.Borderline => T(
            "Пограничный уровень тревоги. Дыхание и заземление помогают стабилизировать состояние.",
            "Borderline anxiety. Breathing and grounding can help stabilize how you feel."),
        _ => T(
            "Выраженная тревога. Рекомендуем обратиться к специалисту и использовать практики саморегуляции.",
            "Elevated anxiety. Consider professional support and self-regulation practices.")
    };

    public static string HadsDepressionScore(HadsBand band) => band switch
    {
        HadsBand.Normal => T("0–7 — норма", "0–7 — normal"),
        HadsBand.Borderline => T("8–10 — пограничный уровень", "8–10 — borderline"),
        _ => T("11–21 — выраженная депрессия", "11–21 — elevated depression")
    };

    public static string HadsDepressionScoreDetail(HadsBand band) => band switch
    {
        HadsBand.Normal => T(
            "Депрессивные симптомы в пределах нормы.",
            "Depressive symptoms are within normal range."),
        HadsBand.Borderline => T(
            "Пограничный уровень. Малые шаги действия и выгрузка мыслей могут помочь.",
            "Borderline level. Small steps and writing thoughts may help."),
        _ => T(
            "Выраженные симптомы. Рекомендуем обратиться к специалисту.",
            "Elevated symptoms. Professional support is recommended.")
    };

    public static string RsesScore(RsesBand band) => band switch
    {
        RsesBand.Low => T("0–14 — низкая самооценка", "0–14 — low self-esteem"),
        RsesBand.Normal => T("15–25 — нормальная самооценка", "15–25 — normal self-esteem"),
        _ => T("26–30 — высокая самооценка", "26–30 — high self-esteem")
    };

    public static string RsesScoreDetail(RsesBand band) => band switch
    {
        RsesBand.Low => T(
            "Низкая самооценка. Практики самосострадания и поддерживающий внутренний диалог могут помочь.",
            "Low self-esteem. Self-compassion and supportive self-talk may help."),
        RsesBand.Normal => T(
            "Самооценка в пределах нормы. Продолжайте поддерживать себя в сложные периоды.",
            "Self-esteem is within normal range. Keep supporting yourself during hard periods."),
        _ => T(
            "Высокая самооценка. Сохраняйте баланс самопринятия и реалистичной самооценки.",
            "High self-esteem. Maintain balance between self-acceptance and realistic self-appraisal.")
    };

    public static string TestRecommendationReasonPss10(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При высоком стрессе «Крутилка» помогает снизить эмоциональный заряд.", "With high stress, Spin helps lower emotional charge."),
        RecommendationLane.Mid => T("При умеренном стрессе дыхание успокаивает нервную систему.", "With moderate stress, breathing calms the nervous system."),
        _ => T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.")
    };

    public static string TestRecommendationReasonPhq2(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При признаках депрессии «Один маленький шаг» возвращает движение.", "With signs of depression, One small step restores momentum.")
            : T("Для профилактики полезно выгружать мысли на бумагу.", "For prevention, writing thoughts on paper is helpful.");

    public static string TestRecommendationReasonGad2(RecommendationLane lane) =>
        lane == RecommendationLane.High
            ? T("При тревоге «Квадратное дыхание» успокаивает тело.", "With anxiety, box breathing calms the body.")
            : T("Заземление помогает оставаться в настоящем моменте.", "Grounding helps you stay in the present moment.");

    public static string TestRecommendationReasonHadsAnxiety(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При выраженной тревоге «Крутилка» снижает эмоциональный заряд.", "With elevated anxiety, Spin lowers emotional charge."),
        RecommendationLane.Mid => T("При пограничной тревоге дыхание стабилизирует состояние.", "With borderline anxiety, breathing stabilizes how you feel."),
        _ => T("Заземление поддерживает спокойствие в теле.", "Grounding supports calm in the body.")
    };

    public static string TestRecommendationReasonHadsDepression(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При выраженной депрессии «Один маленький шаг» возвращает движение.", "With elevated depression, One small step restores momentum."),
        RecommendationLane.Mid => T("При пограничном состоянии «Лист бумаги» помогает выгрузить мысли.", "With borderline symptoms, Paper helps unload thoughts."),
        _ => T("«Запись мысли» помогает работать с негативными интерпретациями.", "Thought record helps work with negative interpretations.")
    };

    public static string TestRecommendationReasonRses(RecommendationLane lane) => lane switch
    {
        RecommendationLane.High => T("При низкой самооценке «Добрые слова себе» смягчают самокритику.", "With low self-esteem, Kind words to yourself soften self-criticism."),
        RecommendationLane.Mid => T("«Один маленький шаг» поддерживает ощущение способности действовать.", "One small step supports a sense of ability to act."),
        _ => T("Модификация опыта укрепляет ресурсное состояние.", "Experience modification strengthens resourceful states.")
    };
}
