using PsychologyApp.Presentation.ViewModels.Review;

namespace PsychologyApp.Presentation.Views.Review;

public partial class FormPage : ContentPage
{
    public FormPage(FormViewModel formViewModel)
    {
        InitializeComponent();
        BindingContext = formViewModel;
    }
}
