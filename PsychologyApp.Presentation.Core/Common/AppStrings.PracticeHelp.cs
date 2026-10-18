namespace PsychologyApp.Presentation.Common;

/// <summary>Copy for filling practices without typing (chips, voice, emotion palette) and for the interactive practice stages.</summary>
public static partial class AppStrings
{
    public static string PracticeVoiceStart => R(nameof(PracticeVoiceStart));
    public static string PracticeVoiceStop => R(nameof(PracticeVoiceStop));
    public static string PracticeVoiceListening => R(nameof(PracticeVoiceListening));
    public static string PracticeVoiceHolding => R(nameof(PracticeVoiceHolding));
    public static string PracticeVoiceNothingHeard => R(nameof(PracticeVoiceNothingHeard));
    public static string PracticeVoicePermissionDenied => R(nameof(PracticeVoicePermissionDenied));
    public static string PracticeVoiceUnavailable => R(nameof(PracticeVoiceUnavailable));
    public static string PracticeRecentProblem(string problem) => F(nameof(PracticeRecentProblem), problem);
    public static string PracticeAssistTitle => R(nameof(PracticeAssistTitle));
    public static string PracticeEmotionHint => R(nameof(PracticeEmotionHint));
    public static string TechniqueStepSkip => R(nameof(TechniqueStepSkip));
    public static string TechniqueStepsSummary(int count) => F(nameof(TechniqueStepsSummary), count);
    public static string PracticeMoreChips(int count) => F(nameof(PracticeMoreChips), count);
    public static string PracticeAnswersTitle => R(nameof(PracticeAnswersTitle));

    public static string StageEmptyText => R(nameof(StageEmptyText));
    public static string StageAgain => R(nameof(StageAgain));

    public static string StageResizeTitle => R(nameof(StageResizeTitle));
    public static string StageResizeHint => R(nameof(StageResizeHint));
    public static string StageResizeAction => R(nameof(StageResizeAction));
    public static string StageResizeDone => R(nameof(StageResizeDone));

    public static string StageWhiteSpotTitle => R(nameof(StageWhiteSpotTitle));
    public static string StageWhiteSpotHint => R(nameof(StageWhiteSpotHint));
    public static string StageWhiteSpotAction => R(nameof(StageWhiteSpotAction));
    public static string StageWhiteSpotStop => R(nameof(StageWhiteSpotStop));
    public static string StageWhiteSpotSeconds(int seconds) => F(nameof(StageWhiteSpotSeconds), seconds);
    public static string StageWhiteSpotDone => R(nameof(StageWhiteSpotDone));

    public static string StagePaperTitle => R(nameof(StagePaperTitle));
    public static string StagePaperHint => R(nameof(StagePaperHint));
    public static string StagePaperCrumple => R(nameof(StagePaperCrumple));
    public static string StagePaperTear => R(nameof(StagePaperTear));
    public static string StagePaperThrow => R(nameof(StagePaperThrow));
    public static string StagePaperDone => R(nameof(StagePaperDone));

    public static string StageRepeatTitle => R(nameof(StageRepeatTitle));
    public static string StageRepeatHint => R(nameof(StageRepeatHint));
    public static string StageRepeatCount(int count) => F(nameof(StageRepeatCount), count);
    public static string StageRepeatCheck => R(nameof(StageRepeatCheck));
    public static string StageRepeatButton => R(nameof(StageRepeatButton));

    public static string StagePauseTitle => R(nameof(StagePauseTitle));
    public static string StagePauseHint => R(nameof(StagePauseHint));
    public static string StagePauseStart => R(nameof(StagePauseStart));
    public static string StagePauseStop => R(nameof(StagePauseStop));
    public static string StagePauseDone => R(nameof(StagePauseDone));

    public static string StageFutureTitle => R(nameof(StageFutureTitle));
    public static string StageFutureHint => R(nameof(StageFutureHint));

    /// <summary>The marks along the "view from the future" slider: now, a month, a year, 5, 10 and 50 years.</summary>
    public static string StageFutureStop(int index) => R($"StageFutureStop{Math.Clamp(index, 0, StageFutureStopCount - 1)}");

    public const int StageFutureStopCount = 6;
}
