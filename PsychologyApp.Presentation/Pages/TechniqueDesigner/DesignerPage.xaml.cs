using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;

namespace PsychologyApp.Presentation.Pages.TechniqueDesigner;

public partial class DesignerPage : ContentPage
{
    public DesignerPage(
        IPageViewModelActivator pageViewModelActivator,
        IDesignerViewModelFactory designerViewModelFactory,
        long id)
    {
        InitializeComponent();
        this.ActivateViewModel(pageViewModelActivator, nav => designerViewModelFactory.Create(nav, id));
    }
}
