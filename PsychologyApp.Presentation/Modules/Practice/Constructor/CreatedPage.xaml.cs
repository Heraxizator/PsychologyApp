using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.TechniquePages.ConstructorPages;

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
