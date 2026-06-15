using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services.Dialogs;

namespace PsychologyApp.Presentation.ViewModels.Review;

public partial class FormViewModel
{
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
        catch (Exception)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewEmailFailed);
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
        catch (Exception)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewSmsFailed);
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
        catch (Exception)
        {
            await _dialogService.ShowAsync(null, AppStrings.ReviewShareFailed);
            return false;
        }
    }
}
