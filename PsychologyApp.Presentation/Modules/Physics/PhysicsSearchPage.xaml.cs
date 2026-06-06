using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Views.Physics;

public partial class PhysicsSearchPage : ContentPage
{
    PhysicsSearchViewModel ViewModel { get; set; }

    public PhysicsSearchPage(
        IPageViewModelActivator pageViewModelActivator,
        IPhysicsSearchViewModelFactory physicsSearchViewModelFactory)
    {
        InitializeComponent();
        ViewModel = this.ActivateViewModel(pageViewModelActivator, nav => physicsSearchViewModelFactory.Create(nav));
    }
}
