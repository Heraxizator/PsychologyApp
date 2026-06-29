using PsychologyApp.Presentation.Shared.Common;
using PsychologyApp.Presentation.Shared.Navigation;
using PsychologyApp.Presentation.App.Providers;
using PsychologyApp.Presentation.Pages.PhysicsSearch;

namespace PsychologyApp.Presentation.Pages.StartPhysics;

public partial class StartPhysicsPage : ContentPage
{
    private readonly StartPhysicsViewModel viewModel;
    private PageAnimationHelper? _animationHelper;

    public StartPhysicsPage(
        IPageViewModelActivator pageViewModelActivator,
        IStartPhysicsViewModelFactory startPhysicsViewModelFactory)
    {
        InitializeComponent();

        viewModel = this.ActivateViewModel(pageViewModelActivator, page => startPhysicsViewModelFactory.Create(page));

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
