using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

public partial class FormViewModel
{
    public string PageTitle => AppStrings.ReviewTitle;
    public string ExplanationHeader => AppStrings.ReviewExplanationHeader;
    private bool SharesInsteadOfSending => ResolveChannel(_settings) == FeedbackChannel.Share;

    public string ExplanationBody => SharesInsteadOfSending ? AppStrings.ReviewExplanationShare : AppStrings.ReviewExplanation;
    public new string FormSectionTitle => AppStrings.FormLabel;
    public string MessageFieldLabel => AppStrings.MessageLabel;
    public string MessagePlaceholder => AppStrings.ReviewMessagePlaceholder;
    public string SendButtonText => SharesInsteadOfSending ? AppStrings.ReviewShareButton : AppStrings.Send;

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
}
