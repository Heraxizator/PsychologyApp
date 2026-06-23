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
    public static string TestsCoLabel => T("Суммарное отклонение от аутогенной нормы (СО)", "Total deviation from autogenic norm (CO)");
    public static string TestsBkLabel => T("Вегетативный коэффициент (ВК)", "Vegetative coefficient (VC)");
    public static string TestsScoreOutOf(int value, string total) => T($"{value} из {total}", $"{value} of {total}");
    public static string TestsDecimalScoreOutOf(double value, string total) =>
        T($"{value} из {total}", $"{value} of {total}");
    public static string Yes => T("Да", "Yes");
    public static string No => T("Нет", "No");
    public static string Ok => T("OK", "OK");

}