using PsychologyApp.Presentation.Common;
using PsychologyApp.Presentation.Services;
using PsychologyApp.Presentation.Services.Factories;
using PsychologyApp.Presentation.ViewModels.Physics;

namespace PsychologyApp.Presentation.Views.Physics;

public partial class StartPhysicsPage : ContentPage
{
    private readonly StartPhysicsViewModel viewModel;
    private PageAnimationHelper? _animationHelper;

    public StartPhysicsPage(
        IPageViewModelActivator pageViewModelActivator,
        IStartPhysicsViewModelFactory startPhysicsViewModelFactory)
    {
        InitializeComponent();

        viewModel = this.ActivateViewModel(pageViewModelActivator, nav => startPhysicsViewModelFactory.Create(nav));

        BindingContext = viewModel;
        _animationHelper = new PageAnimationHelper(viewModel, LoadingProgress, staggerLayout: ContentStack);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _animationHelper?.TryRevealAsync();
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        if (Handler is null)
        {
            _animationHelper?.Dispose();
            _animationHelper = null;
        }
    }
}
