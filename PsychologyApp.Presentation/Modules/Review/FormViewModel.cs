using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Infrastructure;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Review;

public class FormViewModel : BaseViewModel
{
    public ICommand Send { get; private set; } = default!;

    private readonly IDialogService _dialogService;
    private readonly AppSettings _settings;

    public FormViewModel(IDialogService dialogService, IOptions<AppSettings> settings)
    {
        _dialogService = dialogService;
        _settings = settings.Value;
        ModuleName = "Отзовик";
        PageName = "Отзыв";
        MessageText = string.Empty;

        Send = new AsyncCommand(SendAsync);
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrEmpty(MessageText))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.ReviewSmsRecipient))
        {
            await _dialogService.ShowAsync(null, "Получатель SMS не настроен");
            return;
        }

        await SendSmsAsync(MessageText, _settings.ReviewSmsRecipient);
    }

    public async Task SendSmsAsync(string messageText, string recipient)
    {
        try
        {
            SmsMessage message = new(messageText, new[] { recipient });
            await Sms.Default.ComposeAsync(message);
        }
        catch (FeatureNotSupportedException)
        {
            await _dialogService.ShowAsync(null, "Отправка СМС не поддерживается");
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAsync(null, $"Ошибка при отправке СМС: {ex.Message}");
        }
    }

    private string message_text = string.Empty;
    public string MessageText
    {
        get => message_text;
        set
        {
            if (message_text != value)
            {
                message_text = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }
    }
}
