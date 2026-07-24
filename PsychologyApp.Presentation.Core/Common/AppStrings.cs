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


    public static string OptionsTitle => T("Параметры", "Options");
    public static string OptionsSettingsTitle => T("Настройки", "Settings");
    public static string OptionsSettingsSubtitle => T("Тема, цвет и параметры отображения", "Theme, color, and display options");
    public static string ProfileSettingsCardSubtitle => T("Тема, язык, шрифт", "Theme, language, and font");
    public static string OptionsAboutTitle => T("О приложении", "About");
    public static string OptionsAboutSubtitle => T("Информация о приложении", "App information");
    public static string OptionsFeedbackTitle => T("Обратная связь", "Feedback");
    public static string OptionsFeedbackSubtitle => T("Сообщить о проблеме или предложить идею", "Report a problem or suggest an idea");
    public static string OptionsDonateTitle => T("Поддержать проект", "Support the project");
    public static string OptionsDonateSubtitle => T("Помочь развитию приложения", "Help the app grow");
    public static string ProfileOptionsCardSubtitle => T(
        "Настройки, поддержка, Alice AI",
        "Settings, support, Alice AI");
    public static string OptionsAliceTitle => T("Alice AI", "Alice AI");
    public static string OptionsAliceSubtitle => T(
        "Голосовой собеседник от Яндекса",
        "Yandex voice assistant");
    public static string AliceDisclaimerHeader => T("Важно", "Important");
    public static string AliceDisclaimerBody => T(
        "Alice — внешний сервис Яндекса. Для работы нужен интернет. Ответы носят ознакомительный характер и не заменяют консультацию специалиста.",
        "Alice is an external Yandex service. An internet connection is required. Responses are informational only and do not replace professional care.");
    public static string AliceOpenInBrowser => T("Открыть в браузере", "Open in browser");
    public static string AliceOpenFailed => T(
        "Не удалось открыть Alice",
        "Failed to open Alice");
    public static string AliceLoadingText => T("Загрузка Alice…", "Loading Alice…");

    public static string SettingsTitle => T("Настройки", "Settings");
    public static string SettingsDesignSection => T("Дизайн", "Design");
    public static string SettingsFontSection => T("Шрифт", "Font");
    public static string SettingsLanguageLabel => T("Язык", "Language");
    public static string SettingsThemeLabel => T("Тема", "Theme");
    public static string SettingsColorLabel => T("Цвет", "Color");
    public static string SettingsFormLabel => T("Форма", "Shape");
    public static string SettingsSizeLabel => T("Размер", "Size");
    public static string SettingsBoldLabel => T("Жирный", "Bold");
    public static string SettingsTestsSection => T("Тесты", "Tests");
    public static string SettingsPracticeSection => T("Практика", "Practice");
    public static string SettingsPracticeRemindersLabel => T("Напоминания о практике", "Practice reminders");
    public static string SettingsQuotesSection => T("Цитаты", "Quotes");
    public static string SettingsJournalSection => T("Дневник", "Journal");
    public static string SettingsPracticeReminderHourLabel => T("Время напоминания", "Reminder time");
    public static string SettingsPracticeReminderHourPickerTitle => T("Время", "Time");
    public static string PracticeReminderTitle => T("Пора позаниматься", "Time to practice");
    public static string PracticeReminderBody => T(
        "Уделите несколько минут практике — это поддержит ваш прогресс.",
        "Take a few minutes to practice and keep your progress going.");
    public static string PracticeReminderTitleNamed(string techniqueName) =>
        T($"Пора: {techniqueName}", $"Time for {techniqueName}");
    public static string PracticeReminderBodyNamed(string techniqueName, string reason) =>
        string.IsNullOrWhiteSpace(reason)
            ? T(
                $"Сегодня: {techniqueName}. Уделите несколько минут практике.",
                $"Today: {techniqueName}. Take a few minutes to practice.")
            : T($"{reason} — {techniqueName}", $"{reason} — {techniqueName}");
    public static string SettingsPrimaryConcernLabel => T("Главный запрос", "Primary concern");
    public static string SettingsPrimaryConcernPickerTitle => T("Что вас беспокоит?", "What troubles you?");
    public static string SettingsPrimaryConcernSection => T("Персонализация", "Personalization");
    public static string SettingsQuestionnaireAutoAdvanceLabel => T(
        "Автопереход к следующему вопросу",
        "Auto-advance to the next question");
    public static string SettingsApplyButton => T("Применить", "Apply");
    public static string SettingsPickerOptions => T("Варианты", "Options");
    public static string SettingsPickerColors => T("Цвета", "Colors");
    public static string SettingsPickerShapes => T("Формы", "Shapes");
    public static string SettingsPickerSizes => T("Размеры", "Sizes");
    public static string SettingsPickerLanguages => T("Языки", "Languages");
    public static string SettingsAppliedTitle => T("Информация", "Information");
    public static string SettingsAppliedMessage => T("Настройки применены", "Settings applied");
    public static string SettingsFormHelper => T(
        "Скругление углов карточек и полей ввода",
        "Corner rounding for cards and input fields");
    public static string SettingsColorHelper => T(
        "Основной цвет кнопок и акцентов",
        "Primary color for buttons and accents");
    public static string SettingsReplayOnboarding => T(
        "Пройти знакомство снова",
        "Retake onboarding");

    public static string TechniqueTheory => T("Теория", "Theory");
    public static string TechniqueAlgorithm => T("Алгоритм", "Algorithm");
    public static string TechniqueFinish => T("Завершить", "Finish");
    public static string TechniqueTitle => T("Техника", "Technique");
    public static string Back => T("Назад", "Back");
    public static string Save => T("Сохранить", "Save");
    public static string Send => T("Отправить", "Send");
    public static string Edit => T("Изменить", "Edit");
    public static string Remove => T("Удалить", "Delete");
    public static string NameLabel => T("Название", "Name");
    public static string DescriptionLabel => T("Описание", "Description");
    public static string Saving => T("Сохранение…", "Saving…");
    public static string DesignerLoadError => T(
        "Не удалось загрузить технику",
        "Could not load technique");
    public static string DesignerSaveError => T(
        "Не удалось сохранить технику",
        "Could not save technique");
    public static string ThemeLabel => T("Тема", "Theme");
    public static string AuthorLabel => T("Автор", "Author");
    public static string MessageLabel => T("Сообщение", "Message");
    public static string FormLabel => T("Форма", "Form");
    public static string ActionsListLabel => T("Список действий", "Action list");

    public static string PracticeHomeTitle => T("Главная", "Home");
    public static string PracticeMyTechniques => T("Мои техники", "My techniques");
    public static string PracticeCatalog => T("Каталог практик", "Practice catalog");
    public static string PracticeCatalogHint => T(
        "Выберите технику или создайте свою",
        "Pick a technique or create your own");
    public static string PracticeCreate => T("Создать", "Create");
    public static string PracticeTechniquesList => T("Список техник", "Techniques list");
    public static string PracticeInitError => T("Ошибка при инициализации", "Initialization failed");
    public static string PracticeLoadMoreError => T(
        "Не удалось подгрузить техники",
        "Could not load more techniques");
    public static string PracticeLoadingText => T("Загрузка практик", "Loading practices");
    public static string PracticeLoadingMoreText => T("Загрузка…", "Loading…");
    public static string PracticeCustomTechniqueNumber(long id) =>
        T($"Своя техника №{id}", $"Custom technique #{id}");
    public static string PracticeDesignTitle => T("Создание техники", "Create technique");
    public static string PracticeConstructor => T("Конструктор", "Designer");
    public static string PracticeCustomTechnique => T("Своя техника", "Custom technique");
    public static string PracticeDeleteConfirm => T(
        "Вы уверены, что хотите удалить свою технику",
        "Are you sure you want to delete your technique?");

    public static string ReviewTitle => T("Обратная связь", "Feedback");
    public static string ReviewPage => T("Отзыв", "Review");
    public static string ReviewExplanationHeader => T("Как это работает", "How it works");
    public static string ReviewExplanation => T(
        "Вы можете сообщить о проблеме или предложить свои идеи о том, как сделать приложение ещё лучше. Служба поддержки получит ваше сообщение.",
        "You can report a problem or suggest ideas to improve the app. Support will receive your message.");
    public static string ReviewMessagePlaceholder => T(
        "Опишите проблему или идею…",
        "Describe the issue or idea…");
    public static string ReviewMessageRequired => T(
        "Введите сообщение перед отправкой",
        "Enter a message before sending");
    public static string ReviewSendSuccessTitle => T("Спасибо", "Thank you");
    public static string ReviewSendSuccessMessage => T(
        "Ваше сообщение отправлено",
        "Your message has been sent");
    public static string ReviewEmailSubject => T("Отзыв о приложении Psychology", "Psychology App feedback");
    public static string ReviewSmsRecipientMissing => T(
        "Получатель SMS не настроен",
        "SMS recipient is not configured");
    public static string ReviewSmsNotSupported => T(
        "Отправка СМС не поддерживается",
        "SMS is not supported on this device");
    public static string ReviewSmsFailed => T(
        "Не удалось открыть приложение для отправки СМС",
        "Failed to open the SMS app");
    public static string ReviewEmailNotSupported => T(
        "Отправка email не поддерживается",
        "Email is not supported on this device");
    public static string ReviewEmailFailed => T(
        "Не удалось открыть приложение для отправки email",
        "Failed to open the email app");
    public static string ReviewShareTitle => T("Отзыв о приложении", "App feedback");
    public static string ReviewShareFailed => T(
        "Не удалось открыть меню отправки",
        "Failed to open the share menu");

    public static string DonateTitle => T("Пожертвования", "Donations");
    public static string DonateMoreInfo => T("Подробнее", "More info");
    public static string DonateBody => T(
        "Наш проект существует исключительно на пожертвования. Мы не размещаем рекламу и не оказываем платных услуг. Пожертвования используются на работу программистов и дизайнеров.",
        "This project runs on donations only. We do not show ads or sell paid services. Donations support developers and designers.");
    public static string DonateButton => T("Пожертвовать", "Donate");
    public static string DonateOpenFailed => T(
        "Не удалось открыть страницу пожертвования",
        "Failed to open the donation page");

    public static string InfoAboutBody => T(
        "Приложение представляет собой список простых, но в то же время мощных техник, которые помогут вам справиться с такими проблемами, как стресс, страх, сомнения, навязчивые мысли, ограничивающие убеждения и деструктивные установки. Все методики являются общеизвестными и проверены временем. Некоторые взяты из НЛП или трудов Живорада Славинского. Эти инструменты позволят вам сэкономить много времени, сил и денег. Они не потребуют глубоких знаний в области психологии и программирования подсознания. Желаем успеха в проработках!",
        "This app is a collection of simple yet powerful techniques to help with stress, fear, doubt, intrusive thoughts, limiting beliefs, and destructive patterns. The methods are well known and time-tested. Some come from NLP or the work of Zivorad Slavinski. These tools can save you time, energy, and money. They do not require deep knowledge of psychology or subconscious reprogramming. We wish you success in your practice!");

    public static string CleanerPrayersPage => T("Молитвы", "Prayers");
    public static string CleanerPrayerCollection => T("Сборник молитв", "Prayer collection");
    public static string CleanerLoad => T("Загрузить", "Load");
    public static string CleanerSearchingPrayers => T("Поиск молитв", "Loading prayers");
    public static string CleanerPreparingAudio => T("Подготовка аудио…", "Preparing audio…");
    public static string CleanerPlaybackError => T(
        "Не удалось воспроизвести аудио. Проверьте подключение к интернету.",
        "Could not play audio. Check your internet connection.");
    public static string CleanerOfflineBadge => T("Доступно офлайн", "Available offline");
    public static string CleanerPlayNext => T("Далее", "Next");
    public static string CleanerReplay => T("Сначала", "Replay");
    public static string CleanerMoreInfoBody => T(
        "Аудиомолитвы для утра, вечера и тихой практики. Для первого прослушивания нужен интернет; затем трек можно слушать офлайн.",
        "Audio prayers for morning, evening, and quiet practice. Internet is required for the first listen; tracks can then be played offline.");
    public static string CleanerCollectionSubtitle => T(
        "Выберите молитву и нажмите на карточку, чтобы начать прослушивание.",
        "Choose a prayer and tap a card to start listening.");
    public static string CleanerCategoryAll => T("Все", "All");
    public static string CleanerCategoryMorning => T("Утренние", "Morning");
    public static string CleanerCategoryEvening => T("Вечерние", "Evening");
    public static string CleanerCategoryPenitential => T("Покаянные", "Penitential");
    public static string CleanerCategoryCore => T("Основные", "Core");
    public static string CleanerPrayerMain => T("Основная молитва", "Main prayer");
    public static string CleanerPsalm50 => T("Псалом 50", "Psalm 50");
    public static string CleanerPsalm50Desc => T(
        "Покаянный псалом; читают три раза в сутки",
        "A penitential psalm; traditionally read three times a day");
    public static string CleanerPsalm90 => T("Псалом 90", "Psalm 90");
    public static string CleanerPsalm90Desc => T(
        "Молитва о защите и помощи Божией",
        "A prayer for God's protection and help");
    public static string CleanerOurFather => T("Отче Наш", "Our Father");
    public static string CleanerOurFatherDesc => T(
        "Главная молитва христиан",
        "The central Christian prayer");
    public static string CleanerJesusPrayer => T("Иисусова молитва", "Jesus Prayer");
    public static string CleanerJesusPrayerDesc => T(
        "Краткая молитва сердечного обращения к Христу",
        "A short prayer of the heart to Christ");
    public static string CleanerHeavenlyKing => T("Царю небесный", "Heavenly King");
    public static string CleanerHeavenlyKingDesc => T(
        "Начало утреннего правила",
        "Opening prayer of the morning rule");
    public static string CleanerMorningPrayer => T("Утренние молитвы", "Morning prayers");
    public static string CleanerMorningPrayerDesc => T(
        "Краткий утренний цикл молитв",
        "A short morning prayer cycle");
    public static string CleanerSymbolOfFaith => T("Символ веры", "Symbol of Faith");
    public static string CleanerSymbolOfFaithDesc => T(
        "Краткое изложение православного вероучения",
        "A concise statement of Orthodox faith");
    public static string CleanerEveningPrayer => T("Вечерние молитвы", "Evening prayers");
    public static string CleanerEveningPrayerDesc => T(
        "Краткий вечерний цикл молитв",
        "A short evening prayer cycle");
    public static string CleanerTrisagion => T("Трисвятое", "Trisagion");
    public static string CleanerTrisagionDesc => T(
        "«Святый Боже, Святый Крепкий…»",
        "\"Holy God, Holy Mighty…\"");
    public static string CleanerVirginMary => T("Богородице Дево", "Hail, O Virgin");
    public static string CleanerVirginMaryDesc => T(
        "Молитва Пресвятой Богородице",
        "A prayer to the Most Holy Theotokos");
    public static string CleanerHolySpirit => T("Молитва Святому Духу", "Prayer to the Holy Spirit");
    public static string CleanerHolySpiritDesc => T(
        "Просьба о дарах и укреплении Духом",
        "A prayer for the gifts and strengthening of the Spirit");
    public static string CleanerDoxology => T("Славословие", "Doxology");
    public static string CleanerDoxologyDesc => T(
        "Великое славословие Богу",
        "The great doxology to God");
    public static string CleanerSearchPlaceholder => T("Поиск молитвы", "Search prayers");
    public static string CleanerNoPrayersFound => T("Ничего не найдено", "No prayers found");
    public static string CleanerCatalogEmpty => T(
        "Пока нет молитв в каталоге",
        "No prayers in the catalog yet");
    public static string CleanerNowPlaying => T("Сейчас играет", "Now playing");

    public static string DesignerNamePlaceholder => T("Крутилка Славинского", "Slavinski spin technique");
    public static string DesignerDescriptionPlaceholder => T(
        "Метод мгновенной нейтрализации...",
        "Instant neutralization method...");
    public static string DesignerThemePlaceholder => T("Эпизоды", "Episodes");
    public static string DesignerAuthorPlaceholder => T("Живорад Славинский", "Zivorad Slavinski");

    public static string Add => T("Добавить", "Add");
    public static string Repeat => T("Повторить", "Repeat");
    public static string Cancel => T("Отмена", "Cancel");
    public static string ConcernLabel => T("Беспокойство", "Concern");
    public static string FirstPolarityLabel => T("Первая полярность", "First polarity");
    public static string SecondPolarityLabel => T("Вторая полярность", "Second polarity");
    public static string PoleNumber(int number) => T($"Полюс №{number}", $"Pole #{number}");
    public static string RecordNumber(int number) => T($"Запись №{number}", $"Entry #{number}");
    public static string ProverbLabel => T("Пословица", "Proverb");
    public static string QuoteAddFavoriteHint => T("Добавить в избранное", "Add to favorites");
    public static string QuoteCopyHint => T("Копировать цитату", "Copy quote");
    public static string QuoteShareHint => T("Поделиться цитатой", "Share quote");
    public static string PolarityNegativePlaceholder => T("Невроз", "Neurosis");
    public static string PolarityPositivePlaceholder => T("Покой", "Calm");

    public static string StartupErrorTitle => T("Ошибка запуска", "Startup error");
    public static string StartupErrorMessage => T(
        "Не удалось инициализировать приложение. Перезапустите приложение.",
        "Failed to initialize the app. Please restart.");
    public static string ErrorTitle => T("Ошибка", "Error");
    public static string UnexpectedErrorMessage => T(
        "Произошла непредвиденная ошибка. Попробуйте ещё раз.",
        "An unexpected error occurred. Please try again.");
    public static string TestsResultSaveFailedMessage => T(
        "Не удалось сохранить результат теста. Попробуйте ещё раз.",
        "Failed to save the test result. Please try again.");
    public static string TestsResultNavigationFailedMessage => T(
        "Результат сохранён, но не удалось открыть экран результата. Нажмите «Завершить» ещё раз.",
        "The result was saved, but the result screen could not be opened. Tap Finish again.");
    public static string TechniqueNotFound => T("Техника не найдена.", "Technique not found.");
    public static string QuoteNotFound => T("Цитата не найдена.", "Quote not found.");

    public static string PracticeEmptyTitle => T("Пока нет техник", "No techniques yet");
    public static string PracticeEmptyBody => T(
        "Создайте свою первую технику в конструкторе",
        "Create your first technique in the designer");
    public static string TestsEmptyTitle => T("Тесты пока недоступны", "Tests are not available yet");
    public static string TestsEmptyBody => T(
        "Попробуйте обновить список или вернитесь позже",
        "Try refreshing the list or come back later");
    public static string TestsLoadingText => T("Загрузка тестов", "Loading tests");
    public static string QuotesEmptyTitle => T("Цитаты не найдены", "No quotes found");
    public static string QuotesEmptyBody => T(
        "Нажмите «Обновить», чтобы загрузить цитаты снова",
        "Tap Refresh to load quotes again");
    public static string QuotesRefreshButton => T("Обновить", "Refresh");
    public static string ProfileQuotesEmpty => T("Пока нет избранных цитат", "No favorite quotes yet");
    public static string QuotesFavoritesEmptyBody => T(
        "Нажмите сердечко на цитате, чтобы добавить её сюда.",
        "Tap the heart on a quote to add it here.");

    public static string PhysicsSolutionHeader => T("Что делать", "What to do");
    public static string PhysicsRecommendedPractices => T("Практики, которые могут помочь", "Practices that may help");
    public static string PhysicsTryPractice => T("Попробовать практику", "Try a practice");

    public static string ProfileTestsCompleted => T("Пройдено тестов", "Tests completed");
    public static string ProfileStreakDays => T("Дней подряд", "Day streak");
    public static string ProfileStreakHint => T(
        "Считаются дни с завершённой практикой",
        "Counts days with a completed practice");
    public static string ProfileStreakCount(int days) => T($"{days} дн.", $"{days} days");

    public static string TodayForYou => T("Сегодня для вас", "Today for you");
    public static string TodayRecommended => T("Рекомендуемая практика", "Recommended practice");
    public static string TodayStartPractice => T("Начать", "Start");
    public static string StreakAtRiskBanner(int days) => T(
        $"Сохраните серию из {days} дн. — позанимайтесь сегодня",
        $"Keep your {days}-day streak — practice today");
    public static string ComebackBanner => T(
        "С возвращением — начните с короткой практики",
        "Welcome back — start with a short practice");
    public static string ComebackBannerWithTechnique(string name) => T(
        $"С возвращением — продолжите «{name}»",
        $"Welcome back — continue {name}");
    public static string WeeklyInsightLine(int practiceCount, string moodTrend) =>
        string.IsNullOrEmpty(moodTrend)
            ? T($"На этой неделе: {practiceCount} {PracticeCountWord(practiceCount)}",
                $"This week: {practiceCount} {PracticeCountWordEn(practiceCount)}")
            : T($"На этой неделе: {practiceCount} {PracticeCountWord(practiceCount)} · настроение {moodTrend}",
                $"This week: {practiceCount} {PracticeCountWordEn(practiceCount)} · mood {moodTrend}");
    public static string WeeklyInsightMoodOnly(string moodTrend) => T(
        $"На этой неделе: настроение {moodTrend}",
        $"This week: mood {moodTrend}");
    public static string MoodTrendUp => "↑";
    public static string MoodTrendFlat => "→";
    public static string MoodTrendDown => "↓";
    private static string PracticeCountWord(int count) => count switch
    {
        1 => "практика",
        >= 2 and <= 4 => "практики",
        _ => "практик"
    };
    private static string PracticeCountWordEn(int count) => count == 1 ? "practice" : "practices";
    public static string OnboardingRemindersLabel => T(
        "Напоминать о практике",
        "Remind me to practice");
    public static string OnboardingReminderHourLabel => T("Время напоминания", "Reminder time");
    public static string TodayRecommendationReason(string concern) => concern switch
    {
        "anxiety" => T("Подходит при тревоге", "Good for anxiety"),
        "body" => T("Для работы с телом и симптомами", "For body and symptoms"),
        "mood" => T("Помогает при тяжёлом настроении", "Helps when mood is low"),
        _ => T("Практика дня", "Practice of the day")
    };

    public static string TodayRecommendationReasonFromTest(string testId) =>
        T($"После недавнего теста ({testId})", $"After a recent test ({testId})");

    public static string TodayRecommendationReasonLowMood() =>
        T("Когда настроение низкое", "When mood is low");
    public static string TodayRecommendationReasonContinueDraft() => T(
        "Продолжите с того места, где остановились",
        "Continue where you left off");
    public static string WeeklyInsightStreakPart(int days) =>
        T($"серия {days}", $"streak {days}");
    public static string WeeklyInsightTestImprovedPart() => T("тест ↑", "test ↑");
    public static string WeeklyInsightTestWorsePart() => T("тест ↓", "test ↓");
    public static string WeeklyInsightWithExtra(string baseLine, string extra) =>
        string.IsNullOrWhiteSpace(extra) ? baseLine : $"{baseLine} · {extra}";
    public static string TodayMoodQuestion => T("Как настроение?", "How are you feeling?");
    public static string TodayMoodSaved => T("Настроение сохранено", "Mood saved");
    public static string TodayMoodLine(int level, int max) =>
        T($"Сегодня: {MoodEmoji(level)} {level}/{max}", $"Today: {MoodEmoji(level)} {level}/{max}");
    public static string MoodHistoryTitle => T("Недавнее", "Recent");
    public static string MoodHistoryEntry(string date, int level, int max) =>
        T($"{date}: {MoodEmoji(level)} {level}/{max}", $"{date}: {MoodEmoji(level)} {level}/{max}");
    public static string ProfileMoodTrendTitle => T("Настроение", "Mood");
    public static string ProfileMoodCheckInTitle => T("Как настроение сегодня?", "How are you feeling today?");
    public static string ProfileWeeklyInsightTitle => T("На этой неделе", "This week");
    public static string JournalTitle => T("Дневник", "Journal");
    public static string OpenJournalLabel => T("Как настроение?", "How are you feeling?");
    public static string JournalCardSubtitle => T(
        "Настроение, заметки и динамика",
        "Mood, notes, and trends");
    public static string JournalTodayTitle => T("Сегодня", "Today");
    public static string JournalEntriesTitle => T("Записи", "Entries");
    public static string JournalWeekEmpty => T(
        "Пока мало данных за неделю — отметьте настроение",
        "Not much this week yet — log a mood check-in");
    public static string JournalNotePlaceholder => T(
        "Заметка к этому дню (необязательно)",
        "Note for this day (optional)");
    public static string JournalNoteSectionTitle => T("Заметка", "Note");
    public static string JournalNoteSaveHint => T(
        "Настроение сохраняется сразу. Заметку — кнопкой ниже.",
        "Mood saves instantly. Use the button below for the note.");
    public static string JournalSaveLabel => T("Сохранить заметку", "Save note");
    public static string JournalDeleteLabel => T("Удалить", "Delete");
    public static string JournalDeleteConfirmTitle => T("Удалить запись?", "Delete entry?");
    public static string JournalDeleteConfirmMessage => T(
        "Отметка настроения и заметка за этот день будут удалены.",
        "This day's mood check-in and note will be removed.");
    public static string JournalDeleteConfirmAccept => T("Удалить", "Delete");
    public static string JournalDeleteConfirmCancel => T("Отмена", "Cancel");
    public static string JournalNoNoteCaption => T("Без заметки", "No note");
    public static string JournalEditTodayHint => T(
        "Изменить сегодня",
        "Edit today");
    public static string JournalDayEmptyHint(DateOnly day) =>
        T($"Нет записи за {day:d} — можно добавить", $"No entry for {day:d} — you can add one");
    public static string JournalDayMoodLine(DateOnly day, int level, int max) =>
        T($"{day:d}: {MoodEmoji(level)} {level}/{max}", $"{day:d}: {MoodEmoji(level)} {level}/{max}");
    public static string JournalMoodStatsTitle => T("Обзор", "Overview");
    public static string JournalMoodStreakLabel => T("Серия дней", "Day streak");
    public static string JournalDynamicsTitle => T("Динамика", "Trend");
    public static string JournalOverviewInsightEmpty => T(
        "Пока мало отметок — отметьте настроение на этой неделе",
        "Not many check-ins yet — log your mood this week");
    public static string JournalOverviewInsightLine(
        int checkIns,
        string averageMood,
        string trend,
        string streak)
    {
        string baseLine = string.IsNullOrWhiteSpace(trend)
            ? T(
                $"{checkIns} {MoodCheckInWord(checkIns)} · ср. {averageMood}",
                $"{checkIns} {MoodCheckInWordEn(checkIns)} · avg {averageMood}")
            : T(
                $"{checkIns} {MoodCheckInWord(checkIns)} · ср. {averageMood} · настроение {trend}",
                $"{checkIns} {MoodCheckInWordEn(checkIns)} · avg {averageMood} · mood {trend}");

        if (string.IsNullOrWhiteSpace(streak) || streak == MetricEmptyValue)
        {
            return baseLine;
        }

        return T($"{baseLine} · серия {streak}", $"{baseLine} · streak {streak}");
    }

    public static string JournalWeekInsightLine(int checkIns, string trend, string streak)
    {
        if (checkIns <= 0)
        {
            return string.Empty;
        }

        List<string> parts = [];
        if (!string.IsNullOrWhiteSpace(trend))
        {
            parts.Add(T($"настроение {trend}", $"mood {trend}"));
        }

        if (!string.IsNullOrWhiteSpace(streak) && streak != MetricEmptyValue)
        {
            parts.Add(T($"серия {streak}", $"streak {streak}"));
        }

        if (parts.Count == 0)
        {
            return T(
                $"{checkIns} {MoodCheckInWord(checkIns)} за неделю",
                $"{checkIns} {MoodCheckInWordEn(checkIns)} this week");
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

    public static string JournalFilter7Days => T("7 дней", "7 days");
    public static string JournalFilter30Days => T("30 дней", "30 days");
    public static string JournalFilter90Days => T("90 дней", "90 days");
    public static string JournalYesterdayTitle => T("Вчера", "Yesterday");
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
    public static string JournalPastDayCheckInTitle => T(
        "Как было настроение в этот день?",
        "How was your mood that day?");
    public static string JournalPromptHelped => T("Что помогло?", "What helped?");
    public static string JournalPromptBlocked => T("Что мешало?", "What got in the way?");
    public static string JournalPromptGrateful => T("За что благодарен?", "Grateful for?");
    public static string JournalPromptNext => T("Что сделаю дальше?", "What next?");
    public static string JournalSearchPlaceholder => T("Поиск по заметкам", "Search notes");
    public static string JournalSearchEmpty => T(
        "Ничего не найдено",
        "No matching notes");
    public static string JournalBestWorstPill(int best, int worst) =>
        T($"Лучший {MoodEmoji(best)} {best} · Худший {MoodEmoji(worst)} {worst}",
            $"Best {MoodEmoji(best)} {best} · Worst {MoodEmoji(worst)} {worst}");
    public static string JournalTimelineEmpty => T(
        "Пока нет отметок настроения в этом периоде",
        "No mood check-ins in this period yet");
    public static string MoodNotesEmpty => T(
        "Пока нет отметок настроения — сделайте check-in выше",
        "No mood check-ins yet — log one above");
    public static string JournalNeedMoodToSave => T(
        "Сначала выберите настроение",
        "Choose a mood first");
    public static string WeekRangeLabel(DateOnly start, DateOnly end) =>
        T($"{start:dd MMM} – {end:dd MMM}", $"{start:dd MMM} – {end:dd MMM}");
    public static string WeekPracticesLabel => T("Практики", "Practices");
    public static string WeekMoodCheckInsLabel => T("Отметки", "Check-ins");
    public static string WeekAvgMoodLabel => T("Ср. настроение", "Avg mood");
    public static string WeekRiskLabel => T("Риск", "Risk");
    public static string WeekStreakLabel => T("Серия", "Streak");
    public static string MetricEmptyValue => "—";
    public static string FormatAverageMood(double average) =>
        average <= 0 ? MetricEmptyValue : average.ToString("0.0");
    public static string MoodLevelPill(int level, int max = 5) =>
        $"{MoodEmoji(level)} {level}/{max}";
    public static string ProfileMoodTrendHint => T(
        "Отмечайте настроение здесь, чтобы увидеть динамику",
        "Track mood here to see your trend");

    public static string ChartFirstMeasurement => T("Первое измерение", "First measurement");
    public static string ChartSparseHint(int count) => T(
        $"{count} измерения — тренд уточняется",
        $"{count} measurements — trend still forming");
    public static string ResolveChartSubtitle(int pointCount) =>
        pointCount switch
        {
            1 => ChartFirstMeasurement,
            >= 2 and <= 4 => ChartSparseHint(pointCount),
            _ => string.Empty
        };
    public static string ChartDateLabel(DateTime date) =>
        date.ToString("dd MMM", System.Globalization.CultureInfo.CurrentCulture);
    public static string PracticeReflectionQuestion => T("Как вы себя чувствуете?", "How do you feel now?");
    public static string PracticeReflectionNotePlaceholder => T("Короткая заметка (необязательно)", "Short note (optional)");
    public static string PracticePreSudsLabel => T("Интенсивность до (0–10)", "Intensity before (0–10)");
    public static string PracticePostSudsLabel => T("Интенсивность после (0–10)", "Intensity after (0–10)");
    public static string PracticeSudsDelta(int before, int after) => $"{before} → {after}";
    public static string PracticeSudsSectionTitle => T("Как изменилась интенсивность?", "How did intensity change?");
    public static string PracticeReflectionSectionTitle => T("Настроение", "Mood");
    public static string PracticeCompletedTitle => T("Готово!", "Done!");
    public static string PracticeCompletedBody(int streak) =>
        T($"Отличная работа! Серия: {streak} дн.", $"Great job! Streak: {streak} days");
    public static bool IsStreakMilestone(int streak) =>
        streak is 3 or 7 or 14 or 30;
    public static string PracticeMilestoneTitle(int streak) => streak switch
    {
        3 => T("3 дня подряд!", "3 days in a row!"),
        7 => T("Неделя подряд!", "A full week!"),
        14 => T("2 недели подряд!", "2 weeks in a row!"),
        30 => T("Месяц подряд!", "A full month!"),
        _ => PracticeCompletedTitle
    };
    public static string PracticeMilestoneBody(int streak) => streak switch
    {
        3 => T("Хорошее начало — так держать.", "A solid start — keep it going."),
        7 => T("7 дней подряд — отличный ритм.", "7 days in a row — great rhythm."),
        14 => T("14 дней — сильная привычка.", "14 days — a strong habit."),
        30 => T("30 дней — впечатляющая серия.", "30 days — an impressive streak."),
        _ => PracticeCompletedBody(streak)
    };
    public static string PracticeMoodDelta(int before, int after) =>
        T($"Было {MoodEmoji(before)} {before}/5 → стало {MoodEmoji(after)} {after}/5",
            $"Was {MoodEmoji(before)} {before}/5 → now {MoodEmoji(after)} {after}/5");
    public static string ProfileMoodNotesTitle => T("Заметки к настроению", "Mood notes");
    public static string PracticeGoHomeButton => T("На главную", "Go home");
    public static string PracticeMoreButton => T("Ещё практика", "More practice");
    public static string PracticeNextCaption => T("Следующая практика", "Next practice");
    public static string PracticeNextReason => T("Продолжим серию", "Keep the momentum");
    public static string PracticeHistoryTitle => T("Недавние практики", "Recent practices");
    public static string PracticeHistoryEmpty => T("Пока нет завершённых практик", "No completed practices yet");
    public static string PracticeHistoryEntry(string date, string name) =>
        T($"{date}: {name}", $"{date}: {name}");
    public static string InfoAppVersion(string version) =>
        T($"Версия {version}", $"Version {version}");
    public static string QuoteCopied => T("Скопировано", "Copied");
    public static string TestHistoryTitle => T("История результатов", "Result history");
    public static string TestHistoryEmpty => T("Пока нет сохранённых результатов", "No saved results yet");
    public static string TestHistoryEntry(string date, string summary) =>
        T($"{date}: {summary}", $"{date}: {summary}");
    public static string TestOpenHistory => T("История", "History");
    public static string ProfileLastPractice(string date) =>
        T($"Последняя практика: {date}", $"Last practice: {date}");
    public static string PhysicsNoResultsSubhint => T(
        "Попробуйте: плечо, шея, спина, живот, голова…",
        "Try: shoulder, neck, back, stomach, head…");

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
    public static string TechniqueContinueBadge => T("Продолжить", "Continue");
    public static string TechniqueLastPractice(string date) => T($"Последняя практика: {date}", $"Last practice: {date}");
    public static string TechniqueNotTriedYet => T("Не пробовали", "Not tried yet");
    public static string TechniqueDuration(int minutes) => T($"~{minutes} мин", $"~{minutes} min");
    public static string TechniqueMetaLine(string duration, string theme) => T($"{duration} · {theme}", $"{duration} · {theme}");
    public static string TechniqueRatingValue(int value) => T($"Оценка: {value} из 10", $"Rating: {value} of 10");
    public static string TechniqueRatingNegValue(int value) => T($"Оценка: {value} (от −10 до 10)", $"Rating: {value} (from −10 to 10)");

    public static string TestLastResult(string summary) => T($"Последний результат: {summary}", $"Last result: {summary}");
    public static string TestTryTechnique => T("Попробовать технику", "Try a technique");
    public static string TestResultImproved => T("Лучше прошлого раза", "Better than last time");
    public static string TestResultWorse => T("Хуже прошлого раза", "Worse than last time");
    public static string TestResultSame => T("Как в прошлый раз", "Same as last time");

    public static string OnboardingAppName => T("PsychologyApp", "PsychologyApp");
    public static string OnboardingAppTagline => T(
        "Спокойствие рядом — даже без интернета",
        "Calm within reach — even offline");
    public static string OnboardingWelcomeTitle => T("Пространство для себя", "A space for you");
    public static string OnboardingWelcomeBody => T(
        "Здесь можно выдохнуть, разобраться с собой и найти опору — в своём темпе.",
        "Here you can breathe, understand yourself, and find support — at your own pace.");
    public static string OnboardingValueOffline => T("Офлайн", "Offline");
    public static string OnboardingValueNoJudgment => T("Без осуждения", "No judgment");
    public static string OnboardingValueOnDevice => T("На устройстве", "On your device");
    public static string OnboardingStepOf(int current, int total) =>
        T($"{current} из {total}", $"{current} of {total}");
    public static string OnboardingBack => T("Назад", "Back");
    public static string OnboardingOverviewTitle => T("Пять опор в одном месте", "Five pillars in one place");
    public static string OnboardingOverviewSubtitle => T(
        "Практики, тесты, психосоматика, молитвы и цитаты — редкое сочетание в одном компаньоне",
        "Practices, tests, psychosomatic search, prayers, and quotes — a rare mix in one companion");
    public static string OnboardingOverviewLead => T(
        "Всё под рукой в нижней панели",
        "Everything is one tap away in the tab bar");
    public static string OnboardingModulePracticeHint => T(
        "Успокоить нервную систему за минуты",
        "Calm your nervous system in minutes");
    public static string OnboardingModuleTestsHint => T(
        "Узнать себя через опросники и Люшера",
        "Know yourself through questionnaires and Lüscher");
    public static string OnboardingModuleSomaticHint => T(
        "Понять связь тела и эмоций",
        "Understand the body–emotion link");
    public static string OnboardingModuleMusicHint => T(
        "Найти слова и звук для души",
        "Find words and sound for the soul");
    public static string OnboardingModuleQuotesHint => T(
        "Поддержать настроение одной мыслью",
        "Lift your mood with one thought");
    public static string OnboardingConcernTitle => T("Что вас беспокоит?", "What troubles you?");
    public static string OnboardingConcernSubtitle => T(
        "Подберём первую практику под ваш запрос",
        "We'll pick a first practice for your needs");
    public static string OnboardingConcernFooterHint => T(
        "Нажмите на вариант, чтобы продолжить",
        "Tap an option to continue");
    public static string OnboardingConcernAnxiety => T("Тревога", "Anxiety");
    public static string OnboardingConcernBody => T("Тело / симптомы", "Body / symptoms");
    public static string OnboardingConcernMood => T("Настроение", "Mood");
    public static string OnboardingConcernExplore => T("Просто попробовать", "Just exploring");
    public static string OnboardingConcernAnxietyHint => T(
        "Когда мысли не отпускают",
        "When thoughts won't let go");
    public static string OnboardingConcernBodyHint => T(
        "Когда тело сигналит о стрессе",
        "When your body signals stress");
    public static string OnboardingConcernMoodHint => T(
        "Когда тяжело внутри",
        "When it feels heavy inside");
    public static string OnboardingConcernExploreHint => T(
        "Хочу просто посмотреть",
        "I just want to look around");
    public static string OnboardingFinishTitle => T("Всё готово — начнём?", "All set — shall we begin?");
    public static string OnboardingFinishSubtitle(string practiceName) => T(
        $"Рекомендуем начать с «{practiceName}»",
        $"We recommend starting with \"{practiceName}\"");
    public static string OnboardingRecommendedCaption => T("Рекомендуемая практика", "Recommended practice");
    public static string OnboardingDisclaimerTitle => T("Важно", "Important");
    public static string OnboardingDisclaimerBody => T(
        "Приложение не заменяет профессиональную помощь. При тяжёлых состояниях обратитесь к специалисту.",
        "This app does not replace professional care. Seek a specialist for severe conditions.");
    public static string OnboardingStart => T("Попробовать сейчас", "Try it now");
    public static string OnboardingSkip => T("Пропустить", "Skip");
    public static string OnboardingNext => T("Далее", "Next");

    public static string QuoteShareFooter => T("PsychologyApp", "PsychologyApp");

    public static string PhysicsTitle => T("Психосоматик", "Psychosomatic");
    public static string PhysicsIntroPage => T("С введением", "Introduction");
    public static string PhysicsSearchPage => T("Поисковик", "Search");
    public static string PhysicsSearchTitle => T("Психосоматика", "Psychosomatic");
    public static string PhysicsExplanationHeader => T("Пояснение", "Explanation");
    public static string PhysicsExplanationBody => T(
        "Известно, что около половины всех болезней возникает на психической основе. Поэтому по любому физическому недомоганию можно определить то, что вас беспокоит, но не осознаётся.",
        "About half of all illnesses have a psychological component. Any physical symptom can point to an emotional cause that troubles you but stays unconscious.");
    public static string PhysicsDescriptionHeader => T("Описание", "Description");
    public static string PhysicsDescriptionBody => T(
        "Тест поможет вам в пару кликов найти эмоциональную причину любого вашего физического недомогания. Всё очень просто.",
        "This tool helps you find a possible emotional cause of a physical symptom in just a few taps. It's simple.");
    public static string PhysicsAlgorithmStep1 => T(
        "1. Назвать болезнь или часть тела, которая болит",
        "1. Name the illness or body part that hurts");
    public static string PhysicsAlgorithmStep2 => T(
        "2. Узнать несколько возможных причин.",
        "2. Explore several possible emotional causes.");
    public static string PhysicsSearchToolbar => T("Найти", "Search");
    public static string PhysicsProblemLabel => T("Проблема", "Problem");
    public static string PhysicsIllnessPlaceholder => T("Болезнь", "Condition");
    public static string PhysicsEmptySearchHint => T("Введите запрос", "Enter a search term");
    public static string PhysicsEmptySearchSubhint => T("Болезнь или часть тела", "Illness or body part");
    public static string PhysicsNoResultsHint => T("Ничего не найдено", "No results found");
    public static string PhysicsLoadingText => T("Поиск причин", "Searching causes");
    public static string PhysicsSearchFilteringText => T("Подбор результатов", "Filtering results");
    public static string PhysicsSearchError => T(
        "Не удалось выполнить поиск",
        "Search failed");
    public static string QuotesSearchError => T(
        "Не удалось выполнить поиск цитат",
        "Quote search failed");
    public static string LoadFailed => T("Не удалось загрузить", "Failed to load");
    public static string RetryQuestion => T("Попробовать ещё раз?", "Try again?");
    public static string LoadError => T("Ошибка при загрузке", "Failed to load");

    public static string ProfileTitle => T("Профиль", "Profile");
    public static string ProfileLoadingText => T("Загрузка профиля", "Loading profile");
    public static string ProfileUserLabel => T("Пользователь", "User");
    public static string ProfileStandardUser => T("Стандартный", "Standard");
    public static string ProfileTechniquesCompleted => T("Пройдено техник", "Techniques completed");
    public static string ProfileFollowers => T("Подписчиков", "Followers");
    public static string ProfileRecommended => T("Советуем пройти", "Recommended");
    public static string ProfileBestQuotes => T("Избранные цитаты", "Favorite quotes");
    public static string ProfileQuotesSeeAll => T("Все избранные цитаты", "All favorite quotes");
    public static string ProfileQuotesSeeAllSubtitle => T("Открыть вкладку «Цитаты»", "Open Quotes tab");
    public static string FormatProfileQuotesPreviewSubtitle(int shown, int total)
    {
        if (shown <= 0 || total <= 0)
        {
            return string.Empty;
        }

        if (IsEnglish(Language))
        {
            if (total > shown)
            {
                return $"Showing {shown} of {total} favorites";
            }

            return total == 1 ? "Showing 1 favorite" : $"Showing {total} favorites";
        }

        if (total > shown)
        {
            return $"Показано {shown} из {total} избранных";
        }

        return shown == 1 ? "Показана 1 избранная" : $"Показано {shown} избранных";
    }
    public static string QuotesFavoriteAdded => T("Добавлено в избранное", "Added to favorites");
    public static string QuotesFavoriteRemoved => T("Убрано из избранного", "Removed from favorites");
    public static string QuotesGoToTab => T("Перейти к цитатам", "Go to quotes");
    public static string QuotesFeedAll => T("Все", "All");
    public static string QuotesFeedFavorites => T("Избранное", "Favorites");
    public static string QuotesFeedForYou => T("Для вас", "For you");
    public static string QuotesThemeAll => T("Все темы", "All themes");
    public static string QuotesDailyTitle => T("Цитата дня", "Quote of the day");
    public static string QuotesSearchPlaceholder => T("Поиск по цитатам", "Search quotes");
    public static string QuotesSearchEmptyTitle => T("Ничего не найдено", "Nothing found");
    public static string QuotesSearchEmptyBody => T("Попробуйте другой запрос.", "Try a different query.");
    public static string QuotesShowAgain => T("Показать снова", "Show again");
    public static string QuotesForYouHint => T(
        "Подобрано под ваш запрос из онбординга",
        "Picked for your onboarding focus");
    public static string QuotesForYouEmptyTitle => T("Пока нет цитат для вас", "No quotes for you yet");
    public static string QuotesForYouEmptyBody => T(
        "Мы подбираем цитаты под ваш фокус из онбординга. Загляните позже или переключитесь на «Все».",
        "We pick quotes for your onboarding focus. Check back later or switch to All.");
    public static string SettingsQuoteRemindersLabel => T("Цитата дня", "Daily quote");
    public static string SettingsQuoteReminderHourLabel => T("Время цитаты", "Quote time");
    public static string SettingsQuoteReminderHourPickerTitle => T("Время", "Time");
    public static string SettingsMoodRemindersLabel => T("Напоминать о check-in", "Remind me to check in");
    public static string SettingsMoodReminderHourLabel => T("Время check-in", "Check-in time");
    public static string SettingsMoodReminderHourPickerTitle => T("Время", "Time");
    public static string QuoteReminderTitle => T("Цитата дня", "Quote of the day");
    public static string QuoteReminderBody => T(
        "Откройте приложение и прочитайте мысль дня.",
        "Open the app and read today's thought.");
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
    public static string QuoteThemeWisdom => T("Мудрость", "Wisdom");
    public static string QuoteThemeMotivation => T("Мотивация", "Motivation");
    public static string QuoteThemeResilience => T("Стойкость", "Resilience");
    public static string QuoteThemeSelfAwareness => T("Осознанность", "Self-awareness");
    public static string QuoteThemeMindfulness => T("Осмыленность", "Mindfulness");
    public static string QuoteThemeSelfEsteem => T("Самооценка", "Self-esteem");
    public static string QuoteThemeHope => T("Надежда", "Hope");
    public static string QuoteThemeEmpathy => T("Эмпатия", "Empathy");
    public static string QuoteThemeHappiness => T("Счастье", "Happiness");
    public static string QuoteThemeHabits => T("Привычки", "Habits");
    public static string QuoteThemeLove => T("Любовь", "Love");
    public static string QuoteThemeRelationships => T("Отношения", "Relationships");
    public static string QuoteThemeResponsibility => T("Ответственность", "Responsibility");
    public static string QuoteThemePurpose => T("Смысл", "Purpose");
    public static string QuoteThemeGrowth => T("Рост", "Growth");
    public static string QuoteThemeHealing => T("Исцеление", "Healing");
    public static string QuoteThemeSelfLove => T("Любовь к себе", "Self-love");
    public static string QuoteThemeAcceptance => T("Принятие", "Acceptance");
    public static string QuoteThemeGratitude => T("Благодарность", "Gratitude");
    public static string QuoteThemeCalm => T("Спокойствие", "Calm");
    public static string QuoteThemeAnxiety => T("Тревога", "Anxiety");
    public static string QuoteThemeGeneral => T("Общее", "General");
    public static string QuotesAllReadTitle => T("Вы всё прочитали", "You are all caught up");
    public static string QuotesAllReadBody => T(
        "Новых цитат пока нет. Откройте избранное или обновите позже.",
        "No new quotes right now. Open favorites or try again later.");
    public static string QuotesShowFavorites => T("Показать избранное", "Show favorites");
    public static string ProfileBsffSubtitle => T(
        "Методика депрограммирования подсознания",
        "Subconscious deprogramming method");

    public static string MotivatorTitle => ShellTabMotivatorShort;
    public static string QuotesSearching => T("Поиск цитат", "Searching quotes");
    public static string QuotesLoading => T("Загрузка цитат", "Loading quotes");
    public static string QuoteShareTitle => T("Цитата", "Quote");
    public static string UnknownAuthor => T("Неизвестный автор", "Unknown author");

    public static string CrisisHubTitle => T("Срочная помощь", "Crisis help");
    public static string CrisisHubLead => T(
        "Если вам тяжело прямо сейчас, сначала позаботьтесь о безопасности. Это приложение не заменяет экстренную помощь.",
        "If you are struggling right now, prioritize safety first. This app does not replace emergency care.");
    public static string CrisisHubSafetyPlanTitle => T("План безопасности", "Safety plan");
    public static string CrisisHubSafetyPlanStep1 => T(
        "Отойдите от триггеров и займите безопасное место.",
        "Step away from triggers and find a safer place.");
    public static string CrisisHubSafetyPlanStep2 => T(
        "Свяжитесь с кем-то надёжным — другом, родным или специалистом.",
        "Reach a trusted person — a friend, relative, or specialist.");
    public static string CrisisHubSafetyPlanStep3 => T(
        "Если есть угроза жизни — звоните в экстренные службы (112).",
        "If life is at risk — call emergency services (112).");
    public static string CrisisHubSafetyPlanStepNumber1 => "1";
    public static string CrisisHubSafetyPlanStepNumber2 => "2";
    public static string CrisisHubSafetyPlanStepNumber3 => "3";
    public static string CrisisHubSafetyPlanBody =>
        $"{CrisisHubSafetyPlanStepNumber1}. {CrisisHubSafetyPlanStep1}\n{CrisisHubSafetyPlanStepNumber2}. {CrisisHubSafetyPlanStep2}\n{CrisisHubSafetyPlanStepNumber3}. {CrisisHubSafetyPlanStep3}";
    public static string CrisisHubHotlineTitle => T("Позвонить сейчас", "Call now");
    public static string CrisisHubHotlineRu => T(
        "Россия: 8-800-2000-122 (телефон доверия), 112 — экстренные службы",
        "Russia: 8-800-2000-122 (helpline), 112 — emergency services");
    public static string CrisisHubHotlineRuNumber => "88002000122";
    public static string CrisisHubEmergencyNumber => "112";
    public static string CrisisHubHotlineIntl => T(
        "Международно: findahelpline.com",
        "International: findahelpline.com");
    public static string CrisisHubCallHotlineRu => T("Позвонить на 8-800-2000-122", "Call 8-800-2000-122");
    public static string CrisisHubCallEmergency => T("Позвонить в 112", "Call 112");
    public static string CrisisHubOpenHelpline => T("Открыть findahelpline.com", "Open findahelpline.com");
    public static string CrisisHubRecheck => T("Перепроверить состояние", "Recheck how I'm doing");
    public static string CrisisHubContinueSoft => T("Вернуться к мягким практикам", "Return to gentle practices");
    public static string CrisisHubSpecialistHint => T(
        "Рекомендуем обратиться к психологу или врачу. Самопомощь подходит для лёгких и умеренных состояний.",
        "Please consider a psychologist or doctor. Self-help fits mild to moderate states.");

    public static string RiskCheckTitle => T("Проверка безопасности", "Safety check");
    public static string RiskCheckLead => T(
        "Если есть мысли о самоповреждении — откройте помощь сразу. Можно не ждать конца опроса.",
        "If you have thoughts of self-harm — open help now. You do not need to finish the check first.");
    public static string RiskCheckSubtitle => T(
        "Ответьте честно. Это помогает подобрать безопасный сценарий.",
        "Answer honestly. This helps choose a safe path.");
    public static string RiskCheckSelfHarm => T(
        "Есть мысли о самоповреждении или суициде",
        "Thoughts of self-harm or suicide");
    public static string RiskCheckDisorientation => T(
        "Сильная дезориентация или потеря связи с реальностью",
        "Severe disorientation or loss of contact with reality");
    public static string RiskCheckSubstance => T(
        "Риск, связанный с веществами / алкоголем",
        "Substance or alcohol-related risk");
    public static string RiskCheckInsomnia => T(
        "Тяжёлая бессонница и истощение",
        "Severe insomnia and exhaustion");
    public static string RiskCheckSubmit => T("Готово", "Done");
    public static string RiskCheckOpenHelpNow => T("Открыть помощь сейчас", "Open help now");
    public static string RiskCheckYes => T("Да", "Yes");
    public static string RiskCheckNo => T("Нет", "No");
    public static string RiskCheckSourceOnboarding => "onboarding";
    public static string RiskCheckSourcePeriodic => "periodic";
    public static string RiskCheckSourceManual => "manual";
    public static string RiskCheckSourceProfile => "profile";

    public static string OptionsCrisisTitle => T("Срочная помощь", "Crisis help");
    public static string OptionsCrisisSubtitle => T(
        "План безопасности и горячие линии",
        "Safety plan and hotlines");

    public static string ClinicalScorecardTitle => T("Недельный обзор", "Weekly overview");
    public static string PracticeHistorySeeAll => T("Все", "All");
    public static string PracticeHistoryPageTitle => T("История практик", "Practice history");
    public static string ProfileRiskCheckLabel => T("Как я сейчас?", "How am I right now?");
    public static string ProfileRiskCheckSubtitle => T(
        "Короткие вопросы о безопасности",
        "Short questions about safety");
    public static string ProfileMoodTrendPreview(string avgMood, string risk) =>
        T($"Настроение: {avgMood} · риск: {risk}", $"Mood: {avgMood} · risk: {risk}");

    public static string JournalOverviewTitle => T("Обзор", "Overview");
    public static string JournalTimelineTitle => T("Записи", "Entries");
    public static string JournalOpenOverview => T("Обзор", "Overview");
    public static string JournalOpenTimeline => T("Записи", "Entries");
    public static string JournalRecentDaysTitle => T("Эта неделя", "This week");
    public static string JournalPromptHelpedShort => T("Что помогло", "What helped");
    public static string JournalPromptNextShort => T("Что дальше", "What's next");
    public static string JournalShareLabel => T("Поделиться", "Share");
    public static string JournalShareTitle => T("Запись дневника", "Journal entry");
    public static string JournalShareText(string day, string mood, string note) =>
        string.IsNullOrWhiteSpace(note)
            ? T($"{day}: настроение {mood}", $"{day}: mood {mood}")
            : T($"{day}: настроение {mood}\n{note}", $"{day}: mood {mood}\n{note}");
    public static string JournalReminderToggle => T("Напоминать о check-in", "Remind me to check in");
    public static string JournalReminderHint => T(
        "Время можно изменить в настройках",
        "Change the time in Settings");
    public static string MoodReminderTitle => T("Дневник", "Journal");
    public static string MoodReminderBody => T(
        "Как настроение сегодня? Отметьте в дневнике.",
        "How are you feeling today? Log it in your journal.");

    public static string ClinicalScorecardEmpty => T(
        "Пока мало данных — практики и настроение появятся здесь",
        "Not enough data yet — practices and mood will appear here");
    public static string ClinicalScorecardSummary(int practices, int moods, string riskLabel) => T(
        $"За неделю: {practices} практик, {moods} отметок настроения · риск: {riskLabel}",
        $"This week: {practices} practices, {moods} mood check-ins · risk: {riskLabel}");
    public static string ClinicalRiskGreen => T("низкий", "low");
    public static string ClinicalRiskAmber => T("повышенный", "elevated");
    public static string ClinicalRiskRed => T("высокий", "high");

    public static string TherapyProgramTitle => T("Ваш протокол", "Your program");
    public static string TherapyProgramAnxiety => T("Тревога", "Anxiety");
    public static string TherapyProgramMood => T("Настроение", "Mood");
    public static string TherapyProgramStress => T("Стресс", "Stress");
    public static string TherapyProgramWeekLabel(int week) => T($"Неделя {week}", $"Week {week}");
    public static string TherapyProgramWeekGoal(int week) => week switch
    {
        1 => T("Стабилизация: короткие ежедневные практики", "Stabilize with short daily practices"),
        2 => T("Наблюдение за мыслями и телом", "Observe thoughts and body signals"),
        3 => T("Гибкость: пробовать разные техники", "Flexibility: try varied techniques"),
        _ => T("Закрепление и самостоятельный выбор", "Consolidate and choose independently")
    };
    public static string TherapyProgramBanner(string programName, int week, string goal, int? completed = null, int? target = null)
    {
        if (completed is null or < 0 || target is null or <= 0)
        {
            return T(
                $"{programName} · неделя {week}: {goal}",
                $"{programName} · week {week}: {goal}");
        }

        return T(
            $"{programName} · нед. {week} · {completed}/{target}",
            $"{programName} · wk {week} · {completed}/{target}");
    }
    public static string ClinicalAmberBanner => T(
        "Состояние требует внимания — доступна срочная помощь",
        "Your state needs attention — crisis help is available");
    public static string ClinicalRedBanner => T(
        "Сначала откройте срочную помощь",
        "Open crisis help first");

    private static string T(string russian, string english) =>
        IsEnglish(Language) ? english : russian;

    public static bool IsEnglish(string language) =>
        language.Equals("en", StringComparison.OrdinalIgnoreCase)
        || language.Equals("English", StringComparison.OrdinalIgnoreCase)
        || language.Equals("Английский", StringComparison.OrdinalIgnoreCase);
}
