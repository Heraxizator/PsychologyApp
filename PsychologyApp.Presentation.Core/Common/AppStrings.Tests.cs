namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string TestsDetectorTitle => ShellTabDetectorShort;
    public static string TestsFindProblemTitle => R(nameof(TestsFindProblemTitle));
    public static string TestsAboutPassageTitle => R(nameof(TestsAboutPassageTitle));
    public static string TestsDescriptionHeader => R(nameof(TestsDescriptionHeader));
    public static string TestsAlgorithmHeader => R(nameof(TestsAlgorithmHeader));
    public static string TestsNoteHeader => R(nameof(TestsNoteHeader));
    public static string TestsStartButton => R(nameof(TestsStartButton));
    public static string TestsQuestionnaireTitle => R(nameof(TestsQuestionnaireTitle));
    public static string TestsQuestionPrefix => R(nameof(TestsQuestionPrefix));
    public static string TestsFinishButton => R(nameof(TestsFinishButton));
    public static string TestsStandardTitle => R(nameof(TestsStandardTitle));
    public static string TestsBriefTitle => R(nameof(TestsBriefTitle));
    public static string TestsColorInstruction => R(nameof(TestsColorInstruction));
    public static string TestsMoreInfo => R(nameof(TestsMoreInfo));
    public static string TestsRestart => R(nameof(TestsRestart));
    public static string TestRetakeButton => R(nameof(TestRetakeButton));
    public static string TestsFirstColor => R(nameof(TestsFirstColor));
    public static string TestsSecondColor => R(nameof(TestsSecondColor));
    public static string TestsLuscherResultsTitle => R(nameof(TestsLuscherResultsTitle));
    public static string TestsLuscherWantedRole => R(nameof(TestsLuscherWantedRole));
    public static string TestsLuscherUnwantedRole => R(nameof(TestsLuscherUnwantedRole));
    public static string TestsLuscherFirstInstruction => R(nameof(TestsLuscherFirstInstruction));
    public static string TestsLuscherSecondInstruction => R(nameof(TestsLuscherSecondInstruction));
    public static string TestsLuscherSecondPassInstruction => R(nameof(TestsLuscherSecondPassInstruction));
    public static string TestsLuscherPassOf(int current, int total) => F(nameof(TestsLuscherPassOf), current, total);
    public static string TestsLuscherHistoryFirstPass => R(nameof(TestsLuscherHistoryFirstPass));
    public static string TestsLuscherHistorySecondPass => R(nameof(TestsLuscherHistorySecondPass));
    public static string TestsLuscherHistoryBk(double bk) => T($"{TestsBkLabel}: {bk}", $"{TestsBkLabel}: {bk}");
    public static string TestsStandardDescription => R(nameof(TestsStandardDescription));
    public static string TestsBriefDescription => R(nameof(TestsBriefDescription));
    public static string TestsAnswerAllToast => R(nameof(TestsAnswerAllToast));
    public static string TestsAnswerCurrentToast => R(nameof(TestsAnswerCurrentToast));
    public static string TestsStepOf(int current, int total) => F(nameof(TestsStepOf), current, total);
    public static string TestsNextButton => R(nameof(TestsNextButton));
    public static string TestsPreviousButton => R(nameof(TestsPreviousButton));
    public static string TestsResultTitle(int score) => F(nameof(TestsResultTitle), score);
    public static string TestsResultPageTitle => R(nameof(TestsResultPageTitle));
    public static string TestsBackToList => R(nameof(TestsBackToList));
    public static string TestsResultRecommendationHint => R(nameof(TestsResultRecommendationHint));
    public static string TestResultExplorePractice => R(nameof(TestResultExplorePractice));
    public static string TestDuration(int minutes) => F(nameof(TestDuration), minutes);
    public static string TestQuestionCount(int count) => F(nameof(TestQuestionCount), count);
    public static string TestRecommendationFor(string techniqueTitle) => F(nameof(TestRecommendationFor), techniqueTitle);
    public static string TestRecommendationReason(string reason) => reason;
    public static string TestsContinueButton => R(nameof(TestsContinueButton));
    public static string TestsListSectionTitle => R(nameof(TestsListSectionTitle));
    public static string TestsListSectionSubtitle => R(nameof(TestsListSectionSubtitle));
    public static string TestHistoryScore(int score) => F(nameof(TestHistoryScore), score);
    public static string TestHistoryTrendTitle => R(nameof(TestHistoryTrendTitle));
    public static string TestResultDuration(int seconds) =>
        seconds < 60
            ? F("TestResultDuration.1", seconds)
            : F("TestResultDuration.2", seconds / 60, seconds % 60);
    public static string TestResultAnswersTitle => R(nameof(TestResultAnswersTitle));
    public static string TestsIntroLead => R(nameof(TestsIntroLead));
    public static string TestsQuestionLead => R(nameof(TestsQuestionLead));
    public static string TestsMultiChoiceHint => R(nameof(TestsMultiChoiceHint));
    public static string TestsSingleChoiceHint => R(nameof(TestsSingleChoiceHint));
    public static string TestsAnswerSelected => R(nameof(TestsAnswerSelected));
    public static string TestsAnswerNotSelected => R(nameof(TestsAnswerNotSelected));
    public static string TestsAnswerOption => R(nameof(TestsAnswerOption));
    public static string TestsRemainingDuration(int minutes) => F(nameof(TestsRemainingDuration), minutes);
    public static string TestsCoLabel => R(nameof(TestsCoLabel));
    public static string TestsBkLabel => R(nameof(TestsBkLabel));
    public static string TestsScoreOutOf(int value, string total) => F(nameof(TestsScoreOutOf), value, total);
    public static string TestsDecimalScoreOutOf(double value, string total) => F(nameof(TestsDecimalScoreOutOf), value, total);
    public static string Yes => R(nameof(Yes));
    public static string No => R(nameof(No));
    public static string Ok => R(nameof(Ok));

}