using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.Features.RunTechniqueSession.DependencyInjection;

namespace PsychologyApp.Presentation.Pages.RunTechniqueSession.TechniqueDesigner;

public partial class DesignerPage : ContentPage
{
    public DesignerPage(
        IPageViewModelActivator pageViewModelActivator,
        IDesignerViewModelFactory designerViewModelFactory,
        long id)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, page => designerViewModelFactory.Create(page, id));
    }
}
