using PsychologyApp.Application.Configuration;
using PsychologyApp.Presentation.Common;

namespace PsychologyApp.Presentation.ViewModels.Review;

public partial class FormViewModel
{
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

        _ = ResolveChannel(_settings) switch
        {
            FeedbackChannel.Email => await SendEmailAsync(MessageText, _settings.ReviewEmailAddress),
            FeedbackChannel.Sms => await SendSmsAsync(MessageText, _settings.ReviewSmsRecipient),
            FeedbackChannel.Share => await SendShareAsync(MessageText),
            _ => false
        };
    }
}
