using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.ManageProfile.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.ManageProfile.ProfileOptions;

public partial class OptionsPage : ContentPage
{
    public OptionsPage(IPageViewModelActivator pageViewModelActivator, IOptionsViewModelFactory optionsViewModelFactory)
    {
        InitializeComponent();
        BindingContext = this.ActivateViewModel(pageViewModelActivator, page => optionsViewModelFactory.Create(page));
    }
}
