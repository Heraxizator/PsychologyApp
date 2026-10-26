using PsychologyApp.Presentation.Features.ReadArticles;

namespace PsychologyApp.Presentation.Pages.ReadArticles.AnchorThoughts;

public partial class AnchorThoughtsPage : ContentPage
{
    public AnchorThoughtsPage(IAnchorThoughtsViewModelFactory viewModelFactory)
    {
        InitializeComponent();
        BindingContext = viewModelFactory.Create(this);
    }
}
