using PsychologyApp.Presentation.Shared.Common;

namespace PsychologyApp.Presentation.Features.SendReviewForm;

public partial class FormViewModel
{
    public string PageTitle => AppStrings.ReviewTitle;
    public string ExplanationHeader => AppStrings.PhysicsExplanationHeader;
    public string ExplanationBody => AppStrings.ReviewExplanation;
    public new string FormSectionTitle => AppStrings.FormLabel;
    public string MessageFieldLabel => AppStrings.MessageLabel;
    public string MessagePlaceholder => AppStrings.ReviewMessagePlaceholder;
    public string SendButtonText => AppStrings.Send;

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
