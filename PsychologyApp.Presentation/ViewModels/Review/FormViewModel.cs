using Microsoft.Extensions.Options;
using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Services.Dialogs;
using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.ViewModels;
using System.Windows.Input;

namespace PsychologyApp.Presentation.ViewModels.Review;

public enum FeedbackChannel
{
    Email,
    Sms,
    Share
}

public class FormViewModel : BaseViewModel
{
    public const string FeedbackEmailSubject = "Psychology App feedback";

    public ICommand Send { get; private set; } = default!;

    private readonly IDialogService _dialogService;
    private readonly AppSettings _settings;

    public string PageTitle => AppStrings.ReviewTitle;
    public string ExplanationHeader => AppStrings.PhysicsExplanationHeader;
    public string ExplanationBody => AppStrings.ReviewExplanation;
    public new string FormSectionTitle => AppStrings.FormLabel;
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
    }

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ExplanationHeader),
            nameof(ExplanationBody),
            nameof(FormSectionTitle),
            nameof(MessageFieldLabel),
            nameof(MessagePlaceholder),
            nameof(SendButtonText));
    }

    internal static FeedbackChannel ResolveChannel(AppSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.ReviewEmailAddress))
        {
            return FeedbackChannel.Email;
        }

        if (!string.IsNullOrWhiteSpace(settings.ReviewSmsRecipient))
        {
            return FeedbackChannel.Sms;
        }

        return FeedbackChannel.Share;
    }

    private async Task SendAsync()
    {
        if (string.IsNullOrWhiteSpace(MessageText))
        {
            return;
        }

        bool sent = ResolveChannel(_settings) switch
        {
            FeedbackChannel.Email => await SendEmailAsync(MessageText, _settings.ReviewEmailAddress),
            FeedbackChannel.Sms => await SendSmsAsync(MessageText, _settings.ReviewSmsRecipient),
            FeedbackChannel.Share => await SendShareAsync(MessageText),
            _ => false
        };

        if (!sent)
        {
            return;
        }

        MessageText = string.Empty;
        await _dialogService.ShowAsync(AppStrings.ReviewSentTitle, AppStrings.ReviewSentMessage);
    }

    public async Task<bool> SendEmailAsync(string messageText, string recipient)
    {
        try
        {
            await Email.Default.ComposeAsync(new EmailMessage
            {
                Subject = FeedbackEmailSubject,
                Body = messageText,
                To = [recipient]
            });
            return true;
        }
        catch (FeatureNotSupportedException)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewEmailNotSupported);
            return false;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewEmailError(ex.Message));
            return false;
        }
    }

    public async Task<bool> SendSmsAsync(string messageText, string recipient)
    {
        try
        {
            SmsMessage message = new(messageText, [recipient]);
            await Sms.Default.ComposeAsync(message);
            return true;
        }
        catch (FeatureNotSupportedException)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsNotSupported);
            return false;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsError(ex.Message));
            return false;
        }
    }

    public async Task<bool> SendShareAsync(string messageText)
    {
        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = AppStrings.ReviewShareTitle,
                Text = messageText
            });
            return true;
        }
        catch (Exception ex)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewShareError(ex.Message));
            return false;
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
