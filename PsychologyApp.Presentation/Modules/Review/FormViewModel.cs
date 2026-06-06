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

    public string PageTitle => AppStrings.ReviewTitle;
    public string ExplanationHeader => AppStrings.PhysicsExplanationHeader;
    public string ExplanationBody => AppStrings.ReviewExplanation;
    public string FormSectionTitle => AppStrings.FormLabel;
    public string MessageFieldLabel => AppStrings.MessageLabel;
    public string MessagePlaceholder => AppStrings.ReviewMessagePlaceholder;
    public string SendButtonText => AppStrings.Send;

    public FormViewModel(IDialogService dialogService, IOptions<AppSettings> settings)
    {
        _dialogService = dialogService;
        _settings = settings.Value;
        ModuleName = AppStrings.ReviewTitle;
        PageName = AppStrings.ReviewPage;
        MessageText = string.Empty;

        Send = new AsyncCommand(SendAsync);
        UserPreferences.Changed += OnPreferencesChanged;
    }

    private void OnPreferencesChanged()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(ExplanationHeader));
        OnPropertyChanged(nameof(ExplanationBody));
        OnPropertyChanged(nameof(FormSectionTitle));
        OnPropertyChanged(nameof(MessageFieldLabel));
        OnPropertyChanged(nameof(MessagePlaceholder));
        OnPropertyChanged(nameof(SendButtonText));
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrEmpty(MessageText))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_settings.ReviewSmsRecipient))
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsRecipientMissing);
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
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsNotSupported);
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsError(ex.Message));
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
