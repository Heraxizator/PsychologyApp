using PsychologyApp.Presentation.Templates;
using PsychologyApp.Presentation.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;

namespace PsychologyApp.Presentation.Modules.Practic.Techniques.AIPsychologist;

public class AIPsychologistViewModel : BaseViewModel
{
    private const string OpenRouterApiKey = "openrouter_api_key";
    private const string DefaultOpenRouterApiKey = "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"; // Пользователь должен ввести свой ключ

    private IAIPsychologistProvider? _aiProvider;

    public ObservableCollection<ChatMessage> Messages { get; private set; } = [];

    private string _userMessage = string.Empty;
    public string UserMessage
    {
        get => _userMessage;
        set => SetProperty(ref _userMessage, value);
    }

    private bool _isLoading = false;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            SetProperty(ref _isLoading, value);
            OnPropertyChanged(nameof(IsNotLoading));
        }
    }

    public bool IsNotLoading => !_isLoading;

    public ICommand SendMessageCommand { get; private set; } = default!;

    public AIPsychologistViewModel() { }

    public AIPsychologistViewModel(INavigation navigation)
    {
        Navigation = navigation;

        ModuleName = "Практик";
        PageName = "ИИ-психолог";

        Algorithm =
        [
            "1. Введите ваше сообщение в поле ввода",
            "2. Нажмите 'Отправить' или Enter",
            "3. Дождитесь ответа от AI-психолога",
            "4. Продолжайте диалог для получения поддержки"
        ];

        Info = "ИИ-психолог - это AI-модель на базе DeepSeek R1 для психологической поддержки. Работает через OpenRouter API в облаке. Модель обучена оказывать психологическую поддержку, помогать в решении проблем и предоставлять советы. Работает на всех платформах и не требует локальной установки.";

        SendMessageCommand = new Command(async () => await SendMessageAsync(), () => IsNotLoading);

        // Инициализация модели в фоне
        _ = Task.Run(async () => await InitializeModelAsync());
    }

    private async Task InitializeModelAsync()
    {
        try
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                IsLoading = true;
                AddSystemMessage("Инициализация AI-модели через OpenRouter API...");
            });

            // Получаем API ключ OpenRouter
            var apiKey = Preferences.Default.Get(OpenRouterApiKey, DefaultOpenRouterApiKey);

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AddSystemMessage("⚠️ API ключ OpenRouter не найден");
                    AddSystemMessage("");
                    AddSystemMessage("Для работы требуется API ключ доступа к OpenRouter.");
                    AddSystemMessage("Получите ключ на: https://openrouter.ai/keys");
                    IsLoading = false;
                });
                return;
            }

            _aiProvider = new OpenRouterProvider(apiKey);

            var initialized = await _aiProvider.InitializeAsync();

            if (!initialized)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AddSystemMessage("⚠️ Не удалось подключиться к OpenRouter API");
                    AddSystemMessage("");
                    AddSystemMessage("Возможные причины:");
                    AddSystemMessage("• Неверный API ключ");
                    AddSystemMessage("• Проблемы с интернет-соединением");
                    AddSystemMessage("• Временная недоступность сервиса");
                    AddSystemMessage("");
                    AddSystemMessage("Проверьте ключ на: https://openrouter.ai/keys");
                    IsLoading = false;
                });
                return;
            }

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                AddSystemMessage($"✅ {_aiProvider.ProviderName} успешно инициализирован!");
                AddSystemMessage("Модель готова к работе. Можете начать диалог.");
                IsLoading = false;
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                AddSystemMessage($"Ошибка при инициализации: {ex.Message}");
                if (ex.InnerException != null)
                {
                    AddSystemMessage($"Детали: {ex.InnerException.Message}");
                }
                IsLoading = false;
            });
        }
    }


    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(UserMessage) || IsLoading)
        {
            return;
        }

        if (_aiProvider == null || !_aiProvider.IsInitialized)
        {
            AddSystemMessage("⚠️ AI провайдер не инициализирован. Пожалуйста, дождитесь завершения инициализации.");
            return;
        }

        var userText = UserMessage.Trim();
        UserMessage = string.Empty;

        // Добавляем сообщение пользователя
        AddUserMessage(userText);

        // Генерируем ответ
        IsLoading = true;
        ((Command)SendMessageCommand).ChangeCanExecute();

        try
        {
            var response = await GenerateResponseAsync(userText);
            AddAIMessage(response);
        }
        catch (Exception ex)
        {
            AddSystemMessage($"Ошибка: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            ((Command)SendMessageCommand).ChangeCanExecute();
        }
    }

    private async Task<string> GenerateResponseAsync(string userMessage)
    {
        if (_aiProvider == null || !_aiProvider.IsInitialized)
        {
            return "AI провайдер не инициализирован.";
        }

        try
        {
            return await _aiProvider.GenerateResponseAsync(userMessage);
        }
        catch (Exception ex)
        {
            return $"Ошибка генерации ответа: {ex.Message}";
        }
    }

    private void AddUserMessage(string text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Add(new ChatMessage
            {
                Text = text,
                IsUser = true,
                Timestamp = DateTime.Now
            });
        });
    }

    private void AddAIMessage(string text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Add(new ChatMessage
            {
                Text = text,
                IsUser = false,
                Timestamp = DateTime.Now
            });
        });
    }

    private void AddSystemMessage(string text)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Messages.Add(new ChatMessage
            {
                Text = text,
                IsUser = false,
                Timestamp = DateTime.Now
            });
        });
    }
}