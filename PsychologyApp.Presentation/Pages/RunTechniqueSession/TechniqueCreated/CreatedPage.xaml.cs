using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueCreated;

public partial class CreatedPage : ContentPage
{
    public CreatedPage(
        IPageViewModelActivator pageViewModelActivator,
        ICreatedViewModelFactory createdViewModelFactory,
        long id)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, page => createdViewModelFactory.Create(page, id));
    }
}
