namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string TestsDetectorTitle => ShellTabDetectorShort;
    public static string TestsFindProblemTitle => T("Поиск проблемы", "Find a problem");
    public static string TestsAboutPassageTitle => T("О прохождении", "About this test");
    public static string TestsDescriptionHeader => T("Описание", "Description");
    public static string TestsAlgorithmHeader => T("Алгоритм", "Steps");
    public static string TestsNoteHeader => T("Замечание", "Note");
    public static string TestsStartButton => T("Начать", "Start");
    public static string TestsQuestionnaireTitle => T("Опросник", "Questionnaire");
    public static string TestsQuestionPrefix => T("Вопрос ", "Question ");
    public static string TestsFinishButton => T("Завершить", "Finish");
    public static string TestsStandardTitle => T("Стандартный тест", "Standard test");
    public static string TestsBriefTitle => T("Краткий тест", "Brief test");
    public static string TestsColorInstruction => T(
        "Выбирайте цвета в приятной вам последовательности",
        "Choose colors in an order that feels pleasant to you");
    public static string TestsMoreInfo => T("Подробнее", "More info");
    public static string TestsRestart => T("Заново", "Start over");
    public static string TestRetakeButton => T("Пройти снова", "Take again");
    public static string TestsFirstColor => T("Первый цвет", "First color");
    public static string TestsSecondColor => T("Второй цвет", "Second color");
    public static string TestsLuscherFirstInstruction => T(
        "Выберите самый приятный для вас цвет",
        "Choose the color you find most pleasant");
    public static string TestsLuscherSecondInstruction => T(
        "Из оставшихся выберите самый неприятный для вас цвет",
        "From the remaining colors, choose the one you find least pleasant");
    public static string TestsStandardDescription => T(
        "Это стандартная версия теста Люшера. Она может более точно оценить настроение, чем альтернативная.",
        "This is the full Lüscher test. It can assess mood more precisely than the brief version.");
    public static string TestsBriefDescription => T(
        "Это краткая версия теста Люшера. Выберите два цвета — самый приятный и самый неприятный.",
        "This is the brief Lüscher test. Choose two colors — the most and least pleasant.");
    public static string TestsAnswerAllToast => T("Нужно ответить на все вопросы", "Please answer all questions");
    public static string TestsAnswerCurrentToast => T("Ответьте на этот вопрос", "Answer this question");
    public static string TestsStepOf(int current, int total) => T($"{current} из {total}", $"{current} of {total}");
    public static string TestsNextButton => T("Далее", "Next");
    public static string TestsPreviousButton => T("Назад", "Back");
    public static string TestsResultTitle(int score) => T($"Ваш результат: {score}", $"Your score: {score}");
    public static string TestsResultPageTitle => T("Результат теста", "Test result");
    public static string TestsBackToList => T("К списку тестов", "Back to tests");
    public static string TestsResultRecommendationHint => T(
        "На основе результата мы подобрали практику, которая может помочь",
        "Based on your result, we picked a practice that may help");
    public static string TestDuration(int minutes) => T($"~{minutes} мин", $"~{minutes} min");
    public static string TestQuestionCount(int count) => T($"{count} вопр.", $"{count} questions");
    public static string TestRecommendationFor(string techniqueTitle) =>
        T($"Рекомендуем: {techniqueTitle}", $"Recommended: {techniqueTitle}");
    public static string TestRecommendationReason(string reason) => reason;
    public static string TestsContinueButton => T("Продолжить", "Continue");
    public static string TestsListSectionTitle => T("Психологические тесты", "Psychological tests");
    public static string TestsListSectionSubtitle => T(
        "Выберите тест и узнайте больше о своём состоянии",
        "Pick a test to learn more about how you feel");
    public static string TestHistoryScore(int score) => T($"Балл: {score}", $"Score: {score}");
    public static string TestHistoryTrendTitle => T("Динамика баллов", "Score trend");
    public static string TestResultDuration(int seconds) =>
        seconds < 60
            ? T($"{seconds} сек", $"{seconds} sec")
            : T($"{seconds / 60} мин {seconds % 60} сек", $"{seconds / 60} min {seconds % 60} sec");
    public static string TestResultAnswersTitle => T("Ваши ответы", "Your answers");
    public static string TestsIntroLead => T(
        "Несколько минут — и вы получите персональную интерпретацию",
        "A few minutes for a personal interpretation");
    public static string TestsQuestionLead => T(
        "Отвечайте честно — здесь нет правильных или неправильных ответов",
        "Answer honestly — there are no right or wrong answers");
    public static string TestsMultiChoiceHint => T(
        "Можно выбрать несколько вариантов",
        "You can select more than one option");
    public static string TestsSingleChoiceHint => T(
        "Один вариант ответа",
        "Single answer");
    public static string TestsAnswerSelected => T("Выбрано", "Selected");
    public static string TestsAnswerNotSelected => T("Не выбрано", "Not selected");
    public static string TestsAnswerOption => T("Вариант ответа", "Answer option");
    public static string TestsRemainingDuration(int minutes) => T($"~{minutes} мин осталось", $"~{minutes} min left");
    public static string TestsCoLabel => T("Суммарное отклонение от аутогенной нормы (СО)", "Total deviation from autogenic norm (CO)");
    public static string TestsBkLabel => T("Вегетативный коэффициент (ВК)", "Vegetative coefficient (VC)");
    public static string TestsScoreOutOf(int value, string total) => T($"{value} из {total}", $"{value} of {total}");
    public static string TestsDecimalScoreOutOf(double value, string total) =>
        T($"{value} из {total}", $"{value} of {total}");
    public static string Yes => T("Да", "Yes");
    public static string No => T("Нет", "No");
    public static string Ok => T("OK", "OK");

}