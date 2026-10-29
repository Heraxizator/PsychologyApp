namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public const string DefaultLanguage = "ru";

    public static string? LanguageOverride { get; set; }

    public static Func<string>? LanguageProvider { get; set; }

    public static string Language =>
        LanguageOverride
        ?? LanguageProvider?.Invoke()
        ?? DefaultLanguage;


    public static string OptionsTitle => R(nameof(OptionsTitle));
    public static string OptionsSettingsTitle => R(nameof(OptionsSettingsTitle));
    public static string OptionsSettingsSubtitle => R(nameof(OptionsSettingsSubtitle));
    public static string OptionsAppSection => R(nameof(OptionsAppSection));
    public static string OptionsSafetySection => R(nameof(OptionsSafetySection));
    public static string OptionsSupportSection => R(nameof(OptionsSupportSection));
    public static string ProfileSettingsCardSubtitle => R(nameof(ProfileSettingsCardSubtitle));
    public static string OptionsAboutTitle => R(nameof(OptionsAboutTitle));
    public static string OptionsFeedbackTitle => R(nameof(OptionsFeedbackTitle));
    public static string OptionsDonateTitle => R(nameof(OptionsDonateTitle));

    public static string SettingsTitle => R(nameof(SettingsTitle));
    public static string SettingsDesignSection => R(nameof(SettingsDesignSection));
    public static string SettingsFontSection => R(nameof(SettingsFontSection));
    public static string SettingsLanguageLabel => R(nameof(SettingsLanguageLabel));
    public static string SettingsThemeLabel => R(nameof(SettingsThemeLabel));
    public static string SettingsColorLabel => R(nameof(SettingsColorLabel));
    public static string SettingsFormLabel => R(nameof(SettingsFormLabel));
    public static string SettingsSizeLabel => R(nameof(SettingsSizeLabel));
    public static string SettingsBoldLabel => R(nameof(SettingsBoldLabel));
    public static string SettingsTestsSection => R(nameof(SettingsTestsSection));
    public static string SettingsRemindersSection => R(nameof(SettingsRemindersSection));
    public static string SettingsPracticeRemindersLabel => R(nameof(SettingsPracticeRemindersLabel));
    public static string SettingsPracticeReminderHourLabel => R(nameof(SettingsPracticeReminderHourLabel));
    public static string SettingsPracticeReminderHourPickerTitle => R(nameof(SettingsPracticeReminderHourPickerTitle));
    public static string PracticeReminderTitle => R(nameof(PracticeReminderTitle));
    public static string PracticeReminderBody => R(nameof(PracticeReminderBody));
    public static string PracticeReminderTitleNamed(string techniqueName) => F(nameof(PracticeReminderTitleNamed), techniqueName);
    public static string PracticeReminderBodyNamed(string techniqueName, string reason) =>
        string.IsNullOrWhiteSpace(reason)
            ? F("PracticeReminderBodyNamed.1", techniqueName)
            : $"{reason} — {techniqueName}";
    public static string SettingsPrimaryConcernLabel => R(nameof(SettingsPrimaryConcernLabel));
    public static string SettingsPrimaryConcernPickerTitle => R(nameof(SettingsPrimaryConcernPickerTitle));
    public static string SettingsPrimaryConcernSection => R(nameof(SettingsPrimaryConcernSection));
    public static string SettingsQuestionnaireAutoAdvanceLabel => R(nameof(SettingsQuestionnaireAutoAdvanceLabel));
    public static string SettingsPickerShapes => R(nameof(SettingsPickerShapes));
    public static string SettingsPickerSizes => R(nameof(SettingsPickerSizes));
    public static string SettingsPickerLanguages => R(nameof(SettingsPickerLanguages));
    public static string SettingsFormHelper => R(nameof(SettingsFormHelper));
    public static string SettingsColorHelper => R(nameof(SettingsColorHelper));
    public static string SettingsReplayOnboarding => R(nameof(SettingsReplayOnboarding));

    public static string TechniqueTheory => R(nameof(TechniqueTheory));
    public static string TechniqueAlgorithm => R(nameof(TechniqueAlgorithm));
    public static string TechniqueFinish => R(nameof(TechniqueFinish));
    public static string TechniqueTitle => R(nameof(TechniqueTitle));
    public static string Back => R(nameof(Back));
    public static string Save => R(nameof(Save));
    public static string Send => R(nameof(Send));
    public static string Edit => R(nameof(Edit));
    public static string Remove => R(nameof(Remove));
    public static string NameLabel => R(nameof(NameLabel));
    public static string DescriptionLabel => R(nameof(DescriptionLabel));
    public static string Saving => R(nameof(Saving));
    public static string DesignerLoadError => R(nameof(DesignerLoadError));
    public static string DesignerSaveError => R(nameof(DesignerSaveError));
    public static string ThemeLabel => R(nameof(ThemeLabel));
    public static string AuthorLabel => R(nameof(AuthorLabel));
    public static string FormLabel => R(nameof(FormLabel));
    public static string ActionsListLabel => R(nameof(ActionsListLabel));

    public static string PracticeHomeTitle => R(nameof(PracticeHomeTitle));
    public static string PracticeMyTechniques => R(nameof(PracticeMyTechniques));
    public static string PracticeCatalog => R(nameof(PracticeCatalog));
    public static string PracticeCatalogHint => R(nameof(PracticeCatalogHint));
    public static string PracticeCreate => R(nameof(PracticeCreate));
    public static string PracticeTechniquesList => R(nameof(PracticeTechniquesList));
    public static string PracticeInitError => R(nameof(PracticeInitError));
    public static string PracticeLoadMoreError => R(nameof(PracticeLoadMoreError));
    public static string PracticeLoadingText => R(nameof(PracticeLoadingText));
    public static string PracticeLoadingMoreText => R(nameof(PracticeLoadingMoreText));
    public static string PracticeCustomTechniqueNumber(long id) => F(nameof(PracticeCustomTechniqueNumber), id);
    public static string PracticeDesignTitle => R(nameof(PracticeDesignTitle));
    public static string PracticeConstructor => R(nameof(PracticeConstructor));
    public static string PracticeCustomTechnique => R(nameof(PracticeCustomTechnique));
    public static string PracticeDeleteConfirm => R(nameof(PracticeDeleteConfirm));

    public static string ReviewTitle => R(nameof(ReviewTitle));
    public static string ReviewPage => R(nameof(ReviewPage));
    public static string ReviewExplanation => R(nameof(ReviewExplanation));
    // Without a configured address the message goes to the phone's share sheet, so the text must not promise that support receives it.
    public static string ReviewExplanationShare => R(nameof(ReviewExplanationShare));
    public static string ReviewShareButton => R(nameof(ReviewShareButton));
    public static string ReviewMessagePlaceholder => R(nameof(ReviewMessagePlaceholder));
    public static string ReviewMessageRequired => R(nameof(ReviewMessageRequired));
    public static string ReviewSendSuccessTitle => R(nameof(ReviewSendSuccessTitle));
    public static string ReviewSendSuccessMessage => R(nameof(ReviewSendSuccessMessage));
    public static string ReviewEmailSubject => R(nameof(ReviewEmailSubject));
    public static string ReviewSmsRecipientMissing => R(nameof(ReviewSmsRecipientMissing));
    public static string ReviewSmsNotSupported => R(nameof(ReviewSmsNotSupported));
    public static string ReviewSmsFailed => R(nameof(ReviewSmsFailed));
    public static string ReviewEmailNotSupported => R(nameof(ReviewEmailNotSupported));
    public static string ReviewEmailFailed => R(nameof(ReviewEmailFailed));
    public static string ReviewShareTitle => R(nameof(ReviewShareTitle));
    public static string ReviewShareFailed => R(nameof(ReviewShareFailed));

    public static string DonateTitle => R(nameof(DonateTitle));
    public static string DonateMoreInfo => R(nameof(DonateMoreInfo));
    public static string DonateBody => R(nameof(DonateBody));
    public static string DonateButton => R(nameof(DonateButton));
    public static string DonateOpenFailed => R(nameof(DonateOpenFailed));

    public static string InfoAboutBody => R(nameof(InfoAboutBody));

    public static string CleanerPrayersPage => R(nameof(CleanerPrayersPage));
    public static string CleanerPrayerCollection => R(nameof(CleanerPrayerCollection));
    public static string CleanerLoad => R(nameof(CleanerLoad));
    public static string CleanerSearchingPrayers => R(nameof(CleanerSearchingPrayers));
    public static string CleanerPreparingAudio => R(nameof(CleanerPreparingAudio));
    public static string CleanerPlaybackError => R(nameof(CleanerPlaybackError));
    public static string CleanerOfflineBadge => R(nameof(CleanerOfflineBadge));
    public static string CleanerPlayNext => R(nameof(CleanerPlayNext));
    public static string CleanerReplay => R(nameof(CleanerReplay));
    public static string CleanerMoreInfoBody => R(nameof(CleanerMoreInfoBody));
    public static string CleanerCollectionSubtitle => R(nameof(CleanerCollectionSubtitle));
    public static string CleanerCategoryAll => R(nameof(CleanerCategoryAll));
    public static string CleanerCategoryMorning => R(nameof(CleanerCategoryMorning));
    public static string CleanerCategoryEvening => R(nameof(CleanerCategoryEvening));
    public static string CleanerCategoryPenitential => R(nameof(CleanerCategoryPenitential));
    public static string CleanerCategoryCore => R(nameof(CleanerCategoryCore));
    public static string CleanerPrayerMain => R(nameof(CleanerPrayerMain));
    public static string CleanerPsalm50 => R(nameof(CleanerPsalm50));
    public static string CleanerPsalm50Desc => R(nameof(CleanerPsalm50Desc));
    public static string CleanerPsalm90 => R(nameof(CleanerPsalm90));
    public static string CleanerPsalm90Desc => R(nameof(CleanerPsalm90Desc));
    public static string CleanerOurFather => R(nameof(CleanerOurFather));
    public static string CleanerOurFatherDesc => R(nameof(CleanerOurFatherDesc));
    public static string CleanerJesusPrayer => R(nameof(CleanerJesusPrayer));
    public static string CleanerJesusPrayerDesc => R(nameof(CleanerJesusPrayerDesc));
    public static string CleanerHeavenlyKing => R(nameof(CleanerHeavenlyKing));
    public static string CleanerHeavenlyKingDesc => R(nameof(CleanerHeavenlyKingDesc));
    public static string CleanerMorningPrayer => R(nameof(CleanerMorningPrayer));
    public static string CleanerMorningPrayerDesc => R(nameof(CleanerMorningPrayerDesc));
    public static string CleanerSymbolOfFaith => R(nameof(CleanerSymbolOfFaith));
    public static string CleanerSymbolOfFaithDesc => R(nameof(CleanerSymbolOfFaithDesc));
    public static string CleanerEveningPrayer => R(nameof(CleanerEveningPrayer));
    public static string CleanerEveningPrayerDesc => R(nameof(CleanerEveningPrayerDesc));
    public static string CleanerTrisagion => R(nameof(CleanerTrisagion));
    public static string CleanerTrisagionDesc => R(nameof(CleanerTrisagionDesc));
    public static string CleanerVirginMary => R(nameof(CleanerVirginMary));
    public static string CleanerVirginMaryDesc => R(nameof(CleanerVirginMaryDesc));
    public static string CleanerHolySpirit => R(nameof(CleanerHolySpirit));
    public static string CleanerHolySpiritDesc => R(nameof(CleanerHolySpiritDesc));
    public static string CleanerDoxology => R(nameof(CleanerDoxology));
    public static string CleanerDoxologyDesc => R(nameof(CleanerDoxologyDesc));
    public static string CleanerSearchPlaceholder => R(nameof(CleanerSearchPlaceholder));
    public static string CleanerNoPrayersFound => R(nameof(CleanerNoPrayersFound));
    public static string CleanerCatalogEmpty => R(nameof(CleanerCatalogEmpty));
    public static string CleanerNowPlaying => R(nameof(CleanerNowPlaying));

    public static string DesignerNamePlaceholder => R(nameof(DesignerNamePlaceholder));
    public static string DesignerDescriptionPlaceholder => R(nameof(DesignerDescriptionPlaceholder));
    public static string DesignerThemePlaceholder => R(nameof(DesignerThemePlaceholder));
    public static string DesignerAuthorPlaceholder => R(nameof(DesignerAuthorPlaceholder));

    public static string Add => R(nameof(Add));
    public static string Repeat => R(nameof(Repeat));
    public static string Cancel => R(nameof(Cancel));
    public static string ConcernLabel => R(nameof(ConcernLabel));
    public static string FirstPolarityLabel => R(nameof(FirstPolarityLabel));
    public static string SecondPolarityLabel => R(nameof(SecondPolarityLabel));
    public static string PoleNumber(int number) => F(nameof(PoleNumber), number);
    public static string RecordNumber(int number) => F(nameof(RecordNumber), number);
    public static string PracticeEntryCount(int count) => F(nameof(PracticeEntryCount), count);
    public static string TechniqueStepProgress(int step, int total) => F(nameof(TechniqueStepProgress), step, total);
    public static string TechniqueStepBack => R(nameof(TechniqueStepBack));
    public static string TechniqueStepNext => R(nameof(TechniqueStepNext));
    public static string ProverbLabel => R(nameof(ProverbLabel));
    public static string QuoteAddFavoriteHint => R(nameof(QuoteAddFavoriteHint));
    public static string QuoteCopyHint => R(nameof(QuoteCopyHint));
    public static string QuoteShareHint => R(nameof(QuoteShareHint));
    public static string PolarityNegativePlaceholder => R(nameof(PolarityNegativePlaceholder));
    public static string PolarityPositivePlaceholder => R(nameof(PolarityPositivePlaceholder));

    public static string StartupErrorTitle => R(nameof(StartupErrorTitle));
    public static string StartupErrorMessage => R(nameof(StartupErrorMessage));
    public static string ErrorTitle => R(nameof(ErrorTitle));
    public static string UnexpectedErrorMessage => R(nameof(UnexpectedErrorMessage));
    public static string TestsResultSaveFailedMessage => R(nameof(TestsResultSaveFailedMessage));
    public static string TestsResultNavigationFailedMessage => R(nameof(TestsResultNavigationFailedMessage));
    public static string TechniqueNotFound => R(nameof(TechniqueNotFound));
    public static string QuoteNotFound => R(nameof(QuoteNotFound));

    public static string PracticeEmptyTitle => R(nameof(PracticeEmptyTitle));
    public static string PracticeEmptyBody => R(nameof(PracticeEmptyBody));
    public static string TestsEmptyTitle => R(nameof(TestsEmptyTitle));
    public static string TestsEmptyBody => R(nameof(TestsEmptyBody));
    public static string TestsEmptyRefresh => R(nameof(TestsEmptyRefresh));
    public static string TestsLoadingText => R(nameof(TestsLoadingText));
    public static string QuotesEmptyTitle => R(nameof(QuotesEmptyTitle));
    public static string QuotesEmptyBody => R(nameof(QuotesEmptyBody));
    public static string QuotesRefreshButton => R(nameof(QuotesRefreshButton));
    public static string ProfileQuotesEmpty => R(nameof(ProfileQuotesEmpty));
    public static string QuotesFavoritesEmptyBody => R(nameof(QuotesFavoritesEmptyBody));


    public static string ProfileTestsCompleted => R(nameof(ProfileTestsCompleted));
    public static string ProfileActiveDaysLabel => R(nameof(ProfileActiveDaysLabel));
    public static string ProfileActiveDaysValue(int days) => F(nameof(ProfileActiveDaysValue), days);

    public static string TodayForYou => R(nameof(TodayForYou));
    public static string TodayRecommended => R(nameof(TodayRecommended));
    public static string TodayStartPractice => R(nameof(TodayStartPractice));
    public static string ComebackBanner => R(nameof(ComebackBanner));
    public static string ComebackBannerWithTechnique(string name) => F(nameof(ComebackBannerWithTechnique), name);
    public static string MoodTrendUp => "↑";
    public static string MoodTrendFlat => "→";
    public static string MoodTrendDown => "↓";
    public static string OnboardingRemindersLabel => R(nameof(OnboardingRemindersLabel));
    public static string OnboardingReminderHourLabel => R(nameof(OnboardingReminderHourLabel));
    public static string TodayRecommendationReason(string concern) => concern switch
    {
        "anxiety" => R("TodayRecommendationReason.anxiety"),
        "body" => R("TodayRecommendationReason.body"),
        "mood" => R("TodayRecommendationReason.mood"),
        _ => R("TodayRecommendationReason.default")
    };

    public static string TodayRecommendationReasonFromTest(string testId) => F(nameof(TodayRecommendationReasonFromTest), testId);

    public static string TodayRecommendationReasonLowMood() =>
        R("TodayRecommendationReasonLowMood.1");
    public static string TodayRecommendationReasonContinueDraft() => R("TodayRecommendationReasonContinueDraft.1");
    public static string TodayMoodQuestion => R(nameof(TodayMoodQuestion));
    public static string TodayMoodSaved => R(nameof(TodayMoodSaved));
    public static string TodayMoodLine(int level, int max) =>
        F("TodayMoodLine.1", MoodEmoji(level), level, max);
    public static string MoodHistoryTitle => R(nameof(MoodHistoryTitle));
    public static string MoodHistoryEntry(string date, int level, int max) =>
        $"{date}: {MoodEmoji(level)} {level}/{max}";
    public static string ProfileMoodTrendTitle => R(nameof(ProfileMoodTrendTitle));
    public static string ProfileMoodCheckInTitle => R(nameof(ProfileMoodCheckInTitle));
    public static string JournalTitle => R(nameof(JournalTitle));
    public static string JournalCardSubtitle => R(nameof(JournalCardSubtitle));
    public static string JournalTodayTitle => R(nameof(JournalTodayTitle));
    public static string JournalEntriesTitle => R(nameof(JournalEntriesTitle));
    public static string JournalWeekEmpty => R(nameof(JournalWeekEmpty));
    public static string JournalNoteSectionTitle => R(nameof(JournalNoteSectionTitle));
    public static string JournalAddNoteLabel => R(nameof(JournalAddNoteLabel));
    public static string JournalShowHintsLabel => R(nameof(JournalShowHintsLabel));
    public static string JournalHideHintsLabel => R(nameof(JournalHideHintsLabel));
    public static string JournalMoreMenuLabel => R(nameof(JournalMoreMenuLabel));
    public static string JournalNoteSaveHint => R(nameof(JournalNoteSaveHint));
    public static string JournalSaveLabel => R(nameof(JournalSaveLabel));
    public static string JournalDeleteLabel => R(nameof(JournalDeleteLabel));
    public static string JournalDeleteConfirmTitle => R(nameof(JournalDeleteConfirmTitle));
    public static string JournalDeleteConfirmMessage => R(nameof(JournalDeleteConfirmMessage));
    public static string JournalDeleteConfirmAccept => R(nameof(JournalDeleteConfirmAccept));
    public static string JournalDeleteConfirmCancel => R(nameof(JournalDeleteConfirmCancel));
    public static string JournalNoNoteCaption => R(nameof(JournalNoNoteCaption));
    public static string JournalEditTodayHint => R(nameof(JournalEditTodayHint));
    public static string JournalDayEmptyHint(DateOnly day) =>
        F("JournalDayEmptyHint.1", day);
    public static string JournalPickMoodHint => R(nameof(JournalPickMoodHint));
    public static string JournalDayMoodLine(DateOnly day, int level, int max) =>
        $"{day:d}: {MoodEmoji(level)} {level}/{max}";
    public static string JournalMoodStatsTitle => R(nameof(JournalMoodStatsTitle));
    public static string JournalMoodStreakLabel => R(nameof(JournalMoodStreakLabel));
    public static string JournalDynamicsTitle => R(nameof(JournalDynamicsTitle));
    public static string JournalOverviewInsightEmpty => R(nameof(JournalOverviewInsightEmpty));
    public static string JournalPracticeMoodInsight(int practiceDays, string averageMood) => F(nameof(JournalPracticeMoodInsight), practiceDays, averageMood);
    public static string JournalPracticeMoodCompareInsight(
        int practiceDays,
        string averageOnPractice,
        string averageWithoutPractice) => F(nameof(JournalPracticeMoodCompareInsight), averageOnPractice, averageWithoutPractice, practiceDays);
    public static string JournalTryQuietQuote => R(nameof(JournalTryQuietQuote));
    public static string PracticeOpenJournalRow => R(nameof(PracticeOpenJournalRow));
    public static string JournalQuestionsSectionTitle => R(nameof(JournalQuestionsSectionTitle));
    public static string JournalFactorsSectionTitle => R(nameof(JournalFactorsSectionTitle));
    public static string JournalFactorsSummaryLine(string labels) => F(nameof(JournalFactorsSummaryLine), labels);
    public static string JournalWeekNavPrev => R(nameof(JournalWeekNavPrev));
    public static string JournalWeekNavNext => R(nameof(JournalWeekNavNext));
    public static string JournalMonthTitle(DateOnly month) =>
        month.ToDateTime(TimeOnly.MinValue).ToString("MMMM yyyy", System.Globalization.CultureInfo.CurrentCulture);
    public static string JournalFactorCountPill(string label, int count) => F(nameof(JournalFactorCountPill), label, count);
    public static string JournalWeekInsightLine(int checkIns, string trend, string streak)
    {
        if (checkIns <= 0)
        {
            return string.Empty;
        }

        List<string> parts = [];
        if (!string.IsNullOrWhiteSpace(trend))
        {
            parts.Add(F("JournalWeekInsightLine.1", trend));
        }

        if (!string.IsNullOrWhiteSpace(streak) && streak != MetricEmptyValue)
        {
            parts.Add(F("JournalWeekInsightLine.2", streak));
        }

        if (parts.Count == 0)
        {
            return F("JournalWeekInsightLine.3", checkIns, MoodCheckInWord(checkIns), MoodCheckInWordEn(checkIns));
        }

        return string.Join(" · ", parts);
    }

    private static string MoodCheckInWord(int count) => count switch
    {
        1 => "отметка",
        >= 2 and <= 4 => "отметки",
        _ => "отметок"
    };

    private static string MoodCheckInWordEn(int count) => count == 1 ? "check-in" : "check-ins";

    public static string JournalFilter7Days => R(nameof(JournalFilter7Days));
    public static string JournalFilter30Days => R(nameof(JournalFilter30Days));
    public static string JournalFilter90Days => R(nameof(JournalFilter90Days));
    public static string JournalYesterdayTitle => R(nameof(JournalYesterdayTitle));
    public static string JournalEditorDayTitle(DateOnly day)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        if (day == today)
        {
            return JournalTodayTitle;
        }

        if (day == today.AddDays(-1))
        {
            return JournalYesterdayTitle;
        }

        return day.ToDateTime(TimeOnly.MinValue).ToString("d MMM");
    }
    public static string JournalPastDayCheckInTitle => R(nameof(JournalPastDayCheckInTitle));
    public static string JournalPromptHelped => R(nameof(JournalPromptHelped));
    public static string JournalPromptBlocked => R(nameof(JournalPromptBlocked));
    public static string JournalPromptGrateful => R(nameof(JournalPromptGrateful));
    public static string JournalPromptNext => R(nameof(JournalPromptNext));
    public static string JournalSearchPlaceholder => R(nameof(JournalSearchPlaceholder));
    public static string JournalSearchEmpty => R(nameof(JournalSearchEmpty));
    public static string JournalBestWorstPill(int best, int worst) =>
        F("JournalBestWorstPill.1", MoodEmoji(best), best, MoodEmoji(worst), worst);
    public static string JournalTimelineEmpty => R(nameof(JournalTimelineEmpty));
    public static string JournalPromptBlockedShort => R(nameof(JournalPromptBlockedShort));
    public static string JournalPromptGratefulShort => R(nameof(JournalPromptGratefulShort));
    public static string JournalFactorSleep => R(nameof(JournalFactorSleep));
    public static string JournalFactorPeople => R(nameof(JournalFactorPeople));
    public static string JournalFactorPractice => R(nameof(JournalFactorPractice));
    public static string JournalFactorWalk => R(nameof(JournalFactorWalk));
    public static string JournalFactorWork => R(nameof(JournalFactorWork));
    public static string JournalFactorSport => R(nameof(JournalFactorSport));
    public static string JournalFactorRest => R(nameof(JournalFactorRest));
    public static string JournalFactorStress => R(nameof(JournalFactorStress));
    public static string JournalFactorHome => R(nameof(JournalFactorHome));
    public static string JournalFactorSleepLabel => R(nameof(JournalFactorSleepLabel));
    public static string JournalFactorPeopleLabel => R(nameof(JournalFactorPeopleLabel));
    public static string JournalFactorPracticeLabel => R(nameof(JournalFactorPracticeLabel));
    public static string JournalFactorWalkLabel => R(nameof(JournalFactorWalkLabel));
    public static string JournalFactorWorkLabel => R(nameof(JournalFactorWorkLabel));
    public static string JournalFactorSportLabel => R(nameof(JournalFactorSportLabel));
    public static string JournalFactorRestLabel => R(nameof(JournalFactorRestLabel));
    public static string JournalFactorStressLabel => R(nameof(JournalFactorStressLabel));
    public static string JournalFactorHomeLabel => R(nameof(JournalFactorHomeLabel));
    public static string JournalActivityCorrelationPill(string label, int count, string averageMood) => F(nameof(JournalActivityCorrelationPill), label, count, averageMood);
    public static string JournalYearNavPrev => R(nameof(JournalYearNavPrev));
    public static string JournalYearNavNext => R(nameof(JournalYearNavNext));
    public static string JournalYearTitle(int year) => F(nameof(JournalYearTitle), year);
    public static string JournalYearHeatmapTitle => R(nameof(JournalYearHeatmapTitle));
    public static string JournalCalendarScaleWeek => R(nameof(JournalCalendarScaleWeek));
    public static string JournalCalendarScaleMonth => R(nameof(JournalCalendarScaleMonth));
    public static string JournalCalendarScaleYear => R(nameof(JournalCalendarScaleYear));
    public static string JournalCalendarSectionTitle => R(nameof(JournalCalendarSectionTitle));
    public static string JournalStatsSectionTitle => R(nameof(JournalStatsSectionTitle));
    public static string JournalCheckInDaysLabel => R(nameof(JournalCheckInDaysLabel));
    public static string JournalCheckInDays(int days, int periodDays) => F(nameof(JournalCheckInDays), days, periodDays);
    public static string JournalPeriodNavPrev => R(nameof(JournalPeriodNavPrev));
    public static string JournalPeriodNavNext => R(nameof(JournalPeriodNavNext));
    public static string JournalPastDayPlaceholder => R(nameof(JournalPastDayPlaceholder));

    /// <summary>Question <paramref name="number"/> (1..<see cref="PsychologyApp.Presentation.Features.ManageJournal.JournalPrompts.Count"/>) above the journal note.</summary>
    public static string JournalPrompt(int number) => R($"JournalPrompt{number:00}");

    public static string JournalOnThisDayLastYear(int moodLevel, string? noteSnippet)
    {
        string mood = MoodLevelPill(moodLevel);
        if (string.IsNullOrWhiteSpace(noteSnippet))
        {
            return F("JournalOnThisDayLastYear.1", mood);
        }

        string snippet = noteSnippet.Length > 80 ? noteSnippet[..80].TrimEnd() + "…" : noteSnippet;
        return F("JournalOnThisDayLastYear.2", mood, snippet);
    }
    public static string JournalSlotMorning => R(nameof(JournalSlotMorning));
    public static string JournalLowMoodTitle => R(nameof(JournalLowMoodTitle));
    public static string JournalLowMoodBody => R(nameof(JournalLowMoodBody));
    public static string JournalLowMoodBreath => R(nameof(JournalLowMoodBreath));
    public static string JournalLowMoodAnchors => R(nameof(JournalLowMoodAnchors));
    public static string JournalLowMoodHelp => R(nameof(JournalLowMoodHelp));
    public static string JournalEmptyTitle => R(nameof(JournalEmptyTitle));
    public static string JournalEmptyBody => R(nameof(JournalEmptyBody));
    public static string JournalEmptyPrivacy => R(nameof(JournalEmptyPrivacy));
    public static string JournalFactorsShow => R(nameof(JournalFactorsShow));
    public static string JournalWeekSummary(int checkIns, string average) => F(nameof(JournalWeekSummary), checkIns, average);
    public static string JournalSlotEvening => R(nameof(JournalSlotEvening));
    public static string BalloonsFieldDescription => R(nameof(BalloonsFieldDescription));
    public static string BalloonsPopOne => R(nameof(BalloonsPopOne));
    public static string BalloonsStart => R(nameof(BalloonsStart));
    public static string BalloonsIntro => R(nameof(BalloonsIntro));
    public static string BalloonsAgain => R(nameof(BalloonsAgain));
    public static string BalloonsCount(int popped, int total) => F(nameof(BalloonsCount), popped, total);
    public static string BalloonsDone(int popped, int total) => F(nameof(BalloonsDone), popped, total);
    public static string BreathTopUp => R(nameof(BreathTopUp));
    public static string MuscleIntro => R(nameof(MuscleIntro));
    public static string MuscleStart => R(nameof(MuscleStart));
    public static string MuscleStop => R(nameof(MuscleStop));
    public static string MuscleAgain => R(nameof(MuscleAgain));
    public static string MuscleTense => R(nameof(MuscleTense));
    public static string MuscleRelease => R(nameof(MuscleRelease));
    public static string MuscleReleaseHint => R(nameof(MuscleReleaseHint));
    public static string MuscleGroupHands => R(nameof(MuscleGroupHands));
    public static string MuscleGroupShoulders => R(nameof(MuscleGroupShoulders));
    public static string MuscleGroupFace => R(nameof(MuscleGroupFace));
    public static string MuscleGroupBelly => R(nameof(MuscleGroupBelly));
    public static string MuscleGroupLegs => R(nameof(MuscleGroupLegs));
    public static string MuscleStep(int group, int total) => F(nameof(MuscleStep), group, total);
    public static string MuscleDone => R(nameof(MuscleDone));
    public static string BusyMindIntro => R(nameof(BusyMindIntro));
    public static string BusyMindStart => R(nameof(BusyMindStart));
    public static string BusyMindNext => R(nameof(BusyMindNext));
    public static string BusyMindSkip => R(nameof(BusyMindSkip));
    public static string BusyMindStep(int step, int total) => F(nameof(BusyMindStep), step, total);
    public static string BusyMindDone => R(nameof(BusyMindDone));
    public static string BusyMindAgain => R(nameof(BusyMindAgain));

    /// <summary>Task <paramref name="number"/> (1..<see cref="PsychologyApp.Presentation.Core.Practice.BusyMindRound.TaskCount"/>) of "keep the mind busy".</summary>
    public static string BusyMindTask(int number) => R($"BusyMindTask{number:00}");
    public static string JournalExportLabel => R(nameof(JournalExportLabel));
    public static string JournalExportTitle => R(nameof(JournalExportTitle));
    public static string JournalExportEmpty => R(nameof(JournalExportEmpty));
    public static string JournalExportFailed => R(nameof(JournalExportFailed));
    public static string JournalSlotFullHint => R(nameof(JournalSlotFullHint));
    public static string MoodNotesEmpty => R(nameof(MoodNotesEmpty));
    public static string JournalNeedMoodToSave => R(nameof(JournalNeedMoodToSave));
    public static string WeekRangeLabel(DateOnly start, DateOnly end) =>
        $"{start:dd MMM} – {end:dd MMM}";
    public static string WeekPracticesLabel => R(nameof(WeekPracticesLabel));
    public static string WeekMoodCheckInsLabel => R(nameof(WeekMoodCheckInsLabel));
    public static string WeekAvgMoodLabel => R(nameof(WeekAvgMoodLabel));
    public static string WeekRiskLabel => R(nameof(WeekRiskLabel));
    public static string WeekStreakLabel => R(nameof(WeekStreakLabel));
    public static string MetricEmptyValue => "—";
    public static string FormatAverageMood(double average) =>
        average <= 0 ? MetricEmptyValue : average.ToString("0.0");
    /// <summary>The level in words, for screen readers: a drawn face says nothing to TalkBack.</summary>
    public static string MoodLevelName(int level) => R($"MoodLevelName{Math.Clamp(level, 1, 5)}");
    public static string MoodLevelPill(int level, int max = 5) =>
        $"{MoodEmoji(level)} {level}/{max}";
    public static string ProfileMoodTrendHint => R(nameof(ProfileMoodTrendHint));

    public static string ChartFirstMeasurement => R(nameof(ChartFirstMeasurement));
    public static string ChartSparseHint(int count) => F(nameof(ChartSparseHint), count);
    public static string ResolveChartSubtitle(int pointCount) =>
        pointCount switch
        {
            1 => ChartFirstMeasurement,
            >= 2 and <= 4 => ChartSparseHint(pointCount),
            _ => string.Empty
        };
    public static string ChartDateLabel(DateTime date) =>
        date.ToString("dd MMM", System.Globalization.CultureInfo.CurrentCulture);
    public static string PracticeReflectionQuestion => R(nameof(PracticeReflectionQuestion));
    public static string PracticeReflectionNotePlaceholder => R(nameof(PracticeReflectionNotePlaceholder));
    public static string PracticePreSudsLabel => R(nameof(PracticePreSudsLabel));
    public static string PracticePostSudsLabel => R(nameof(PracticePostSudsLabel));
    public static string PracticeSudsDelta(int before, int after) => $"{before} → {after}";
    public static string PracticeSudsSectionTitle => R(nameof(PracticeSudsSectionTitle));
    public static string PracticeReflectionSectionTitle => R(nameof(PracticeReflectionSectionTitle));
    public static string PracticeLastNoteTitle => R(nameof(PracticeLastNoteTitle));
    public static string PracticeCompletedTitle => R(nameof(PracticeCompletedTitle));
    public static string PracticeCrisis => R(nameof(PracticeCrisis));
    public static string PracticeCompletedBody => R(nameof(PracticeCompletedBody));
    public static bool IsLifetimeMilestone(long total) =>
        total is 10 or 25 or 50 or 100 or 250 or 500 or 1000;
    public static string PracticeLifetimeMilestoneTitle(long total) => total switch
    {
        10 => R("PracticeLifetimeMilestoneTitle.10"),
        25 => R("PracticeLifetimeMilestoneTitle.25"),
        50 => R("PracticeLifetimeMilestoneTitle.50"),
        100 => R("PracticeLifetimeMilestoneTitle.100"),
        250 => R("PracticeLifetimeMilestoneTitle.250"),
        500 => R("PracticeLifetimeMilestoneTitle.500"),
        1000 => R("PracticeLifetimeMilestoneTitle.1000"),
        _ => PracticeCompletedTitle
    };
    public static string PracticeLifetimeMilestoneBody(long total) => total switch
    {
        10 => R("PracticeLifetimeMilestoneBody.10"),
        25 => R("PracticeLifetimeMilestoneBody.25"),
        50 => R("PracticeLifetimeMilestoneBody.50"),
        100 => R("PracticeLifetimeMilestoneBody.100"),
        250 => R("PracticeLifetimeMilestoneBody.250"),
        500 => R("PracticeLifetimeMilestoneBody.500"),
        1000 => R("PracticeLifetimeMilestoneBody.1000"),
        _ => PracticeCompletedBody
    };
    public static string PracticeMoodDelta(int before, int after) =>
        F("PracticeMoodDelta.1", MoodEmoji(before), before, MoodEmoji(after), after);
    public static string ProfileMoodNotesTitle => R(nameof(ProfileMoodNotesTitle));
    public static string PracticeGoHomeButton => R(nameof(PracticeGoHomeButton));
    public static string PracticeMoreButton => R(nameof(PracticeMoreButton));
    public static string PracticeNextCaption => R(nameof(PracticeNextCaption));
    public static string PracticeNextReason => R(nameof(PracticeNextReason));
    public static string PracticeHistoryTitle => R(nameof(PracticeHistoryTitle));
    public static string PracticeHistoryEmpty => R(nameof(PracticeHistoryEmpty));
    public static string PracticeHistoryEntry(string date, string name) => F(nameof(PracticeHistoryEntry), date, name);
    public static string InfoAppVersion(string version) => F(nameof(InfoAppVersion), version);
    public static string QuoteCopied => R(nameof(QuoteCopied));
    public static string TestHistoryTitle => R(nameof(TestHistoryTitle));
    public static string TestHistoryEmpty => R(nameof(TestHistoryEmpty));
    public static string TestHistoryEntry(string date, string summary) => F(nameof(TestHistoryEntry), date, summary);
    public static string TestOpenHistory => R(nameof(TestOpenHistory));

    private static string MoodEmoji(int level) => level switch
    {
        1 => "😞",
        2 => "😕",
        3 => "😐",
        4 => "🙂",
        5 => "😊",
        _ => "😐"
    };

    public static string MoodEmojiFor(int level) => MoodEmoji(level);
    public static string TechniqueContinueBadge => R(nameof(TechniqueContinueBadge));
    public static string TechniqueLastPractice(string date) => F(nameof(TechniqueLastPractice), date);
    public static string TechniqueNotTriedYet => R(nameof(TechniqueNotTriedYet));
    /// <summary>The button beside the quote search that shows the theme chips.</summary>
    public static string QuotesThemesToggle => R(nameof(QuotesThemesToggle));
    public static string TechniqueDuration(int minutes) => F(nameof(TechniqueDuration), minutes);
    public static string TechniqueMetaLine(string duration, string theme) => F(nameof(TechniqueMetaLine), duration, theme);

    public static string RatingScaleMin => R(nameof(RatingScaleMin));

    public static string RatingScaleMax => R(nameof(RatingScaleMax));
    public static string TechniqueRatingValue(int value) => F(nameof(TechniqueRatingValue), value);
    public static string TechniqueRatingNegValue(int value) => F(nameof(TechniqueRatingNegValue), value);

    public static string TestLastResult(string summary) => F(nameof(TestLastResult), summary);
    public static string TestLastResultDated(string summary, string date) => F(nameof(TestLastResultDated), date, summary);
    public static string TestCompletedAt(string date) => F(nameof(TestCompletedAt), date);
    public static string TestTryTechnique => R(nameof(TestTryTechnique));
    public static string TestResultImproved => R(nameof(TestResultImproved));
    public static string TestResultWorse => R(nameof(TestResultWorse));
    public static string TestResultSame => R(nameof(TestResultSame));

    public static string OnboardingAppName => R(nameof(OnboardingAppName));
    public static string OnboardingAppTagline => R(nameof(OnboardingAppTagline));
    public static string OnboardingWelcomeTitle => R(nameof(OnboardingWelcomeTitle));
    public static string OnboardingWelcomeBody => R(nameof(OnboardingWelcomeBody));
    public static string OnboardingValueOffline => R(nameof(OnboardingValueOffline));
    public static string OnboardingValueNoJudgment => R(nameof(OnboardingValueNoJudgment));
    public static string OnboardingValueOnDevice => R(nameof(OnboardingValueOnDevice));
    public static string OnboardingStepOf(int current, int total) => F(nameof(OnboardingStepOf), current, total);
    public static string OnboardingBack => R(nameof(OnboardingBack));
    public static string OnboardingOverviewTitle => R(nameof(OnboardingOverviewTitle));
    public static string OnboardingOverviewSubtitle => R(nameof(OnboardingOverviewSubtitle));
    public static string OnboardingOverviewLead => R(nameof(OnboardingOverviewLead));
    public static string OnboardingModulePracticeHint => R(nameof(OnboardingModulePracticeHint));
    public static string OnboardingModuleTestsHint => R(nameof(OnboardingModuleTestsHint));
    public static string OnboardingModuleChatHint => R(nameof(OnboardingModuleChatHint));
    public static string OnboardingModuleLibraryHint => R(nameof(OnboardingModuleLibraryHint));
    public static string OnboardingModuleJournalHint => R(nameof(OnboardingModuleJournalHint));
    public static string OnboardingConcernTitle => R(nameof(OnboardingConcernTitle));
    public static string OnboardingConcernSubtitle => R(nameof(OnboardingConcernSubtitle));
    public static string OnboardingConcernFooterHint => R(nameof(OnboardingConcernFooterHint));
    public static string OnboardingConcernAnxiety => R(nameof(OnboardingConcernAnxiety));
    public static string OnboardingConcernBody => R(nameof(OnboardingConcernBody));
    public static string OnboardingConcernMood => R(nameof(OnboardingConcernMood));
    public static string OnboardingConcernExplore => R(nameof(OnboardingConcernExplore));
    public static string OnboardingConcernAnxietyHint => R(nameof(OnboardingConcernAnxietyHint));
    public static string OnboardingConcernBodyHint => R(nameof(OnboardingConcernBodyHint));
    public static string OnboardingConcernMoodHint => R(nameof(OnboardingConcernMoodHint));
    public static string OnboardingConcernExploreHint => R(nameof(OnboardingConcernExploreHint));
    public static string OnboardingFinishTitle => R(nameof(OnboardingFinishTitle));
    public static string OnboardingFinishSubtitle(string practiceName) => F(nameof(OnboardingFinishSubtitle), practiceName);
    public static string OnboardingRecommendedCaption => R(nameof(OnboardingRecommendedCaption));
    public static string OnboardingDisclaimerTitle => R(nameof(OnboardingDisclaimerTitle));
    public static string OnboardingDisclaimerBody => R(nameof(OnboardingDisclaimerBody));
    public static string OnboardingStart => R(nameof(OnboardingStart));
    public static string OnboardingSkip => R(nameof(OnboardingSkip));
    public static string OnboardingNext => R(nameof(OnboardingNext));

    public static string QuoteShareFooter => R(nameof(QuoteShareFooter));

    public static string QuotesSearchError => R(nameof(QuotesSearchError));
    public static string LoadFailed => R(nameof(LoadFailed));
    public static string RetryQuestion => R(nameof(RetryQuestion));
    public static string LoadError => R(nameof(LoadError));

    public static string ProfileTitle => R(nameof(ProfileTitle));
    public static string ProfileLoadingText => R(nameof(ProfileLoadingText));
    public static string ProfileTechniquesCompleted => R(nameof(ProfileTechniquesCompleted));
    public static string ProfileFollowers => R(nameof(ProfileFollowers));
    public static string ProfileRecommended => R(nameof(ProfileRecommended));
    public static string ProfileBestQuotes => R(nameof(ProfileBestQuotes));
    public static string ProfileQuotesSeeAll => R(nameof(ProfileQuotesSeeAll));
    public static string ProfileQuotesSeeAllSubtitle => R(nameof(ProfileQuotesSeeAllSubtitle));
    public static string FormatProfileQuotesPreviewSubtitle(int shown, int total)
    {
        if (shown <= 0 || total <= 0)
        {
            return string.Empty;
        }

        if (total > shown)
        {
            return F("ProfileQuotesShowingSome", shown, total);
        }

        return total == 1 ? R("ProfileQuotesShowingOne") : F("ProfileQuotesShowingAll", total);
    }
    public static string QuotesFavoriteAdded => R(nameof(QuotesFavoriteAdded));
    public static string QuotesFavoriteRemoved => R(nameof(QuotesFavoriteRemoved));
    public static string QuotesGoToTab => R(nameof(QuotesGoToTab));
    public static string QuotesFeedAll => R(nameof(QuotesFeedAll));
    public static string QuotesFeedFavorites => R(nameof(QuotesFeedFavorites));
    public static string QuotesFeedForYou => R(nameof(QuotesFeedForYou));
    public static string QuotesThemeAll => R(nameof(QuotesThemeAll));
    public static string AnchorThoughtsTitle => R(nameof(AnchorThoughtsTitle));
    public static string AnchorThoughtsIntro => R(nameof(AnchorThoughtsIntro));
    public static string AnchorThoughtsReadMore(string articleTitle) => F(nameof(AnchorThoughtsReadMore), articleTitle);
    public static string AnchorThoughtsHelpNow => R(nameof(AnchorThoughtsHelpNow));
    public static string AnchorThemeAll => R(nameof(AnchorThemeAll));
    public static string AnchorThemeOthersWords => R(nameof(AnchorThemeOthersWords));
    public static string AnchorThemeMistakes => R(nameof(AnchorThemeMistakes));
    public static string AnchorThemeThoughtsAndFeelings => R(nameof(AnchorThemeThoughtsAndFeelings));
    public static string AnchorThemeRelationships => R(nameof(AnchorThemeRelationships));
    public static string AnchorThemeHardTimes => R(nameof(AnchorThemeHardTimes));
    public static string QuotesDailyTitle => R(nameof(QuotesDailyTitle));
    public static string QuotesSearchPlaceholder => R(nameof(QuotesSearchPlaceholder));
    public static string QuotesSearchEmptyTitle => R(nameof(QuotesSearchEmptyTitle));
    public static string QuotesSearchEmptyBody => R(nameof(QuotesSearchEmptyBody));
    public static string QuotesShowAgain => R(nameof(QuotesShowAgain));
    public static string QuotesForYouHint => R(nameof(QuotesForYouHint));
    public static string QuotesForYouEmptyTitle => R(nameof(QuotesForYouEmptyTitle));
    public static string QuotesForYouEmptyBody => R(nameof(QuotesForYouEmptyBody));
    public static string SettingsQuoteRemindersLabel => R(nameof(SettingsQuoteRemindersLabel));
    public static string SettingsQuoteReminderHourLabel => R(nameof(SettingsQuoteReminderHourLabel));
    public static string SettingsQuoteReminderHourPickerTitle => R(nameof(SettingsQuoteReminderHourPickerTitle));
    public static string SettingsMoodRemindersLabel => R(nameof(SettingsMoodRemindersLabel));
    public static string SettingsMoodReminderHourLabel => R(nameof(SettingsMoodReminderHourLabel));
    public static string SettingsMoodReminderHourPickerTitle => R(nameof(SettingsMoodReminderHourPickerTitle));
    public static string SettingsMoodReminderAuto => R(nameof(SettingsMoodReminderAuto));
    public static string SettingsChatRemindersLabel => R(nameof(SettingsChatRemindersLabel));
    public static string SettingsChatReminderHourLabel => R(nameof(SettingsChatReminderHourLabel));
    public static string SettingsChatReminderHourPickerTitle => R(nameof(SettingsChatReminderHourPickerTitle));
    public static string QuoteReminderTitle => R(nameof(QuoteReminderTitle));
    public static string QuoteReminderBody => R(nameof(QuoteReminderBody));
    public static string QuoteReminderBodySnippet(string quoteText)
    {
        string trimmed = quoteText.Trim();
        if (trimmed.Length == 0)
        {
            return QuoteReminderBody;
        }

        const int maxLen = 100;
        if (trimmed.Length <= maxLen)
        {
            return trimmed;
        }

        return trimmed.Substring(0, maxLen - 1).TrimEnd() + "…";
    }
    public static string QuoteThemeWisdom => R(nameof(QuoteThemeWisdom));
    public static string QuoteThemeMotivation => R(nameof(QuoteThemeMotivation));
    public static string QuoteThemeResilience => R(nameof(QuoteThemeResilience));
    public static string QuoteThemeSelfAwareness => R(nameof(QuoteThemeSelfAwareness));
    public static string QuoteThemeMindfulness => R(nameof(QuoteThemeMindfulness));
    public static string QuoteThemeSelfEsteem => R(nameof(QuoteThemeSelfEsteem));
    public static string QuoteThemeHope => R(nameof(QuoteThemeHope));
    public static string QuoteThemeEmpathy => R(nameof(QuoteThemeEmpathy));
    public static string QuoteThemeHappiness => R(nameof(QuoteThemeHappiness));
    public static string QuoteThemeHabits => R(nameof(QuoteThemeHabits));
    public static string QuoteThemeLove => R(nameof(QuoteThemeLove));
    public static string QuoteThemeRelationships => R(nameof(QuoteThemeRelationships));
    public static string QuoteThemeResponsibility => R(nameof(QuoteThemeResponsibility));
    public static string QuoteThemePurpose => R(nameof(QuoteThemePurpose));
    public static string QuoteThemeGrowth => R(nameof(QuoteThemeGrowth));
    public static string QuoteThemeHealing => R(nameof(QuoteThemeHealing));
    public static string QuoteThemeSelfLove => R(nameof(QuoteThemeSelfLove));
    public static string QuoteThemeAcceptance => R(nameof(QuoteThemeAcceptance));
    public static string QuoteThemeGratitude => R(nameof(QuoteThemeGratitude));
    public static string QuoteThemeCalm => R(nameof(QuoteThemeCalm));
    public static string QuoteThemeAnxiety => R(nameof(QuoteThemeAnxiety));
    public static string QuoteThemeGeneral => R(nameof(QuoteThemeGeneral));
    public static string QuotesAllReadTitle => R(nameof(QuotesAllReadTitle));
    public static string QuotesAllReadBody => R(nameof(QuotesAllReadBody));
    public static string QuotesShowFavorites => R(nameof(QuotesShowFavorites));
    public static string ProfileBsffSubtitle => R(nameof(ProfileBsffSubtitle));

    public static string MotivatorTitle => ShellTabMotivatorShort;
    public static string QuotesSearching => R(nameof(QuotesSearching));
    public static string QuotesLoading => R(nameof(QuotesLoading));
    public static string QuoteShareTitle => R(nameof(QuoteShareTitle));
    public static string UnknownAuthor => R(nameof(UnknownAuthor));

    public static string JournalCrisisPromptTitle => R(nameof(JournalCrisisPromptTitle));
    public static string JournalCrisisPromptBody => R(nameof(JournalCrisisPromptBody));
    public static string JournalCrisisPromptAccept => R(nameof(JournalCrisisPromptAccept));
    public static string JournalCrisisPromptDecline => R(nameof(JournalCrisisPromptDecline));
    public static string CrisisHubTitle => R(nameof(CrisisHubTitle));
    public static string CrisisHubLead => R(nameof(CrisisHubLead));
    public static string CrisisHubSafetyPlanTitle => R(nameof(CrisisHubSafetyPlanTitle));
    public static string CrisisHubSafetyPlanStep1 => R(nameof(CrisisHubSafetyPlanStep1));
    public static string CrisisHubSafetyPlanStep2 => R(nameof(CrisisHubSafetyPlanStep2));
    public static string CrisisHubSafetyPlanStep3(string emergencyNumber) => F(nameof(CrisisHubSafetyPlanStep3), emergencyNumber);
    public static string CrisisHubSafetyPlanStepNumber1 => "1";
    public static string CrisisHubSafetyPlanStepNumber2 => "2";
    public static string CrisisHubSafetyPlanStepNumber3 => "3";
    public static string CrisisHubHotlineRuName => R(nameof(CrisisHubHotlineRuName));
    public static string CrisisHubCallHotline(string name, string number) => F(nameof(CrisisHubCallHotline), name, number);
    public static string CrisisHubCallEmergency(string number) => F(nameof(CrisisHubCallEmergency), number);
    public static string CrisisHubOpenHelpline => R(nameof(CrisisHubOpenHelpline));
    public static string CrisisHubRecheck => R(nameof(CrisisHubRecheck));
    public static string CrisisHubContinueSoft => R(nameof(CrisisHubContinueSoft));
    public static string CrisisHubSpecialistHint => R(nameof(CrisisHubSpecialistHint));
    public static string DataBackupTitle => R(nameof(DataBackupTitle));
    public static string DataBackupLead => R(nameof(DataBackupLead));
    public static string DataBackupExportTitle => R(nameof(DataBackupExportTitle));
    public static string DataBackupExportSubtitle => R(nameof(DataBackupExportSubtitle));
    public static string DataBackupImportTitle => R(nameof(DataBackupImportTitle));
    public static string DataBackupImportSubtitle => R(nameof(DataBackupImportSubtitle));
    public static string DataBackupSummaryTitle => R(nameof(DataBackupSummaryTitle));
    public static string DataBackupSummarySubtitle => R(nameof(DataBackupSummarySubtitle));
    public static string DataEraseTitle => R(nameof(DataEraseTitle));
    public static string DataEraseSubtitle => R(nameof(DataEraseSubtitle));
    public static string DataEraseConfirmTitle => R(nameof(DataEraseConfirmTitle));
    public static string DataEraseConfirmBody => R(nameof(DataEraseConfirmBody));
    public static string DataEraseConfirmAccept => R(nameof(DataEraseConfirmAccept));
    public static string DataEraseFinalTitle => R(nameof(DataEraseFinalTitle));
    public static string DataEraseFinalBody => R(nameof(DataEraseFinalBody));
    public static string DataEraseFailedToast => R(nameof(DataEraseFailedToast));
    public static string AppLockLabel => R(nameof(AppLockLabel));
    public static string AppLockHint => R(nameof(AppLockHint));
    public static string AppLockNoScreenLock => R(nameof(AppLockNoScreenLock));
    public static string AppLockTitle => R(nameof(AppLockTitle));
    public static string AppLockLead => R(nameof(AppLockLead));
    public static string AppLockUnlock => R(nameof(AppLockUnlock));
    public static string AppLockHelpTitle => R(nameof(AppLockHelpTitle));
    public static string AppLockPromptTitle => R(nameof(AppLockPromptTitle));
    public static string AppLockPromptSubtitle => R(nameof(AppLockPromptSubtitle));
    public static string AppLockEnableTitle => R(nameof(AppLockEnableTitle));
    public static string DataBackupExportedToast => R(nameof(DataBackupExportedToast));
    public static string DataBackupImportedToast(int moods, int tests, int completions, int chats, int skipped = 0) => skipped > 0
        ? F("DataBackupImportedToast.1", moods, tests, completions, chats, skipped)
        : F("DataBackupImportedToast.2", moods, tests, completions, chats);
    public static string ErrorLogTitle => R(nameof(ErrorLogTitle));
    public static string ErrorLogSubtitle => R(nameof(ErrorLogSubtitle));
    public static string ErrorLogEmptyToast => R(nameof(ErrorLogEmptyToast));
    public static string DataBackupImportRolledBackToast => R(nameof(DataBackupImportRolledBackToast));
    public static string BackupProtectTitle => R(nameof(BackupProtectTitle));
    public static string BackupProtectBody => R(nameof(BackupProtectBody));
    public static string BackupProtectWith => R(nameof(BackupProtectWith));
    public static string BackupProtectWithout => R(nameof(BackupProtectWithout));
    public static string BackupPassphraseTitle => R(nameof(BackupPassphraseTitle));
    public static string BackupPassphraseNewBody => R(nameof(BackupPassphraseNewBody));
    public static string BackupPassphraseConfirmBody => R(nameof(BackupPassphraseConfirmBody));
    public static string BackupPassphraseOpenBody => R(nameof(BackupPassphraseOpenBody));
    public static string BackupPassphrasePlaceholder => R(nameof(BackupPassphrasePlaceholder));
    public static string BackupPassphraseTooShortToast => R(nameof(BackupPassphraseTooShortToast));
    public static string BackupPassphraseMismatchToast => R(nameof(BackupPassphraseMismatchToast));
    public static string BackupPassphraseWrongToast => R(nameof(BackupPassphraseWrongToast));
    public static string BreathInhale => R(nameof(BreathInhale));
    public static string BreathHold => R(nameof(BreathHold));
    public static string BreathExhale => R(nameof(BreathExhale));
    public static string BreathStart => R(nameof(BreathStart));
    public static string BreathFullscreen => R(nameof(BreathFullscreen));
    public static string ReliefDescription(int before, int after) => F(nameof(ReliefDescription), before, after);
    public static string BreathStop => R(nameof(BreathStop));
    public static string BreathAgain => R(nameof(BreathAgain));
    public static string BreathCycleFormat => R(nameof(BreathCycleFormat));
    public static string BreathReady => R(nameof(BreathReady));
    public static string BreathDone => R(nameof(BreathDone));
    public static string BreathCircleLabel => R(nameof(BreathCircleLabel));
    public static string TensionPickHint => R(nameof(TensionPickHint));
    public static string TensionPickConfirm(int value) => F(nameof(TensionPickConfirm), value);
    public static string TensionChangeFormat => R(nameof(TensionChangeFormat));
    public static string TensionCalmWord => R(nameof(TensionCalmWord));
    public static string TensionStrongWord => R(nameof(TensionStrongWord));
    public static string ChartPracticeLegend => R(nameof(ChartPracticeLegend));
    public static string SkeletonLoadingLabel => R(nameof(SkeletonLoadingLabel));
    public static string PracticeFilterBody => R(nameof(PracticeFilterBody));
    public static string PracticeFilterMind => R(nameof(PracticeFilterMind));
    public static string PracticeFilterHeart => R(nameof(PracticeFilterHeart));
    public static string PracticeFilterAction => R(nameof(PracticeFilterAction));
    public static string DataBackupImportFailedToast => R(nameof(DataBackupImportFailedToast));
    public static string CrisisHubSafetyPlanLinkTitle => R(nameof(CrisisHubSafetyPlanLinkTitle));
    public static string CrisisHubSafetyPlanLinkSubtitle => R(nameof(CrisisHubSafetyPlanLinkSubtitle));

    public static string SafetyPlanPageTitle => R(nameof(SafetyPlanPageTitle));
    public static string SafetyPlanLead => R(nameof(SafetyPlanLead));
    public static string SafetyPlanWarningSignsTitle => R(nameof(SafetyPlanWarningSignsTitle));
    public static string SafetyPlanWarningSignsPlaceholder => R(nameof(SafetyPlanWarningSignsPlaceholder));
    public static string SafetyPlanCopingTitle => R(nameof(SafetyPlanCopingTitle));
    public static string SafetyPlanCopingPlaceholder => R(nameof(SafetyPlanCopingPlaceholder));
    public static string SafetyPlanReasonsTitle => R(nameof(SafetyPlanReasonsTitle));
    public static string SafetyPlanReasonsPlaceholder => R(nameof(SafetyPlanReasonsPlaceholder));
    public static string SafetyPlanContactsTitle => R(nameof(SafetyPlanContactsTitle));
    public static string SafetyPlanContactNamePlaceholder => R(nameof(SafetyPlanContactNamePlaceholder));
    public static string SafetyPlanContactPhonePlaceholder => R(nameof(SafetyPlanContactPhonePlaceholder));
    public static string SafetyPlanCallAction => R(nameof(SafetyPlanCallAction));

    public static string RiskCheckTitle => R(nameof(RiskCheckTitle));
    public static string RiskCheckLead => R(nameof(RiskCheckLead));
    public static string RiskCheckSubtitle => R(nameof(RiskCheckSubtitle));
    public static string RiskCheckSelfHarm => R(nameof(RiskCheckSelfHarm));
    public static string RiskCheckDisorientation => R(nameof(RiskCheckDisorientation));
    public static string RiskCheckSubstance => R(nameof(RiskCheckSubstance));
    public static string RiskCheckInsomnia => R(nameof(RiskCheckInsomnia));
    public static string RiskCheckSubmit => R(nameof(RiskCheckSubmit));
    public static string RiskCheckAnswerAll => R(nameof(RiskCheckAnswerAll));
    public static string RiskCheckOpenHelpNow => R(nameof(RiskCheckOpenHelpNow));
    public static string RiskCheckYes => R(nameof(RiskCheckYes));
    public static string RiskCheckNo => R(nameof(RiskCheckNo));
    public static string RiskCheckSourcePeriodic => "periodic";
    public static string RiskCheckSourceManual => "manual";
    public static string RiskCheckSourceProfile => "profile";

    public static string OptionsCrisisTitle => R(nameof(OptionsCrisisTitle));
    public static string OptionsCrisisSubtitle => R(nameof(OptionsCrisisSubtitle));

    public static string PracticeHistorySeeAll => R(nameof(PracticeHistorySeeAll));
    public static string PracticeHistoryPageTitle => R(nameof(PracticeHistoryPageTitle));
    public static string ProfileRiskCheckLabel => R(nameof(ProfileRiskCheckLabel));
    public static string ProfileRiskCheckSubtitle => R(nameof(ProfileRiskCheckSubtitle));
    public static string ProfileMoodTrendPreview(string avgMood, string risk) => F(nameof(ProfileMoodTrendPreview), avgMood, risk);

    public static string JournalOverviewTitle => R(nameof(JournalOverviewTitle));
    public static string JournalTimelineTitle => R(nameof(JournalTimelineTitle));
    /// <summary>Under a morning or evening without a check-in yet: a tap on the row opens it.</summary>
    public static string JournalSlotAdd => R(nameof(JournalSlotAdd));
    public static string JournalEarlierWeeks => R(nameof(JournalEarlierWeeks));
    public static string JournalOpenOverview => R(nameof(JournalOpenOverview));
    public static string JournalOpenTimeline => R(nameof(JournalOpenTimeline));
    public static string JournalRecentDaysTitle => R(nameof(JournalRecentDaysTitle));
    public static string JournalPromptHelpedShort => R(nameof(JournalPromptHelpedShort));
    public static string JournalPromptNextShort => R(nameof(JournalPromptNextShort));
    public static string JournalShareLabel => R(nameof(JournalShareLabel));
    public static string JournalShareTitle => R(nameof(JournalShareTitle));
    public static string JournalShareText(string day, string mood, string note) =>
        string.IsNullOrWhiteSpace(note)
            ? F("JournalShareText.1", day, mood)
            : F("JournalShareText.2", day, mood, note);
    public static string JournalShareEntryWithFactors(string day, int moodLevel, string? note, IReadOnlyList<string> factorLabels)
    {
        string emoji = MoodEmojiFor(moodLevel);
        string mood = FormatAverageMood(moodLevel);
        string header = $"{day}: {emoji} {mood}";
        List<string> parts = [header];
        if (factorLabels.Count > 0)
        {
            parts.Add(JournalFactorsSummaryLine(string.Join(", ", factorLabels)));
        }

        string body = StripFactorLines(note);
        if (!string.IsNullOrWhiteSpace(body))
        {
            parts.Add(body);
        }

        return string.Join('\n', parts);
    }

    private static string StripFactorLines(string? note)
    {
        if (string.IsNullOrWhiteSpace(note))
        {
            return string.Empty;
        }

        string[] prefixes =
        [
            JournalFactorSleep,
            JournalFactorPeople,
            JournalFactorPractice,
            JournalFactorWalk,
            JournalFactorWork,
            JournalFactorSport,
            JournalFactorRest,
            JournalFactorStress,
            JournalFactorHome
        ];
        return string.Join(
            Environment.NewLine,
            note.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(line => !prefixes.Any(prefix =>
                    line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))));
    }
    public static string JournalReminderToggle => R(nameof(JournalReminderToggle));
    public static string JournalReminderHint => R(nameof(JournalReminderHint));
    public static string MoodReminderTitle => R(nameof(MoodReminderTitle));
    public static string MoodReminderBody => R(nameof(MoodReminderBody));


    public static string LuscherCoStable => R(nameof(LuscherCoStable));
    public static string LuscherCoMildTension => R(nameof(LuscherCoMildTension));
    public static string LuscherCoModerateTension => R(nameof(LuscherCoModerateTension));
    public static string LuscherCoElevatedTension => R(nameof(LuscherCoElevatedTension));
    public static string LuscherCoHighTension => R(nameof(LuscherCoHighTension));

    public static string LuscherBkExhausted => R(nameof(LuscherBkExhausted));
    public static string LuscherBkConserving => R(nameof(LuscherBkConserving));
    public static string LuscherBkOptimal => R(nameof(LuscherBkOptimal));
    public static string LuscherBkOveraroused => R(nameof(LuscherBkOveraroused));

    public static string TherapyProgramTitle => R(nameof(TherapyProgramTitle));
    public static string TherapyProgramAnxiety => R(nameof(TherapyProgramAnxiety));
    public static string TherapyProgramMood => R(nameof(TherapyProgramMood));
    public static string TherapyProgramStress => R(nameof(TherapyProgramStress));
    public static string TherapyProgramWeekLabel(int week) => F(nameof(TherapyProgramWeekLabel), week);
    public static string TherapyProgramWeekGoal(int week) => week switch
    {
        1 => R("TherapyProgramWeekGoal.1"),
        2 => R("TherapyProgramWeekGoal.2"),
        3 => R("TherapyProgramWeekGoal.3"),
        _ => R("TherapyProgramWeekGoal.default")
    };
    public static string TherapyProgramBanner(string programName, int week, string goal, int? completed = null, int? target = null)
    {
        if (completed is null or < 0 || target is null or <= 0)
        {
            return F("TherapyProgramBanner.1", programName, week, goal);
        }

        return F("TherapyProgramBanner.2", programName, week, completed, target);
    }
    public static string ClinicalAmberBanner => R(nameof(ClinicalAmberBanner));
    public static string ClinicalRedBanner => R(nameof(ClinicalRedBanner));
    public static string ClinicalStatusUnavailableBanner => R(nameof(ClinicalStatusUnavailableBanner));

    /// <summary>One resource set per language: StringsRu.resx, StringsEn.resx, StringsFr.resx and so on.</summary>
    private static readonly Dictionary<string, System.Resources.ResourceManager> Texts = AppLanguages.All.ToDictionary(
        key => key,
        key => new System.Resources.ResourceManager(
            $"PsychologyApp.Presentation.Common.Strings{char.ToUpperInvariant(key[0])}{key[1..]}", typeof(AppStrings).Assembly),
        StringComparer.Ordinal);

    /// <summary>The text for the key in the current language; a text not translated yet falls back to English, then to Russian.</summary>
    private static string R(string key) => In(Language, key);

    /// <summary>The text for the key in the given language, falling back the same way. For labels that must be read in a language other than the current one (a stored setting shown or parsed back).</summary>
    public static string In(string language, string key) =>
        Text(AppLanguages.Normalize(language), key)
            ?? Text(AppLanguages.English, key)
            ?? Text(AppLanguages.Russian, key)
            ?? key;

    // A language whose resx is not in the assembly yet reads as having no text, not as an error.
    private static string? Text(string language, string key)
    {
        try
        {
            return Texts[language].GetString(key);
        }
        catch (System.Resources.MissingManifestResourceException)
        {
            return null;
        }
    }

    private static string F(string key, params object?[] args) =>
        string.Format(System.Globalization.CultureInfo.CurrentCulture, R(key), args);

    /// <summary>Whether the content for this interface language is the English one: true for every language but Russian (<see cref="AppLanguages.ContentLanguage"/>).</summary>
    public static bool IsEnglish(string language) =>
        AppLanguages.ContentLanguage(language) == AppLanguages.English;
}
