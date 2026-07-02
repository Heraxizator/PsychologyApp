using PsychologyApp.Presentation.Features.SendReviewForm;

namespace PsychologyApp.Presentation.Pages.SendReviewForm.ReviewForm;

public partial class FormPage : ContentPage
{
    public FormPage(IFormViewModelFactory formViewModelFactory)
    {
        InitializeComponent();
        BindingContext = formViewModelFactory.Create(this);
    }
}
