namespace PsychologyApp.Presentation.Infrastructure;

public static class AppStrings
{
    public static string? LanguageOverride { get; set; }

    public static string Language
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(LanguageOverride))
            {
                return LanguageOverride;
            }

            try
            {
                return UserPreferences.Load().Language;
            }
            catch
            {
                return UserPreferences.DefaultLanguage;
            }
        }
    }

    public static string ShellTabPractice => T("Практик", "Practice");
    public static string ShellTabDetector => T("Детектор", "Detector");
    public static string ShellTabSomatic => T("Соматик", "Somatic");
    public static string ShellTabCleaner => T("Очиститель", "Cleaner");
    public static string ShellTabMotivator => T("Мотиватор", "Motivator");

    public static string ShellTabPracticeShort => T("Практик", "Practice");
    public static string ShellTabDetectorShort => T("Тесты", "Tests");
    public static string ShellTabSomaticShort => T("Тело", "Body");
    public static string ShellTabCleanerShort => T("Звук", "Audio");
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
    public static string ReviewSmsError(string message) =>
        T($"Ошибка при отправке СМС: {message}", $"Failed to send SMS: {message}");
    public static string ReviewEmailNotSupported => T(
        "Отправка email не поддерживается",
        "Email is not supported on this device");
    public static string ReviewEmailError(string message) =>
        T($"Ошибка при отправке email: {message}", $"Failed to send email: {message}");
    public static string ReviewShareTitle => T("Отзыв о приложении", "App feedback");
    public static string ReviewShareError(string message) =>
        T($"Не удалось открыть меню отправки: {message}", $"Failed to open share menu: {message}");

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
    public static string CleanerMoreInfoBody => T(
        "Это сборник сильных молитв, который поможет вам очиститься от негатива. Нужен хороший интернет.",
        "A collection of powerful prayers to help you release negativity. A stable internet connection is required.");
    public static string CleanerPrayerMain => T("Основная молитва", "Main prayer");
    public static string CleanerPsalm50 => T("Псалом 50", "Psalm 50");
    public static string CleanerPsalm50Desc => T(
        "Читается 3 раза в течение суток",
        "Read three times within a day");
    public static string CleanerOurFather => T("Отче Наш", "Our Father");
    public static string CleanerJesusPrayer => T("Иисусова Молитва", "Jesus Prayer");
    public static string CleanerHeavenlyKing => T("Царю небесный", "Heavenly King");
    public static string CleanerDoxology => T("Славословие", "Doxology");
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

    public static string TestsDetectorTitle => T("Детектор", "Detector");
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
    public static string TestsEmptyTitle => T("Тесты загружаются", "Loading tests");
    public static string TestsEmptyBody => T("Подождите немного", "Please wait a moment");
    public static string TestsLoadingText => T("Загрузка тестов", "Loading tests");
    public static string QuotesEmptyTitle => T("Цитаты не найдены", "No quotes found");
    public static string QuotesEmptyBody => T("Потяните вниз, чтобы обновить", "Pull down to refresh");
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
    public static string TodayMoodQuestion => T("Как настроение?", "How are you feeling?");
    public static string TodayMoodSaved => T("Настроение сохранено", "Mood saved");
    public static string TodayMoodLine(int level, int max) =>
        T($"Сегодня: {MoodEmoji(level)} {level}/{max}", $"Today: {MoodEmoji(level)} {level}/{max}");
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
    public static string ProfileBestQuotes => T("Лучшие цитаты", "Top quotes");
    public static string ProfileBsffSubtitle => T(
        "Методика депрограммирования подсознания",
        "Subconscious deprogramming method");

    public static string MotivatorTitle => T("Мотиватор", "Motivator");
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

    public static string HeckHessScore(int ball) =>
        ball <= 24
            ? T("0-24 - невысокий уровень невротизации", "0-24 - low neuroticism")
            : T("25-40 - высокий уровень невротизации", "25-40 - high neuroticism");

    public static string HaerScore(int ball) =>
        ball <= 29
            ? T("0-28 - невысокий уровень психопатии", "0-28 - low psychopathy")
            : T("29-40 - высокий уровень психопатии", "29-40 - high psychopathy");

    public static string PochebutScore(int ball) => ball switch
    {
        <= 10 => T("0-10 - низкий уровень агрессивности", "0-10 - low aggressiveness"),
        <= 24 => T("11-24 - средний уровень агрессивности", "11-24 - moderate aggressiveness"),
        _ => T("25-40 - высокий уровень агрессивности", "25-40 - high aggressiveness")
    };

    private static string T(string russian, string english) =>
        UserPreferences.IsEnglish(Language) ? english : russian;
}
