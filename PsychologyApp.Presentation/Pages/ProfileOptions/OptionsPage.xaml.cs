using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.ProfileOptions;

public partial class OptionsPage : ContentPage
{
    public OptionsPage(IPageViewModelActivator pageViewModelActivator, IOptionsViewModelFactory optionsViewModelFactory)
    {
        InitializeComponent();
        BindingContext = this.ActivateViewModel(pageViewModelActivator, page => optionsViewModelFactory.Create(page));
    }
}
