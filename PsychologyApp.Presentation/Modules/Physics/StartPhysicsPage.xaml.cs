using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Views.Physics;

public partial class StartPhysicsPage : ContentPage
{
    private readonly StartPhysicsViewModel viewModel;

    public StartPhysicsPage(
        IPageViewModelActivator pageViewModelActivator,
        IStartPhysicsViewModelFactory startPhysicsViewModelFactory)
    {
        InitializeComponent();

        viewModel = this.ActivateViewModel(pageViewModelActivator, nav => startPhysicsViewModelFactory.Create(nav));

        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        viewModel.SetDone();
    }
}
