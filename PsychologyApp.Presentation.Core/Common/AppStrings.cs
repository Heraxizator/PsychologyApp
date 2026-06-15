namespace PsychologyApp.Presentation.Common;

public static class AppStrings
{
    public const string DefaultLanguage = "ru";

    public static string? LanguageOverride { get; set; }

    public static Func<string>? LanguageProvider { get; set; }

    public static string Language =>
        LanguageOverride
        ?? LanguageProvider?.Invoke()
        ?? DefaultLanguage;

    public static string ShellTabPractice => T("Практик", "Practice");
    public static string ShellTabDetector => T("Детектор", "Detector");
    public static string ShellTabSomatic => T("Соматик", "Somatic");
    public static string ShellTabCleaner => T("Очиститель", "Cleaner");
    public static string ShellTabMotivator => T("Мотиватор", "Motivator");

    public static string ShellTabPracticeShort => T("Практик", "Practice");
    public static string ShellTabDetectorShort => T("Тесты", "Tests");
    public static string ShellTabSomaticShort => T("Тело", "Body");
    public static string ShellTabCleanerShort => T("Молитвы", "Prayers");
    public static string ShellTabMotivatorShort => T("Цитаты", "Quotes");

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

    public static string OnboardingWelcomeTitle => T("Добро пожаловать", "Welcome");
    public static string OnboardingWelcomeBody => T(
        "Короткие практики, тесты и психосоматика — офлайн и без осуждения",
        "Short practices, tests, and psychosomatic insights — offline and judgment-free");
    public static string OnboardingConcernTitle => T("Что вас беспокоит?", "What troubles you?");
    public static string OnboardingConcernAnxiety => T("Тревога", "Anxiety");
    public static string OnboardingConcernBody => T("Тело / симптомы", "Body / symptoms");
    public static string OnboardingConcernMood => T("Настроение", "Mood");
    public static string OnboardingConcernExplore => T("Просто попробовать", "Just exploring");
    public static string OnboardingDisclaimerTitle => T("Важно", "Important");
    public static string OnboardingDisclaimerBody => T(
        "Приложение не заменяет профессиональную помощь. При тяжёлых состояниях обратитесь к специалисту.",
        "This app does not replace professional care. Seek a specialist for severe conditions.");
    public static string OnboardingStart => T("Начать практику", "Start a practice");
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

    private static string T(string russian, string english) =>
        IsEnglish(Language) ? english : russian;

    public static bool IsEnglish(string language) =>
        language.Equals("en", StringComparison.OrdinalIgnoreCase)
        || language.Equals("English", StringComparison.OrdinalIgnoreCase)
        || language.Equals("Английский", StringComparison.OrdinalIgnoreCase);
}
