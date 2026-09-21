namespace PsychologyApp.Presentation.Common;

public static partial class AppStrings
{
    public static string LocalAiTitle => T("Локальный ИИ", "On-device AI");

    public static string LocalAiDescription => T(
        "Небольшая языковая модель делает ответы собеседника живее. Она работает прямо на телефоне: ваши слова никуда не отправляются. Без неё собеседник тоже работает.",
        "A small language model makes the companion's replies more natural. It runs on your phone: your words are never sent anywhere. The companion also works without it.");

    public static string LocalAiExperimentalNote => T(
        "Экспериментально. Ответы модели могут быть неточными, это не замена специалисту.",
        "Experimental. The model can be inaccurate and is not a replacement for a professional.");

    public static string LocalAiDownloadAction(long megabytes) => T($"Скачать ({megabytes} МБ)", $"Download ({megabytes} MB)");

    public static string LocalAiConfirmTitle => T("Скачать модель?", "Download the model?");

    public static string LocalAiConfirmBody(string name, long megabytes) => T(
        $"Будет скачано около {megabytes} МБ ({name}). Лучше подключиться к Wi-Fi. Нужно свободное место на телефоне. Условия использования модели задаёт её автор.",
        $"About {megabytes} MB will be downloaded ({name}). Wi-Fi is recommended and you need free space on the phone. The model's terms of use are set by its author.");

    public static string LocalAiConfirmAccept => T("Скачать", "Download");
    public static string LocalAiCancelDialog => T("Отмена", "Cancel");
    public static string LocalAiCancelDownload => T("Отменить загрузку", "Cancel download");

    public static string LocalAiDownloading(int percent) => T($"Загрузка: {percent}%", $"Downloading: {percent}%");

    public static string LocalAiInstalled => T("Модель установлена и работает на устройстве", "The model is installed and runs on this device");

    public static string LocalAiDeleteAction => T("Удалить модель", "Delete the model");

    public static string LocalAiDeleteConfirm => T(
        "Удалить модель с устройства? Собеседник продолжит работать без неё.",
        "Delete the model from this device? The companion will keep working without it.");

    public static string LocalAiDeleteAccept => T("Удалить", "Delete");

    public static string LocalAiFailed => T(
        "Не удалось скачать модель. Проверьте соединение и свободное место, затем попробуйте снова: загрузка продолжится с того же места.",
        "Could not download the model. Check your connection and free space, then try again: the download resumes where it stopped.");

    public static string LocalAiNotEnoughMemory(long gigabytes) => T(
        $"Для локальной модели нужно не меньше {gigabytes} ГБ оперативной памяти, на этом устройстве её меньше. Собеседник работает и без неё.",
        $"The on-device model needs at least {gigabytes} GB of RAM and this device has less. The companion works without it.");

    public static string LocalAiNotEnoughStorage(long megabytes) => T(
        $"Не хватает места: нужно около {megabytes} МБ свободного пространства. Освободите место и вернитесь сюда.",
        $"Not enough space: about {megabytes} MB of free storage is needed. Free some up and come back.");

    public static string LocalAiCanceled => T("Загрузка остановлена. Её можно продолжить позже.", "Download stopped. You can resume it later.");
}
