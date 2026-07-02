using PsychologyApp.Presentation.Features.SendReviewForm;

namespace PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;

public partial class FormPage : ContentPage
{
    public FormPage(FormViewModel formViewModel)
    {
        InitializeComponent();
        BindingContext = formViewModel;
    }
}
