using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.TechniqueCreated;

public partial class CreatedPage : ContentPage
{
    public CreatedPage(
        IPageViewModelActivator pageViewModelActivator,
        ICreatedViewModelFactory createdViewModelFactory,
        long id)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => createdViewModelFactory.Create(nav, id));
    }
}
