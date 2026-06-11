using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;

namespace PsychologyApp.Presentation.Views.Practice.Constructor;

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
