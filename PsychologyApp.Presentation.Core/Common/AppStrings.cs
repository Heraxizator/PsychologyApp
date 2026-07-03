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
    public static string SettingsPracticeReminderHourLabel => T("Время напоминания", "Reminder time");
    public static string SettingsPracticeReminderHourPickerTitle => T("Время", "Time");
    public static string PracticeReminderTitle => T("Пора позаниматься", "Time to practice");
    public static string PracticeReminderBody => T(
        "Уделите несколько минут практике — это поддержит ваш прогресс.",
        "Take a few minutes to practice and keep your progress going.");
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
    public static string PracticeLoadingText => T("Загрузка практик", "Loading practices");
    public static string PracticeCustomTechniqueNumber(long id) =>
        T($"Своя техника №{id}", $"Custom technique #{id}");
    public static string PracticeDesignTitle => T("Создание техники", "Create technique");
    public static string PracticeConstructor => T("Конструктор", "Designer");
    public static string PracticeCustomTechnique => T("Своя техника", "Custom technique");
    public static string PracticeDeleteConfirm => T(
        "Вы уверены, что хотите удалить свою технику",
        "Are you sure you want to delete your technique?");

    public static string ReviewTitle => T("Отзовик", "Feedback");
    public static string ReviewPage => T("Отзыв", "Review");
    public static string ReviewExplanation => T(
        "Вы можете сообщить о проблеме или предложить свои идеи о том, как сделать приложение ещё лучше. Служба поддержки получит ваше сообщение.",
        "You can report a problem or suggest ideas to improve the app. Support will receive your message.");
    public static string ReviewMessagePlaceholder => T("Проблема на странице X", "Issue on page X");
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

    public static string InfoAboutBody => T(
        "Приложение представляет собой список простых, но в тоже время мощных техник, которые помогут вам справиться с такими проблемами, как стресс, страх, сомнения, навязчивые мысли, ограничивающие убеждения и деструктивные установки. Все методики являются общеизвестными и проверены временем. Некоторые взяты из НЛП или трудов Живорада Славинского. Эти инструменты позволят вам сэкономить много времени, сил и денег. Они не потребуют глубоких знаний в области психологии и программирования подсознания. Желаем успеха в проработках!",
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

    public static string PhysicsSolutionHeader => T("Что делать", "What to do");
    public static string PhysicsRecommendedPractices => T("Практики, которые могут помочь", "Practices that may help");
    public static string PhysicsTryPractice => T("Попробовать практику", "Try a practice");

    public static string ProfileTestsCompleted => T("Пройдено тестов", "Tests completed");
    public static string ProfileStreakDays => T("Дней подряд", "Day streak");
    public static string ProfileStreakCount(int days) => T($"{days} дн.", $"{days} days");

    public static string TodayForYou => T("Сегодня для вас", "Today for you");
    public static string TodayRecommended => T("Рекомендуемая практика", "Recommended practice");
    public static string TodayStartPractice => T("Начать", "Start");
    public static string TodayRecommendationReason(string concern) => concern switch
    {
        "anxiety" => T("Подходит при тревоге", "Good for anxiety"),
        "body" => T("Для работы с телом и симптомами", "For body and symptoms"),
        "mood" => T("Помогает при тяжёлом настроении", "Helps when mood is low"),
        _ => T("Практика дня", "Practice of the day")
    };
    public static string TodayMoodQuestion => T("Как настроение?", "How are you feeling?");
    public static string TodayMoodSaved => T("Настроение сохранено", "Mood saved");
    public static string TodayMoodLine(int level, int max) =>
        T($"Сегодня: {MoodEmoji(level)} {level}/{max}", $"Today: {MoodEmoji(level)} {level}/{max}");
    public static string MoodHistoryTitle => T("Недавнее", "Recent");
    public static string MoodHistoryEntry(string date, int level, int max) =>
        T($"{date}: {MoodEmoji(level)} {level}/{max}", $"{date}: {MoodEmoji(level)} {level}/{max}");
    public static string PracticeCompletedTitle => T("Готово!", "Done!");
    public static string PracticeCompletedBody(int streak) =>
        T($"Отличная работа! Серия: {streak} дн.", $"Great job! Streak: {streak} days");
    public static string PracticeGoHomeButton => T("На главную", "Go home");
    public static string PracticeMoreButton => T("Ещё практика", "More practice");
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
    public static string QuotesFavoriteAdded => T("Добавлено в избранное", "Added to favorites");
    public static string QuotesFavoriteRemoved => T("Убрано из избранного", "Removed from favorites");
    public static string QuotesGoToTab => T("Перейти к цитатам", "Go to quotes");
    public static string QuotesFeedAll => T("Все", "All");
    public static string QuotesFeedFavorites => T("Избранное", "Favorites");
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
    private static string T(string russian, string english) =>
        IsEnglish(Language) ? english : russian;

    public static bool IsEnglish(string language) =>
        language.Equals("en", StringComparison.OrdinalIgnoreCase)
        || language.Equals("English", StringComparison.OrdinalIgnoreCase)
        || language.Equals("Английский", StringComparison.OrdinalIgnoreCase);
}
