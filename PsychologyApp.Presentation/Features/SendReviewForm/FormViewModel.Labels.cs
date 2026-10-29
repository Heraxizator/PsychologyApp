using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

public partial class FormViewModel
{
    public string PageTitle => AppStrings.ReviewTitle;
    private bool SharesInsteadOfSending => ResolveChannel(_settings) == FeedbackChannel.Share;

    public string ExplanationBody => SharesInsteadOfSending ? AppStrings.ReviewExplanationShare : AppStrings.ReviewExplanation;
    public string MessagePlaceholder => AppStrings.ReviewMessagePlaceholder;
    public string SendButtonText => SharesInsteadOfSending ? AppStrings.ReviewShareButton : AppStrings.Send;

    protected override void RefreshLocalizedProperties()
    {
        Notify(
            nameof(PageTitle),
            nameof(ExplanationBody),
            nameof(MessagePlaceholder),
            nameof(SendButtonText));
    }
}
